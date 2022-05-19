using System;
using System.Collections.Generic;
using Xunit;
using Authgear.Xamarin;
using Authgear.Xamarin.Oauth;

namespace UnitTest
{
    public class NetExtensionsTests
    {
        [Fact]
        public void StringStringDict_ToQueryParameters()
        {
            var dict = new Dictionary<string, string>()
            {
                ["response_type"] = "code",
                ["client_id"] = "clientId",
                ["redirect_uri"] = "http://host/path",
            };
            var param = dict.ToQueryParameter();
            var u = new Uri("https://localhost:80/");
            var builder = new UriBuilder(u)
            {
                Query = param
            };
            // Current implementation doesn't trim last &
            Assert.Equal("https://localhost:80/?response_type=code&client_id=clientId&redirect_uri=http%3A%2F%2Fhost%2Fpath&", builder.ToString());
        }

        [Fact]
        public void OidcAuthenticationRequest_ToQueryParameters()
        {
            var request = new OidcAuthenticationRequest
            {
                RedirectUri = "http://host/path",
                ResponseType = "code",
                Scope = new List<string> { "openid", "email" },
                State = "state",
                Prompt = new List<PromptOption> { PromptOption.Login },
                LoginHint = "loginHint",
                UiLocales = new List<string> { "en-US", "zh-HK" },
                IdTokenHint = "idTokenHint",
                MaxAge = 1000000000,
                Page = AuthenticatePage.Login,
                SuppressIdpSessionCookie = true,
            };
            var dict = request.ToQuery("clientId", "codeChallenge").ToQueryParameter();
            var u = new Uri("https://localhost:80/");
            var builder = new UriBuilder(u)
            {
                Query = dict
            };
            Assert.Equal("https://localhost:80/?client_id=clientId&response_type=code&redirect_uri=http%3A%2F%2Fhost%2Fpath&scope=openid+email&x_platform=xamarin&code_challenge_method=S256&code_challenge=codeChallenge&state=state&prompt=login&login_hint=loginHint&ui_locales=en-US+zh-HK&id_token_hint=idTokenHint&max_age=1000000000&x_page=login&x_suppress_idp_session_cookie=true&", builder.ToString());
        }
    }
}
