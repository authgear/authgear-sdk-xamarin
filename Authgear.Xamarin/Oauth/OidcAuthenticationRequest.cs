using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Authgear.Xamarin.CsExtensions;

[assembly: InternalsVisibleTo("UnitTest")]
namespace Authgear.Xamarin.Oauth
{
    internal class OidcAuthenticationRequest
    {
        public string RedirectUri { get; set; }
        public string ResponseType { get; set; }
        public List<string> Scope { get; set; }
        public string State { get; set; }
        public List<PromptOption> Prompt { get; set; }
        public int? MaxAge { get; set; }
        public string LoginHint { get; set; }
        public List<string> UiLocales { get; set; }
        public ColorScheme? ColorScheme { get; set; }
        public string IdTokenHint { get; set; }
        public AuthenticatePage? Page { get; set; }
        public bool? SuppressIdpSessionCookie { get; set; }
        internal Dictionary<string, string> ToQuery(string clientId, string challenge)
        {
            var query = new Dictionary<string, string>()
            {
                ["client_id"] = clientId,
                ["response_type"] = ResponseType,
                ["redirect_uri"] = RedirectUri,
                ["scope"] = string.Join(" ", Scope),
                ["x_platform"] = "xamarin"
            };
            if (challenge != null)
            {
                query["code_challenge_method"] = "S256";
                query["code_challenge"] = challenge;
            }
            if (State != null)
            {
                query["state"] = State;
            }
            if (Prompt != null)
            {
                query["prompt"] = string.Join(" ", Prompt.Select(x => x.GetDescription()));
            }
            if (LoginHint != null)
            {
                query["login_hint"] = LoginHint;
            }
            if (UiLocales != null)
            {
                query["ui_locales"] = string.Join(" ", UiLocales);
            }
            if (ColorScheme != null)
            {
                query["x_color_scheme"] = ColorScheme.GetDescription();
            }
            if (IdTokenHint != null)
            {
                query["id_token_hint"] = IdTokenHint;
            }
            if (MaxAge != null)
            {
                query["max_age"] = MaxAge.ToString();
            }
            if (Page != null)
            {
                query["x_page"] = Page.GetDescription();
            }
            if (SuppressIdpSessionCookie == true)
            {
                query["x_suppress_idp_session_cookie"] = "true";
            }
            return query;
        }
        internal Dictionary<string, string> ToQuery(string clientId, CodeVerifier codeVerifier)
        {
            return ToQuery(clientId, codeVerifier?.Challenge);
        }
    }
}
