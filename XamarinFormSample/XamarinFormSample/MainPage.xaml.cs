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

        private async void Configure_Clicked(object sender, EventArgs e)
        {
            try
            {
                await MainViewModel.ConfigureAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void Authorize_Clicked(object sender, EventArgs e)
        {
            try
            {
                await MainViewModel.AuthorizeAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void Logout_Clicked(object sender, EventArgs e)
        {
            try
            {
                await MainViewModel.LogoutAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void EnableBiometric_Clicked(object sender, EventArgs e)
        {
            try
            {
                await MainViewModel.EnableBiometricAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void AuthenticateBiometric_Clicked(object sender, EventArgs e)
        {
            try
            {
                await MainViewModel.AuthenticateBiometricAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void AuthenticateAnontmously_Clicked(object sender, EventArgs e)
        {
            try
            {
                await MainViewModel.AuthenticateAnonymouslyAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
