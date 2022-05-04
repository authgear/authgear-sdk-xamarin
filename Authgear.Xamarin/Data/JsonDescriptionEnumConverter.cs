using Authgear.Xamarin.CsExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Authgear.Xamarin.Data
{
    internal class JsonDescriptionEnumConverter<T> : JsonConverter<T> where T : Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var enumValue = reader.GetString();
            var values = Enum.GetValues(typeToConvert);
            foreach (T value in values)
            {
                if (enumValue == value.GetDescription())
                {
                    return value;
                }
            }
            return default;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.GetDescription());
        }
    }
}
