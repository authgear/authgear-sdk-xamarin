using System;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin
{
    public class DeviceInfoIosUiDevice
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("systemName")]
        public string SystemName { get; set; }

        [JsonPropertyName("systemVersion")]
        public string SystemVersion { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("userInterfaceIdiom")]
        public string UserInterfaceIdiom { get; set; }
    }
}
