using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Authgear.Xamarin.DeviceInfo;

namespace Authgear.Xamarin.Data
{
    internal class KeyRepo : IKeyRepo
    {
        public Task<KeyJwtResult> GetOrCreateAnonymousJwtAsync(string keyId, string challenge, DeviceInfoRoot deviceInfo)
        {
            throw new NotImplementedException();
        }

        public Task<string> PromoteAnonymousUserAsync(string keyId, string challenge, DeviceInfoRoot deviceInfo)
        {
            throw new NotImplementedException();
        }
    }
}
