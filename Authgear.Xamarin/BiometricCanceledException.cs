using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    internal class BiometricCanceledException : OperationCanceledException
    {
        public BiometricCanceledException() : base() { }
        public BiometricCanceledException(string message) : base(message) { }
        public BiometricCanceledException(Exception innerException) : base("Biometric was canceled.", innerException) { }
        public BiometricCanceledException(string message, Exception innerException) : base(message, innerException) { }
    }
}
