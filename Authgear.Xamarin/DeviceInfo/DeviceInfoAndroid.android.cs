using Android.Content;
using System;
using System.Collections.Generic;
using System.Text;

using AndroidBuild = Android.OS.Build;

namespace Authgear.Xamarin
{
    public partial class DeviceInfoAndroid
    {
        public static DeviceInfoAndroid Get(Context context)
        {
            return new DeviceInfoAndroid
            {
                Build = new DeviceInfoAndroidBuild
                {
                    Board = AndroidBuild.Board,
                    Brand = AndroidBuild.Brand,
                    Model = AndroidBuild.Model,
                    Device = AndroidBuild.Device,
                    Display = AndroidBuild.Display,
                    Hardware = AndroidBuild.Hardware,
                    Manufacturer = AndroidBuild.Manufacturer,
                    Product = AndroidBuild.Product,
                },
                PackageInfo = new DeviceInfoAndroidPackageInfo
                {

                },
                Settings = new DeviceInfoAndroidSettings
                {
                    Secure = new DeviceInfoAndroidSecure
                    {

                    },
                    Global = new DeviceInfoAndroidSettingsGlobal
                    {

                    }
                }
            };
        }
    }
}
