using System;
using System.Diagnostics;

namespace FWF
{
    [DebuggerStepThrough]
    public static class StringExtensions
    {

        public static bool EqualsIgnoreCase(this string str, string compareTo)
        {
            if (string.IsNullOrEmpty(str))
            {
                return (string.IsNullOrEmpty(compareTo));
            }

            return str.Equals(compareTo, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsPresent(this string value)
        {
            return !string.IsNullOrWhiteSpace(value) && !string.IsNullOrEmpty(value);
        }

        public static bool IsMissing(this string value)
        {
            return string.IsNullOrWhiteSpace(value) || string.IsNullOrEmpty(value);
        }

    }
}



