using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    internal class BiometricLockoutException : AuthgearException
    {
        public BiometricLockoutException() : base("") { }
        public BiometricLockoutException(string message) : base(message)
        {
        }
        public BiometricLockoutException(Exception ex) : base("", ex)
        {
        }
        public BiometricLockoutException(string message, Exception ex) : base(message, ex) { }
    }
}
