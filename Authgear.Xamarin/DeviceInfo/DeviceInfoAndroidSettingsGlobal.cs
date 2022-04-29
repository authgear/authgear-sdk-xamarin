using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin
{
    public class DeviceInfoAndroidSettingsGlobal
    {
        [JsonPropertyName("DEVICE_NAME")]
        public string DeviceName { get; set; }
    }
}
