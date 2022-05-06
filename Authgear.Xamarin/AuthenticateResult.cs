using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    public class AuthenticateResult
    {
        public UserInfo UserInfo { get; set; }
        public string State { get; set; }
    }
}
