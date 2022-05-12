using Authgear.Xamarin.CsExtensions;
using Foundation;
using Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    internal partial class Jwk
    {
        public static Jwk FromPrivateKey(string kid, SecKey secKey)
        {
            var publicKey = secKey.GetPublicKey();
            var data = publicKey.GetExternalRepresentation();
            var size = data.Length;
            // Copy and pasted from flutter. TODO: Document what these magic numbers are.
            var modulus = data.Subdata(new NSRange(size > 269 ? 9 : 8, 256));
            var exponent = data.Subdata(new NSRange(Convert.ToInt32(size - 3), 3));
            // Note: Convert doesn't provide a stream based conversion so we have to copy anyways
            var maxSize = Math.Max(modulus.Length, exponent.Length);
            var buffer = new byte[maxSize];
            return new Jwk
            {
                Kid = kid,
                N = ConvertExtensions.ToBase64UrlSafeStringFromBase64(modulus.GetBase64EncodedString(NSDataBase64EncodingOptions.None)),
                E = ConvertExtensions.ToBase64UrlSafeStringFromBase64(modulus.GetBase64EncodedString(NSDataBase64EncodingOptions.None))
            };
        }
    }
}
