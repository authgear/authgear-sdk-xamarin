using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin.Data
{
    internal class ServerErrorResult
    {
        [JsonPropertyName("error")]
        public ServerError Error { get; set; } 
    }
}
