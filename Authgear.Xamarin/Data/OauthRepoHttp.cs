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

        public Task<OidcTokenResponse> OidcTokenRequest(OidcTokenRequest request)
        {
            throw new NotImplementedException();
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
