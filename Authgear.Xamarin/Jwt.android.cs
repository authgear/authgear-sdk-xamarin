using System;
using System.Collections.Generic;
using System.Text;
using Authgear.Xamarin.CsExtensions;
using Authgear.Xamarin.Data;
using Java.Security;

namespace Authgear.Xamarin
{
    internal partial class Jwt
    {
        public static string Sign(Signature signature, JwtHeader header, JwtPayload payload)
        {
            var headerStr = ConvertExtensions.ToBase64UrlSafeString(AuthgearJson.Serialize(header), Encoding.UTF8);
            var payloadStr = ConvertExtensions.ToBase64UrlSafeString(AuthgearJson.Serialize(payload), Encoding.UTF8);
            var data = $"{headerStr}.{payloadStr}";
            signature.Update(Encoding.UTF8.GetBytes(data));
            var sig = signature.Sign();
            return $"{data}.{ConvertExtensions.ToBase64UrlSafeString(sig)}";
        }
    }
}
