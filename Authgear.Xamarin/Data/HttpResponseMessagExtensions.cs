using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Authgear.Xamarin.Data;
using Authgear.Xamarin.Oauth;

namespace Authgear.Xamarin
{
    internal static class HttpResponseMessageExtensions
    {
        public static async Task EnsureSuccessOrAuthgearExceptionAsync(this HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                try
                {
                    var errorStr = await responseMessage.Content.ReadAsStringAsync();
                    try
                    {
                        // Try parsing it as server exception
                        var serverErrorResult = AuthgearJson.Deserialize<ServerErrorResult>(errorStr);
                        var error = serverErrorResult.Error;
                        throw new ServerException(error.Name, error.Reason, error.Message, error.Info);
                    }
                    catch (JsonException)
                    {
                        // It's an oauth exception
                        var error = AuthgearJson.Deserialize<OauthError>(errorStr);
                        throw new OauthException(error.Error, error.ErrorDescription, error.State, error.ErrorUri);
                    }
                }
                catch (Exception ex)
                {
                    throw new AuthgearException(ex);
                }
            }
        }
        public static async Task<T> GetJsonAsync<T>(this HttpResponseMessage responseMessage)
        {
            await responseMessage.EnsureSuccessOrAuthgearExceptionAsync();
            var responseStream = await responseMessage.Content.ReadAsStreamAsync();
            return AuthgearJson.Deserialize<T>(responseStream);
        }
    }
}
