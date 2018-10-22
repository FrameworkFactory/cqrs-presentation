using System;

namespace FWF.Json.Converters
{
    internal class AppVersionJsonConverter : IJsonConverter
    {

        public bool CanConvert(Type objectType)
        {
            return objectType == typeof(AppVersion);
        }

        public object Write(object value)
        {
            if (value.IsNull())
            {
                return null;
            }

            return value.ToString();
        }

        public object Read(object value)
        {
            var appVersionString = value as string;

            return new AppVersion(appVersionString);
        }

    }
}

