using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Authgear.Xamarin
{
    internal class WebView : IWebView
    {
        public async Task ShowAsync(string url)
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    _ = await WebAuthenticator.AuthenticateAsync(new WebAuthenticatorOptions
                    {
                        Url = new Uri(url),
                        CallbackUrl = new Uri("nocallback:///"),
                        PrefersEphemeralWebBrowserSession = true
                    }).ConfigureAwait(false);
                }).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Cancel = going back, which is expected in this case, no-op.
            }
        }
    }
}
