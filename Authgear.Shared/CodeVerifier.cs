using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Authgear.Xamarin.CsExtensions;

namespace Authgear.Xamarin
{
    internal class CodeVerifier
    {
        public string Verifier { get; private set; }
        public string Challenge { get; private set; }
        public CodeVerifier(RandomNumberGenerator generator)
        {
            const int byteCount = 32;
            var bytes = new byte[byteCount];
            using (var provider = generator)
            {
                provider.GetBytes(bytes);
                Verifier = string.Join("", bytes.Select(x => x.ToString("x2", CultureInfo.InvariantCulture)));
            }
            Challenge = ComputeCodeChallenge(Verifier);
        }

        private static string ComputeCodeChallenge(string verifier)
        {
            var hash = Sha256(verifier);
            return ConvertExtensions.ToBase64UrlSafeString(hash);
        }

        private static byte[] Sha256(string input)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        }
    }
}
