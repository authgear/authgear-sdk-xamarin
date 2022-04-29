using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    internal static class UriExtensions
    {
        /// <summary>
        /// HttpUtility is not available in android and ios. Polyfill and use dictionary instead.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseQueryString(this Uri uri)
        {
            var query = uri.Query.Replace("?", "");
            var entries = query.Split('&');
            var dict = new Dictionary<string, string>();
            foreach (var entry in entries)
            {
                var keyValue = entry.Split('=');
                if (keyValue.Length == 2)
                {
                    dict.Add(keyValue[0], keyValue[1]);
                }
                else if (keyValue[0] != null && keyValue[0].Length > 0)
                {
                    dict.Add(keyValue[0], null);
                }
            }
            return dict;
        }
    }
}
