using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    public class ReauthenticateResult
    {
        public UserInfo UserInfo { get; }
        public string State { get; }
        internal ReauthenticateResult(UserInfo userInfo, string state)
        {
            UserInfo = userInfo;
            State = state;
        }
    }
}
