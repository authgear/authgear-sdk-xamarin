using Authgear.Xamarin;
using Authgear.Xamarin.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace XamarinFormSample
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public delegate void ErrorRaisedHandler(object sender, Exception e);
        private AuthgearSdk authgear;
        private readonly IAuthgearFactory authgearFactory;
        public readonly string RedirectUri = "com.authgear.exampleapp.xamarin://host/path";
        public string ClientId { get; set; } = Preferences.Get("authgear.clientID", "");
        public string AuthgearEndpoint { get; set; } = Preferences.Get("authgear.endpoint", "");
        public SessionState SessionState { get; set; } = SessionState.Unknown;
        public string State { get; private set; } = "<no-authgear-instance>";
        public bool IsNotLoading
        {
            get
            {
                return !IsLoading;
            }
        }
        public bool IsLoading { get; private set; } = false;

        public bool UseTransientStorage { get; set; }
        public bool ShareSessioWithDeviceBrowser { get; set; }
        public AuthenticatePage? AuthenticatePageToShow { get; set; }
        public ColorScheme? ExplicitColorScheme { get; set; }
        public UserInfo UserInfo { get; private set; }

        private ColorScheme? ColorScheme
        {
            get
            {
                if (ExplicitColorScheme != null)
                {
                    return ExplicitColorScheme;
                }
                return SystemColorScheme;
            }
        }
        private ColorScheme? SystemColorScheme
        {
            get
            {
                return AppInfo.RequestedTheme == AppTheme.Dark ? Authgear.Xamarin.ColorScheme.Dark : Authgear.Xamarin.ColorScheme.Light;
            }
        }

        public DateTimeOffset? AuthTime
        {
            get
            {
                return authgear.AuthTime;
            }
        }
        private bool IsAnonymous
        {
            get
            {
                if (UserInfo == null)
                {
                    return false;
                }
                return UserInfo.IsAnonymous;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private bool IsConfigured
        {
            get
            {
                return authgear != null;
            }
        }

        private bool IsBiometricEnabled = false;

        public bool IsEnabledAuthenticate
        {
            get
            {
                return IsConfigured && IsNotLoading && SessionState == SessionState.NoSession;
            }
        }

        public bool IsEnabledAuthenticateBiometric
        {
            get
            {
                return IsConfigured && IsNotLoading && SessionState == SessionState.NoSession && IsBiometricEnabled;
            }
        }

        public bool IsEnabledReauthenticateWeb
        {
            get
            {
                return IsConfigured && IsNotLoading && SessionState == SessionState.Authenticated;
            }
        }

        public bool IsEnabledReauthenticate
        {
            get
            {
                return IsEnabledReauthenticateWeb;
            }
        }

        public bool IsEnabledEnableBiometric
        {
            get
            {
                return IsConfigured && IsNotLoading && SessionState == SessionState.Authenticated && !IsAnonymous && !IsBiometricEnabled;
            }
        }
        public bool IsEnabledDisableBiometric
        {
            get
            {
                return IsConfigured && IsNotLoading && SessionState == SessionState.Authenticated && !IsAnonymous && IsBiometricEnabled;
            }
        }
        public bool IsEnabledFetchUserInfo
        {
            get
            {
                return IsConfigured && IsNotLoading && SessionState == SessionState.Authenticated;
            }
        }
        public bool IsEnabledOpenSettings
        {
            get
            {
                return IsConfigured && IsNotLoading && SessionState == SessionState.Authenticated;
            }
        }
        public bool IsEnabledShowAuthTime
        {
            get
            {
                return IsConfigured && IsNotLoading;
            }
        }
        public bool IsEnabledLogout
        {
            get
            {
                return IsConfigured && IsNotLoading && SessionState == SessionState.Authenticated;
            }
        }

        public event ErrorRaisedHandler ErrorRaised;

        public MainViewModel()
        {
            authgearFactory = DependencyService.Get<IAuthgearFactory>();
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
                Preferences.Set("authgear.endpoint", AuthgearEndpoint);
                Preferences.Set("authgear.clientID", ClientId);
                authgear.SessionStateChange += (sender, e) =>
                {
                    _ = SyncAuthgearState();
                };
                await authgear.ConfigureAsync();
                await SyncAuthgearState();
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
                    ColorScheme = ColorScheme,
                });
                Debug.WriteLine(result.State ?? "No state");
                UserInfo = result.UserInfo;
            }
            finally
            {
                SetIsLoading(false);
                Notify();
            }
        }

        public async Task OpenAsync(SettingsPage page)
        {
            EnsureAuthgear();
            try
            {
                SetIsLoading(true);
                await authgear.OpenAsync(page, new SettingsOptions
                {
                    ColorScheme = ColorScheme,
                });
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
                    Page = AuthenticatePageToShow,
                    ColorScheme = ColorScheme,
                });
                Debug.WriteLine(result.State ?? "No state");
                UserInfo = result.UserInfo;
            }
            finally
            {
                SetIsLoading(false);
            }
        }

        public async Task<UserInfo> FetchUserInfoAsync()
        {
            EnsureAuthgear();
            try
            {
                SetIsLoading(true);
                var userInfo = await authgear.FetchUserInfoAsync();
                UserInfo = userInfo;
                return userInfo;
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
                UserInfo = await authgear.ReauthenticateAsync(new ReauthenticateOptions
                {
                    RedirectUri = RedirectUri,
                    ColorScheme = ColorScheme,
                }, useBiometric ? CreateBiometricOptions() : null);
            }
            finally
            {
                SetIsLoading(false);
            }
        }

        public async Task RefreshIdTokenAsync()
        {
            EnsureAuthgear();
            try
            {
                SetIsLoading(true);
                await authgear.RefreshIdTokenAsync();
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
                var result = await authgear.AuthenticateBiometricAsync(CreateBiometricOptions());
                UserInfo = result;
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
                await authgear.EnableBiometricAsync(CreateBiometricOptions());
                await SyncAuthgearState();
            }
            finally
            {
                SetIsLoading(false);
            }
        }

        private BiometricOptions CreateBiometricOptions()
        {
            return new BiometricOptions
            {
                Android = new BiometricOptionsAndroid
                {
                    Title = "Enable biometric title",
                    Subtitle = "subtitle",
                    Description = "description",
                    NegativeButtonText = "Cancel",
                    AccessConstraint = BiometricAccessConstraintAndroid.BiometricOnly,
                },
                Ios = new BiometricOptionsIos
                {
                    LocalizedReason = "Reason",
                    AccessConstraint = BiometricAccessConstraintIos.BiometricAny
                }
            };
        }

        public async Task DisableBiometricAsync()
        {
            EnsureAuthgear();
            try
            {
                SetIsLoading(true);
                await authgear.DisableBiometricAsync();
                await SyncAuthgearState();
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
            IsLoading = isLoading;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNotLoading)));
            Notify();
        }
        private async Task SyncAuthgearState()
        {
            SessionState = authgear.SessionState;
            State = SessionState.ToString();
            IsBiometricEnabled = await authgear.GetIsBiometricEnabledAsync();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionState)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AuthTime)));
        }
        private void Notify()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabledAuthenticate)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabledAuthenticateBiometric)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabledReauthenticateWeb)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabledReauthenticate)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabledEnableBiometric)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabledDisableBiometric)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabledFetchUserInfo)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabledOpenSettings)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabledShowAuthTime)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabledLogout)));
        }
    }
}
