﻿using System;
using System.Collections.Generic;
using System.Text;
using Authgear.Xamarin.Oauth;

namespace Authgear.Xamarin
{
    public class SettingsOptions
    {
        public ColorScheme? ColorScheme { get; set; }
        public IReadOnlyCollection<string>? UiLocales { get; set; }
        internal OidcAuthenticationRequest ToRequest(string url, string loginHint, bool isSsoEnabled)
        {
            return new OidcAuthenticationRequest(url, "none", new List<string> { "openid", "offline_access", "https://authgear.com/scopes/full-access" }, isSsoEnabled)
            {
                Prompt = new List<PromptOption>() { PromptOption.None },
                LoginHint = loginHint,
                ColorScheme = ColorScheme,
                UiLocales = UiLocales,
            };
        }
    }
}
