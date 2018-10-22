
using System;
using System.Threading.Tasks;

namespace FWF.Json
{
    public interface IJsonWriter : IDisposable
    {
        void WriteStartObject();
        void WriteEndObject();

        void WriteStartArray();
        void WriteEndArray();

        void WritePropertyName(string propertyName);
        void Write<T>(T value);

        void WriteProperty<T>(string propertyName, T value);

        void WriteNull();
        void WriteToken(JsonToken jsonToken, object value);


        void Flush();
        Task FlushAsync();

        bool AutoCompleteOnClose
        {
            get; set;
        }
        bool CloseOutput
        {
            get; set;
        }

    }
}
