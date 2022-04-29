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
        public MainViewModel MainViewModel { get; private set; }
        public MainPage()
        {
            InitializeComponent();
            MainViewModel = new MainViewModel();
            BindingContext = MainViewModel;
        }

        private void Configure_Clicked(object sender, EventArgs e)
        {
            _ = ConfigureAsync();
        }

        private async Task ConfigureAsync()
        {
            try
            {
                await MainViewModel.ConfigureAsync();
            }
            catch (Exception error)
            {
                await DisplayAlert("Error", error.Message, "OK");
            }
        }
    }
}
