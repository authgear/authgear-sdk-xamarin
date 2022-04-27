using Authgear.Xamarin;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinFormSample
{
    public partial class App : Application
    {
        private static readonly AuthgearSdk Authgear = new AuthgearSdk(new AuthgearOptions
        {
            ClientId = "",
            AuthgearEndpoint = ""
        });
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
