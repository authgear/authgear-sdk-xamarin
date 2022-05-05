using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Authgear.Xamarin;
using Xamarin.Forms;

namespace XamarinFormSample.Droid
{
    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
    [IntentFilter(new[] { Android.Content.Intent.ActionView },
        Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable },
        DataScheme = "com.authgear.exampleapp.xamarin")]
    public class WebAuthenticationCallbackActivity : Xamarin.Essentials.WebAuthenticatorCallbackActivity
    {
    }
}