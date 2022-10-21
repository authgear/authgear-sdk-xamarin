using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Authgear.Xamarin.Data
{
    internal class PersistentContainerStorage : IContainerStorage
    {
        private const string KeyAnonymousId = "anonymousId";
        private const string KeyBiometricKeyId = "biometricKeyId";
        public void DeleteAnonymousKeyId(string aNamespace)
        {
            SecureStorage.Remove($"{aNamespace}_{KeyAnonymousId}");
        }

        public void DeleteBiometricKeyId(string aNamespace)
        {
            SecureStorage.Remove($"{aNamespace}_{KeyBiometricKeyId}");
        }

        public Task<string> GetAnonymousKeyIdAsync(string aNamespace)
        {
            return SecureStorage.GetAsync($"{aNamespace}_{KeyAnonymousId}");
        }

        public Task<string> GetBiometricKeyIdAsync(string aNamespace)
        {
            return SecureStorage.GetAsync($"{aNamespace}_{KeyBiometricKeyId}");
        }

        public void SetAnonymousKeyId(string aNamespace, string keyId)
        {
            SecureStorage.SetAsync($"{aNamespace}_{KeyAnonymousId}", keyId);
        }

        public void SetBiometricKeyId(string aNamespace, string keyId)
        {
            SecureStorage.SetAsync($"{aNamespace}_{KeyBiometricKeyId}", keyId);
        }
    }
}
