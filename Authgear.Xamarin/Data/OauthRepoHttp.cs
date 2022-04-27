using Authgear.Xamarin.Oauth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin.Data
{
    internal class OauthRepoHttp : IOauthRepo
    {
        public string Endpoint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public OidcConfiguration OidcConfiguration => throw new NotImplementedException();

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

        public OidcTokenResponse OidcTokenRequest(OidcTokenRequest request)
        {
            throw new NotImplementedException();
        }

        public UserInfo OidcUserInfoRequest(string accessToken)
        {
            throw new NotImplementedException();
        }

        public void WechatAuthCallback(string code, string state)
        {
            throw new NotImplementedException();
        }
    }
}
