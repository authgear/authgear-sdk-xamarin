using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    internal class AuthenticationCanceledException : OperationCanceledException
    {
        public AuthenticationCanceledException() : base() { }
        public AuthenticationCanceledException(string message) : base(message) { }
        public AuthenticationCanceledException(Exception innerException) : base("Authenticated was canceled.", innerException) { }
        public AuthenticationCanceledException(string message, Exception innerException) : base(message, innerException) { }
    }
}
