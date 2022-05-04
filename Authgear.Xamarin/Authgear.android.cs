using Android.Content;
using Authgear.Xamarin.Data;
using System;
using System.Collections.Generic;
using System.Text;

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
