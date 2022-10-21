using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    public class BiometricOptionsIos
    {
        public string LocalizedReason { get; set; }
        public BiometricAccessConstraintIos AccessConstraint { get; set; }
        public BiometricLAPolicy Policy { get; set; }

        public BiometricOptionsIos(
            string localizedReason,
            BiometricAccessConstraintIos accessConstraint,
            BiometricLAPolicy policy
        )
        {
            LocalizedReason = localizedReason;
            AccessConstraint = accessConstraint;
            Policy = policy;
        }
    }
}
