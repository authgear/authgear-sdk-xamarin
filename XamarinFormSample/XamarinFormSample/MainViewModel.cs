using Authgear.Xamarin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using AuthgearPage = Authgear.Xamarin.Page;

namespace XamarinFormSample
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private AuthgearSdk authgear;
        private readonly IAuthgearFactory authgearFactory;
        public string RedirectUri { get; set; }
        public string ClientId { get; set; }
        public string AuthgearEndpoint { get; set; }
        public string State { get; private set; } = "<no-authgear-instance>";
        public bool IsNotLoading { get; private set; } = true;
        public bool IsLoading { get; private set; } = false;
        public UserInfo UserInfo { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public MainViewModel()
        {
            authgearFactory = DependencyService.Get<IAuthgearFactory>();
        }
        public async Task ConfigureAsync()
        {
            try
            {
                SetIsLoading(true);
                authgear = authgearFactory.CreateAuthgear(new AuthgearOptions
                {
                    ClientId = ClientId,
                    AuthgearEndpoint = AuthgearEndpoint
                });
                State = authgear.SessionState.ToString();
                authgear.SessionStateChange += (sender, e) =>
                {
                    State = authgear.SessionState.ToString();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
                };
                await authgear.Configure();
            }
            finally
            {
                SetIsLoading(false);
            }
        }

        public async Task AuthorizeAsync()
        {
            try
            {
                SetIsLoading(true);
                var result = await authgear.AuthorizeAsync(new AuthorizeOptions
                {
                    RedirectUri = RedirectUri,
                    Page = AuthgearPage.Login,
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

        private void SetIsLoading(bool isLoading)
        {
            IsLoading = IsLoading;
            IsNotLoading = !isLoading;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNotLoading)));
        }
    }
}
