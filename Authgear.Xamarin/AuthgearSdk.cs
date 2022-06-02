using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Authgear.Xamarin.CsExtensions;
using Authgear.Xamarin.Data;
using Authgear.Xamarin.Data.Oauth;
using Authgear.Xamarin.DeviceInfo;
using Authgear.Xamarin.Oauth;
using Xamarin.Essentials;

namespace Authgear.Xamarin
{
    public partial class AuthgearSdk
    {
        /// <summary>
        /// To prevent user from using expired access token, we have to check in advance
        ///whether it had expired and refresh it accordingly in refreshAccessTokenIfNeeded. If we
        /// use the expiry time in OidcTokenResponse directly to check for expiry, it is possible
        /// that the access token had passed the check but ends up being expired when it arrives at
        /// the server due to slow traffic or unfair scheduler.
        ///
        /// To compat this, we should consider the access token expired earlier than the expiry time
        /// calculated using OidcTokenResponse.expiresIn. Current implementation uses
        /// ExpireInPercentage of OidcTokenResponse.expiresIn to calculate the expiry time.
        /// </summary>
        private const float ExpireInPercentage = 0.9f;
        const string LoginHintFormat = "https://authgear.com/login_hint?type=app_session_token&app_session_token={0}";
        public string ClientId
        { get; private set; }
        public SessionState SessionState
        { get; private set; }
        public string? AccessToken
        { get; private set; }
        public string? IdTokenHint
        { get; private set; }
        public DateTimeOffset? AuthTime
        {
            get
            {
                var idToken = IdTokenHint;
                if (idToken == null) { return null; }
                var jsonDocument = Jwt.Decode(idToken);
                if (!jsonDocument.RootElement.TryGetProperty("auth_time", out var authTimeJsonValue)) { return null; };
                if (authTimeJsonValue.ValueKind != JsonValueKind.Number) { return null; }
                var authTime = authTimeJsonValue.GetInt64();
                return DateTimeOffset.FromUnixTimeSeconds(authTime);
            }
        }
        private DateTimeOffset? expiredAt;
        private readonly string authgearEndpoint;
        private readonly bool shareSessionWithSystemBrowser;
        private readonly ITokenStorage tokenStorage;
        private readonly IContainerStorage containerStorage;
        private readonly IOauthRepo oauthRepo;
        private readonly IKeyRepo keyRepo;
        private readonly IBiometric biometric;
        private readonly IWebView webView;
        private readonly string name;
        private bool isInitialized = false;
        private string? refreshToken = null;
        private Task? refreshAccessTokenTask = null;
        public event EventHandler<SessionStateChangeReason> SessionStateChange;
        private bool ShouldSuppressIDPSessionCookie
        {
            get { return !shareSessionWithSystemBrowser; }
        }
        public bool CanReauthenticate
        {
            get
            {
                var idToken = IdTokenHint;
                if (idToken == null) { return false; }
                var jsonDocument = Jwt.Decode(idToken);
                if (!jsonDocument.RootElement.TryGetProperty("https://authgear.com/claims/user/can_reauthenticate", out var can)) { return false; }
                return can.ValueKind == JsonValueKind.True;
            }
        }
        private readonly object tokenStateLock = new object();
        // These fields are configured in the platform's respective constructors
        // For brevity, they are not passed into this contructor. If it's proven that bugs are caused by some platform missing initializing
        // constructors, change the constructor to accept all those dependencies (think of it as each platform's constructor injecting
        // dependencies into this constructor)
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private AuthgearSdk(AuthgearOptions options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (options.ClientId == null)
            {
                throw new ArgumentNullException(nameof(options.ClientId));
            }
            if (options.AuthgearEndpoint == null)
            {
                throw new ArgumentNullException(nameof(options.AuthgearEndpoint));
            }
            ClientId = options.ClientId;
            authgearEndpoint = options.AuthgearEndpoint;
            shareSessionWithSystemBrowser = options.ShareSessionWithSystemBrowser;
            var httpClient = new HttpClient();
            tokenStorage = options.TokenStorage ?? new PersistentTokenStorage();
            name = options.Name ?? "default";
            containerStorage = new PersistentContainerStorage();
            oauthRepo = new OauthRepoHttp(httpClient, authgearEndpoint);
        }

        private void EnsureIsInitialized()
        {
            if (!isInitialized)
            {
                throw new InvalidOperationException("Authgear is not configured. Did you forget to call Configure?");
            }
        }
        public async Task ConfigureAsync()
        {
            isInitialized = true;
            var refreshToken = await tokenStorage.GetRefreshTokenAsync(name);
            this.refreshToken = refreshToken;
            if (refreshToken != null)
            {
                UpdateSessionState(SessionState.Authenticated, SessionStateChangeReason.FoundToken);
            }
            else
            {
                UpdateSessionState(SessionState.NoSession, SessionStateChangeReason.NoToken);
            }
        }

