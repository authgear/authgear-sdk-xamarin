using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    internal class AnonymousUserNotFoundException : Exception
    {
        public AnonymousUserNotFoundException() : base() { }
    }
}
