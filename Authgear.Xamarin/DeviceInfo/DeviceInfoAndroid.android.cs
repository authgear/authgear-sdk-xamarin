using System;
using System.Collections.Generic;
using System.Text;
using Android.Content;
using Android.Content.PM;
using AndroidBuild = Android.OS.Build;
using AndroidSettings = Android.Provider.Settings;

namespace Authgear.Xamarin
{
    public partial class DeviceInfoAndroid
    {
        public static DeviceInfoAndroid Get(Context context)
        {
            PackageInfo packageInfo;
            try
            {
                packageInfo = context.PackageManager.GetPackageInfo(context.PackageName, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var contentResolver = context.ContentResolver;
            var baseOs = "";
            var previewSdkInt = "";
            var securityPatch = "";
            if (AndroidBuild.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                baseOs = AndroidBuild.VERSION.BaseOs;
                securityPatch = AndroidBuild.VERSION.SecurityPatch;
                previewSdkInt = AndroidBuild.VERSION.PreviewSdkInt.ToString();
            }
            var longVersionCode = "";
            if (AndroidBuild.VERSION.SdkInt >= Android.OS.BuildVersionCodes.P)
            {
                longVersionCode = packageInfo.LongVersionCode.ToString();
            }
            var releaseOrCodeName = "";
            if (AndroidBuild.VERSION.SdkInt >= Android.OS.BuildVersionCodes.R)
            {
                releaseOrCodeName = AndroidBuild.VERSION.ReleaseOrCodename;
            }
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
                    Version = new DeviceInfoAndroidVersion
                    {
                        BaseOs = baseOs,
                        CodeName = AndroidBuild.VERSION.Codename,
                        Incremental = AndroidBuild.VERSION.Incremental,
                        PreviewSdkInt = previewSdkInt,
                        Release = AndroidBuild.VERSION.Release,
                        ReleaseOrCodeName = releaseOrCodeName,
#pragma warning disable CS0618 // Type or member is obsolete
                        Sdk = AndroidBuild.VERSION.Sdk,
#pragma warning restore CS0618 // Type or member is obsolete
                        SdkInt = AndroidBuild.VERSION.SdkInt.ToString(),
                        SecurityPatch = securityPatch
                    }
                },
                PackageInfo = new DeviceInfoAndroidPackageInfo
                {
                    PackageName = context.PackageName,
                    VersionName = packageInfo.VersionName,
#pragma warning disable CS0618 // Type or member is obsolete
                    VersionCode = packageInfo.VersionCode.ToString(),
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
                        DeviceName = AndroidSettings.Secure.GetString(contentResolver, AndroidSettings.Global.DeviceName) ?? "",
                    }
                },
                AplicationInfoLabel = context.ApplicationInfo.LoadLabel(context.PackageManager)
            };
        }
    }
}
