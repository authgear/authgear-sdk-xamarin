﻿using System;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin.DeviceInfo
{
    internal class DeviceInfoIosProcessInfo
    {
        [JsonPropertyName("isMacCatalystApp")]
        public bool IsMacCatalystApp { get; set; } = false;

        [JsonPropertyName("isiOSAppOnMac")]
        public bool IsIosAppOnMac { get; set; } = false;
    }
}
