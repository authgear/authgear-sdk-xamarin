using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Foundation;
using Security;

namespace Authgear.Xamarin
{
    internal class BiometricIosException : AuthgearException
    {
        public NSError Error { get; }

        public BiometricIosException(NSError error) : base("")
        {
            Error = error;
        }

        public BiometricIosException(SecStatusCode code) : this(new NSError(NSError.OsStatusErrorDomain, Convert.ToInt32(code, CultureInfo.InvariantCulture)))
        {

        }
    }
}
