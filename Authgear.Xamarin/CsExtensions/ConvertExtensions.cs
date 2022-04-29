using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin.CsExtensions
{   
    internal static class ConvertExtensions
    {
        public static string ToBase64UrlSafeString(byte[] input)
        {
            var base64 = Convert.ToBase64String(input, Base64FormattingOptions.None);
            return base64.Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
        public static string ToBase64UrlSafeString(string inputStr, Encoding encoding)
        {
            var input = encoding.GetBytes(inputStr);
            return ToBase64UrlSafeString(input);
        }
    }
}
