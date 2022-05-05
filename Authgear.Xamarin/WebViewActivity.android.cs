using Android.App;
using Android.Content.PM;
using AndroidX.AppCompat.App;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
    internal class WebViewActivity : AppCompatActivity
    {
    }
}
