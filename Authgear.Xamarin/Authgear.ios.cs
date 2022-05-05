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
            keyRepo = new KeyRepo();
            webView = new WebView();
        }
        private DeviceInfoRoot PlatformGetDeviceInfo()
        {
            return new DeviceInfoRoot { };
        }
    }
}
