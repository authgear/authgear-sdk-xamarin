using System;
using System.Collections.Generic;
using System.Text;
using Authgear.Xamarin.Data;
using Authgear.Xamarin.DeviceInfo;
using UIKit;

namespace Authgear.Xamarin
{
    public partial class AuthgearSdk
    {
        /// <summary>
        /// </summary>
        /// <param name="app">Dummy tag argument to denote this constructor is for ios</param>
        /// <param name="options"></param>
        public AuthgearSdk(UIApplication app, AuthgearOptions options) : this(options)
        {
            biometric = new Biometric();
            keyRepo = new KeyRepo();
            webView = new WebView();
        }
        private DeviceInfoRoot PlatformGetDeviceInfo()
        {
            return new DeviceInfoRoot
            {
                Ios = DeviceInfoIos.Get()
            };
        }
    }
}
