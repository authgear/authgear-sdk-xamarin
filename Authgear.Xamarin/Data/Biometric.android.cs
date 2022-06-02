using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Security.Keystore;
using AndroidX.Biometric;
using AndroidX.Fragment.App;
using Authgear.Xamarin.DeviceInfo;
using Java.Security;
using Xamarin.Essentials;
using static AndroidX.Biometric.BiometricPrompt;

namespace Authgear.Xamarin.Data
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Xamarin objects are managed")]
    internal class Biometric : IBiometric
    {
        private const int BiometricOnly = BiometricManager.Authenticators.BiometricStrong;
        private const int BiometricOrDeviceCredential = BiometricManager.Authenticators.BiometricStrong | BiometricManager.Authenticators.DeviceCredential;
        private const string AndroidKeyStore = "AndroidKeyStore";
        private const string AliasFormat = "com.authgear.keys.biometric.{0}";
        private const int KeySize = 2048;
        private class AuthenticationCallbackImpl : AuthenticationCallback
        {
            private readonly TaskCompletionSource<string> taskSource;
            private readonly JwtHeader header;
            private readonly JwtPayload payload;
            public AuthenticationCallbackImpl(TaskCompletionSource<string> taskSource, JwtHeader header, JwtPayload payload)
            {
                this.taskSource = taskSource;
                this.header = header;
                this.payload = payload;
            }
            public override void OnAuthenticationFailed()
            {
                // This callback will be invoked EVERY time the recognition failed.
                // So while the prompt is still opened, this callback can be called repetitively.
                // Finally, either onAuthenticationError or onAuthenticationSucceeded will be called.
                // So this callback is not important to the developer.
            }

            public override void OnAuthenticationError(int errorCode, Java.Lang.ICharSequence errString)
            {
                taskSource.SetException(AuthgearException.Wrap(new BiometricPromptAuthenticationException(errorCode)));
            }

            public override void OnAuthenticationSucceeded(AuthenticationResult result)
            {
                try
                {
                    var signature = result.CryptoObject.Signature;
                    var jwt = Jwt.Sign(signature, header, payload);
                    taskSource.SetResult(jwt);
                }
                catch (Exception ex)
                {
                    taskSource.SetException(new AuthgearException(ex));
                }
            }
        }
        private static int ToAuthenticators(BiometricAccessConstraintAndroid? biometricAccessConstraint)
        {
            if (biometricAccessConstraint == null)
            {
                throw new ArgumentNullException(nameof(BiometricOptionsAndroid.AccessConstraint));
            }
            return biometricAccessConstraint == BiometricAccessConstraintAndroid.BiometricOnly ? BiometricOnly : BiometricOrDeviceCredential;
        }
        private static KeyPropertiesAuthType ToKeyPropertiesAuthType(BiometricAccessConstraintAndroid? biometricAccessConstraint)
        {
            if (biometricAccessConstraint == null)
            {
                throw new ArgumentNullException(nameof(BiometricOptionsAndroid.AccessConstraint));
            }
            return biometricAccessConstraint == BiometricAccessConstraintAndroid.BiometricOnly ? KeyPropertiesAuthType.BiometricStrong : KeyPropertiesAuthType.BiometricStrong | KeyPropertiesAuthType.DeviceCredential;
        }

        private readonly Context context;
        internal Biometric(Context context)
        {
            this.context = context;
        }

        private static void EnsureApiLevel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.M)
            {
                throw new InvalidOperationException("Biometric authentication requires at least API Level 23");
            }
        }

        public void EnsureIsSupported(BiometricOptions options)
        {
            if (options.Android == null)
            {
                throw new ArgumentNullException(nameof(options.Android));
            }
            EnsureApiLevel();
            EnsureCanAuthenticate(options);
        }

        public void RemoveBiometric(string kid)
        {
            var alias = string.Format(CultureInfo.InvariantCulture, AliasFormat, kid);
            RemovePrivateKey(alias);
        }
        private static void RemovePrivateKey(string alias)
        {
            var keystore = KeyStore.GetInstance(AndroidKeyStore)!;
            keystore.Load(null);
            keystore.DeleteEntry(alias);
        }

        private static KeyPair GetPrivateKey(string alias)
        {
            var keyStore = KeyStore.GetInstance(AndroidKeyStore)!;
            keyStore.Load(null);
            var entry = keyStore.GetEntry(alias, null);
            var privateKeyEntry = entry as KeyStore.PrivateKeyEntry;
            if (privateKeyEntry == null)
            {
                throw new KeyPermanentlyInvalidatedException();
            }
            return new KeyPair(privateKeyEntry.Certificate!.PublicKey, privateKeyEntry.PrivateKey);
        }

        private void EnsureCanAuthenticate(BiometricOptions options)
        {
            var authenticators = ToAuthenticators(options.Android!.AccessConstraint);
            var result = BiometricManager.From(context).CanAuthenticate(authenticators);
            if (result != BiometricManager.BiometricSuccess)
            {
                throw AuthgearException.Wrap(new BiometricCanAuthenticateException(result));
            }
        }

        public async Task<BiometricEnableResult> EnableBiometricAsync(BiometricOptions options, string challenge, DeviceInfoRoot deviceInfo)
        {
            if (options.Android == null)
            {
                throw new ArgumentNullException(nameof(options.Android));
            }
            EnsureApiLevel();
            EnsureCanAuthenticate(options);
            var optionsAn = options.Android;
            var promptInfo = BuildPromptInfo(optionsAn);
            var kid = Guid.NewGuid().ToString();
            var alias = string.Format(CultureInfo.InvariantCulture, AliasFormat, kid);
            var spec = MakeGenerateKeyPairSpec(alias, ToKeyPropertiesAuthType(optionsAn.AccessConstraint), optionsAn.InvalidatedByBiometricEnrollment);
            var keyPair = CreateKeyPair(spec);
            var jwk = Jwk.FromPublicKey(kid, keyPair.Public!);
            var header = new JwtHeader
            {
                Typ = JwtHeaderType.Biometric,
                Kid = kid,
                Alg = jwk.Alg,
                Jwk = jwk,
            };
            var payload = new JwtPayload(DateTimeOffset.Now, challenge, "setup", deviceInfo);
            var lockedSignature = KeyRepo.MakeSignature(keyPair.Private!);
            var cryptoObject = new CryptoObject(lockedSignature);
            var jwt = await Authenticate(promptInfo, cryptoObject, header, payload).ConfigureAwait(false);
            return new BiometricEnableResult { Kid = kid, Jwt = jwt };
        }

        public async Task<string> AuthenticateBiometricAsync(BiometricOptions options, string kid, string challenge, DeviceInfoRoot deviceInfo)
        {
            if (options.Android == null)
            {
                throw new ArgumentNullException(nameof(options.Android));
            }
            EnsureApiLevel();
            EnsureCanAuthenticate(options);
            var promptInfo = BuildPromptInfo(options.Android);
            var alias = string.Format(CultureInfo.InvariantCulture, AliasFormat, kid);
            try
            {
                var keyPair = GetPrivateKey(alias);
                var jwk = Jwk.FromPublicKey(kid, keyPair.Public!);
                var header = new JwtHeader
                {
                    Typ = JwtHeaderType.Biometric,
                    Kid = kid,
                    Alg = jwk.Alg,
                    Jwk = jwk,
                };
                var payload = new JwtPayload(DateTimeOffset.Now, challenge, "authenticate", deviceInfo);
                var lockedSignature = KeyRepo.MakeSignature(keyPair.Private!);
                var cryptoObject = new CryptoObject(lockedSignature);
                return await Authenticate(promptInfo, cryptoObject, header, payload).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw AuthgearException.Wrap(ex);
            }
        }

        private static PromptInfo BuildPromptInfo(BiometricOptionsAndroid options)
        {
            var authenticators = ToAuthenticators(options.AccessConstraint);
            return BuildPromptInfo(options.Title, options.Subtitle, options.Description, options.NegativeButtonText, authenticators);
        }

        private static PromptInfo BuildPromptInfo(string? title, string? subtitle, string? description, string? negativeButtonText, int authenticators)
        {
            var builder = new PromptInfo.Builder()
                .SetTitle(title)
                .SetSubtitle(subtitle)
                .SetDescription(description)
                .SetAllowedAuthenticators(authenticators);
            // DEVICE_CREDENTIAL and negativeButtonText are mutually exclusive.
            // If DEVICE_CREDENTIAL is absent, then negativeButtonText is mandatory.
            // If DEVICE_CREDENTIAL is present, then negativeButtonText must NOT be set.
            if ((authenticators & BiometricManager.Authenticators.DeviceCredential) == 0)
            {
                builder.SetNegativeButtonText(negativeButtonText);
            }
            return builder.Build();
        }

        private static KeyGenParameterSpec MakeGenerateKeyPairSpec(string alias, KeyPropertiesAuthType type, bool invalidatedByBiometricEnrollment)
        {
            var builder = new KeyGenParameterSpec.Builder(alias, KeyStorePurpose.Sign | KeyStorePurpose.Verify)
                .SetKeySize(KeySize)
                .SetDigests(KeyProperties.DigestSha256)
                .SetSignaturePaddings(KeyProperties.SignaturePaddingRsaPkcs1)
                .SetUserAuthenticationRequired(true);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
                builder.SetUserAuthenticationParameters(0 /* 0 means require authentication every time */, Convert.ToInt32(type, CultureInfo.InvariantCulture));
            }
            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                builder.SetInvalidatedByBiometricEnrollment(invalidatedByBiometricEnrollment);
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
            {
                // Samsung Android 12 treats setUnlockedDeviceRequired in a different way.
                // If setUnlockedDeviceRequired is true, then the device must be unlocked
                // with the same level of security requirement.
                // Otherwise, UserNotAuthenticatedException will be thrown when a cryptographic operation is initialized.
                //
                // The steps to reproduce the bug
                //
                // - Restart the device
                // - Unlock the device with credentials
                // - Create a Signature with a PrivateKey with setUnlockedDeviceRequired(true)
                // - Call Signature.initSign, UserNotAuthenticatedException will be thrown.
                //builder.SetUnlockedDeviceRequired(true);
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
            {
                // User confirmation is not needed because the BiometricPrompt itself is a kind of confirmation.
                // builder.setUserConfirmationRequired(true)
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
            {
                // User presence requires a physical button which is not our intended use case.
                // builder.setUserPresenceRequired(true)
            }
            return builder.Build();
        }

        private static KeyPair CreateKeyPair(KeyGenParameterSpec spec)
        {
            var generator = KeyPairGenerator.GetInstance(KeyProperties.KeyAlgorithmRsa, AndroidKeyStore)!;
            generator.Initialize(spec);
            return generator.GenerateKeyPair()!;
        }

        private static Task<string> Authenticate(PromptInfo promptInfo, CryptoObject cryptoObject, JwtHeader header, JwtPayload payload)
        {
            var taskSource = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            var prompt = new BiometricPrompt(Platform.CurrentActivity as FragmentActivity, new AuthenticationCallbackImpl(taskSource, header, payload));
            new Handler(Looper.MainLooper!).Post(() =>
            {
                prompt.Authenticate(promptInfo, cryptoObject);
            });
            return taskSource.Task;
        }
    }
}
