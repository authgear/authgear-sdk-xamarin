using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Xamarin.Essentials;

namespace Authgear.Xamarin
{
    internal class DeviceInfoRoot
    {
        [JsonPropertyName("android")]
        public DeviceInfoAndroid Android { get; set; }

        [JsonPropertyName("ios")]
        public DeviceInfoIos Ios { get; set; }
    }
}
