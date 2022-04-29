using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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
        public static async Task<T> GetJsonAsync<T>(this HttpResponseMessage responseMessage)
        {
            await responseMessage.EnsureSuccessOrAuthgearExceptionAsync();
            var responseStream = await responseMessage.Content.ReadAsStreamAsync();
            return JsonSerializer.Deserialize<T>(responseStream);
        }
    }
}
