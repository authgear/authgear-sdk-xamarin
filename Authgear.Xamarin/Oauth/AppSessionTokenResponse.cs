using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin.Oauth
{
    internal class AppSessionTokenResponse
    {
        [JsonPropertyName("app_session_token")]
        public string AppSessionToken { get; set; }
        [JsonPropertyName("expire_at")]
        public string ExpireAt { get; set; }
    }
}
