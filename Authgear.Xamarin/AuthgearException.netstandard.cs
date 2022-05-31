using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    public partial class AuthgearException
    {
        internal static Exception? PlatformWrap(Exception ex)
        {
            return null;
        }
    }
}