        private void UpdateSessionState(SessionState state, SessionStateChangeReason reason)
        {
            SessionState = state;
            SessionStateChange?.Invoke(this, reason);
        }

        public async Task<UserInfo> AuthenticateAnonymouslyAsync()
        {
            EnsureIsInitialized();
            var challengeResponse = await oauthRepo.OauthChallengeAsync("anonymous_request");
            var challenge = challengeResponse.Token!;
            var keyId = await containerStorage.GetAnonymousKeyIdAsync(name);
            var deviceInfo = PlatformGetDeviceInfo();
            var jwtResult = await keyRepo.GetOrCreateAnonymousJwtAsync(keyId, challenge, deviceInfo);
            keyId = jwtResult.KeyId;
            var jwt = jwtResult.Jwt;
            var tokenResponse = await oauthRepo.OidcTokenRequestAsync(new OidcTokenRequest(GrantType.Anonymous, ClientId, GetDeviceInfoString(deviceInfo))
            {
                Jwt = jwt
            });
            var userInfo = await oauthRepo.OidcUserInfoRequestAsync(tokenResponse.AccessToken!);
            SaveToken(tokenResponse, SessionStateChangeReason.Authenciated);
            await DisableBiometricAsync();
            containerStorage.SetAnonymousKeyId(name, keyId);
            return userInfo;
        }

