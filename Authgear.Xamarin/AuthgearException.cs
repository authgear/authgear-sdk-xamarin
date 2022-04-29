using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    internal class AuthgearException : Exception
    {
        public AuthgearException(string message) : base(message)
        {
        }
    }
}
