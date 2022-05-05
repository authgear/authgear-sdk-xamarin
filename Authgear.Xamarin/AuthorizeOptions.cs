using Authgear.Xamarin.Oauth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    public class AuthorizeOptions
    {
        public string RedirectUri { get; set; }
        public string State { get; set; }
        public string ResponseType { get; set; }
        public List<PromptOption> PromptOptions { get; set; }
        public string LoginHint { get; set; }
        public List<string> UiLocales { get; set; }
        public string WechatRedirectUri { get; set; }
        public AuthenticatePage Page { get; set; }

        internal OidcAuthenticationRequest ToRequest(bool suppressIdpSessionCookie)
        {
            if (RedirectUri == null)
            {
                throw new ArgumentNullException(nameof(RedirectUri));
            }
            return new OidcAuthenticationRequest
            {
                RedirectUri = this.RedirectUri,
                ResponseType = "code",
                Scope = new List<string> { "openid", "offline_access", "https://authgear.com/scopes/full-access" },
                State = this.State,
                Prompt = this.PromptOptions,
                LoginHint = this.LoginHint,
                IdTokenHint = null,
                MaxAge = null,
                UiLocales = this.UiLocales,
                WechatRedirectUri = this.WechatRedirectUri,
                Page = this.Page,
                SuppressIdpSessionCookie = suppressIdpSessionCookie
            };
        }
    }
}
