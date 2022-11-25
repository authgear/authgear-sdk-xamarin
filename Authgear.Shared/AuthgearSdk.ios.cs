using System;
using System.Collections.Generic;
using System.Text;
using Authgear.Xamarin.Data;
using Authgear.Xamarin.DeviceInfo;
using UIKit;

namespace Authgear.Xamarin
{
    public sealed partial class AuthgearSdk
    {
        /// <summary>
        /// </summary>
        /// <param name="app">Dummy tag argument to denote this constructor is for ios</param>
        /// <param name="options"></param>
        public AuthgearSdk(UIApplication _, AuthgearOptions options) : this(options)
        {
            biometric = new Biometric();
            keyRepo = new KeyRepo();
            webView = new WebView();
        }
        // Other platform's implementation is not static
#pragma warning disable CA1822 // Mark members as static
        private DeviceInfoRoot PlatformGetDeviceInfo()
#pragma warning restore CA1822 // Mark members as static
        {
            return new DeviceInfoRoot
            {
                Ios = DeviceInfoIos.Get()
            };
        }
    }
}
