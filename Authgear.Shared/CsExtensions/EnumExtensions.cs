using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Authgear.Xamarin.CsExtensions
{
    internal static class EnumExtension
    {
        public static string GetDescription<T>(this T source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var fi = source.GetType().GetField(source.ToString() ?? string.Empty);
            if (fi == null) return source.ToString() ?? "";
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : source.ToString() ?? "";
        }
    }
}
