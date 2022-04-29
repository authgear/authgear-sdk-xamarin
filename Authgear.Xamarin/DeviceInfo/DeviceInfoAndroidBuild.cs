using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin
{
    public class DeviceInfoAndroidBuild
    {
        [JsonPropertyName("BOARD")]
        public string Board { get; set; }
        [JsonPropertyName("BRAND")]
        public string Brand { get; set; }
        [JsonPropertyName("MODEL")]
        public string Model { get; set; }
        [JsonPropertyName("DEVICE")]
        public string Device { get; set; }
        [JsonPropertyName("DISPLAY")]
        public string Display { get; set; }
        [JsonPropertyName("HARDWARE")]
        public string Hardware { get; set; }
        [JsonPropertyName("MANUFACTURER")]
        public string Manufacturer { get; set; }
        [JsonPropertyName("PRODUCT")]
        public string Product { get; set; }
    }
}
