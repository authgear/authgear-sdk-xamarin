using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin.DeviceInfo
{
    internal class DeviceInfoAndroidSecure
    {
        [JsonPropertyName("bluetooth_name")]
        public string BluetoothName { get; set; } = "";
        [JsonPropertyName("ANDROID_ID")]
        public string AndroidId { get; set; } = "";
    }
}
