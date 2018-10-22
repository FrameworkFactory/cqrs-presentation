using System;

namespace FWF.Json
{
    public interface IJsonConverter
    {
        bool CanConvert(Type objectType);

        object Read(object value);

        object Write(object value);

    }
}
