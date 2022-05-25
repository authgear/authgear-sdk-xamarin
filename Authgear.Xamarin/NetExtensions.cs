using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

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
    }
}
