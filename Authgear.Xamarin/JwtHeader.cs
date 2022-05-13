using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Authgear.Xamarin.Data;

namespace Authgear.Xamarin
{
    internal class JwtHeader
    {
        [JsonPropertyName("typ")]
        public JwtHeaderType Typ { get; set; }
        [JsonPropertyName("kid")]
        public string Kid { get; set; }
        [JsonPropertyName("alg")]
        public string Alg { get; set; }
        [JsonPropertyName("jwk")]
        public Jwk Jwk { get; set; }
    }
}
