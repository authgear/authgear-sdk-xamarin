using Authgear.Xamarin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinFormSample
{
    public partial class MainPage : ContentPage
    {
        private readonly AuthgearSdk authgear;
        public MainPage()
        {
            InitializeComponent();
            authgear = DependencyService.Get<AuthgearSdk>();
        }

        private void Configure_Clicked(object sender, EventArgs e)
        {
            _ = Configure();
        }

        private async Task Configure()
        {
            try
            {
                await authgear.Configure();
            }
            catch (Exception error)
            {
                await DisplayAlert("Error", error.Message, "OK");
            }
        }
    }
}
