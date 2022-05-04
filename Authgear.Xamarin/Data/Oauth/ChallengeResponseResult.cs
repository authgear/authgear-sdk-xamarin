using Authgear.Xamarin.Oauth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin.Data.Oauth
{
    internal class ChallengeResponseResult
    {
        [JsonPropertyName("result")]
        public ChallengeResponse Result { get; set; }
    }
}
