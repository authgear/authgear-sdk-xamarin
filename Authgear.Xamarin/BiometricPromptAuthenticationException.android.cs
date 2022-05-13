using System;
using System.Collections.Generic;
using System.Text;
using AndroidX.Biometric;

namespace Authgear.Xamarin
{
    internal class BiometricPromptAuthenticationException : Exception
    {
        public int ErrorCode { get; private set; }
        public BiometricPromptAuthenticationException(string message) : base(message) { }

        public BiometricPromptAuthenticationException(Exception ex) : base("", ex) { }

        public BiometricPromptAuthenticationException(int errorCode) : base(ToErrorCodeString(errorCode)) { ErrorCode = errorCode; }

        public static string ToErrorCodeString(int errorCode)
        {
            return errorCode switch
            {
                BiometricPrompt.ErrorCanceled => "ERROR_CANCELED",
                BiometricPrompt.ErrorHwNotPresent => "ERROR_HW_NOT_PRESENT",
                BiometricPrompt.ErrorHwUnavailable => "ERROR_HW_UNAVAILABLE",
                BiometricPrompt.ErrorLockout => "ERROR_LOCKOUT",
                BiometricPrompt.ErrorLockoutPermanent => "ERROR_LOCKOUT_PERMANENT",
                BiometricPrompt.ErrorNegativeButton => "ERROR_NEGATIVE_BUTTON",
                BiometricPrompt.ErrorNoBiometrics => "ERROR_NO_BIOMETRICS",
                BiometricPrompt.ErrorNoDeviceCredential => "ERROR_NO_DEVICE_CREDENTIAL",
                BiometricPrompt.ErrorNoSpace => "ERROR_NO_SPACE",
                BiometricPrompt.ErrorSecurityUpdateRequired => "ERROR_SECURITY_UPDATE_REQUIRED",
                BiometricPrompt.ErrorTimeout => "ERROR_TIMEOUT",
                BiometricPrompt.ErrorUnableToProcess => "ERROR_UNABLE_TO_PROCESS",
                BiometricPrompt.ErrorUserCanceled => "ERROR_USER_CANCELED",
                BiometricPrompt.ErrorVendor => "ERROR_VENDOR",
                _ => "",
            };
        }
    }
}
