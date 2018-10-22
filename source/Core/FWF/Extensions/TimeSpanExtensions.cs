using System;
using System.Globalization;

namespace FWF
{
    public static class TimeSpanExtensions
    {
        #region TimeSpanParseFormats
        public static readonly string[] TimeSpanParseFormats = new string[] {
                                        @"\.f",
                                        @"\.ff",
                                        @"\.fff",
                                        @"\.ffff",
                                        @"\.fffff",
                                        @"\.ffffff",
                                        @"s",
                                        @"ss",
                                        @"sss",
                                        @"s\.f",
                                        @"s\.ff",
                                        @"s\.fff",
                                        @"s\.ffff",
                                        @"s\.fffff",
                                        @"s\.ffffff",
                                        @"ss\.f",
                                        @"ss\.ff",
                                        @"ss\.fff",
                                        @"ss\.ffff",
                                        @"ss\.fffff",
                                        @"ss\.ffffff",                                        
                                        @"m\:ss\.f",
                                        @"m\:ss\.ff",
                                        @"m\:ss\.fff",
                                        @"m\:ss\.ffff",
                                        @"m\:ss\.fffff",
                                        @"m\:ss\.ffffff",
                                        @"mm\:ss\.f",
                                        @"mm\:ss\.ff",
                                        @"mm\:ss\.fff",
                                        @"mm\:ss\.ffff",
                                        @"mm\:ss\.fffff",
                                        @"mm\:ss\.ffffff",
                                        @"HH\:mm\:ss",
                                        @"HH\:mm\:ss\.f",
                                        @"HH\:mm\:ss\.ff",
                                        @"HH\:mm\:ss\.fff",
                                        @"HH\:mm\:ss\.ffff",
                                        @"HH\:mm\:ss\.fffff",
                                        @"HH\:mm\:ss\.ffffff",
                                        @"dd\.HH\:mm\:ss\.f",
                                        @"dd\.HH\:mm\:ss\.ff",
                                        @"dd\.HH\:mm\:ss\.fff",
                                        @"dd\.HH\:mm\:ss\.ffff",
                                        @"dd\.HH\:mm\:ss\.fffff",
                                        @"dd\.HH\:mm\:ss\.ffffff",
                                    };
        #endregion

        public const string TimeSpanLongFormat = "dd.HH:mm:ss.fffffff";
        public const string TimeSpanShortFormat = "HH:mm:ss.fffffff";

        public static TimeSpan? TryParseTimeSpan(this string value)
        {
            TimeSpan result;
            double secondsValue = 0;

            if(TimeSpan.TryParseExact(value, TimeSpanParseFormats, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }
            else if(double.TryParse(value, out secondsValue))
            {
                return TimeSpan.FromSeconds(secondsValue);
            }
            else
            {
                return null;
            }
        }

        public static string ToDefaultString(this TimeSpan ts)
        {
            return ToCustomString(ts, TimeSpanShortFormat);
        }

        public static string ToCustomString(this TimeSpan ts, string format)
        {
            if (ts == TimeSpan.MinValue)
            {
                return TimeSpan.Zero.ToString();
            }
            if (ts.Ticks < 0)
            {
                return TimeSpan.Zero.ToString();
            }

            if (ts.Days > 0)
            {
                return new DateTime(ts.Ticks).AddDays(-1).ToString(TimeSpanLongFormat);
            }

            return new DateTime(ts.Ticks).ToString(format);
        }

        public static TimeSpan RoundToNearest(this TimeSpan input, TimeSpan timeInterval)
        {
            // 1:59 becomes 2:00
            // 2:05 becomes 2:00
            // 2:08 becomes 2:15
            // 2:14 becomes 2:15

            if (timeInterval == TimeSpan.Zero)
            {
                throw new InvalidOperationException("Unable to round to zero.");
            }

            // half the interval for rounding purposes
            var half = timeInterval.Ticks / 2;

            // value of timespan left over
            var rm = input.Ticks % timeInterval.Ticks;

            var distanceToNext = timeInterval.Ticks - rm;
            var distanceToPrevious = rm;

            // If we are passed halfwas, then go to the next time interval, otherwise
            // go to the previous time interval
            if (rm > half)
            {
                return TimeSpan.FromTicks(input.Ticks + distanceToNext);
            }
            else
            {
                return TimeSpan.FromTicks(input.Ticks - distanceToPrevious);
            }
        }

    }
}


