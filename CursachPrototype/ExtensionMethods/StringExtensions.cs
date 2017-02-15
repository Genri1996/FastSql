using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace CursachPrototype.ExtensionMethods
{
    public static class StringExtensions
    {
        public static bool ContainsIgnoreCase(this string container, string str)
        {
            return CultureInfo.InvariantCulture.CompareInfo.IndexOf(container, str, CompareOptions.IgnoreCase) >= 0;
        }

        public static bool Match(this string value, string pattern)
        {
            Regex regex = new Regex(pattern);
            return regex.IsMatch(value);
        }
    }
}