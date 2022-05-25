using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UnitTest")]
namespace Authgear.Xamarin
{
    internal static class NetExtensions
    {
        public static string PercentEncode(string s)
        {
            return Uri.EscapeDataString(s);
        }

        public static string URLPercentEncode(string s)
        {
            return Uri.EscapeDataString(s).Replace("%20", "+");
        }

        public static string PercentDecode(string s)
        {
            return Uri.UnescapeDataString(s);
        }

        public static string URLPercentDecode(string s)
        {
            return Uri.UnescapeDataString(s.Replace("+", "%20"));
        }

        public static string ToFormData(this Dictionary<string, string> dict)
        {
            var arr = new List<string>();
            foreach (var entry in dict)
            {
                var key = PercentEncode(entry.Key);
                var value = PercentEncode(entry.Value);
                arr.Add($"{key}={value}");
            }
            return string.Join("&", arr);
        }

        public static string ToQueryParameter(this Dictionary<string, string> dict)
        {
            var arr = new List<string>();
            foreach (var entry in dict)
            {
                var key = URLPercentEncode(entry.Key);
                var value = URLPercentEncode(entry.Value);
                arr.Add($"{key}={value}");
            }
            return string.Join("&", arr);
        }

        /// <summary>
        /// HttpUtility is not available in android and ios. Polyfill and use dictionary instead.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseQueryString(this Uri uri)
        {
            var dict = new Dictionary<string, string>();
            var query = uri.Query;
            if (query == null)
            {
                return dict;
            }

            query = query.Replace("?", "");
            var entries = query.Split('&');
            foreach (var entry in entries)
            {
                var keyValue = entry.Split('=');
                if (keyValue.Length == 2)
                {
                    var key = URLPercentDecode(keyValue[0]);
                    var value = URLPercentDecode(keyValue[1]);
                    dict.Add(key, value);
                }
                else if (keyValue[0] != null && keyValue[0].Length > 0)
                {
                    var key = URLPercentDecode(keyValue[0]);
                    dict.Add(key, "");
                }
            }
            return dict;
        }
    }
}
