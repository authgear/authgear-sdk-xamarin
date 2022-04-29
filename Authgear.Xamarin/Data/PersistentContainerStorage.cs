using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin.Data
{
    internal class PersistentContainerStorage : IContainerStorage
    {
        public void DeleteAnonymousKeyId(string aNamespace)
        {
            throw new NotImplementedException();
        }

        public void DeleteBiometricKeyId(string aNamespace)
        {
            throw new NotImplementedException();
        }

        public string GetAnonymousKeyId(string aNamespace)
        {
            throw new NotImplementedException();
        }

        public string GetBiometricKeyId(string aNamespace)
        {
            throw new NotImplementedException();
        }

        public string GetOidcCodeVerifier(string aNamespace)
        {
            throw new NotImplementedException();
        }

        public void SetAnonymousKeyId(string aNamespace, string keyId)
        {
            throw new NotImplementedException();
        }

        public void SetBiometricKeyId(string aNamespace, string keyId)
        {
            throw new NotImplementedException();
        }

        public void SetOidcCodeVerifier(string aNamespace, string verifier)
        {
            throw new NotImplementedException();
        }
    }
}
