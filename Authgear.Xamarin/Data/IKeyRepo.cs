using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Authgear.Xamarin.Data
{
    internal interface IKeyRepo
    {
        Task<KeyJwtResult> GetOrCreateAnonymousJwtAsync(string keyId, string challenge, DeviceInfoRoot deviceInfo);

        Task<string> PromoteAnonymousUserAsync(string keyId, string challenge, DeviceInfoRoot deviceInfo);
    }
}
