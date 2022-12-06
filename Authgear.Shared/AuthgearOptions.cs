using System;
using System.Collections.Generic;
using System.Text;
using Authgear.Xamarin.Data;

namespace Authgear.Xamarin
{
    public class AuthgearOptions
    {
        public string ClientId { get; set; }
        public string AuthgearEndpoint { get; set; }
        public ITokenStorage? TokenStorage { get; set; }
        public bool IsSsoEnabled { get; set; }
        public string? Name { get; set; }

        public AuthgearOptions(string clientId, string authgearEndPoint)
        {
            ClientId = clientId;
            AuthgearEndpoint = authgearEndPoint;
            IsSsoEnabled = false;
        }
    }
}
