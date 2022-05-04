using Android.Runtime;
using Authgear.Xamarin.CsExtensions;
using Java.Security;
using Java.Security.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    internal partial class Jwk
    {
        public static Jwk FromPublicKey(string kid, IPublicKey publicKey)
        {
            var rsaPublicKey = publicKey.JavaCast<IRSAPublicKey>();
            return new Jwk
            {
                Kid = kid,
                N = ConvertExtensions.ToBase64UrlSafeString(rsaPublicKey.Modulus.ToByteArray()),
                E = ConvertExtensions.ToBase64UrlSafeString(rsaPublicKey.PublicExponent.ToByteArray())
            };
        }
    }
}
