using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    public class BiometricOptionsIos
    {
        public string? LocalizedReason { get; set; }
        public BiometricAccessConstraintIos? AccessConstraint { get; set; }
    }
}
