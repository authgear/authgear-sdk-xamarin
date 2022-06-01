using System;
using System.Collections.Generic;
using System.Text;
using Android.Content;
using Authgear.Xamarin.Data;
using Authgear.Xamarin.DeviceInfo;

namespace Authgear.Xamarin
{
    public partial class AuthgearSdk
    {
        private readonly Context context;
        public AuthgearSdk(Context context, AuthgearOptions options) : this(options)
        {
            this.context = context;
            biometric = new Biometric(context);
            keyRepo = new KeyRepo();
            webView = new WebView();
        }
        private DeviceInfoRoot PlatformGetDeviceInfo()
        {
            return new DeviceInfoRoot
            {
                Android = DeviceInfoAndroid.Get(context)
            };
        }
    }
}
