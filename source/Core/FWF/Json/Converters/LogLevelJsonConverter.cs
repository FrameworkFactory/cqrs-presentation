using System;
using FWF.Logging;

namespace FWF.Json.Converters
{
    internal class LogLevelJsonConverter : IJsonConverter
    {
        public bool CanConvert(Type objectType)
        {
            return objectType == typeof(LogLevel);
        }

        public object Read(object value)
        {
            if (value.IsNull())
            {
                return null;
            }

            long numericalValue = (long)value;

            long logLevelValue = checked((int)numericalValue);

            // Use the implicit operator
            return (LogLevel)logLevelValue;
        }

        public object Write(object value)
        {
            if (value.IsNull())
            {
                return null;
            }

            var logLevel = value as LogLevel;

            if (logLevel.IsNull())
            {
                return 0;
            }

            return logLevel.Value;
        }
    }
}

