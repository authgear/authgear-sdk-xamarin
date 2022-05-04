using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Authgear.Xamarin.Data
{
    internal static class AuthgearJson
    {
        private static readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
        };
        public static T Deserialize<T>(Stream stream)
        {
            return JsonSerializer.Deserialize<T>(stream, options);
        }
        public static T Deserialize<T>(string input)
        {
            return JsonSerializer.Deserialize<T>(input, options);
        }
        public static string Serialize<T>(T value)
        {
            return JsonSerializer.Serialize(value, options);
        }
    }
}
