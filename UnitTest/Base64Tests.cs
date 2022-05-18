using Authgear.Xamarin.CsExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace UnitTest
{
    public class Base64Tests
    {
        [Fact]
        public void Base64_UrlEncodeFromBase64()
        {
            var input = "ab>ab?a";
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var base64 = Convert.ToBase64String(inputBytes);
            var base64Url = ConvertExtensions.ToBase64UrlSafeStringFromBase64(base64);
            Assert.Equal("YWI-YWI_YQ", base64Url);
        }

        [Fact]
        public void Base64_UrlEncodeFromStringBytes()
        {
            var input = "ab>ab?a";
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var base64Url = ConvertExtensions.ToBase64UrlSafeString(inputBytes);
            Assert.Equal("YWI-YWI_YQ", base64Url);
        }

        [Fact]
        public void Base64_UrlEncodeFromUtf8String()
        {
            var input = "ab>ab?a";
            var base64Url = ConvertExtensions.ToBase64UrlSafeString(input, Encoding.UTF8);
            Assert.Equal("YWI-YWI_YQ", base64Url);
        }

        [Fact]
        public void Base64_FromBase64UrlEncodedUtf8String()
        {
            var base64UrlEncoded = "YWI-YWI_YQ";
            var output = ConvertExtensions.FromBase64UrlSafeString(base64UrlEncoded, Encoding.UTF8);
            Assert.Equal("ab>ab?a", output);
        }
    }
}
