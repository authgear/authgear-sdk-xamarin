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
            try
            {
                var refreshToken = refreshTokens[aNamespace];
                return Task.FromResult(refreshToken);
            }
            catch (KeyNotFoundException)
            {
                return Task.FromResult<string>(null);
            }
        }

        public void SetRefreshToken(string aNamespace, string refreshToken)
        {
            refreshTokens[aNamespace] = refreshToken;
        }
    }
}
