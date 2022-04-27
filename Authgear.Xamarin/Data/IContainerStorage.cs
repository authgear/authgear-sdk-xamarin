using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin.Data
{
    internal interface IContainerStorage
    {
        void SetOIDCCodeVerifier(string aNamespace, string verifier);
        string GetOIDCCodeVerifier(string aNamespace);

        string GetAnonymousKeyId(string aNamespace);
        void SetAnonymousKeyId(string aNamespace, string keyId);
        void DeleteAnonymousKeyId(string aNamespace);

        string GetBiometricKeyId(string aNamespace);
        void SetBiometricKeyId(string aNamespace, string keyId);
        void DeleteBiometricKeyId(string aNamespace);
    }
}
