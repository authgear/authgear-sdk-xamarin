using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin.Oauth
{
    internal class OidcConfiguration
    {
        [JsonPropertyName("authorization_endpoint")]
        public string AuthorizationEndpoint { get; set; }
        [JsonPropertyName("token_endpoint")]
        public string TokenEndpoint { get; set; }
        [JsonPropertyName("userinfo_endpoint")]
        public string UserInfoEndpoint { get; set; }
        [JsonPropertyName("revocation_endpoint")]
        public string RevocationEndpoint { get; set; }
        [JsonPropertyName("end_session_endpoint")]
        public string EndSessionEndpoint { get; set; }
    }
}
