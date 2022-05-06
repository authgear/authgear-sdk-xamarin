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

        Task<BiometricEnableResult> EnableBiometricAsync(BiometricOptions options, string challenge, DeviceInfoRoot deviceInfo);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="kid"></param>
        /// <param name="challenge"></param>
        /// <param name="deviceInfo"></param>
        /// <returns>Jwt</returns>
        Task<string> AuthenticateBiometricAsync(BiometricOptions options, string kid, string challenge, DeviceInfoRoot deviceInfo);
    }
}
