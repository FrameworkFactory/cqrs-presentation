using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;

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



