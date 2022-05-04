using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    internal class BiometricPrivateKeyNotFoundException : Exception
    {
        public BiometricPrivateKeyNotFoundException() : base() { }
        public BiometricPrivateKeyNotFoundException(string msg) : base(msg) { }
        public BiometricPrivateKeyNotFoundException(Exception ex) : base("", ex) { }

        public BiometricPrivateKeyNotFoundException(string msg, Exception ex) : base(msg, ex) { }
    }
}
