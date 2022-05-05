using Authgear.Xamarin.Oauth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Authgear.Xamarin.Data.Oauth
{
    internal interface IOauthRepo
    {
        string Endpoint { get; set; }
        Task<OidcConfiguration> OidcConfiguration();
        Task<OidcTokenResponse> OidcTokenRequest(OidcTokenRequest request);
        Task BiometricSetupRequest(string accessToken, string clientId, string jwt);
        Task OidcRevocationRequest(string refreshToken);
        Task<UserInfo> OidcUserInfoRequest(string accessToken);
        Task<ChallengeResponse> OauthChallenge(string purpose);
        Task<AppSessionTokenResponse> OauthAppSessionToken(string refreshToken);
    }
}
