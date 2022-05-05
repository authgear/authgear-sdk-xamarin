using Authgear.Xamarin.CsExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

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
    }
}
