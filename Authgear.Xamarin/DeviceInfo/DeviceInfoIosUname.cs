using System;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin
{
    public class DeviceInfoIosUname
    {
        [JsonPropertyName("machine")]
        public string Machine { get; set; }

        [JsonPropertyName("nodename")]
        public string NodeName { get; set; }

        [JsonPropertyName("release")]
        public string Release { get; set; }

        [JsonPropertyName("sysname")]
        public string SysName { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }
    }
}
