using System;

namespace FWF.Json.Converters
{
    internal class GuidJsonConverter : IJsonConverter
    {

        public bool CanConvert(Type objectType)
        {
            return objectType == typeof(Guid) || objectType == typeof(Guid?);
        }

        public object Write(object value)
        {
            if (value.IsNull())
            {
                return null;
            }

            var guid = (Guid)value;

            var guidString = guid.ToString("D").ToUpperInvariant();

            return guidString;
        }

        public object Read(object value)
        {
            if (value.IsNull())
            {
                return null;
            }

            var guidString = value as string;

            if (guidString.IsMissing())
            {
                return Guid.Empty;
            }

            Guid guid = Guid.Empty;

            var didParse = Guid.TryParseExact(guidString, "D", out guid);

            if (didParse)
            {
                return guid;
            }

            return null;
        }

    }
}

