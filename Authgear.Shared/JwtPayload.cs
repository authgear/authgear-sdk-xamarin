using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Authgear.Xamarin.DeviceInfo;

namespace Authgear.Xamarin
{
    internal class JwtPayload
    {
        [JsonPropertyName("iat")]
        public long Iat { get; set; }
        [JsonPropertyName("exp")]
        public long Exp { get; set; }
        [JsonPropertyName("challenge")]
        public string Challenge { get; set; }
        [JsonPropertyName("action")]
        public string Action { get; set; }
        [JsonPropertyName("device_info")]
        public DeviceInfoRoot DeviceInfo { get; set; }

        public JwtPayload(DateTimeOffset now, string challenge, string action, DeviceInfoRoot deviceInfo)
        {
            Iat = now.ToUnixTimeSeconds();
            Exp = Iat + 60;
            Challenge = challenge;
            Action = action;
            DeviceInfo = deviceInfo;
        }
    }
}
