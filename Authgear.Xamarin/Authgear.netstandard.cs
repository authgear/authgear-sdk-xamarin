using Authgear.Xamarin.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    public partial class AuthgearSdk
    {
        AuthgearSdk()
        {
            biometric = new Biometric();
        }
        private DeviceInfoRoot PlatformGetDeviceInfo()
        {
            throw new NotImplementedException();
        }
    }
}
