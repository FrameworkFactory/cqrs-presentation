using System;

namespace FWF.Json
{
    public interface IJsonSerializer
    {
        void Serialize(IJsonWriter jsonWriter, object item);

        T Deserialize<T>(IJsonReader reader);
        object Deserialize(IJsonReader reader, Type objectType);
    }
}
