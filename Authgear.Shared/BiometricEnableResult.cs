using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    internal class BiometricEnableResult
    {
        public string Kid { get; set; } = "";
        public string Jwt { get; set; } = "";
    }
}
