using System;
using System.Collections.Generic;
using System.Text;
using LocalAuthentication;

namespace Authgear.Xamarin
{
    internal partial class AuthgearException
    {
        private const int LAErrorUserCancel = -2;
        private const int LAErrorPasscodeNotSet = -5;
        private const int LAErrorBiometryNotAvailable = -6;
        private const int LAErrorBiometryNotEnrolled = -7;
        private const int LAErrorBiometryLockout = -8;
        private const int ErrSecUserCanceled = -128;
        private const int ErrSecItemNotFound = -25300;
        public static Exception PlatformWrap(Exception ex)
        {
            if (ex is BiometricIosException bie)
            {
                var error = bie.Error;
                switch (error.Code)
                {
                    case ErrSecItemNotFound:
                        return new BiometricPrivateKeyNotFoundException();
                    case ErrSecUserCanceled:
                    case LAErrorUserCancel:
                        return new OperationCanceledException("Biometrics or LA user cancelled", ex);
                    case LAErrorBiometryNotAvailable:
                        return new BiometricNotSupportedOrPermissionDeniedException();
                    case LAErrorPasscodeNotSet:
                        return new BiometricNoPasscodeException();
                    case LAErrorBiometryNotEnrolled:
                        return new BiometricNoEnrollmentException();
                    case LAErrorBiometryLockout:
                        return new BiometricLockoutException();
                }
            }
            return null;
        }
    }
}
