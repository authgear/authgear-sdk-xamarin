using Authgear.Xamarin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
            MainViewModel.ErrorRaised += async (s, e) =>
            {
                await DisplayAlert("Error", e.Message, "OK");
            };
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

        private async void Authenticate_Clicked(object sender, EventArgs e)
        {
            try
            {
                await MainViewModel.AuthenticateAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "OK");
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

        private async void DisableBiometric_Clicked(object sender, EventArgs e)
        {
            try
            {
                await MainViewModel.DisableBiometricAsync();
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

        private async void ReauthenticateWeb_Clicked(object sender, EventArgs e)
        {
            try
            {
                await MainViewModel.ReAuthenticateAsync(false);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void Reauthenticate_Clicked(object sender, EventArgs e)
        {
            try
            {
                await MainViewModel.ReAuthenticateAsync(true);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OpenSettings_Clicked(object sender, EventArgs e)
        {
            try
            {
                await MainViewModel.OpenAsync(SettingsPage.Settings);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void FetchUserInfo_Clicked(object sender, EventArgs e)
        {
            try
            {
                await MainViewModel.FetchUserInfoAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
