using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Android.Security.Keystore;
using Authgear.Xamarin.Data;
using Authgear.Xamarin.DeviceInfo;
using Java.Security;

namespace Authgear.Xamarin
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Xamarin objects are managed")]
    internal class KeyRepo : IKeyRepo
    {
        private const string AliasFormat = "com.authgear.keys.anonymous.{0}";
        private const string AndroidKeyStore = "AndroidKeyStore";
        public Task<KeyJwtResult> GetOrCreateAnonymousJwtAsync(string keyId, string challenge, DeviceInfoRoot deviceInfo)
        {
            KeyPair keyPair;
            if (keyId == null)
            {
                keyId = Guid.NewGuid().ToString();
                keyPair = GenerateAnonymousKey(keyId);
            }
            else
            {
                keyPair = GetAnonymousKey(keyId);
                if (keyPair == null)
                {
                    throw new AnonymousUserNotFoundException();
                }
            }
            var jwk = Jwk.FromPublicKey(keyId, keyPair.Public!);
            var header = new JwtHeader
            {
                Typ = JwtHeaderType.Anonymous,
                Jwk = jwk,
                Alg = jwk.Alg,
                Kid = jwk.Kid,
            };
            var payload = new JwtPayload(DateTimeOffset.Now, challenge, "auth", deviceInfo);
            var signature = MakeSignature(keyPair.Private!);
            var jwt = Jwt.Sign(signature, header, payload);
            return Task.FromResult(new KeyJwtResult { Jwt = jwt, KeyId = keyId });
        }

        public Task<string> PromoteAnonymousUserAsync(string keyId, string challenge, DeviceInfoRoot deviceInfo)
        {
            var keyPair = GetAnonymousKey(keyId) ?? throw new AnonymousUserNotFoundException();
            var jwk = Jwk.FromPublicKey(keyId, keyPair.Public!);
            var header = new JwtHeader
            {
                Typ = JwtHeaderType.Anonymous,
                Jwk = jwk,
                Alg = jwk.Alg,
                Kid = jwk.Kid,
            };
            var payload = new JwtPayload(DateTimeOffset.Now, challenge, "promote", deviceInfo);
            var signature = MakeSignature(keyPair.Private!);
            var jwt = Jwt.Sign(signature, header, payload);
            return Task.FromResult(jwt);
        }

        public static Signature MakeSignature(IPrivateKey privateKey)
        {
            var signature = Signature.GetInstance("SHA256withRSA")!;
            signature.InitSign(privateKey);
            return signature;
        }

        private static KeyPair GenerateAnonymousKey(string keyId)
        {
            string alias = string.Format(CultureInfo.InvariantCulture, AliasFormat, keyId);
            var kpg = KeyPairGenerator.GetInstance(KeyProperties.KeyAlgorithmRsa, AndroidKeyStore)!;
            var spec = new KeyGenParameterSpec.Builder(alias, KeyStorePurpose.Sign | KeyStorePurpose.Verify)
                .SetDigests(KeyProperties.DigestSha256)
                .SetSignaturePaddings(KeyProperties.SignaturePaddingRsaPkcs1)
                .Build();
            kpg.Initialize(spec);
            return kpg.GenerateKeyPair()!;
        }

        private static KeyPair GetAnonymousKey(string keyId)
        {
            var alias = string.Format(CultureInfo.InvariantCulture, AliasFormat, keyId);
            var ks = KeyStore.GetInstance(AndroidKeyStore)!;
            ks.Load(null);
            var entry = ks.GetEntry(alias, null);
            var privateKeyEntry = entry as KeyStore.PrivateKeyEntry;
            if (privateKeyEntry == null)
            {
                throw new KeyPermanentlyInvalidatedException();
            }
            return new KeyPair(privateKeyEntry.Certificate!.PublicKey, privateKeyEntry.PrivateKey);
        }
    }
}
