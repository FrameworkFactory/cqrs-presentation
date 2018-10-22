using System;
using System.Globalization;

namespace FWF.Json.Converters
{
    internal class TimeSpanJsonConverter : IJsonConverter
    {
        private const string TimeSpanFormat = "dd.HH:mm:ss.fffffff";

        public bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeSpan);
        }

        public object Write(object value)
        {
            if (value.IsNull())
            {
                return null;
            }

            var ts = (TimeSpan)value;

            return ts.ToCustomString(TimeSpanFormat);
        }

        public object Read(object value)
        {
            var timeSpanString = value as string;

            TimeSpan ts;

            if (TimeSpan.TryParseExact(
                timeSpanString, 
                TimeSpanFormat, 
                CultureInfo.InvariantCulture, 
                out ts
                ))
            {
                return ts;
            }

            return null;
        }

    }
}

