using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Authgear.Xamarin.Data
{
    internal interface IContainerStorage
    {
        void SetOidcCodeVerifier(string aNamespace, string verifier);
        Task<string> GetOidcCodeVerifier(string aNamespace);

        Task<string> GetAnonymousKeyId(string aNamespace);
        void SetAnonymousKeyId(string aNamespace, string keyId);
        void DeleteAnonymousKeyId(string aNamespace);

        Task<string> GetBiometricKeyId(string aNamespace);
        void SetBiometricKeyId(string aNamespace, string keyId);
        void DeleteBiometricKeyId(string aNamespace);
    }
}
