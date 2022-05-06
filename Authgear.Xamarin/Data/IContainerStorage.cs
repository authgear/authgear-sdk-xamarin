using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Authgear.Xamarin.Data
{
    internal interface IContainerStorage
    {
        Task<string> GetAnonymousKeyIdAsync(string aNamespace);
        void SetAnonymousKeyId(string aNamespace, string keyId);
        void DeleteAnonymousKeyId(string aNamespace);

        Task<string> GetBiometricKeyIdAsync(string aNamespace);
        void SetBiometricKeyId(string aNamespace, string keyId);
        void DeleteBiometricKeyId(string aNamespace);
    }
}
