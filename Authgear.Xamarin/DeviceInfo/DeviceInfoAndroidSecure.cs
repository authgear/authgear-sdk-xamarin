using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin
{
    public class DeviceInfoAndroidSecure
    {
        [JsonPropertyName("bluetooth_name")]
        public string BluetoothName { get; set; }
    }
}
