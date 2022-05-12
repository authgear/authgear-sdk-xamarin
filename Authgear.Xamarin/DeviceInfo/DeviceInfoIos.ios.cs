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
                    // TODO:
                },
                UiDevice = new DeviceInfoIosUiDevice
                {
                    Name = UIDevice.CurrentDevice.Name,
                    SystemName = UIDevice.CurrentDevice.SystemName,
                    SystemVersion = UIDevice.CurrentDevice.SystemVersion,
                    Model = UIDevice.CurrentDevice.Model,
                    UserInterfaceIdiom = UIDevice.CurrentDevice.UserInterfaceIdiom.ToString()
                },
                ProcessInfo = new DeviceInfoIosProcessInfo(),
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
