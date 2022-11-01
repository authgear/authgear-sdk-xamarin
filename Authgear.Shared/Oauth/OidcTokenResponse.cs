using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin.Oauth
{
    internal class OidcTokenResponse
    {
        [JsonPropertyName("id_token")]
        public string? IdToken { get; set; }
        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
        [JsonPropertyName("expires_in")]
        public long? ExpiresIn { get; set; }
        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }
    }
}
