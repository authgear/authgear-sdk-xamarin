using System;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin.DeviceInfo
{
    internal partial class DeviceInfoIos
    {
        [JsonPropertyName("uname")]
        public DeviceInfoIosUname? UName { get; set; }

        [JsonPropertyName("UIDevice")]
        public DeviceInfoIosUiDevice? UiDevice { get; set; }

        [JsonPropertyName("NSProcessInfo")]
        public DeviceInfoIosProcessInfo? ProcessInfo { get; set; }

        [JsonPropertyName("NSBundle")]
        public DeviceInfoIosBundle? Bundle { get; set; }
    }
}
