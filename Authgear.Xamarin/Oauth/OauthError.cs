using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin.Oauth
{
    internal class OauthError
    {
        [JsonPropertyName("error")]
        public string Error { get; set; }
        [JsonPropertyName("state")]
        public string State { get; set; }
        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; }
        [JsonPropertyName("error_uri")]
        public string ErrorUri { get; set; }
    }
}
