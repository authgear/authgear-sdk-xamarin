using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin
{
    public class UserInfo
    {
        [JsonPropertyName("sub")]
        public string? Sub { get; set; }
        [JsonPropertyName("https://authgear.com/claims/user/is_anonymous")]
        public bool IsAnonymous { get; set; }
        [JsonPropertyName("https://authgear.com/claims/user/is_verified")]
        public bool IsVerified { get; set; }
    }
}
