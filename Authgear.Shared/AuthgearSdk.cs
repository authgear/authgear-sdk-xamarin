using System;
using System.Collections.Generic;
using System.Globalization;
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
#if Xamarin
using Xamarin.Essentials;
#endif

namespace Authgear.Xamarin
{
    public sealed partial class AuthgearSdk : IDisposable
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
        private readonly bool isSsoEnabled;
        private readonly ITokenStorage tokenStorage;
        private readonly IContainerStorage containerStorage;
        private readonly IOauthRepo oauthRepo;
        private readonly List<IDisposable> disposables;
        private readonly IKeyRepo keyRepo;
        private readonly IBiometric biometric;
        private readonly IWebView webView;
        private readonly string name;
        private bool isInitialized;
        private string? refreshToken;
        private Task? refreshAccessTokenTask;
        public event EventHandler<SessionStateChangeReason> SessionStateChange;
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
            isSsoEnabled = options.IsSsoEnabled;
            var httpClient = new HttpClient();
            tokenStorage = options.TokenStorage ?? new PersistentTokenStorage();
            name = options.Name ?? "default";
            containerStorage = new PersistentContainerStorage();
            var oauthRepoHttp = new OauthRepoHttp(httpClient, authgearEndpoint);
            var oauthRepo = new OauthRepo(oauthRepoHttp);
            oauthRepo.ClearSessionCallback += (s, e) =>
            {
                this.ClearSession(e);
            };
            this.oauthRepo = oauthRepo;

