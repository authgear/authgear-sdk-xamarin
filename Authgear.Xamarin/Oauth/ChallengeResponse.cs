using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin.Oauth
{
    internal class ChallengeResponse
    {
        [JsonPropertyName("token")]
        public string? Token { get; set; }
        [JsonPropertyName("expire_at")]
        public string? ExpireAt { get; set; }
    }
}
