using Authgear.Xamarin.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    public class AuthgearOptions
    {
        public string ClientId { get; set; }
        public string AuthgearEndpoint { get; set; }
        public ITokenStorage TokenStorage { get; set; }
        public bool ShareSessionWithSystemBrowser { get; set; }
        public string Name { get; set; }
    }
}
