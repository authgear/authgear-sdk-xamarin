using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin
{
    internal class Jwk
    {
        [JsonPropertyName("kid")]
        public string Kid { get; set; }
        [JsonPropertyName("alg")]
        public string Alg { get; set; } = "RS256";
        [JsonPropertyName("kty")]
        public string Kty { get; set; } = "RSA";
        [JsonPropertyName("n")]
        public string N { get; set; }
        [JsonPropertyName("e")]
        public string E { get; set; }
    }
}
