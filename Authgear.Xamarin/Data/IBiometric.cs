using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Authgear.Xamarin
{
    internal interface IBiometric
    {
        void RemoveBiometric(string name);

        void EnsureIsSupported(BiometricOptions options);

        Task<BiometricEnableResult> EnableBiometric(BiometricOptions options, string challenge, DeviceInfoRoot deviceInfo);
    }
}
