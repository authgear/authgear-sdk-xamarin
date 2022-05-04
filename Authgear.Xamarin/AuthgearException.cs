using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    internal partial class AuthgearException : Exception
    {
        public AuthgearException() : base("") { }
        public AuthgearException(string message) : base(message)
        {
        }
        public AuthgearException(Exception ex) : base("", ex)
        {
        }
        public AuthgearException(string message, Exception ex) : base(message, ex) { }
        public static Exception Wrap(Exception ex)
        {
            var platformEx = PlatformWrap(ex);
            if (platformEx != null) return platformEx;
            // No wrapping is needed.
            if (ex is AuthgearException) return ex;
            return new AuthgearException(ex);
        }
    }
}
