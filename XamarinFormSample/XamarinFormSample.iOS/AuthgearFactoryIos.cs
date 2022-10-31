using System;
using Authgear.Xamarin;
using AuthgearSample;

namespace XamarinFormSample.iOS
{
    public class AuthgearFactoryIos : IAuthgearFactory
    {
        public AuthgearFactoryIos()
        {
        }

        public AuthgearSdk CreateAuthgear(AuthgearOptions options)
        {
            return new AuthgearSdk(UIKit.UIApplication.SharedApplication, options);
        }
    }
}
