using System;
using System.Collections.Generic;
using System.Text;
using Authgear.Xamarin.Oauth;

namespace Authgear.Xamarin
{
    public class ReauthenticateOptions
    {
        public string RedirectUri { get; set; }
        public string State { get; set; }
        public List<string> UiLocales { get; set; }
        public ColorScheme? ColorScheme { get; set; }
        public int? MaxAge { get; set; }
        internal OidcAuthenticationRequest ToRequest(string idTokenHint, bool suppressIdpSessionCookie)
        {
            if (RedirectUri == null)
            {
                throw new ArgumentNullException(nameof(RedirectUri));
            }
            return new OidcAuthenticationRequest
            {
                RedirectUri = RedirectUri,
                ResponseType = "code",
                Scope = new List<string> { "openid", "https://authgear.com/scopes/full-access" },
                State = State,
                IdTokenHint = idTokenHint,
                MaxAge = MaxAge ?? 0,
                UiLocales = UiLocales,
                ColorScheme = ColorScheme,
                SuppressIdpSessionCookie = suppressIdpSessionCookie
            };
        }
    }
}
