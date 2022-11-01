using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin.DeviceInfo
{
    internal partial class DeviceInfoAndroid
    {
        [JsonPropertyName("Build")]
        public DeviceInfoAndroidBuild? Build { get; set; }
        [JsonPropertyName("PackageInfo")]
        public DeviceInfoAndroidPackageInfo? PackageInfo { get; set; }
        [JsonPropertyName("Settings")]
        public DeviceInfoAndroidSettings? Settings { get; set; }

        [JsonPropertyName("ApplicationInfoLabel")]
        public string AplicationInfoLabel { get; set; } = "";
    }
}
