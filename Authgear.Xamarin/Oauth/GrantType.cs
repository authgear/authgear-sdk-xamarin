using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Authgear.Xamarin
{
    public enum GrantType
    {
        [Description("authorization_token")]
        AuthorizationCode,
        [Description("refresh_token")]
        RefreshToken,
        [Description("urn:authgear:params:oauth:grant-type:anonymous-request")]
        Anonymous,
        [Description("urn:authgear:params:oauth:grant-type:biometric-request")]
        Biometric,
        [Description("urn:authgear:params:oauth:grant-type:id-token")]
        IdToken
    }
}
