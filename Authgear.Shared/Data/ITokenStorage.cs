using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Authgear.Xamarin.Data
{
    public interface ITokenStorage
    {
        void SetRefreshToken(string aNamespace, string refreshToken);
        Task<string?> GetRefreshTokenAsync(string aNamespace);
        void DeleteRefreshToken(string aNamespace);
    }
}
