using Authgear.Xamarin.CsExtensions;
using Authgear.Xamarin.Data;
using Authgear.Xamarin.Data.Oauth;
using Authgear.Xamarin.Oauth;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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
        public string ClientId
        { get; private set; }
        public SessionState SessionState
        { get; private set; }
        public string AccessToken
        { get; private set; }
        public string IdToken
        { get; private set; }
        private DateTime? expiredAt;
        private readonly string authgearEndpoint;
        private readonly bool shareSessionWithSystemBrowser;
        private readonly ITokenStorage tokenStorage;
        private readonly IContainerStorage containerStorage;
        private readonly IOauthRepo oauthRepo;
        private readonly IKeyRepo keyRepo;
        private readonly IBiometric biometric;
        private readonly string name;
        private bool isInitialized = false;
        private string refreshToken = null;
        private Task refreshAccessTokenTask = null;
        public event EventHandler<SessionStateChangeReason> SessionStateChange;
        private bool ShouldSuppressIDPSessionCookie
        {
            get { return !shareSessionWithSystemBrowser; }
        }
        private readonly object tokenStateLock = new object();
        private AuthgearSdk(AuthgearOptions options)
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
            tokenStorage = options.TokenStorage ?? new PersistentTokenStorage();
            name = options.Name ?? "default";
            containerStorage = new PersistentContainerStorage();
            oauthRepo = new OauthRepoHttp
            {
                Endpoint = authgearEndpoint
            };
            keyRepo = new KeyRepoPlatformStore();
        }

        private void EnsureIsInitialized()
        {
            if (!isInitialized)
            {
                throw new InvalidOperationException("Authgear is not configured. Did you forget to call Configure?");
            }
        }
        public async Task Configure()
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

        public async Task<AuthorizeResult> AuthorizeAsync(AuthorizeOptions options)
        {
            EnsureIsInitialized();
            var codeVerifier = SetupVerifier();
            var request = options.ToRequest(ShouldSuppressIDPSessionCookie);
            var authorizeUrl = await AuthorizeEndpoint(request, codeVerifier);
            var deepLink = await OpenAuthorizeUrlAsync(request.RedirectUri, authorizeUrl);
            return await FinishAuthorizationAsync(deepLink);
        }

        public async Task LogoutAsync(bool? force = null)
        {
            EnsureIsInitialized();
            try
            {
                var refreshToken = await tokenStorage.GetRefreshTokenAsync(name) ?? "";
                await oauthRepo.OidcRevocationRequest(refreshToken);
            }
            catch (Exception e)
            {
                if (force != true)
                {
                    throw e;
                }
            }
            ClearSession(SessionStateChangeReason.Logout);
        }

        private VerifierHolder SetupVerifier()
        {
            var verifier = GenerateCodeVerifier();
            containerStorage.SetOidcCodeVerifier(name, verifier);
            return new VerifierHolder { Verifier = verifier, Challenge = ComputeCodeChallenge(verifier) };
        }

        private string GenerateCodeVerifier()
        {
            const int byteCount = 32;
            var bytes = new Byte[byteCount];
            using (var provider = new RNGCryptoServiceProvider())
            {
                provider.GetBytes(bytes);
                return string.Join("", bytes.Select(x => x.ToString("x2")));
            }
        }

        private string ComputeCodeChallenge(string verifier)
        {
            var hash = Sha256(verifier);
            return ConvertExtensions.ToBase64UrlSafeString(hash);
        }

        private byte[] Sha256(string input)
        {
            var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        }

        private bool ShouldRefreshAccessToken
        {
            get
            {
                if (refreshToken == null) return false;
                if (AccessToken == null) return true;
                var expireAt = this.expiredAt;
                if (expiredAt == null) return true;
                var now = DateTime.Now;
                if (expireAt < now) return true;
                return false;
            }
        }

        private async Task RefreshAccessToken()
        {
            var taskSource = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            var existingTask = Interlocked.CompareExchange(ref refreshAccessTokenTask, taskSource.Task, null);
            var isExchanged = existingTask == null;
            if (isExchanged)
            {
                try
                {
                    await DoRefreshAccessToken();
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

        private async Task DoRefreshAccessToken()
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
                var tokenResponse = await oauthRepo.OidcTokenRequest(
                    new OidcTokenRequest
                    {
                        GrantType = GrantType.RefreshToken,
                        ClientId = ClientId,
                        XDeviceInfo = GetDeviceInfoString(),
                        RefreshToken = refreshToken
                    });
                SaveToken(tokenResponse, SessionStateChangeReason.FoundToken);
            }
            catch (Exception ex)
            {
                if (ex is OauthException)
                {
                    var oauthEx = ex as OauthException;
                    if (oauthEx.Error == "invalid_grant")
                    {
                        ClearSession(SessionStateChangeReason.Invalid);
                        return;
                    }
                }
                throw ex;
            }
        }

        private async Task<string> AuthorizeEndpoint(OidcAuthenticationRequest request, VerifierHolder codeVerifier)
        {
            var config = await oauthRepo.OidcConfiguration();
            var query = request.ToQuery(ClientId, codeVerifier);
            return $"{config.AuthorizationEndpoint}?{query.ToQueryParameter()}";
        }

        private async Task<string> OpenAuthorizeUrlAsync(string redirectUrl, string authorizeUrl)
        {
            // WebAuthenticator abstracts the uri for us but we need the uri in FinishAuthorization.
            // Substitute the uri for now.
            var result = await WebAuthenticator.AuthenticateAsync(new Uri(authorizeUrl), new Uri(redirectUrl));
            var builder = new UriBuilder(redirectUrl)
            {
                Query = result.Properties.ToQueryParameter()
            };
            return builder.ToString();
        }

        private async Task<AuthorizeResult> FinishAuthorizationAsync(string deepLink)
        {
            var uri = new Uri(deepLink);
            var path = uri.LocalPath == "/" ? "" : uri.LocalPath;
            var redirectUri = $"{uri.Scheme}://{uri.Authority}{path}";
            var query = uri.ParseQueryString();
            query.TryGetValue("state", out var state);
            query.TryGetValue("error", out var error);
            query.TryGetValue("error_description", out var errorDescription);
            query.TryGetValue("error_uri", out var errorUri);
            if (error != null)
            {
                throw new OauthException(error, errorDescription, state, errorUri);
            }
            query.TryGetValue("code", out var code);
            if (code == null)
            {
                throw new OauthException("invalid_request", "Missing parameter: code", state, errorUri);
            }
            var codeVerifier = await containerStorage.GetOidcCodeVerifier(name);
            var tokenResponse = await oauthRepo.OidcTokenRequest(new OidcTokenRequest
            {
                GrantType = GrantType.AuthorizationCode,
                ClientId = ClientId,
                XDeviceInfo = GetDeviceInfoString(),
                Code = code,
                RedirectUri = redirectUri,
                CodeVerifier = codeVerifier ?? "",
            });
            var userInfo = await oauthRepo.OidcUserInfoRequest(tokenResponse.AccessToken);
            SaveToken(tokenResponse, SessionStateChangeReason.Authenciated);
            await DisableBiometricAsync();
            return new AuthorizeResult { UserInfo = userInfo, State = state };
        }

        public void EnsureBiometricIsSupported(BiometricOptions options)
        {
            EnsureIsInitialized();
            biometric.EnsureIsSupported(options);
        }

        public async Task<bool> IsBiometricEnabled()
        {
            EnsureIsInitialized();
            var kid = await containerStorage.GetBiometricKeyId(name);
            if (kid == null) { return false; }
            return true;
        }

        public async Task DisableBiometricAsync()
        {
            EnsureIsInitialized();
            var kid = await containerStorage.GetBiometricKeyId(name);
            if (kid != null)
            {
                biometric.RemoveBiometric(name);
                containerStorage.DeleteBiometricKeyId(name);
            }
        }

        public async Task EnableBiometricAsync(BiometricOptions options)
        {
            EnsureIsInitialized();
            await RefreshAccessTokenIfNeeded();
            var accessToken = AccessToken ?? throw new UnauthenticatedUserException();
            var challengeResponse = await oauthRepo.OauthChallenge("biometric_request");
            var challenge = challengeResponse.Token;
            var result = await biometric.EnableBiometric(options, challenge, PlatformGetDeviceInfo());
            await oauthRepo.BiometricSetupRequest(accessToken, ClientId, result.Jwt);
            containerStorage.SetBiometricKeyId(name, result.Kid);
        }

        public async Task<UserInfo> AuthenticateBiometricAsync(BiometricOptions options)
        {
            EnsureIsInitialized();
            var kid = await containerStorage.GetBiometricKeyId(name) ?? throw new BiometricPrivateKeyNotFoundException();
            var challengeResponse = await oauthRepo.OauthChallenge("biometric_request");
            var challenge = challengeResponse.Token;
            try
            {
                var deviceInfo = PlatformGetDeviceInfo();
                var jwt = await biometric.AuthenticateBiometricAsync(options, kid, challenge, deviceInfo);
                try
                {
                    var tokenResponse = await oauthRepo.OidcTokenRequest(new OidcTokenRequest
                    {
                        GrantType = GrantType.Biometric,
                        ClientId = ClientId,
                        XDeviceInfo = GetDeviceInfoString(deviceInfo),
                        Jwt = jwt
                    });
                    var userInfo = await oauthRepo.OidcUserInfoRequest(tokenResponse.AccessToken);
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
                    IdToken = tokenResponse.IdToken;
                }
                if (tokenResponse.ExpiresIn != null)
                {
                    expiredAt = DateTime.Now.AddMilliseconds(((float)tokenResponse.ExpiresIn * ExpireInPercentage));
                }
                UpdateSessionState(SessionState.Authenticated, reason);
            }
            if (tokenResponse.RefreshToken != null)
            {
                tokenStorage.SetRefreshToken(name, tokenResponse.RefreshToken);
            }
        }

        public async Task<string> RefreshAccessTokenIfNeeded()
        {
            EnsureIsInitialized();
            if (ShouldRefreshAccessToken)
            {
                await RefreshAccessToken();
            }
            return AccessToken;
        }

        public void ClearSession(SessionStateChangeReason reason)
        {
            tokenStorage.DeleteRefreshToken(name);
            lock (tokenStateLock)
            {
                AccessToken = null;
                refreshToken = null;
                IdToken = null;
                expiredAt = null;
            }
            UpdateSessionState(SessionState.NoSession, reason);
        }
    }
}
