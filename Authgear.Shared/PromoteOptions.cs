using System;
using System.Collections.Generic;
using System.Text;
using Authgear.Xamarin.Oauth;

namespace Authgear.Xamarin
{
    public class PromoteOptions
    {
        public string RedirectUri { get; set; }
        public string? State { get; set; }
        public IReadOnlyCollection<string>? UiLocales { get; set; }
        public ColorScheme? ColorScheme { get; set; }

        public PromoteOptions(string redirectUri)
        {
            RedirectUri = redirectUri;
        }

        internal OidcAuthenticationRequest ToRequest(string loginHint, bool isSsoEnabled)
        {
            if (RedirectUri == null)
            {
                throw new ArgumentNullException(nameof(RedirectUri));
            }
            return new OidcAuthenticationRequest(RedirectUri, "code", new List<string>() { "openid", "offline_access", "https://authgear.com/scopes/full-access" }, isSsoEnabled)
            {
                Prompt = new List<PromptOption>() { PromptOption.Login },
                LoginHint = loginHint,
                State = State,
                UiLocales = UiLocales,
                ColorScheme = ColorScheme,
            };
        }
    }
}
