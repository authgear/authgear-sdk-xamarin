using System;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin
{
    public class UserInfoAddress
    {
        [JsonPropertyName("formatted")]
        public string? Formatted { get; set; }
        [JsonPropertyName("street_address")]
        public string? StreetAddress { get; set; }
        [JsonPropertyName("locality")]
        public string? Locality { get; set; }
        [JsonPropertyName("region")]
        public string? Region { get; set; }
        [JsonPropertyName("postal_code")]
        public string? PostalCode { get; set; }
        [JsonPropertyName("country")]
        public string? Country { get; set; }
    }
}
