using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Xamarin.Essentials;

namespace Authgear.Xamarin.DeviceInfo
{
    internal class DeviceInfoRoot
    {
        [JsonPropertyName("android")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DeviceInfoAndroid Android { get; set; }

        [JsonPropertyName("ios")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DeviceInfoIos Ios { get; set; }
    }
}
