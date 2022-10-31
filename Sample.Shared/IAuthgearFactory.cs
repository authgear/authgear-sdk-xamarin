using Authgear.Xamarin;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthgearSample
{
    public interface IAuthgearFactory
    {
        AuthgearSdk CreateAuthgear(AuthgearOptions options);
    }
}
