using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    public class AuthorizeResult
    {
        public UserInfo UserInfo { get; }
        public string State { get; }
        public AuthorizeResult(UserInfo userInfo, string state)
        {
            UserInfo = userInfo;
            State = state;
        }
    }
}
