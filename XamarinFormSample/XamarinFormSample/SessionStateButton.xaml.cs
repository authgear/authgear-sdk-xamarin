using Authgear.Xamarin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinFormSample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SessionStateButton : ContentView
    {
        public static readonly BindableProperty StateProperty = BindableProperty.Create("State", typeof(SessionState), typeof(SessionStateButton), SessionState.Unknown, propertyChanged: (s, o, n) =>
        {
            var button = s as SessionStateButton;
            button.IsEnabled = (SessionState)n == button.PressableState;
        });
        public SessionState State
        {
            get {
                return (SessionState)GetValue(StateProperty); }
            set {
                SetValue(StateProperty, value); }
        }
        public static readonly BindableProperty PressableStateProperty = BindableProperty.Create("PressableState", typeof(SessionState), typeof(SessionStateButton), SessionState.Unknown, propertyChanged: (s, o, n) =>
        {
            var button = s as SessionStateButton;
            button.IsEnabled = (SessionState)n == button.State;
        });
        public SessionState PressableState
        {
            get {
                return (SessionState)GetValue(PressableStateProperty); }
            set {
                SetValue(PressableStateProperty, value); }
        }
        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(SessionStateButton), "");
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(Command), typeof(SessionStateButton), null);
        public Command Command
        {
            get => GetValue(CommandProperty) as Command;
            set => SetValue(CommandProperty, value);
        }
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter", typeof(object), typeof(SessionStateButton), null);
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }
        public event EventHandler Clicked;

        public SessionStateButton()
        {
            InitializeComponent();
        }
        private void button_Clicked(object sender, EventArgs e)
        {
            Clicked?.Invoke(this, e);
            Command?.Execute(CommandParameter);
        }
    }
}