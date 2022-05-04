using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    internal class BiometricPrivateKeyNotFoundException : Exception
    {
        public BiometricPrivateKeyNotFoundException(string msg) : base(msg) { }
        public BiometricPrivateKeyNotFoundException(Exception ex) : base("", ex) { }
    }
}
