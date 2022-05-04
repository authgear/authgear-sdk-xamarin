using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Authgear.Xamarin.Data
{
    internal class Biometric : IBiometric
    {
        internal Biometric()
        {
        }

        public Task<BiometricEnableResult> EnableBiometric(BiometricOptions options, string challenge, DeviceInfoRoot deviceInfo)
        {
            throw new NotImplementedException();
        }

        public void EnsureIsSupported(BiometricOptions options)
        {
            throw new NotImplementedException();
        }

        public void RemoveBiometric(string name)
        {
            throw new NotImplementedException();
        }
    }
}
