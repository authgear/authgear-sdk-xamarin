using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Authgear.Xamarin.Oauth;

namespace Authgear.Xamarin
{
    public class AuthenticateOptions
    {
        public string RedirectUri { get; set; }
        public string? State { get; set; }
        public IReadOnlyCollection<PromptOption>? PromptOptions { get; set; }
        public string? LoginHint { get; set; }
        public IReadOnlyCollection<string>? UiLocales { get; set; }
        public ColorScheme? ColorScheme { get; set; }
        public AuthenticatePage? Page { get; set; }
        public string? OauthProviderAlias { get; set; }

        public AuthenticateOptions(string redirectUri)
        {
            RedirectUri = redirectUri;
        }
        internal OidcAuthenticationRequest ToRequest(bool suppressIdpSessionCookie)
        {
            if (RedirectUri == null)
            {
                throw new ArgumentNullException(nameof(RedirectUri));
            }
            return new OidcAuthenticationRequest(RedirectUri, "code", new List<string> { "openid", "offline_access", "https://authgear.com/scopes/full-access" })
            {
                State = State,
                Prompt = PromptOptions,
                LoginHint = LoginHint,
                IdTokenHint = null,
                MaxAge = null,
                UiLocales = UiLocales,
                ColorScheme = ColorScheme,
                Page = Page,
                SuppressIdpSessionCookie = suppressIdpSessionCookie,
                OauthProviderAlias = OauthProviderAlias
            };
        }
    }
}
