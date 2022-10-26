using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Versioning;
using System.Text;
using Android.Content;
using Android.Content.PM;
using AndroidBuild = Android.OS.Build;
using AndroidSettings = Android.Provider.Settings;

namespace Authgear.Xamarin.DeviceInfo
{
    internal partial class DeviceInfoAndroid
    {
#if !Xamarin
        [SupportedOSPlatformGuard("android28.0")]
#endif
        private static bool IsAtLeastP()
        {
#if Xamarin
            return AndroidBuild.VERSION.SdkInt >= Android.OS.BuildVersionCodes.P;
#else
            return OperatingSystem.IsAndroidVersionAtLeast(28, 0);
#endif
        }

#if !Xamarin
        [SupportedOSPlatformGuard("android30.0")]
#endif
        private static bool IsAtLeastR()
        {
#if Xamarin
            return AndroidBuild.VERSION.SdkInt >= Android.OS.BuildVersionCodes.R;
#else
            return OperatingSystem.IsAndroidVersionAtLeast(30, 0);
#endif
        }

        public static DeviceInfoAndroid Get(Context context)
        {
            PackageInfo packageInfo;
            packageInfo = context.PackageManager!.GetPackageInfo(context.PackageName ?? "", 0)!;
            var contentResolver = context.ContentResolver;
            var baseOs = "";
            var previewSdkInt = "";
            var securityPatch = "";
            if (ApiLevelException.IsAtLeastM())
            {
                baseOs = AndroidBuild.VERSION.BaseOs ?? "";
                securityPatch = AndroidBuild.VERSION.SecurityPatch ?? "";
                previewSdkInt = AndroidBuild.VERSION.PreviewSdkInt.ToString(CultureInfo.InvariantCulture);
            }
            var longVersionCode = "";
            if (IsAtLeastP())
            {
                longVersionCode = packageInfo.LongVersionCode.ToString(CultureInfo.InvariantCulture);
            }
            var releaseOrCodeName = "";
            if (IsAtLeastR())
            {
                releaseOrCodeName = AndroidBuild.VERSION.ReleaseOrCodename;
            }
            return new DeviceInfoAndroid
            {
                Build = new DeviceInfoAndroidBuild
                {
                    Board = AndroidBuild.Board ?? "",
                    Brand = AndroidBuild.Brand ?? "",
                    Model = AndroidBuild.Model ?? "",
                    Device = AndroidBuild.Device ?? "",
                    Display = AndroidBuild.Display ?? "",
                    Hardware = AndroidBuild.Hardware ?? "",
                    Manufacturer = AndroidBuild.Manufacturer ?? "",
                    Product = AndroidBuild.Product ?? "",
                    Version = new DeviceInfoAndroidVersion
                    {
                        BaseOs = baseOs,
                        CodeName = AndroidBuild.VERSION.Codename ?? "",
                        Incremental = AndroidBuild.VERSION.Incremental ?? "",
                        PreviewSdkInt = previewSdkInt,
                        Release = AndroidBuild.VERSION.Release ?? "",
                        ReleaseOrCodeName = releaseOrCodeName,
#pragma warning disable CS0618 // Type or member is obsolete
                        Sdk = AndroidBuild.VERSION.Sdk ?? "",
#pragma warning restore CS0618 // Type or member is obsolete
                        SdkInt = AndroidBuild.VERSION.SdkInt.ToString(),
                        SecurityPatch = securityPatch
                    }
                },
                PackageInfo = new DeviceInfoAndroidPackageInfo
                {
                    PackageName = context.PackageName ?? "",
                    VersionName = packageInfo.VersionName ?? "",
#pragma warning disable CS0618 // Type or member is obsolete
                    VersionCode = packageInfo.VersionCode.ToString(CultureInfo.InvariantCulture),
#pragma warning restore CS0618 // Type or member is obsolete
                    LongVersionCode = longVersionCode
                },
                Settings = new DeviceInfoAndroidSettings
                {
                    Secure = new DeviceInfoAndroidSecure
                    {
                        BluetoothName = AndroidSettings.Secure.GetString(contentResolver, "bluetooth_name") ?? "",
                        AndroidId = AndroidSettings.Secure.GetString(contentResolver, AndroidSettings.Secure.AndroidId) ?? ""
                    },
                    Global = new DeviceInfoAndroidSettingsGlobal
                    {
                        DeviceName = AndroidSettings.Global.GetString(contentResolver, AndroidSettings.Global.DeviceName) ?? "",
                    }
                },
                AplicationInfoLabel = context.ApplicationInfo!.LoadLabel(context.PackageManager)
            };
        }
    }
}