        /// <summary>
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="AnonymousUserNotFoundException"></exception>
        /// <exception cref="TaskCanceledException"></exception>
        public async Task<UserInfo> PromoteAnonymousUserAsync(PromoteOptions options)
        {
            if (options.RedirectUri == null)
            {
                throw new ArgumentNullException(nameof(options.RedirectUri));
            }
            EnsureIsInitialized();
            var keyId = (await containerStorage.GetAnonymousKeyIdAsync(name)) ?? throw new AnonymousUserNotFoundException();
            var challengeResponse = await oauthRepo.OauthChallengeAsync("anonymous_request");
            var challenge = challengeResponse.Token!;
            var jwt = await keyRepo.PromoteAnonymousUserAsync(keyId, challenge, PlatformGetDeviceInfo());
            var jwtValue = WebUtility.UrlEncode(jwt);
            var loginHint = $"https://authgear.com/login_hint?type=anonymous&jwt={jwtValue}";
            var codeVerifier = new CodeVerifier(new RNGCryptoServiceProvider());
            var request = options.ToRequest(loginHint, ShouldSuppressIDPSessionCookie);
            var authorizeUrl = await GetAuthorizeEndpointAsync(request, codeVerifier);
            var deepLink = await OpenAuthorizeUrlAsync(options.RedirectUri, authorizeUrl);
            var result = await FinishAuthenticationAsync(deepLink, codeVerifier.Verifier);
            containerStorage.DeleteAnonymousKeyId(name);
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="options"></param>
        /// <exception cref="TaskCanceledException"></exception>
        /// <returns></returns>
        public async Task<UserInfo> AuthenticateAsync(AuthenticateOptions options)
        {
            EnsureIsInitialized();
            var codeVerifier = new CodeVerifier(new RNGCryptoServiceProvider());
            var request = options.ToRequest(ShouldSuppressIDPSessionCookie);
            var authorizeUrl = await GetAuthorizeEndpointAsync(request, codeVerifier);
            var deepLink = await OpenAuthorizeUrlAsync(request.RedirectUri, authorizeUrl);
            return await FinishAuthenticationAsync(deepLink, codeVerifier.Verifier);
        }

        /// <summary>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="biometricOptions"></param>
        /// <returns></returns>
        /// <exception cref="AuthgearException"></exception>
        /// <exception cref="TaskCanceledException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        public async Task<UserInfo> ReauthenticateAsync(ReauthenticateOptions options, BiometricOptions biometricOptions)
        {
            EnsureIsInitialized();
            if (await GetIsBiometricEnabledAsync() && biometricOptions != null)
            {
                return await AuthenticateBiometricAsync(biometricOptions);
            }
            if (!CanReauthenticate)
            {
                throw new AuthgearException("CanReauthenticate is false");
            }
            var idTokenHint = IdTokenHint;
            if (idTokenHint == null)
            {
                throw new AuthgearException("Call refreshIdToken first");
            }
            var codeVerifier = new CodeVerifier(new RNGCryptoServiceProvider());
            var request = options.ToRequest(idTokenHint, ShouldSuppressIDPSessionCookie);
            var authorizeUrl = await GetAuthorizeEndpointAsync(request, codeVerifier);
            var deepLink = await OpenAuthorizeUrlAsync(request.RedirectUri, authorizeUrl);
            return await FinishReauthenticationAsync(deepLink, codeVerifier.Verifier);
        }

        public async Task LogoutAsync(bool? force = null)
        {
            EnsureIsInitialized();
            try
            {
                var refreshToken = await tokenStorage.GetRefreshTokenAsync(name) ?? "";
                await oauthRepo.OidcRevocationRequestAsync(refreshToken);
            }
            catch (Exception)
            {
                if (force != true)
                {
                    throw;
                }
            }
            ClearSession(SessionStateChangeReason.Logout);
        }

        public async Task OpenUrlAsync(string path, SettingsOptions? options = null)
        {
            EnsureIsInitialized();
            var refreshToken = await tokenStorage.GetRefreshTokenAsync(name) ?? "";
            var appSessionTokenResponse = await oauthRepo.OauthAppSessionTokenAsync(refreshToken);
            var token = appSessionTokenResponse.AppSessionToken;

            var query = new Dictionary<string, string>();
            var builder = new UriBuilder(new Uri(authgearEndpoint))
            {
                Path = path
            };
            if (options != null)
            {
                if (options.ColorScheme != null)
                {
                    query["x_color_scheme"] = options.ColorScheme.GetDescription();
                }
                if (options.UiLocales != null)
                {
                    query["ui_locales"] = string.Join(" ", options.UiLocales);
                }
            }
            builder.Query = query.ToQueryParameter();
            var url = builder.Uri;

            var loginHint = string.Format(LoginHintFormat, WebUtility.UrlEncode(token));
            if (options == null) { options = new SettingsOptions(); }
            var request = options.ToRequest(url.ToString(), loginHint, ShouldSuppressIDPSessionCookie);
            var authorizeUrl = await GetAuthorizeEndpointAsync(request, null);
            await webView.ShowAsync(authorizeUrl);
        }

        public async Task OpenAsync(SettingsPage page, SettingsOptions? options = null)
        {
            await OpenUrlAsync(page.GetDescription(), options);
        }

        private bool ShouldRefreshAccessToken
        {
            get
            {
                if (refreshToken == null) return false;
                if (AccessToken == null) return true;
                var expireAt = this.expiredAt;
                if (expiredAt == null) return true;
                var now = DateTimeOffset.UtcNow;
                if (expireAt < now) return true;
                return false;
            }
        }

        private async Task RefreshAccessTokenAsync()
        {
            var taskSource = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
            var existingTask = Interlocked.CompareExchange(ref refreshAccessTokenTask, taskSource.Task, null);
            if (existingTask == null)
            {
                try
                {
                    await DoRefreshAccessTokenAsync();
                    taskSource.SetResult(null);
                }
                catch (Exception ex)
                {
                    taskSource.SetException(ex);
                    throw;
                }
                finally
                {
                    refreshAccessTokenTask = null;
                }
            }
            else
            {
                // Shouldn't need to, just in case.
                taskSource.SetCanceled();
                await existingTask;
            }
        }

        private async Task DoRefreshAccessTokenAsync()
        {
            var refreshToken = await tokenStorage.GetRefreshTokenAsync(name);
            if (refreshToken == null)
            {
                // Somehow we are asked to refresh access token but we don't have the refresh token.
                // Something went wrong, clear session.
                ClearSession(SessionStateChangeReason.NoToken);
                return;
            }
            try
            {
                var tokenResponse = await oauthRepo.OidcTokenRequestAsync(
                    new OidcTokenRequest(GrantType.RefreshToken, ClientId, GetDeviceInfoString())
                    {
                        RefreshToken = refreshToken
                    });
                SaveToken(tokenResponse, SessionStateChangeReason.FoundToken);
            }
            catch (Exception ex)
            {
                if (ex is OauthException)
                {
                    var oauthEx = ex as OauthException;
                    if (oauthEx?.Error == "invalid_grant")
                    {
                        ClearSession(SessionStateChangeReason.Invalid);
                        return;
                    }
                }
                throw;
            }
        }

        private async Task<string> GetAuthorizeEndpointAsync(OidcAuthenticationRequest request, CodeVerifier? codeVerifier)
        {
            var config = await oauthRepo.GetOidcConfigurationAsync();
            var query = request.ToQuery(ClientId, codeVerifier);
            return $"{config.AuthorizationEndpoint}?{query.ToQueryParameter()}";
        }

        /// <summary>
        /// </summary>
        /// <param name="redirectUrl"></param>
        /// <param name="authorizeUrl"></param>
        /// <exception cref="TaskCanceledException"></exception>
        /// <returns>Redirect URI with query parameters</returns>
        private async Task<string> OpenAuthorizeUrlAsync(string redirectUrl, string authorizeUrl)
        {
            // WebAuthenticator abstracts the uri for us but we need the uri in FinishAuthorization.
            // Substitute the uri for now.
            var result = await WebAuthenticator.AuthenticateAsync(new WebAuthenticatorOptions
            {
                Url = new Uri(authorizeUrl),
                CallbackUrl = new Uri(redirectUrl),
                PrefersEphemeralWebBrowserSession = !shareSessionWithSystemBrowser
            });
            var builder = new UriBuilder(redirectUrl)
            {
                Query = result.Properties.ToQueryParameter()
            };
            return builder.ToString();
        }

        private async Task<(UserInfo userInfo, OidcTokenResponse tokenResponse, string state)> ParseDeepLinkAndGetUserAsync(string deepLink, string codeVerifier)
        {
            var uri = new Uri(deepLink);
            var path = uri.LocalPath == "/" ? "" : uri.LocalPath;
            var redirectUri = $"{uri.Scheme}://{uri.Authority}{path}";
            var query = uri.ParseQueryString();
            query.TryGetValue("state", out var state);
            query.TryGetValue("error", out var error);
            query.TryGetValue("error_description", out var errorDescription);
            query.TryGetValue("error_uri", out var errorUri);
            if (!string.IsNullOrEmpty(error))
            {
                throw new OauthException(error, errorDescription, state, errorUri);
            }
            query.TryGetValue("code", out var code);
            if (string.IsNullOrEmpty(code))
            {
                throw new OauthException("invalid_request", "Missing parameter: code", state, errorUri);
            }
            var tokenResponse = await oauthRepo.OidcTokenRequestAsync(new OidcTokenRequest(GrantType.AuthorizationCode, ClientId, GetDeviceInfoString())
            {
                Code = code,
                RedirectUri = redirectUri,
                CodeVerifier = codeVerifier ?? "",
            });
            var userInfo = await oauthRepo.OidcUserInfoRequestAsync(tokenResponse.AccessToken!);
            return (userInfo, tokenResponse, state);
        }

        private async Task<UserInfo> FinishAuthenticationAsync(string deepLink, string codeVerifier)
        {
            var (userInfo, tokenResponse, state) = await ParseDeepLinkAndGetUserAsync(deepLink, codeVerifier);
            SaveToken(tokenResponse, SessionStateChangeReason.Authenciated);
            await DisableBiometricAsync();
            return userInfo;
        }

        private async Task<UserInfo> FinishReauthenticationAsync(string deepLink, string codeVerifier)
        {
            var (userInfo, tokenResponse, _) = await ParseDeepLinkAndGetUserAsync(deepLink, codeVerifier);
            if (tokenResponse.IdToken != null)
            {
                IdTokenHint = tokenResponse.IdToken;
            }
            return userInfo;
        }

        public void EnsureBiometricIsSupported(BiometricOptions options)
        {
            EnsureIsInitialized();
            biometric.EnsureIsSupported(options);
        }

        public async Task<bool> GetIsBiometricEnabledAsync()
        {
            EnsureIsInitialized();
            var kid = await containerStorage.GetBiometricKeyIdAsync(name);
            if (kid == null) { return false; }
            return true;
        }

        public async Task DisableBiometricAsync()
        {
            EnsureIsInitialized();
            var kid = await containerStorage.GetBiometricKeyIdAsync(name);
            if (kid != null)
            {
                biometric.RemoveBiometric(kid);
                containerStorage.DeleteBiometricKeyId(name);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="UnauthenticatedUserException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        public async Task EnableBiometricAsync(BiometricOptions options)
        {
            EnsureIsInitialized();
            await RefreshAccessTokenIfNeededAsync();
            var accessToken = AccessToken ?? throw new UnauthenticatedUserException();
            var challengeResponse = await oauthRepo.OauthChallengeAsync("biometric_request");
            var challenge = challengeResponse.Token!;
            var result = await biometric.EnableBiometricAsync(options, challenge, PlatformGetDeviceInfo());
            await oauthRepo.BiometricSetupRequestAsync(accessToken, ClientId, result.Jwt);
            containerStorage.SetBiometricKeyId(name, result.Kid);
        }

        /// <summary>
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="BiometricPrivateKeyNotFoundException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        public async Task<UserInfo> AuthenticateBiometricAsync(BiometricOptions options)
        {
            EnsureIsInitialized();
            var kid = await containerStorage.GetBiometricKeyIdAsync(name) ?? throw new BiometricPrivateKeyNotFoundException();
            var challengeResponse = await oauthRepo.OauthChallengeAsync("biometric_request");
            var challenge = challengeResponse.Token!;
            try
            {
                var deviceInfo = PlatformGetDeviceInfo();
                var jwt = await biometric.AuthenticateBiometricAsync(options, kid, challenge, deviceInfo);
                try
                {
                    var tokenResponse = await oauthRepo.OidcTokenRequestAsync(new OidcTokenRequest(GrantType.Biometric, ClientId, GetDeviceInfoString())
                    {
                        Jwt = jwt
                    });
                    var userInfo = await oauthRepo.OidcUserInfoRequestAsync(tokenResponse.AccessToken!);
                    SaveToken(tokenResponse, SessionStateChangeReason.Authenciated);
                    return userInfo;
                }
                catch (OauthException ex)
                {
                    // In case the biometric was removed remotely.
                    if (ex.Error == "invalid_grant" && ex.ErrorDescription == "InvalidCredentials")
                    {
                        await DisableBiometricAsync();
                    }
                    throw;
                }
            }
            catch (BiometricPrivateKeyNotFoundException)
            {
                await DisableBiometricAsync();
                throw;
            }
            catch (Exception ex)
            {
                throw AuthgearException.Wrap(ex);
            }
        }

        private string GetDeviceInfoString()
        {
            return ConvertExtensions.ToBase64UrlSafeString(JsonSerializer.Serialize(PlatformGetDeviceInfo()), Encoding.UTF8);
        }

        private string GetDeviceInfoString(DeviceInfoRoot deviceInfo)
        {
            return ConvertExtensions.ToBase64UrlSafeString(JsonSerializer.Serialize(deviceInfo), Encoding.UTF8);
        }

        private void SaveToken(OidcTokenResponse tokenResponse, SessionStateChangeReason reason)
        {
            if (tokenResponse == null)
            {
                throw new ArgumentNullException(nameof(tokenResponse));
            }
            lock (tokenStateLock)
            {
                if (tokenResponse.AccessToken != null)
                {
                    AccessToken = tokenResponse.AccessToken;
                }
                if (tokenResponse.RefreshToken != null)
                {
                    refreshToken = tokenResponse.RefreshToken;
                }
                if (tokenResponse.IdToken != null)
                {
                    IdTokenHint = tokenResponse.IdToken;
                }
                if (tokenResponse.ExpiresIn != null)
                {
                    expiredAt = DateTimeOffset.UtcNow.AddMilliseconds(((float)tokenResponse.ExpiresIn * ExpireInPercentage));
                }
                UpdateSessionState(SessionState.Authenticated, reason);
            }
            if (tokenResponse.RefreshToken != null)
            {
                tokenStorage.SetRefreshToken(name, tokenResponse.RefreshToken);
            }
        }

        public async Task<UserInfo> FetchUserInfoAsync()
        {
            EnsureIsInitialized();
            await RefreshAccessTokenIfNeededAsync();
            var accessToken = AccessToken ?? throw new UnauthenticatedUserException();
            return await oauthRepo.OidcUserInfoRequestAsync(accessToken);
        }

        public async Task RefreshIdTokenAsync()
        {
            EnsureIsInitialized();
            await RefreshAccessTokenIfNeededAsync();
            var accessToken = AccessToken ?? throw new UnauthenticatedUserException();
            var tokenResponse = await oauthRepo.OidcTokenRequestAsync(new OidcTokenRequest(GrantType.IdToken, ClientId, GetDeviceInfoString())
            {
                AccessToken = accessToken,
            });
            if (tokenResponse.IdToken != null)
            {
                IdTokenHint = tokenResponse.IdToken;
            }
        }

        public async Task<string> RefreshAccessTokenIfNeededAsync()
        {
            EnsureIsInitialized();
            if (ShouldRefreshAccessToken)
            {
                await RefreshAccessTokenAsync();
            }
            return AccessToken!;
        }

        internal void ClearSession(SessionStateChangeReason reason)
        {
            tokenStorage.DeleteRefreshToken(name);
            lock (tokenStateLock)
            {
                AccessToken = null;
                refreshToken = null;
                IdTokenHint = null;
                expiredAt = null;
            }
            UpdateSessionState(SessionState.NoSession, reason);
        }

        public void ClearSessionState()
        {
            EnsureIsInitialized();
            ClearSession(SessionStateChangeReason.Clear);
        }
    }
}
