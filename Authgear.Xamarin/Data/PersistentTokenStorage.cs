using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin.Data
{
    internal class PersistentTokenStorage : ITokenStorage
    {
        public void DeleteRefreshToken(string aNamespace)
        {
            throw new NotImplementedException();
        }

        public string GetRefreshToken(string aNamespace)
        {
            throw new NotImplementedException();
        }

        public void SetRefreshToken(string aNamespace, string refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}
