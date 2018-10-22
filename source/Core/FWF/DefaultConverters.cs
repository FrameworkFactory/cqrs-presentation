using System;
using System.Globalization;

namespace FWF 
{
    internal static class DefaultConverters
    {
        private const string DefaultDateFormat = "o";
        private const string GuidFormat = "D";

        private static readonly CultureInfo _defaultCulture = CultureInfo.GetCultureInfo(1033);


        public static object StringToInt(object input, object defaultValue)
        {
            var thisString = input as string;

            if (string.IsNullOrEmpty(thisString))
            {
                return defaultValue;
            }

            int x;
            if (int.TryParse(thisString, out x))
            {
                return x;
            }

            return defaultValue;
        }

        public static object IntToString(object input, object defaultValue)
        {
            return ((int)input).ToString(_defaultCulture);
        }

        public static object StringToLong(object input, object defaultValue)
        {
            var thisString = input as string;

            if (string.IsNullOrEmpty(thisString))
            {
                return defaultValue;
            }

            long x;
            if (long.TryParse(thisString, out x))
            {
                return x;
            }

            return defaultValue;
        }

        public static object LongToString(object input, object defaultValue)
        {
            return ((long)input).ToString(_defaultCulture);
        }

        public static object StringToDecimal(object input, object defaultValue)
        {
            var thisString = input as string;

            if (string.IsNullOrEmpty(thisString))
            {
                return defaultValue;
            }

            decimal x;
            if (decimal.TryParse(thisString, out x))
            {
                return x;
            }

            return defaultValue;
        }

        public static object DecimalToString(object input, object defaultValue)
        {
            return ((decimal)input).ToString(_defaultCulture);
        }

        public static object StringToBoolean(object input, object defaultValue)
        {
            var thisString = input as string;

            if (string.IsNullOrEmpty(thisString))
            {
                return defaultValue;
            }

            // Add exception for the "1" and "0" string values
            if (thisString.EqualsIgnoreCase("1"))
            {
                return true;
            }

            if (thisString.EqualsIgnoreCase("0"))
            {
                return false;
            }

            bool x;
            if (bool.TryParse(thisString, out x))
            {
                return x;
            }

            return defaultValue;
        }

        public static object BooleanToString(object input, object defaultValue)
        {
            return ((bool)input).ToString(_defaultCulture);
        }

        public static object StringToDateTime(object input, object defaultValue)
        {
            var thisString = input as string;

            if (string.IsNullOrEmpty(thisString))
            {
                return defaultValue;
            }

            DateTime x;
            if (DateTime.TryParseExact(
                thisString, 
                DefaultDateFormat, 
                _defaultCulture, 
                DateTimeStyles.AdjustToUniversal,
                out x
                )
                )
            {
                return x;
            }
            
            return defaultValue;
        }

        public static object DateTimeToString(object input, object defaultValue)
        {
            var utcDate = ((DateTime) input).ToUniversalTime();

            // Format DateTime.ToString() with the default date format string
            return utcDate.ToString(DefaultDateFormat, _defaultCulture);
        }

        public static object StringToTimeSpan(object input, object defaultValue)
        {
            var thisString = input as string;

            if (string.IsNullOrEmpty(thisString))
            {
                return defaultValue;
            }

            TimeSpan x;
            if (TimeSpan.TryParse(thisString, out x))
            {
                return x;
            }

            return defaultValue;
        }

        public static object TimeSpanToString(object input, object defaultValue)
        {
            // Format TimeSpan.ToString() with the default TimeSpan format string
            return ((TimeSpan)input).ToDefaultString();
        }

        public static object StringToUri(object input, object defaultValue)
        {
            var thisString = input as string;

            if (string.IsNullOrEmpty(thisString))
            {
                return defaultValue;
            }

            if (!Uri.IsWellFormedUriString(thisString, UriKind.RelativeOrAbsolute))
            {
                return defaultValue;
            }

            try
            {
                var x = new Uri(thisString);

                return x;
            }
            catch
            {
                // No logging the exception here
                return defaultValue;
            }
        }

        public static object UriToString(object input, object defaultValue)
        {
            return input.ToString();
        }

        public static object StringToGuid(object input, object defaultValue)
        {
            Guid result = Guid.Empty;

            if (Guid.TryParse(input.ToString(), out result))
            {
                return result;
            }

            return defaultValue;
        }

        public static object GuidToString(object input, object defaultValue)
        {
            if (ReferenceEquals(input, null))
            {
                return defaultValue;
            }

            return ((Guid)input).ToString(GuidFormat).ToUpper();
        }
        
        public static object StringToUShort(object input, object defaultValue)
        {
            if (ReferenceEquals(input, null))
            {
                return (ushort)0;
            }

            return ushort.Parse(input as string);
        }

        public static object UShortToString(object input, object defaultValue)
        {
            if (ReferenceEquals(input, null))
            {
                return defaultValue;
            }

            return input.ToString();
        }

        public static object UrlToString(object input, object defaultValue)
        {
            if (ReferenceEquals(input, null))
            {
                return defaultValue;
            }

            return ((Url)input).ToString();
        }

        public static object StringToUrl(object input, object defaultValue)
        {
            if (ReferenceEquals(input, null))
            {
                return Url.Empty;
            }

            return new Url(input.ToString());
        }


    }
}
