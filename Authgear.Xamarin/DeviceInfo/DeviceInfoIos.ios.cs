using System;
using UIKit;
using Foundation;

namespace Authgear.Xamarin
{
    public partial class DeviceInfoIos
    {
        public static DeviceInfoIos Get()
        {
            var infoDict = NSBundle.MainBundle.InfoDictionary;
            return new DeviceInfoIos
            {
                UName = new DeviceInfoIosUname
                {
                    // These are best-effort approximation
                    Machine = System.Environment.MachineName,
                    NodeName = System.Environment.UserDomainName,
                    Release = System.Environment.OSVersion.Version.ToString(),
                    SysName = System.Environment.OSVersion.Platform.ToString(),
                    Version = System.Environment.OSVersion.VersionString,
                },
                UiDevice = new DeviceInfoIosUiDevice
                {
                    Name = UIDevice.CurrentDevice.Name,
                    SystemName = UIDevice.CurrentDevice.SystemName,
                    SystemVersion = UIDevice.CurrentDevice.SystemVersion,
                    Model = UIDevice.CurrentDevice.Model,
                    UserInterfaceIdiom = UIDevice.CurrentDevice.UserInterfaceIdiom.ToString()
                },
                ProcessInfo = new DeviceInfoIosProcessInfo
                {
                    IsMacCatalystApp = UIDevice.CurrentDevice.CheckSystemVersion(13, 0) && NSProcessInfo.ProcessInfo.IsMacCatalystApplication,
                    IsIosAppOnMac = UIDevice.CurrentDevice.CheckSystemVersion(14, 0) && NSProcessInfo.ProcessInfo.IsiOSApplicationOnMac
                },
                Bundle = new DeviceInfoIosBundle
                {
                    CFBundleIdentifier = infoDict["CFBundleIdentifier"]?.ToString(),
                    CFBundleName = infoDict["CFBundleName"]?.ToString(),
                    CFBundleDisplayName = infoDict["CFBundleDisplayName"]?.ToString(),
                    CFBundleExecutable = infoDict["CFBundleExecutable"]?.ToString(),
                    CFBundleShortVersionString = infoDict["CFBundleShortVersionString"]?.ToString(),
                    CFBundleVersion = infoDict["CFBundleVersion"]?.ToString()
                }
            };
        }
    }
}
