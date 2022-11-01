using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    public class BiometricOptions
    {
        public BiometricOptionsAndroid? Android { get; set; }

        public BiometricOptionsIos? Ios { get; set; }
    }
}
