using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    public class ReauthenticateResult
    {
        public UserInfo UserInfo { get; set; }
        public string State { get; set; }
    }
}
