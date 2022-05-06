using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    public class PromoteOptions
    {
        public string RedirectUri { get; set; }
        public string State { get; set; }
        public List<string> UiLocales { get; set; }
    }
}
