using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin.DeviceInfo
{
    internal class DeviceInfoAndroidSettings
    {
        [JsonPropertyName("Secure")]
        public DeviceInfoAndroidSecure? Secure { get; set; }
        [JsonPropertyName("Global")]
        public DeviceInfoAndroidSettingsGlobal? Global { get; set; }
    }
}
