# Authgear SDK for Xamarin

## Build

### Prerequisites

- Xamarin workload (Android + iOS) in Visual Studio
- [Docfx](https://dotnet.github.io/docfx/tutorial/docfx_getting_started.html) (for doc generation)

## Documentation

View the API reference at [https://authgear.github.io/authgear-sdk-xamarin/](https://authgear.github.io/authgear-sdk-xamarin/).

View Authgear Documentation at [https://docs.authgear.com/](https://docs.authgear.com/)

## Known Limitation

### Build

The sample project can be built on both windows and macOS. The sample has a dependency on the library project, and building the sample would build the library automatically.

However, for packaging the library (e.g. in CICD), since the library project is using [MSBuild.Sdk.Extras](https://github.com/novotnyllc/MSBuildSdkExtras) for its single project, multi-targeting feature (that allows targeting Xamarin.iOS and Xamarin.Android at once within the same project), the project needs a "Desktop msbuild" to package the library. `dotnet pack` or `dotnet msbuild` would not work (it would complain needing desktop msbuild). **Since "Desktop msbuild" is only available on windows, currently, only windows is capable of packaging the library.** 

Current CI already uses windows image and packs the resultant nuget package as a github action artifact so developers shouldn't need to pack the library on their own.

.NET 6's [sdk style project](https://docs.microsoft.com/en-us/dotnet/standard/frameworks) natively support single project, multi-targeting for `net6.0-android`, `net6.0-ios` and can be built, packed with `dotnet build` and `dotnet pack` on both windows and macOS. But adopting it would obviously force customers to migrate to .NET 6, so that route was not taken. When appropriate, future upgrades to this SDK can simply revert `MSBuild.Sdk.Extras` to `Microsoft.NET.Sdk` and multi-target .NET 6+ mobile TFMs.

## Demo Apps

Two demo apps are included in the Github repo under `XamarinFormSample/`. They demonstrate the key features of the SDK on Android and iOS.

### XamarinFormSample.Android

To build the Android app:

1. Open the repo in Visual Studio
1. Make sure "Android SDK Build-Tools 31" is installed.
1. Plug in an Android device in USB debugging mode
1. Select **XamarinFormSample.Android \> Debug \> Your Device** in the top bar and Press Run
1. The demo app will be transferred to your device.

### XamarinFormSample.iOS

To build the iOS app:

1. Open the repo in Visual Studio
1. you need to select a **Provisioning Profile**
1. Right-click "XamarinFormSample.iOS" in the left bar and click "Options"
1. Go to "Build" and "iOS Bundle Signing", select a valid provisioning profile or choose **"Automatic"** for the desired "Configuration" and "Platform" and press "OK" to save the settings
1. Open "Info.plist" and set an appropriate "Bundle identifier"
1. Select **XamarinFormSample.iOS \> Debug \> Your Device** in the top bar and Press Run
