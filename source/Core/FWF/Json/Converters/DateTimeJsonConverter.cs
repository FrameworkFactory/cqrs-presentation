using System;

namespace FWF.Json.Converters
{
    internal class DateTimeJsonConverter : IJsonConverter
    {
        private static long StartDateTicks = new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

        public bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
        }

        public object Write(object value)
        {
            if (value.IsNull())
            {
                return null;
            }

            var dateValue = (DateTime)value;

            // Get the number of ticks for the date
            var ticks = dateValue.Ticks;

            // Get only the number of ticks from epoch
            var subset = ticks - StartDateTicks;

            // Get the subset as bits
            var bits = BitConverter.GetBytes(subset);

            // Write as a double to get the precision required
            return Convert.ToBase64String(bits);
        }

        public object Read(object value)
        {
            var bitString = value as string;

            if (bitString.IsMissing())
            {
                return null;
            }

            var bits = Convert.FromBase64String(bitString);

            if (bits.IsNull() || bits.Length == 0)
            {
                return null;
            }

            // Get the input ticks as long
            var ticks = BitConverter.ToInt64(bits);

            // Add all ticks from the epoch
            var fullTicks = StartDateTicks + ticks;

            // Create a date from the combined tick count
            return new DateTime(fullTicks, DateTimeKind.Utc);
        }

    }
}

