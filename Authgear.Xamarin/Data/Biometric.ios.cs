using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Authgear.Xamarin.DeviceInfo;
using Foundation;
using LocalAuthentication;
using Security;
using UIKit;

namespace Authgear.Xamarin.Data
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Xamarin objects are managed")]
    internal class Biometric : IBiometric
    {
        private const string TagFormat = "com.authgear.keys.biometric.{0}";
        private const int KeySize = 2048;

        private static SecAccessControlCreateFlags ToFlags(BiometricAccessConstraintIos? biometricAccessConstraint)
        {
            if (biometricAccessConstraint == null)
            {
                throw new ArgumentNullException(nameof(BiometricOptionsIos.AccessConstraint));
            }
            switch (biometricAccessConstraint)
            {
                case BiometricAccessConstraintIos.BiometricAny:
                    return SecAccessControlCreateFlags.BiometryAny;
                case BiometricAccessConstraintIos.BiometricCurrentSet:
                    return SecAccessControlCreateFlags.BiometryCurrentSet;
                case BiometricAccessConstraintIos.UserPresence:
                    return SecAccessControlCreateFlags.UserPresence;
                default:
                    throw new ArgumentException($"Unknown access constraint: {biometricAccessConstraint}");
            }
        }

        internal Biometric()
        {
        }

        public Task<string> AuthenticateBiometricAsync(BiometricOptions options, string kid, string challenge, DeviceInfoRoot deviceInfo)
        {
            EnsureIsSupported(options);
            var tag = string.Format(CultureInfo.InvariantCulture, TagFormat, kid);
            var record = new SecRecord(SecKind.Key)
            {
                KeyType = SecKeyType.RSA,
                ApplicationTag = tag,
            };
            var secKeyObject = SecKeyChain.QueryAsConcreteType(record, out var result);
            if (result != SecStatusCode.Success)
            {
                throw AuthgearException.Wrap(new BiometricIosException(result));
            }
            try
            {
                var secKey = (SecKey)secKeyObject;
                return Task.FromResult(SignJwt(kid, secKey, challenge, "authenticate", deviceInfo));
            }
            catch (Exception ex)
            {
                throw AuthgearException.Wrap(ex);
            }
        }

        public async Task<BiometricEnableResult> EnableBiometricAsync(BiometricOptions options, string challenge, DeviceInfoRoot deviceInfo)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (options.Ios == null)
            {
                throw new ArgumentNullException(nameof(options.Ios));
            }
            EnsureIsSupported(options);
            var kid = Guid.NewGuid().ToString();
            var tag = string.Format(CultureInfo.InvariantCulture, TagFormat, kid);
            var flags = ToFlags(options.Ios.AccessConstraint);
            var context = new LAContext();
            var (_, error) = await context.EvaluatePolicyAsync(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, options.Ios.LocalizedReason ?? "").ConfigureAwait(false);
            if (error != null)
            {
                throw AuthgearException.Wrap(new BiometricIosException(error));
            }
            var keyGenParameters = new SecKeyGenerationParameters
            {
                KeyType = SecKeyType.RSA,
                KeySizeInBits = KeySize
            };
            var secKey = SecKey.CreateRandomKey(keyGenParameters.Dictionary, out error);
            if (error != null)
            {
                throw AuthgearException.Wrap(new BiometricIosException(error));
            }
            var accessControl = new SecAccessControl(SecAccessible.WhenPasscodeSetThisDeviceOnly, flags);
            var record = new SecRecord(secKey)
            {
                ApplicationTag = tag,
                AccessControl = accessControl,
                AuthenticationContext = context
            };
            var status = SecKeyChain.Add(record);
            if (status != SecStatusCode.Success)
            {
                throw AuthgearException.Wrap(new BiometricIosException(status));
            }
            var jwt = SignJwt(kid, secKey, challenge, "setup", deviceInfo);
            return new BiometricEnableResult
            {
                Kid = kid,
                Jwt = jwt,
            };
        }
        private static string SignJwt(string kid, SecKey privateKey, string challenge, string action, DeviceInfoRoot deviceInfo)
        {
            var jwk = Jwk.FromPrivateKey(kid, privateKey);
            var header = new JwtHeader
            {
                Typ = JwtHeaderType.Biometric,
                Kid = kid,
                Alg = jwk.Alg,
                Jwk = jwk,
            };
            var payload = new JwtPayload(DateTimeOffset.Now, challenge, action, deviceInfo);
            var jwt = Jwt.Sign(privateKey, header, payload);
            return jwt;
        }

        private static void EnsureApiLevel()
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(11, 3))
            {
                throw new InvalidOperationException("Biometric authentication requires at least iOS version 11.3");
            }
        }

        public void EnsureIsSupported(BiometricOptions options)
        {
            EnsureApiLevel();
            var context = new LAContext();
            _ = context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out var error);
            if (error != null)
            {
                throw AuthgearException.Wrap(new BiometricIosException(error));
            }
        }

        public void RemoveBiometric(string kid)
        {
            var tag = string.Format(CultureInfo.InvariantCulture, TagFormat, kid);
            var record = new SecRecord(SecKind.Key)
            {
                KeyType = SecKeyType.RSA,
                ApplicationTag = tag
            };
            var status = SecKeyChain.Remove(record);
            if (status != SecStatusCode.Success && status != SecStatusCode.ItemNotFound)
            {
                throw AuthgearException.Wrap(new BiometricIosException(status));
            }
        }
    }
}
