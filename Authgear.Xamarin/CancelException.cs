using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    internal class CancelException : AuthgearException
    {
        public CancelException() : base("") { }
        public CancelException(string message) : base(message) { }
        public CancelException(Exception innerException) : base(innerException) { }
        public CancelException(string message, Exception innerException) : base(message, innerException) { }
    }
}
