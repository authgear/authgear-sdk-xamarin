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
        Task<OidcConfiguration> GetOidcConfigurationAsync();
        Task<OidcTokenResponse> OidcTokenRequestAsync(OidcTokenRequest request);
        Task BiometricSetupRequestAsync(string accessToken, string clientId, string jwt);
        Task OidcRevocationRequestAsync(string refreshToken);
        Task<UserInfo> OidcUserInfoRequestAsync(string accessToken);
        Task<ChallengeResponse> OauthChallengeAsync(string purpose);
        Task<AppSessionTokenResponse> OauthAppSessionTokenAsync(string refreshToken);
    }
}
