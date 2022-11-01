using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Authgear.Xamarin.CsExtensions;

namespace Authgear.Xamarin.Data
{
    internal class JsonDescriptionEnumConverter<T> : JsonConverter<T> where T : struct, Enum
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
            // TODO: Investigate how to do it properly without !
            return default!;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.GetDescription());
        }
    }
}
