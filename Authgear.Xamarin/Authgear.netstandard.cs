using System;
using System.Collections.Generic;
using System.Text;
using Authgear.Xamarin.Data;
using Authgear.Xamarin.DeviceInfo;

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
            throw new NotImplementedException();
        }
    }
}
