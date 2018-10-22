

namespace FWF.Json
{
    public interface IJsonSerializable
    {
        void ToJson(IJsonWriter writer);
    }
}

