using Authgear.Xamarin;
using Authgear.Xamarin.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinFormSample
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public delegate void ErrorRaisedHandler(object sender, Exception e);
        private AuthgearSdk authgear;
        private readonly IAuthgearFactory authgearFactory;
        public readonly string RedirectUri = "com.authgear.exampleapp.xamarin://host/path";
        public string ClientId { get; set; }
        public string AuthgearEndpoint { get; set; }
        public SessionState SessionState { get; set; } = SessionState.Unknown;
        public string State { get; private set; } = "<no-authgear-instance>";
        public bool IsNotLoading { get; private set; } = true;
        public bool IsLoading { get; private set; } = false;

        public bool UseTransientStorage { get; set; }

        public bool ShareSessioWithDeviceBrowser { get; set; }

        public AuthenticatePage? AuthenticatePageToShow { get; set; }
        public UserInfo UserInfo { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public event ErrorRaisedHandler ErrorRaised;

        public Command OpenCommand { get; private set; }
        public MainViewModel()
        {
            authgearFactory = DependencyService.Get<IAuthgearFactory>();
            OpenCommand = new Command(async (param) =>
            {
                var page = (SettingsPage)param;
                try
                {
                    await OpenAsync(page);
                }
                catch (Exception ex)
                {
                    ErrorRaised?.Invoke(this, ex);
                }
            });
        }
        public async Task ConfigureAsync()
        {
            try
            {
                SetIsLoading(true);
                // Ternary operator not working in C#7.3, only availabe in 9.0 :thinking:
                ITokenStorage tokenStorage;
                if (UseTransientStorage) { tokenStorage = new TransientTokenStorage(); }
                else { tokenStorage = new PersistentTokenStorage(); }
                authgear = authgearFactory.CreateAuthgear(new AuthgearOptions
                {
                    ClientId = ClientId,
                    AuthgearEndpoint = AuthgearEndpoint,
                    TokenStorage = tokenStorage,
                    ShareSessionWithSystemBrowser = ShareSessioWithDeviceBrowser
                });
                State = authgear.SessionState.ToString();
                authgear.SessionStateChange += (sender, e) =>
                {
                    SetState(authgear.SessionState);
                };
                SetState(authgear.SessionState);
                await authgear.ConfigureAsync();
            }
            finally
            {
                SetIsLoading(false);
            }
        }

        private void EnsureAuthgear()
        {
            if (authgear == null)
            {
                throw new InvalidOperationException("Authgear is not configured. Did you forget to click configure?");
            }
        }

        public async Task AuthenticateAnonymouslyAsync()
        {
            EnsureAuthgear();
            try
            {
                SetIsLoading(true);
                var userInfo = await authgear.AuthenticateAnonymouslyAsync();
                UserInfo = userInfo;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserInfo)));
            }
            finally
            {
                SetIsLoading(false);
            }
        }

        public async Task PromoteAnonymousUserAsync()
        {
            EnsureAuthgear();
            try
            {
                SetIsLoading(true);
                var result = await authgear.PromoteAnonymousUserAsync(new PromoteOptions
                {
                    RedirectUri = RedirectUri,
                });
                Debug.WriteLine(result.State ?? "No state");
                UserInfo = result.UserInfo;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserInfo)));
            }
            finally
            {
                SetIsLoading(false);
            }
        }

        public async Task OpenAsync(SettingsPage page)
        {
            EnsureAuthgear();
            try
            {
                SetIsLoading(true);
                await authgear.OpenAsync(page);
            }
            finally
            {
                SetIsLoading(false);
            }
        }

        public async Task AuthenticateAsync()
        {
            EnsureAuthgear();
            try
            {
                SetIsLoading(true);
                var result = await authgear.AuthenticateAsync(new AuthenticateOptions
                {
                    RedirectUri = RedirectUri,
                    Page = AuthenticatePageToShow
                });
                Debug.WriteLine(result.State ?? "No state");
                UserInfo = result.UserInfo;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserInfo)));
            }
            finally
            {
                SetIsLoading(false);
            }
        }

        public async Task FetchUserInfoAsync()
        {
            EnsureAuthgear();
            try
            {
                SetIsLoading(true);
                var userInfo = await authgear.FetchUserInfoAsync();
                UserInfo = userInfo;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserInfo)));
            }
            finally
            {
                SetIsLoading(false);
            }
        }

        public async Task ReAuthenticateAsync(bool useBiometric)
        {
            EnsureAuthgear();
            try
            {
                SetIsLoading(true);
                await authgear.RefreshIdTokenAsync();
                var result = await authgear.ReauthenticateAsync(new ReauthenticateOptions
                {
                    RedirectUri = RedirectUri,
                }, useBiometric ? new BiometricOptions
                {

                } : null);
                UserInfo = result.UserInfo;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserInfo)));
            }
            finally
            {
                SetIsLoading(false);
            }
        }

        public async Task AuthenticateBiometricAsync()
        {
            EnsureAuthgear();
            try
            {
                SetIsLoading(true);
                var result = await authgear.AuthenticateBiometricAsync(new BiometricOptions
                {
                    Android = new BiometricOptionsAndroid
                    {
                        Title = "Authenticate biometric title",
                        Subtitle = "subtitle",
                        Description = "description",
                        NegativeButtonText = "Cancel",
                        AccessContraint = BiometricAccessConstraintAndroid.BiometricOnly,
                    }
                });
                UserInfo = result;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserInfo)));
            }
            finally
            {
                SetIsLoading(false);
            }
        }

        public async Task EnableBiometricAsync()
        {
            EnsureAuthgear();
            try
            {
                SetIsLoading(true);
                await authgear.EnableBiometricAsync(new BiometricOptions
                {
                    Android = new BiometricOptionsAndroid
                    {
                        Title = "Enable biometric title",
                        Subtitle = "subtitle",
                        Description = "description",
                        NegativeButtonText = "Cancel",
                        AccessContraint = BiometricAccessConstraintAndroid.BiometricOnly,
                    }
                });
            }
            finally
            {
                SetIsLoading(false);
            }
        }

        public async Task DisableBiometricAsync()
        {
            EnsureAuthgear();
            try
            {
                SetIsLoading(true);
                await authgear.DisableBiometricAsync();
            }
            finally
            {
                SetIsLoading(false);
            }
        }

        public async Task LogoutAsync()
        {
            EnsureAuthgear();
            try
            {
                SetIsLoading(true);
                await authgear.LogoutAsync();
            }
            finally
            {
                SetIsLoading(false);
            }
        }

        private void SetIsLoading(bool isLoading)
        {
            IsLoading = IsLoading;
            IsNotLoading = !isLoading;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNotLoading)));
        }
        private void SetState(SessionState state)
        {
            SessionState = state;
            State = state.ToString();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionState)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
        }
    }
}
