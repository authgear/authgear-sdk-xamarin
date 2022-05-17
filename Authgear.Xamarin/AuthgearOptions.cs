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
        public ITokenStorage TokenStorage { get; set; }
        /// <value>ShareSessionWithSystemBrowser is supported on iOS only. It has no effect on Android.</value>
        public bool ShareSessionWithSystemBrowser { get; set; }
        public string Name { get; set; }
    }
}
