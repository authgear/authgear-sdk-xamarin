using Foundation;
using Security;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Authgear.Xamarin
{
    internal partial class Jwt
    {
        public static string Sign(SecKey privateKey, JwtHeader header, JwtPayload payload)
        {
            return Sign(header, payload, (input) =>
            {
                using (var sha256 = SHA256.Create())
                {
                    sha256.Initialize();
                    var hash = sha256.ComputeHash(input);
                    var hashNsData = NSData.FromArray(hash);
                    var signedNsData = privateKey.CreateSignature(SecKeyAlgorithm.RsaSignatureDigestPkcs1v15Sha256, hashNsData, out var error);
                    if (error != null)
                    {
                        throw new BiometricIosException(error);
                    }
                    // According to the following ref, this is faster than .ToArray()
                    // https://stackoverflow.com/questions/6239636/how-to-go-from-nsdata-to-byte?msclkid=6c02bbb8d11b11ecab5688675cf7d0a3
                    var signedData = new byte[signedNsData.Length];
                    System.Runtime.InteropServices.Marshal.Copy(signedNsData.Bytes, signedData, 0, Convert.ToInt32(signedNsData.Length));
                    return signedData;
                }
            });
        }
    }
}
