using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin.Data
{
    internal class BiometricAndroid : IBiometric
    {
        private readonly IContainerStorage containerStorage;
        internal BiometricAndroid(IContainerStorage containerStorage)
        {
            this.containerStorage = containerStorage;
        }
        public void RemoveBiometric(string name)
        {
            var kid = containerStorage.GetBiometricKeyId(name);
            if (kid != null)
            {
                var alias = $"com.authgear.keys.biometric.{kid}";
                RemovePrivateKey(alias);
                containerStorage.DeleteBiometricKeyId(name);
            }
        }
        private void RemovePrivateKey(string alias)
        {
            throw new NotImplementedException();
        }
    }
}
