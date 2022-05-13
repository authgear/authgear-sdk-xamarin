using System;
using Foundation;
using Security;

namespace Authgear.Xamarin
{
    internal class AnonymousUserIosException : AuthgearException
    {
        public NSError Error { get; }

        public AnonymousUserIosException(NSError error) : base("")
        {
            Error = error;
        }

        public AnonymousUserIosException(SecStatusCode code) : this(new NSError(NSError.OsStatusErrorDomain, Convert.ToInt32(code)))
        {

        }
    }
}
