using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    public class BiometricOptionsAndroid
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string NegativeButtonText { get; set; }
        public BiometricAccessConstraintAndroid AccessConstraint { get; set; }
        public bool InvalidatedByBiometricEnrollment { get; set; }
    }
}
