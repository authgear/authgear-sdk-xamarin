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

        public BiometricOptionsAndroid(string title, string subtitle, string description, string negativeButtonText, BiometricAccessConstraintAndroid accessConstraint, bool invalidatedByBiometricEnrollment)
        {
            Title = title;
            Subtitle = subtitle;
            Description = description;
            NegativeButtonText = negativeButtonText;
            AccessConstraint = accessConstraint;
            InvalidatedByBiometricEnrollment = invalidatedByBiometricEnrollment;
        }
    }
}
