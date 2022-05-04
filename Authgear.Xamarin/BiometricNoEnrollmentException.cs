using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    internal class BiometricNoEnrollmentException : AuthgearException
    {
        public BiometricNoEnrollmentException() : base("") { }
        public BiometricNoEnrollmentException(string message) : base(message)
        {
        }
        public BiometricNoEnrollmentException(Exception ex) : base("", ex)
        {
        }
        public BiometricNoEnrollmentException(string message, Exception ex) : base(message, ex) { }
    }
}
