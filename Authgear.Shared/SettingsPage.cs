using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Authgear.Xamarin
{
    public enum SettingsPage
    {
        [Description("/settings")]
        Settings,
        [Description("/settings/identity")]
        Identity
    }
}
