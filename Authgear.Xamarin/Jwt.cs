using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Authgear.Xamarin.CsExtensions;
using Authgear.Xamarin.Data;

namespace Authgear.Xamarin
{
    internal partial class Jwt
    {
        public static JsonDocument Decode(string jwt)
        {
            var parts = jwt.Split('.');
            if (parts.Length != 3)
            {
                throw new AuthgearException($"invalid jwt: {jwt}");
            }
            var base64UrlEncoded = parts[1];
            var utf8 = ConvertExtensions.FromBase64UrlSafeString(base64UrlEncoded, Encoding.UTF8);
            return JsonDocument.Parse(utf8);
        }
        private static string Sign(JwtHeader header, JwtPayload payload, Func<byte[], byte[]> signer)
        {
            var headerStr = ConvertExtensions.ToBase64UrlSafeString(AuthgearJson.Serialize(header), Encoding.UTF8);
            var payloadStr = ConvertExtensions.ToBase64UrlSafeString(AuthgearJson.Serialize(payload), Encoding.UTF8);
            var data = $"{headerStr}.{payloadStr}";
            var sig = signer(Encoding.UTF8.GetBytes(data));
            return $"{data}.{ConvertExtensions.ToBase64UrlSafeString(sig)}";
        }
    }
}
