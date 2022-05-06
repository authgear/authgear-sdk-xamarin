using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Authgear.Xamarin.Data
{
    public class TransientTokenStorage : ITokenStorage
    {
        private readonly Dictionary<string, string> refreshTokens = new Dictionary<string, string>();
        public void DeleteRefreshToken(string aNamespace)
        {
            refreshTokens.Remove(aNamespace);
        }

        public Task<string> GetRefreshTokenAsync(string aNamespace)
        {
            return Task.FromResult(refreshTokens[aNamespace]);
        }

        public void SetRefreshToken(string aNamespace, string refreshToken)
        {
            refreshTokens[aNamespace] = refreshToken;
        }
    }
}
