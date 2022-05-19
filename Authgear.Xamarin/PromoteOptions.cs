using System;
using System.Collections.Generic;
using System.Text;
using Authgear.Xamarin.Oauth;

namespace Authgear.Xamarin
{
    public class PromoteOptions
    {
        public string RedirectUri { get; set; }
        public string State { get; set; }
        public List<string> UiLocales { get; set; }
        public ColorScheme? ColorScheme { get; set; }

        internal OidcAuthenticationRequest ToRequest(string loginHint, bool suppressIdpSessionCookie)
        {
            return new OidcAuthenticationRequest
            {
                RedirectUri = RedirectUri,
                ResponseType = "code",
                Scope = new List<string>() { "openid", "offline_access", "https://authgear.com/scopes/full-access" },
                Prompt = new List<PromptOption>() { PromptOption.Login },
                LoginHint = loginHint,
                State = State,
                UiLocales = UiLocales,
                ColorScheme = ColorScheme,
                SuppressIdpSessionCookie = suppressIdpSessionCookie,
            };
        }
    }
}
