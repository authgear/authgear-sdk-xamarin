using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    public class BiometricNotSupportedOrPermissionDeniedException : AuthgearException
    {
        public BiometricNotSupportedOrPermissionDeniedException() : base("") { }
        public BiometricNotSupportedOrPermissionDeniedException(string message) : base(message)
        {
        }
        public BiometricNotSupportedOrPermissionDeniedException(Exception ex) : base("", ex)
        {
        }
        public BiometricNotSupportedOrPermissionDeniedException(string message, Exception ex) : base(message, ex) { }
    }
}
