# Authgear SDK for Flutter

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
