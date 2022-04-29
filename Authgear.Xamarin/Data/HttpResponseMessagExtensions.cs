using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Authgear.Xamarin
{
    internal static class HttpResponseMessageExtensions
    {
        public static async Task EnsureSuccessOrAuthgearExceptionAsync(this HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                var responseStr = await responseMessage.Content.ReadAsStringAsync();
                throw new AuthgearException(responseStr);
            }
        }
    }
}
