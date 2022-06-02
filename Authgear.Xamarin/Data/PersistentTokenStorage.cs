using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Authgear.Xamarin.Data
{
    public class PersistentTokenStorage : ITokenStorage
    {
        private const string KeyRefreshToken = "refresh_token";
        public void DeleteRefreshToken(string aNamespace)
        {
            SecureStorage.Remove($"{aNamespace}_{KeyRefreshToken}");
        }

        public Task<string?> GetRefreshTokenAsync(string aNamespace)
        {
            return SecureStorage.GetAsync($"{aNamespace}_{KeyRefreshToken}");
        }

        public void SetRefreshToken(string aNamespace, string refreshToken)
        {
            SecureStorage.SetAsync($"{aNamespace}_{KeyRefreshToken}", refreshToken);
        }
    }
}