            this.disposables = new List<IDisposable> { httpClient, oauthRepoHttp };
        }

        public void Dispose()
        {
            foreach (IDisposable d in this.disposables)
            {
                d.Dispose();
            }
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
            var refreshToken = await tokenStorage.GetRefreshTokenAsync(name).ConfigureAwait(false);
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
            MainThread.BeginInvokeOnMainThread(() =>
            {
                SessionStateChange?.Invoke(this, reason);
            });
        }

        public async Task<UserInfo> AuthenticateAnonymouslyAsync()
        {
            EnsureIsInitialized();
            var challengeResponse = await oauthRepo.OauthChallengeAsync("anonymous_request").ConfigureAwait(false);
            var challenge = challengeResponse.Token!;
            var keyId = await containerStorage.GetAnonymousKeyIdAsync(name).ConfigureAwait(false);
            var deviceInfo = PlatformGetDeviceInfo();
            var jwtResult = await keyRepo.GetOrCreateAnonymousJwtAsync(keyId, challenge, deviceInfo).ConfigureAwait(false);
            keyId = jwtResult.KeyId;
            var jwt = jwtResult.Jwt;
            var tokenResponse = await oauthRepo.OidcTokenRequestAsync(new OidcTokenRequest(GrantType.Anonymous, ClientId, GetDeviceInfoString(deviceInfo))
            {
                Jwt = jwt
            }).ConfigureAwait(false);
            var userInfo = await oauthRepo.OidcUserInfoRequestAsync(tokenResponse.AccessToken!).ConfigureAwait(false);
            SaveToken(tokenResponse, SessionStateChangeReason.Authenciated);
            await DisableBiometricAsync().ConfigureAwait(false);
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
            var keyId = (await containerStorage.GetAnonymousKeyIdAsync(name).ConfigureAwait(false)) ?? throw new AnonymousUserNotFoundException();
            var challengeResponse = await oauthRepo.OauthChallengeAsync("anonymous_request").ConfigureAwait(false);
            var challenge = challengeResponse.Token!;
            var jwt = await keyRepo.PromoteAnonymousUserAsync(keyId, challenge, PlatformGetDeviceInfo()).ConfigureAwait(false);
            var jwtValue = WebUtility.UrlEncode(jwt);
            var loginHint = $"https://authgear.com/login_hint?type=anonymous&jwt={jwtValue}";
            var codeVerifier = new CodeVerifier();
            var request = options.ToRequest(loginHint, isSsoEnabled);
            var authorizeUrl = await GetAuthorizeEndpointAsync(request, codeVerifier).ConfigureAwait(false);
            var deepLink = await OpenAuthorizeUrlAsync(options.RedirectUri, authorizeUrl).ConfigureAwait(false);
            var result = await FinishAuthenticationAsync(deepLink, codeVerifier.Verifier).ConfigureAwait(false);
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
            var codeVerifier = new CodeVerifier();
            var request = options.ToRequest(isSsoEnabled);
            var authorizeUrl = await GetAuthorizeEndpointAsync(request, codeVerifier).ConfigureAwait(false);
            var deepLink = await OpenAuthorizeUrlAsync(request.RedirectUri, authorizeUrl).ConfigureAwait(false);
            return await FinishAuthenticationAsync(deepLink, codeVerifier.Verifier).ConfigureAwait(false);
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
            if (await GetIsBiometricEnabledAsync().ConfigureAwait(false) && biometricOptions != null)
            {
                return await AuthenticateBiometricAsync(biometricOptions).ConfigureAwait(false);
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
            var codeVerifier = new CodeVerifier();
            var request = options.ToRequest(idTokenHint, isSsoEnabled);
            var authorizeUrl = await GetAuthorizeEndpointAsync(request, codeVerifier).ConfigureAwait(false);
            var deepLink = await OpenAuthorizeUrlAsync(request.RedirectUri, authorizeUrl).ConfigureAwait(false);
            return await FinishReauthenticationAsync(deepLink, codeVerifier.Verifier).ConfigureAwait(false);
        }

        public async Task LogoutAsync(bool? force = null)
        {
            EnsureIsInitialized();
            try
            {
                var refreshToken = await tokenStorage.GetRefreshTokenAsync(name).ConfigureAwait(false) ?? "";
                await oauthRepo.OidcRevocationRequestAsync(refreshToken).ConfigureAwait(false);
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
            var refreshToken = await tokenStorage.GetRefreshTokenAsync(name).ConfigureAwait(false) ?? "";
            var appSessionTokenResponse = await oauthRepo.OauthAppSessionTokenAsync(refreshToken).ConfigureAwait(false);
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

            var loginHint = string.Format(CultureInfo.InvariantCulture, LoginHintFormat, WebUtility.UrlEncode(token));
            if (options == null) { options = new SettingsOptions(); }
            var request = options.ToRequest(url.ToString(), loginHint, isSsoEnabled);
            var authorizeUrl = await GetAuthorizeEndpointAsync(request, null).ConfigureAwait(false);
            await webView.ShowAsync(authorizeUrl).ConfigureAwait(false);
        }

        public async Task OpenAsync(SettingsPage page, SettingsOptions? options = null)
        {
            await OpenUrlAsync(page.GetDescription(), options).ConfigureAwait(false);
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
                    await DoRefreshAccessTokenAsync().ConfigureAwait(false);
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
                await existingTask.ConfigureAwait(false);
            }
        }

        private async Task DoRefreshAccessTokenAsync()
        {
            var refreshToken = await tokenStorage.GetRefreshTokenAsync(name).ConfigureAwait(false);
            if (refreshToken == null)
            {
                // Somehow we are asked to refresh access token but we don't have the refresh token.
                // Something went wrong, clear session.
                ClearSession(SessionStateChangeReason.NoToken);
                return;
            }
            var tokenResponse = await oauthRepo.OidcTokenRequestAsync(
                new OidcTokenRequest(GrantType.RefreshToken, ClientId, GetDeviceInfoString())
                {
                    RefreshToken = refreshToken
                }).ConfigureAwait(false);
            SaveToken(tokenResponse, SessionStateChangeReason.FoundToken);
        }

        private async Task<string> GetAuthorizeEndpointAsync(OidcAuthenticationRequest request, CodeVerifier? codeVerifier)
        {
            var config = await oauthRepo.GetOidcConfigurationAsync().ConfigureAwait(false);
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
            return await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var result = await WebAuthenticator.AuthenticateAsync(new WebAuthenticatorOptions
                {
                    Url = new Uri(authorizeUrl),
                    CallbackUrl = new Uri(redirectUrl),
                    PrefersEphemeralWebBrowserSession = !isSsoEnabled
                }).ConfigureAwait(false);
                // WebAuthenticator abstracts the redirect uri (WebAuthenticatorResult) for us but we need the uri in FinishAuthorization.
                // Substitute the uri for now.
                var builder = new UriBuilder(redirectUrl)
                {
                    Query = result.Properties.ToQueryParameter()
                };
                return builder.ToString();
            }).ConfigureAwait(false);
        }

        private async Task<(UserInfo userInfo, OidcTokenResponse tokenResponse)> ParseDeepLinkAndGetUserAsync(string deepLink, string codeVerifier)
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
            }).ConfigureAwait(false);
            var userInfo = await oauthRepo.OidcUserInfoRequestAsync(tokenResponse.AccessToken!).ConfigureAwait(false);
            return (userInfo, tokenResponse);
        }

        private async Task<UserInfo> FinishAuthenticationAsync(string deepLink, string codeVerifier)
        {
            var (userInfo, tokenResponse) = await ParseDeepLinkAndGetUserAsync(deepLink, codeVerifier).ConfigureAwait(false);
            SaveToken(tokenResponse, SessionStateChangeReason.Authenciated);
            await DisableBiometricAsync().ConfigureAwait(false);
            return userInfo;
        }

        private async Task<UserInfo> FinishReauthenticationAsync(string deepLink, string codeVerifier)
        {
            var (userInfo, tokenResponse) = await ParseDeepLinkAndGetUserAsync(deepLink, codeVerifier).ConfigureAwait(false);
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
            var kid = await containerStorage.GetBiometricKeyIdAsync(name).ConfigureAwait(false);
            if (kid == null) { return false; }
            return true;
        }

        public async Task DisableBiometricAsync()
        {
            EnsureIsInitialized();
            var kid = await containerStorage.GetBiometricKeyIdAsync(name).ConfigureAwait(false);
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
            await RefreshAccessTokenIfNeededAsync().ConfigureAwait(false);
            var accessToken = AccessToken ?? throw new UnauthenticatedUserException();
            var challengeResponse = await oauthRepo.OauthChallengeAsync("biometric_request").ConfigureAwait(false);
            var challenge = challengeResponse.Token!;
            var result = await biometric.EnableBiometricAsync(options, challenge, PlatformGetDeviceInfo()).ConfigureAwait(false);
            await oauthRepo.BiometricSetupRequestAsync(accessToken, ClientId, result.Jwt).ConfigureAwait(false);
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
            var kid = await containerStorage.GetBiometricKeyIdAsync(name).ConfigureAwait(false) ?? throw new BiometricPrivateKeyNotFoundException();
            var challengeResponse = await oauthRepo.OauthChallengeAsync("biometric_request").ConfigureAwait(false);
            var challenge = challengeResponse.Token!;
            try
            {
                var deviceInfo = PlatformGetDeviceInfo();
                var jwt = await biometric.AuthenticateBiometricAsync(options, kid, challenge, deviceInfo).ConfigureAwait(false);
                try
                {
                    var tokenResponse = await oauthRepo.OidcTokenRequestAsync(new OidcTokenRequest(GrantType.Biometric, ClientId, GetDeviceInfoString())
                    {
                        Jwt = jwt
                    }).ConfigureAwait(false);
                    var userInfo = await oauthRepo.OidcUserInfoRequestAsync(tokenResponse.AccessToken!).ConfigureAwait(false);
                    SaveToken(tokenResponse, SessionStateChangeReason.Authenciated);
                    return userInfo;
                }
                catch (OauthException ex)
                {
                    // In case the biometric was removed remotely.
                    if (ex.Error == "invalid_grant" && ex.ErrorDescription == "InvalidCredentials")
                    {
                        await DisableBiometricAsync().ConfigureAwait(false);
                    }
                    throw;
                }
            }
            catch (BiometricPrivateKeyNotFoundException)
            {
                await DisableBiometricAsync().ConfigureAwait(false);
                throw;
            }
            catch (Exception ex)
            {
                throw AuthgearException.Wrap(ex);
            }
        }

        private string GetDeviceInfoString()
        {
            return GetDeviceInfoString(PlatformGetDeviceInfo());
        }

        private static string GetDeviceInfoString(DeviceInfoRoot deviceInfo)
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
                    expiredAt = DateTimeOffset.UtcNow.AddSeconds(((float)tokenResponse.ExpiresIn * ExpireInPercentage));
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
            await RefreshAccessTokenIfNeededAsync().ConfigureAwait(false);
            var accessToken = AccessToken ?? throw new UnauthenticatedUserException();
            return await oauthRepo.OidcUserInfoRequestAsync(accessToken).ConfigureAwait(false);
        }

        public async Task RefreshIdTokenAsync()
        {
            EnsureIsInitialized();
            await RefreshAccessTokenIfNeededAsync().ConfigureAwait(false);
            var accessToken = AccessToken ?? throw new UnauthenticatedUserException();
            var tokenResponse = await oauthRepo.OidcTokenRequestAsync(new OidcTokenRequest(GrantType.IdToken, ClientId, GetDeviceInfoString())
            {
                AccessToken = accessToken,
            }).ConfigureAwait(false);
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
                await RefreshAccessTokenAsync().ConfigureAwait(false);
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
