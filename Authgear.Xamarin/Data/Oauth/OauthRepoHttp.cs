using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Authgear.Xamarin.CsExtensions;
using Authgear.Xamarin.Oauth;

namespace Authgear.Xamarin.Data.Oauth
{
    internal class OauthRepoHttp : IOauthRepo
    {

        public string Endpoint { get; set; }

        private OidcConfiguration config;

        private readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

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
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(Endpoint)
                };
                var responseMessage = await client.GetAsync("/.well-known/openid-configuration");
                config = await responseMessage.GetJsonAsync<OidcConfiguration>();
                return config;
            }
            finally { locker.ExitWriteLock(); }
        }

        public async Task BiometricSetupRequestAsync(string accessToken, string clientId, string jwt)
        {
            var config = await GetOidcConfigurationAsync();
            var body = new Dictionary<string, string>()
            {
                ["client_id"] = clientId,
                ["grant_type"] = GrantType.Biometric.GetDescription(),
                ["jwt"] = jwt
            };
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("authorization", $"Bearer {accessToken}");
            var content = new FormUrlEncodedContent(body);
            var responseMessage = await client.PostAsync(config.TokenEndpoint, content);
            await responseMessage.EnsureSuccessOrAuthgearExceptionAsync();
        }

        public async Task<AppSessionTokenResponse> OauthAppSessionTokenAsync(string refreshToken)
        {
            var body = new Dictionary<string, string>()
            {
                ["refresh_token"] = refreshToken,
            };
            var client = new HttpClient()
            {
                BaseAddress = new Uri(Endpoint)
            };
            var content = new StringContent(AuthgearJson.Serialize(body), Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync("/oauth2/app_session_token", content);
            var result = await responseMessage.GetJsonAsync<AppSessionTokenResponseResult>();
            return result.Result;
        }

        public async Task<ChallengeResponse> OauthChallengeAsync(string purpose)
        {
            var body = new Dictionary<string, string>
            {
                ["purpose"] = purpose
            };
            var client = new HttpClient()
            {
                BaseAddress = new Uri(Endpoint)
            };
            var content = new StringContent(AuthgearJson.Serialize(body), Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync("/oauth2/challenge", content);
            var result = await responseMessage.GetJsonAsync<ChallengeResponseResult>();
            return result.Result;
        }

        public async Task OidcRevocationRequestAsync(string refreshToken)
        {
            var config = await GetOidcConfigurationAsync();
            var body = new Dictionary<string, string>()
            {
                ["token"] = refreshToken
            };
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(body);
            var responseMessage = await client.PostAsync(config.RevocationEndpoint, content);
            await responseMessage.EnsureSuccessOrAuthgearExceptionAsync();
        }

        public async Task<OidcTokenResponse> OidcTokenRequestAsync(OidcTokenRequest request)
        {
            var config = await GetOidcConfigurationAsync();
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
            var client = new HttpClient();
            if (request.AccessToken != null)
            {
                client.DefaultRequestHeaders.Add("authorization", $"Bearer {request.AccessToken}");
            };
            var content = new FormUrlEncodedContent(body);
            var responseMessage = await client.PostAsync(config.TokenEndpoint, content);
            return await responseMessage.GetJsonAsync<OidcTokenResponse>();
        }

        public async Task<UserInfo> OidcUserInfoRequestAsync(string accessToken)
        {
            var config = await GetOidcConfigurationAsync();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("authorization", $"Bearer {accessToken}");
            var responseMessage = await client.GetAsync(config.UserInfoEndpoint);
            return await responseMessage.GetJsonAsync<UserInfo>();
        }
    }
}
