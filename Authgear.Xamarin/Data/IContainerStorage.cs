using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin.Data
{
    internal interface IContainerStorage
    {
        void SetOidcCodeVerifier(string aNamespace, string verifier);
        string GetOidcCodeVerifier(string aNamespace);

        string GetAnonymousKeyId(string aNamespace);
        void SetAnonymousKeyId(string aNamespace, string keyId);
        void DeleteAnonymousKeyId(string aNamespace);

        string GetBiometricKeyId(string aNamespace);
        void SetBiometricKeyId(string aNamespace, string keyId);
        void DeleteBiometricKeyId(string aNamespace);
    }
}
