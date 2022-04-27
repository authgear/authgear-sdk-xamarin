using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin.Data
{
    public interface ITokenStorage
    {
        void SetRefreshToken(String aNamespace, String refreshToken);
        void GetRefreshToken(String aNamespace);
        void DeleteRefreshToken(String aNamespace);
    }
}
