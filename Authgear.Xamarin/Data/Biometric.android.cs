using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin.Data
{
    internal class Biometric : IBiometric
    {
        private readonly IContainerStorage containerStorage;
        internal Biometric(IContainerStorage containerStorage)
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
            // TODO:
        }
    }
}
