using System;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin
{
    public class DeviceInfoIosBundle
    {
        [JsonPropertyName("CFBundleIdentifier")]
        public string CFBundleIdentifier { get; set; }

        [JsonPropertyName("CFBundleName")]
        public string CFBundleName { get; set; }

        [JsonPropertyName("CFBundleDisplayName")]
        public string CFBundleDisplayName { get; set; }

        [JsonPropertyName("CFBundleExecutable")]
        public string CFBundleExecutable { get; set; }

        [JsonPropertyName("CFBundleShortVersionString")]
        public string CFBundleShortVersionString { get; set; }

        [JsonPropertyName("CFBundleVersion")]
        public string CFBundleVersion { get; set; }
    }
}
