using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin.CsExtensions
{   
    internal static class ConvertExtensions
    {
        public static string ToBase64UrlSafeStringFromBase64(string base64)
        {
            return base64.Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
        public static string ToBase64UrlSafeString(byte[] input)
        {
            var base64 = Convert.ToBase64String(input, Base64FormattingOptions.None);
            return ToBase64UrlSafeStringFromBase64(base64);
        }

        public static string ToBase64UrlSafeString(string inputStr, Encoding encoding)
        {
            var input = encoding.GetBytes(inputStr);
            return ToBase64UrlSafeString(input);
        }

        // Ref: https://www.rfc-editor.org/rfc/rfc7515.html
        public static byte[] FromBase64UrlSafeString(string input)
        {
            var base64 = input.Replace("-", "+").Replace("_", "/");
            switch (base64.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: base64 += "=="; break; // Two pad chars
                case 3: base64 += "="; break; // One pad char
                default:
                    throw new AuthgearException("Illegal base64url string!");
            }
            return Convert.FromBase64String(base64);
        }

        public static string FromBase64UrlSafeString(string input, Encoding encoding)
        {
            var bytes = FromBase64UrlSafeString(input);
            return encoding.GetString(bytes);
        }
    }
}
