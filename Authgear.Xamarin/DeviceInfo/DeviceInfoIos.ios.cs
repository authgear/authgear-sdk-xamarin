using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
using UIKit;
using Xamarin.Essentials;

namespace Authgear.Xamarin
{
    public partial class DeviceInfoIos
    {
        [DllImport(Constants.SystemLibrary, EntryPoint = "sysctlbyname")]
        internal static extern int SysctlByName([MarshalAs(UnmanagedType.LPStr)] string property, IntPtr output, IntPtr oldLen, IntPtr newp, uint newlen);

        internal static string GetSystemLibraryProperty(string property)
        {
            var lengthPtr = Marshal.AllocHGlobal(sizeof(int));
            SysctlByName(property, IntPtr.Zero, lengthPtr, IntPtr.Zero, 0);

            var propertyLength = Marshal.ReadInt32(lengthPtr);

            if (propertyLength == 0)
            {
                Marshal.FreeHGlobal(lengthPtr);
                throw new InvalidOperationException("Unable to read length of property.");
            }

            var valuePtr = Marshal.AllocHGlobal(propertyLength);
            SysctlByName(property, valuePtr, lengthPtr, IntPtr.Zero, 0);

            var returnValue = Marshal.PtrToStringAnsi(valuePtr);

            Marshal.FreeHGlobal(lengthPtr);
            Marshal.FreeHGlobal(valuePtr);

            return returnValue;
        }

        private static string GetBySysCtlName(string name)
        {
            try
            {
                return GetSystemLibraryProperty(name);
            }
            catch
            {
                return "";
            }
        }

        public static DeviceInfoIos Get()
        {
            var infoDict = NSBundle.MainBundle.InfoDictionary;
            return new DeviceInfoIos
            {
                UName = new DeviceInfoIosUname
                {
                    // These are best-effort approximation
                    Machine = GetBySysCtlName("hw.machine"),
                    NodeName = DeviceInfo.Name,
                    Release = GetBySysCtlName("kern.osrelease"),
                    SysName = GetBySysCtlName("kern.ostype"),
                    Version = GetBySysCtlName("kern.version"),
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
