using Authgear.Xamarin;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace UnitTest
{
    public class CodeVerifierTests
    {
        private class FixedGenerator : RandomNumberGenerator
        {
            public override void GetBytes(byte[] data)
            {
                var length = data.Length;
                for (var i = 0; i < length; i++)
                {
                    data[i] = Convert.ToByte(i);
                }
            }
        }
        [Fact]
        public void CodeVerifier_Create()
        {
            var codeVerifier = new CodeVerifier(new FixedGenerator());
            Assert.Equal("000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f", codeVerifier.Verifier);
            Assert.Equal("bIbGqsX7JLz12ZOct9fVZFzjlBj0SeA7Ji3U-hS0uSs", codeVerifier.Challenge);
        }
    }
}
