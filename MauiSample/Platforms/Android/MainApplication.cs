using Android.App;
using Android.Runtime;
using AuthgearSample;

namespace MauiSample
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
            DependencyService.RegisterSingleton<IAuthgearFactory>(new AuthgearFactoryAndroid(this));
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}