using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    public class BiometricNoPasscodeException : AuthgearException
    {
        public BiometricNoPasscodeException() : base("") { }
        public BiometricNoPasscodeException(string message) : base(message)
        {
        }
        public BiometricNoPasscodeException(Exception ex) : base("", ex)
        {
        }
        public BiometricNoPasscodeException(string message, Exception ex) : base(message, ex) { }
    }
}
