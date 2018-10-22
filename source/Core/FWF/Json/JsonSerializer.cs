using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using FWF.Logging;
using System;
using System.Collections.Generic;

namespace FWF.Json
{
    internal class JsonSerializer : IJsonSerializer
    {
        private readonly Newtonsoft.Json.JsonSerializer _jsonSerializer;
        private readonly ILog _log;

        private class CustomJsonConverter : JsonConverter
        {
            private readonly IJsonConverter _converter;

            public CustomJsonConverter(IJsonConverter converter)
            {
                _converter = converter;
            }

            public override bool CanConvert(Type objectType)
            {
                return _converter.CanConvert(objectType);
            }

            public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                var response = _converter.Read(reader.Value);

                return response;
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
            {
                var newValue = _converter.Write(value);

                writer.WriteValue(newValue);
            }
        }

        public JsonSerializer(
            IEnumerable<IJsonConverter> converters,
            ILogFactory logFactory
            )
        {
            _log = logFactory.CreateForType(this);

            _jsonSerializer = new Newtonsoft.Json.JsonSerializer()
            {
                ConstructorHandling = JSON.ConstructorHandlingDefault,
                NullValueHandling = JSON.NullValueHandlingDefault,
                TypeNameAssemblyFormatHandling = JSON.TypeNameAssemblyFormatHandlingDefault,
                TypeNameHandling = JSON.TypeNameHandlingDefault,
                FloatFormatHandling = JSON.FloatFormatHandlingDefault,
                DateFormatHandling = JSON.DateFormatHandlingDefault,
                DateTimeZoneHandling = JSON.DateTimeZoneHandlingDefault,
                Formatting = JSON.FormattingDefault,
            };

            _jsonSerializer.Converters.Add(new KeyValuePairConverter());
            _jsonSerializer.Converters.Add(new StringEnumConverter());

            foreach (var converter in converters)
            {
                var newtonsoftConverter = new CustomJsonConverter(converter);

                _jsonSerializer.Converters.Add(newtonsoftConverter);
            }

        }

        public void Serialize(IJsonWriter jsonWriter, object item)
        {
            var newtonsoftWriter = (jsonWriter as JsonWriter).Writer;

            _jsonSerializer.Serialize(newtonsoftWriter, item);
        }

        public T Deserialize<T>(IJsonReader jsonReader)
        {
            var newtonsoftReader = (jsonReader as JsonReader).Reader;

            return _jsonSerializer.Deserialize<T>(newtonsoftReader);
        }

        public object Deserialize(IJsonReader jsonReader, Type objectType)
        {
            var newtonsoftReader = (jsonReader as JsonReader).Reader;

            return _jsonSerializer.Deserialize(newtonsoftReader, objectType);
        }

    }
}
