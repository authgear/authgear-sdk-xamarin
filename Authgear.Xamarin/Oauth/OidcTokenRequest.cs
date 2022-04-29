using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin.Oauth
{
    internal class OidcTokenRequest
    {
        [JsonPropertyName("grant_type")]
        // TODO: Add serializer for Enum with Description
        public GrantType GrantType { get; set; }
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }
        [JsonPropertyName("x_device_info")]
        public string XDeviceInfo { get; set; }
        [JsonPropertyName("redirect_uri")]
        public string RedirectUri { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("code_verifier")]
        public string CodeVerifier { get; set; }
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("jwt")]
        public string Jwt { get; set; }
    }
}
