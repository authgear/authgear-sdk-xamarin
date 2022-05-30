using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Authgear.Xamarin.CsExtensions;
using Authgear.Xamarin.Oauth;

namespace Authgear.Xamarin.Data.Oauth
{
    internal class OauthRepoHttp : IOauthRepo, IDisposable
    {
        private readonly HttpClient httpClient;
        public string Endpoint { get; }

        private OidcConfiguration? config;

        private readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        public OauthRepoHttp(HttpClient client, string endpoint)
        {
            httpClient = client;
            Endpoint = endpoint;
        }

        public async Task<OidcConfiguration> GetOidcConfigurationAsync()
        {
            try
            {
                locker.EnterReadLock();
                if (config != null) return config;
            }
            finally { locker.ExitReadLock(); }
            // Double-checked locking
            try
            {
                locker.EnterWriteLock();
                var configAfterLock = config;
                if (configAfterLock != null) return configAfterLock;
                var responseMessage = await httpClient.GetAsync(new Uri(new Uri(Endpoint), "/.well-known/openid-configuration")).ConfigureAwait(false);
                config = await responseMessage.GetJsonAsync<OidcConfiguration>().ConfigureAwait(false);
                return config;
            }
            finally { locker.ExitWriteLock(); }
        }

        public async Task BiometricSetupRequestAsync(string accessToken, string clientId, string jwt)
        {
            var config = await GetOidcConfigurationAsync().ConfigureAwait(false);
            var body = new Dictionary<string, string>()
            {
                ["client_id"] = clientId,
                ["grant_type"] = GrantType.Biometric.GetDescription(),
                ["jwt"] = jwt
            };
            var content = new FormUrlEncodedContent(body);
            using var request = new HttpRequestMessage(HttpMethod.Post, config.TokenEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = content;
            var responseMessage = await httpClient.SendAsync(request).ConfigureAwait(false);
            await responseMessage.EnsureSuccessOrAuthgearExceptionAsync().ConfigureAwait(false);
        }

        public async Task<AppSessionTokenResponse> OauthAppSessionTokenAsync(string refreshToken)
        {
            var body = new Dictionary<string, string>()
            {
                ["refresh_token"] = refreshToken,
            };
            using var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var responseMessage = await httpClient.PostAsync(new Uri(new Uri(Endpoint), "/oauth2/app_session_token"), content).ConfigureAwait(false);
            var result = await responseMessage.GetJsonAsync<AppSessionTokenResponseResult>().ConfigureAwait(false);
            return result.Result!;
        }

        public async Task<ChallengeResponse> OauthChallengeAsync(string purpose)
        {
            var body = new Dictionary<string, string>
            {
                ["purpose"] = purpose
            };
            using var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var responseMessage = await httpClient.PostAsync(new Uri(new Uri(Endpoint), "/oauth2/challenge"), content).ConfigureAwait(false);
            var result = await responseMessage.GetJsonAsync<ChallengeResponseResult>().ConfigureAwait(false);
            return result.Result!;
        }

        public async Task OidcRevocationRequestAsync(string refreshToken)
        {
            var config = await GetOidcConfigurationAsync().ConfigureAwait(false);
            var body = new Dictionary<string, string>()
            {
                ["token"] = refreshToken
            };
            using var content = new FormUrlEncodedContent(body);
            var responseMessage = await httpClient.PostAsync(new Uri(config.RevocationEndpoint), content).ConfigureAwait(false);
            await responseMessage.EnsureSuccessOrAuthgearExceptionAsync().ConfigureAwait(false);
        }

        public async Task<OidcTokenResponse> OidcTokenRequestAsync(OidcTokenRequest request)
        {
            var config = await GetOidcConfigurationAsync().ConfigureAwait(false);
            var body = new Dictionary<string, string>()
            {
                ["grant_type"] = request.GrantType.GetDescription(),
                ["client_id"] = request.ClientId,
                ["x_device_info"] = request.XDeviceInfo,
            };
            if (request.RedirectUri != null)
            {
                body["redirect_uri"] = request.RedirectUri;
            }
            if (request.Code != null)
            {
                body["code"] = request.Code;
            }
            if (request.CodeVerifier != null)
            {
                body["code_verifier"] = request.CodeVerifier;
            }
            if (request.RefreshToken != null)
            {
                body["refresh_token"] = request.RefreshToken;
            }
            if (request.Jwt != null)
            {
                body["jwt"] = request.Jwt;
            }
            using var content = new FormUrlEncodedContent(body);
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, config.TokenEndpoint);
            if (request.AccessToken != null)
            {
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.AccessToken);
            }
            httpRequest.Content = content;
            var responseMessage = await httpClient.SendAsync(httpRequest).ConfigureAwait(false);
            return await responseMessage.GetJsonAsync<OidcTokenResponse>().ConfigureAwait(false);
        }

        public async Task<UserInfo> OidcUserInfoRequestAsync(string accessToken)
        {
            var config = await GetOidcConfigurationAsync().ConfigureAwait(false);
            using var request = new HttpRequestMessage(HttpMethod.Get, config.UserInfoEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var responseMessage = await httpClient.SendAsync(request).ConfigureAwait(false);
            return await responseMessage.GetJsonAsync<UserInfo>().ConfigureAwait(false);
        }

        public void Dispose()
        {
            locker.Dispose();
        }
    }
}
