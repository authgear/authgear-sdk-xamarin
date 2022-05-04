using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin.Data
{
    internal class ServerError
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("reason")]
        public string Reason { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("info")]
        public JsonDocument Info { get; set; }
    }
}
