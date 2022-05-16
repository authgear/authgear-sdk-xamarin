using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin
{
    internal class DeviceInfoAndroidPackageInfo
    {
        [JsonPropertyName("packageName")]
        public string PackageName { get; set; }
        [JsonPropertyName("versionName")]
        public string VersionName { get; set; }
        [JsonPropertyName("versionCode")]
        public string VersionCode { get; set; }
        [JsonPropertyName("longVersionCode")]
        public string LongVersionCode { get; set; }
    }
}
