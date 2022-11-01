using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text;
using Android.OS;

namespace Authgear.Xamarin
{
    public partial class ApiLevelException : AuthgearException
    {
#if !Xamarin
        [SupportedOSPlatformGuard("android23.0")]
#endif
        public static bool IsAtLeastM()
        {
#if Xamarin
            return Build.VERSION.SdkInt >= BuildVersionCodes.M;
#else
            return OperatingSystem.IsAndroidVersionAtLeast(23, 0);
#endif
        }

        public ApiLevelException(string msg) : base(msg) { }
    }
}
