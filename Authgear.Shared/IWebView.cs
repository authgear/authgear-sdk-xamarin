using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Authgear.Xamarin
{
    internal interface IWebView
    {
        Task ShowAsync(string url);
    }
}
