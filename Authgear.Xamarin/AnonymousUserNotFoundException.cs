using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    public class AnonymousUserNotFoundException : Exception
    {
        public AnonymousUserNotFoundException() : base() { }
    }
}
