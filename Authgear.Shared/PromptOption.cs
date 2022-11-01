using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Authgear.Xamarin
{
    public enum PromptOption
    {
        [Description("none")]
        None,
        [Description("login")]
        Login,
        [Description("consent")]
        Consent,
        [Description("select_account")]
        SelectAccount
    }
}
