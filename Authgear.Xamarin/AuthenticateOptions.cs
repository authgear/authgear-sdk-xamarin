using System;
using System.Collections.Generic;
using System.Text;
using Authgear.Xamarin.Oauth;

namespace Authgear.Xamarin
{
    public class AuthenticateOptions
    {
        public string RedirectUri { get; set; }
        public string State { get; set; }
        public string ResponseType { get; set; }
        public List<PromptOption> PromptOptions { get; set; }
        public string LoginHint { get; set; }
        public List<string> UiLocales { get; set; }
        public ColorScheme? ColorScheme { get; set; }
        public AuthenticatePage? Page { get; set; }

        internal OidcAuthenticationRequest ToRequest(bool suppressIdpSessionCookie)
        {
            if (RedirectUri == null)
            {
                throw new ArgumentNullException(nameof(RedirectUri));
            }
            return new OidcAuthenticationRequest
            {
                RedirectUri = RedirectUri,
                ResponseType = "code",
                Scope = new List<string> { "openid", "offline_access", "https://authgear.com/scopes/full-access" },
                State = State,
                Prompt = PromptOptions,
                LoginHint = LoginHint,
                IdTokenHint = null,
                MaxAge = null,
                UiLocales = UiLocales,
                ColorScheme = ColorScheme,
                Page = Page,
                SuppressIdpSessionCookie = suppressIdpSessionCookie
            };
        }
    }
}
