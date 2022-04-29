using Authgear.Xamarin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinFormSample
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly AuthgearSdk authgear;
        public string State { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public MainViewModel()
        {
            authgear = DependencyService.Get<AuthgearSdk>();
            State = authgear.SessionState.ToString();
            authgear.SessionStateChange += (sender, e) =>
            {
                State = authgear.SessionState.ToString();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
            };
        }
        public async Task ConfigureAsync()
        {
            await authgear.Configure();
        }
    }
}
