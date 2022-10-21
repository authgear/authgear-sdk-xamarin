using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
#if Xamarin
using Xamarin.Essentials;
#endif

namespace Authgear.Xamarin.Data
{
    public class PersistentTokenStorage : ITokenStorage
    {
        private const string KeyRefreshToken = "refresh_token";
        public void DeleteRefreshToken(string aNamespace)
        {
            SecureStorage.Remove($"{aNamespace}_{KeyRefreshToken}");
        }

        public async Task<string?> GetRefreshTokenAsync(string aNamespace)
        {
            return await SecureStorage.GetAsync($"{aNamespace}_{KeyRefreshToken}").ConfigureAwait(false);
        }

        public void SetRefreshToken(string aNamespace, string refreshToken)
        {
            SecureStorage.SetAsync($"{aNamespace}_{KeyRefreshToken}", refreshToken);
        }
    }
}
