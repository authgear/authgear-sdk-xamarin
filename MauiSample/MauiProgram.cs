using Microsoft.Maui.LifecycleEvents;
using AuthgearSample;

namespace MauiSample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureLifecycleEvents(events =>
                {
#if IOS
                    events.AddiOS(ios => ios.FinishedLaunching((app, dict) =>
                    {
                        DependencyService.RegisterSingleton<IAuthgearFactory>(new AuthgearFactoryIos());
                        return true;
                    }));
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            return builder.Build();
        }
    }
}