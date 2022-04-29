using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Authgear.Xamarin.Data
{
    public interface ITokenStorage
    {
        void SetRefreshToken(String aNamespace, String refreshToken);
        Task<string> GetRefreshTokenAsync(String aNamespace);
        void DeleteRefreshToken(String aNamespace);
    }
}
