using Authgear.Xamarin.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest
{
    [JsonConverter(typeof(JsonDescriptionEnumConverter<TestEnum>))]
    internal enum TestEnum
    {
        [Description("test_enum_a")]
        A,
        [Description("test_enum_b")]
        B
    }

    public class JsonEnumConverterTests
    {
        [Fact]
        public void JsonEnum_Correct()
        {
            var value = TestEnum.A;
            var json = JsonSerializer.Serialize(value);
            Assert.Equal("\"test_enum_a\"", json);
        }
    }
}
