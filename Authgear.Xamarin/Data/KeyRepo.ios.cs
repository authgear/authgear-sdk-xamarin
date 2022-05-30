using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Authgear.Xamarin.DeviceInfo;
using Security;
using UIKit;

namespace Authgear.Xamarin.Data
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Xamarin Interop objects are managed")]
    internal class KeyRepo : IKeyRepo
    {
        private const string TagFormat = "com.authgear.keys.biometric.{0}";
        private const int KeySize = 2048;

        public Task<KeyJwtResult> GetOrCreateAnonymousJwtAsync(string keyId, string challenge, DeviceInfoRoot deviceInfo)
        {
            EnsureApiLevel();
            SecKey? privateKey;
            if (keyId == null)
            {
                keyId = Guid.NewGuid().ToString();
                var tag = string.Format(CultureInfo.InvariantCulture, TagFormat, keyId);
                privateKey = GeneratePrivateKey(tag);
            }
            else
            {
                var tag = string.Format(CultureInfo.InvariantCulture, TagFormat, keyId);
                privateKey = GetPrivateKey(tag);
                if (privateKey == null)
                {
                    throw new AnonymousUserNotFoundException();
                }
            }
            var jwk = Jwk.FromPrivateKey(keyId, privateKey);
            var header = new JwtHeader
            {
                Typ = JwtHeaderType.Anonymous,
                Jwk = jwk,
                Alg = jwk.Alg,
                Kid = jwk.Kid,
            };
            var payload = new JwtPayload(DateTimeOffset.Now, challenge, "auth", deviceInfo);
            var jwt = Jwt.Sign(privateKey, header, payload);
            return Task.FromResult(new KeyJwtResult
            {
                KeyId = keyId,
                Jwt = jwt
            });
        }

        public Task<string> PromoteAnonymousUserAsync(string keyId, string challenge, DeviceInfoRoot deviceInfo)
        {
            throw new NotImplementedException();
        }

        private static void EnsureApiLevel()
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(11, 3))
            {
                throw new InvalidOperationException("Anonymous user is only supported on 11.3+");
            }
        }

        private static SecKey GeneratePrivateKey(string tag)
        {
            var keyGenParam = new SecKeyGenerationParameters()
            {
                KeyType = SecKeyType.RSA,
                KeySizeInBits = KeySize
            };
            var privateKey = SecKey.CreateRandomKey(keyGenParam.Dictionary, out var error);
            if (error != null)
            {
                throw AuthgearException.Wrap(new AnonymousUserIosException(error));
            }
            var secRecord = new SecRecord(privateKey)
            {
                ApplicationTag = tag
            };
            var status = SecKeyChain.Add(secRecord);
            if (status != SecStatusCode.Success)
            {
                throw AuthgearException.Wrap(new AnonymousUserIosException(status));
            }
            return privateKey;
        }

        private static SecKey? GetPrivateKey(string tag)
        {
            var secRecord = new SecRecord(SecKind.Key)
            {
                KeyType = SecKeyType.RSA,
                ApplicationTag = tag
            };
            var privateKey = SecKeyChain.QueryAsConcreteType(secRecord, out var result);
            if (result != SecStatusCode.Success)
            {
                return null;
            }
            try
            {
                return (SecKey)privateKey;
            }
            catch
            {
                return null;
            }
        }
    }
}
