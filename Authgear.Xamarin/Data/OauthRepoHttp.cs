using Authgear.Xamarin.CsExtensions;
using Authgear.Xamarin.Oauth;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Authgear.Xamarin.Data
{
    internal class OauthRepoHttp : IOauthRepo
    {
        public string Endpoint { get; set; }

        private OidcConfiguration config;

        private ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        public async Task<OidcConfiguration> OidcConfiguration()
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
                var stream = await client.GetStreamAsync("/.well-known/openid-configuration");
                config = JsonSerializer.Deserialize<OidcConfiguration>(stream);
                return config;
            }
            finally { locker.ExitWriteLock(); }
        }

        public void BiometricSetupRequest(string accessToken, string clientId, string jwt)
        {
            throw new NotImplementedException();
        }

        public AppSessionTokenResponse OauthAppSessionToken(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public ChallengeResponse OauthChallenge(string purpose)
        {
            throw new NotImplementedException();
        }

        public void OidcRevocationRequest(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public async Task<OidcTokenResponse> OidcTokenRequest(OidcTokenRequest request)
        {
            var config = await OidcConfiguration();
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
            var content = new FormUrlEncodedContent(body);
            if (request.AccessToken != null)
            {
                content.Headers.Add("authorization", $"Bearer {request.AccessToken}");
            };
            var responseMessage = await client.PostAsync(config.TokenEndpoint, content);
            await responseMessage.EnsureSuccessOrAuthgearExceptionAsync();
            var responseStream = await responseMessage.Content.ReadAsStreamAsync();
            return JsonSerializer.Deserialize<OidcTokenResponse>(responseStream);
        }

        public Task<UserInfo> OidcUserInfoRequest(string accessToken)
        {
            throw new NotImplementedException();
        }

        public void WechatAuthCallback(string code, string state)
        {
            throw new NotImplementedException();
        }
    }
}
