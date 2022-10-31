using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Authgear.Xamarin;
using AuthgearSample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XamarinFormSample.Droid
{
    internal class AuthgearFactoryAndroid : IAuthgearFactory
    {
        private readonly Context context;
        public AuthgearFactoryAndroid(Context context)
        {
            this.context = context;
        }
        public AuthgearSdk CreateAuthgear(AuthgearOptions options)
        {
            return new AuthgearSdk(context, options);
        }
    }
}