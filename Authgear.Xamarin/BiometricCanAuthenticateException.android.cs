using System;
using System.Collections.Generic;
using System.Text;
using AndroidX.Biometric;

namespace Authgear.Xamarin
{
    internal class BiometricCanAuthenticateException : AuthgearException
    {
        public int Result { get; private set; }
        public BiometricCanAuthenticateException(int result) : base(ToResultString(result)) { Result = result; }
        public static string ToResultString(int result)
        {
            return result switch
            {
                BiometricManager.BiometricSuccess => "BIOMETRIC_SUCCESS",
                BiometricManager.BiometricStatusUnknown => "BIOMETRIC_STATUS_UNKNOWN",
                BiometricManager.BiometricErrorNoHardware => "BIOMETRIC_ERROR_NO_HARDWARE",
                BiometricManager.BiometricErrorHwUnavailable => "BIOMETRIC_ERROR_HW_UNAVAILABLE",
                BiometricManager.BiometricErrorNoneEnrolled => "BIOMETRIC_ERROR_NONE_ENROLLED",
                BiometricManager.BiometricErrorSecurityUpdateRequired => "BIOMETRIC_ERROR_SECURITY_UPDATE_REQUIRED",
                BiometricManager.BiometricErrorUnsupported => "BIOMETRIC_ERROR_UNSUPPORTED",
                _ => ""
            };
        }
    }
}
