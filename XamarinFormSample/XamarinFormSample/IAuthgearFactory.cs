using Authgear.Xamarin;
using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinFormSample
{
    public interface IAuthgearFactory
    {
        AuthgearSdk CreateAuthgear(AuthgearOptions options);
    }
}
