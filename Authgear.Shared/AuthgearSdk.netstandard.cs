using System;
using System.Collections.Generic;
using System.Text;
using Authgear.Xamarin.Data;
using Authgear.Xamarin.DeviceInfo;

namespace Authgear.Xamarin
{
    public sealed partial class AuthgearSdk
    {
        // Netstandard is dummy implementation anyways so ignore errors
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        AuthgearSdk()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            biometric = new Biometric();
            keyRepo = new KeyRepo();
            webView = new WebView();
        }
        private DeviceInfoRoot PlatformGetDeviceInfo()
        {
            throw new NotImplementedException();
        }
    }
}
