using Android.Security.Keystore;
using AndroidX.Biometric;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    internal partial class AuthgearException
    {
        public static Exception PlatformWrap(Exception ex)
        {
            if (ex is KeyPermanentlyInvalidatedException)
            {
                return new BiometricPrivateKeyNotFoundException(ex);
            }
            if (ex is BiometricPromptAuthenticationException bpae)
            {
                if (bpae.ErrorCode == BiometricPrompt.ErrorCanceled || bpae.ErrorCode == BiometricPrompt.ErrorNegativeButton || bpae.ErrorCode == BiometricPrompt.ErrorUserCanceled)
                {
                    return new CancelException(ex);
                }
                if (bpae.ErrorCode == BiometricPrompt.ErrorHwNotPresent || bpae.ErrorCode == BiometricPrompt.ErrorHwUnavailable || bpae.ErrorCode == BiometricPrompt.ErrorSecurityUpdateRequired)
                {
                    return new BiometricNotSupportedOrPermissionDeniedException(ex);
                }
                if (bpae.ErrorCode == BiometricPrompt.ErrorNoBiometrics)
                {
                    return new BiometricNoEnrollmentException(ex);
                }
                if (bpae.ErrorCode == BiometricPrompt.ErrorNoDeviceCredential)
                {
                    return new BiometricNoPasscodeException(ex);
                }
                if (bpae.ErrorCode == BiometricPrompt.ErrorLockout || bpae.ErrorCode == BiometricPrompt.ErrorLockoutPermanent)
                {
                    return new BiometricLockoutException(ex);
                }
            }
            if (ex is BiometricCanAuthenticateException bcae)
            {
                if (bcae.Result == BiometricManager.BiometricErrorHwUnavailable || bcae.Result == BiometricManager.BiometricErrorNoHardware || bcae.Result == BiometricManager.BiometricErrorSecurityUpdateRequired || bcae.Result == BiometricManager.BiometricErrorUnsupported)
                {
                    return new BiometricNotSupportedOrPermissionDeniedException(ex);
                }
                if (bcae.Result == BiometricManager.BiometricErrorNoneEnrolled)
                {
                    return new BiometricNoEnrollmentException(ex);
                }
            }
            return null;
        }
    }
}
