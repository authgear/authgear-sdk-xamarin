using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Authgear.Xamarin
{
    internal static class NetExtensions
    {
        public static string ToFormData(this Dictionary<string, string> dict)
        {
            return dict.ToQueryParameter();
        }
        public static string ToQueryParameter(this Dictionary<string, string> dict)
        {
            var builder = new StringBuilder();
            foreach (var entry in dict)
            {
                var key = WebUtility.UrlEncode(entry.Key);
                var value = WebUtility.UrlEncode(entry.Value);
                builder.Append($"{key}={value}&");
            }
            return builder.ToString();
        }
    }
}
