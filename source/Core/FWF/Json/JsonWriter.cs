using FWF.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FWF.Json
{
    internal class JsonWriter : DisposableObject, IJsonWriter
    {
        private readonly List<IJsonConverter> _converters = new List<IJsonConverter>();

        private readonly Newtonsoft.Json.JsonWriter _jsonWriter;
        private readonly ILog _log;

        public JsonWriter(
            Newtonsoft.Json.JsonWriter jsonWriter,
            IEnumerable<IJsonConverter> converters,
            ILogFactory logFactory
            )
        {
            _jsonWriter = jsonWriter;

            _log = logFactory.CreateForType(this);

            _converters.AddRange(converters);
        }

        public override void Dispose(bool disposing)
        {
            _jsonWriter.Close();
            base.Dispose(disposing);
        }

        internal Newtonsoft.Json.JsonWriter Writer
        {
            get { return _jsonWriter; }
        }

        public void WriteStartObject()
        {
            _jsonWriter.WriteStartObject();
        }
        public void WriteEndObject()
        {
            _jsonWriter.WriteEndObject();
        }

        public void WriteStartArray()
        {
            _jsonWriter.WriteStartArray();
        }
        public void WriteEndArray()
        {
            _jsonWriter.WriteEndArray();
        }

        public void WritePropertyName(string propertyName)
        {
            _jsonWriter.WritePropertyName(propertyName);
        }

        public void Write<T>(T value)
        {
            var converter = _converters.FirstOrDefault(x => x.CanConvert(typeof(T)));

            if (converter.IsNotNull())
            {
                var writtenValue = converter.Write(value);

                _jsonWriter.WriteValue(writtenValue);
                return;
            }

            _jsonWriter.WriteValue(value);
        }

        public void WriteNull()
        {
            _jsonWriter.WriteNull();
        }
        public void WriteToken(JsonToken jsonToken, object value)
        {
            var newtonsoftJsonToken = (Newtonsoft.Json.JsonToken)Enum.Parse(typeof(Newtonsoft.Json.JsonToken), jsonToken.ToString());

            _jsonWriter.WriteToken(newtonsoftJsonToken, value);
        }


        public void Flush()
        {
            _jsonWriter.Flush();
        }
        public async Task FlushAsync()
        {
            await _jsonWriter.FlushAsync();
        }

        public bool AutoCompleteOnClose
        {
            get { return _jsonWriter.AutoCompleteOnClose; }
            set { _jsonWriter.AutoCompleteOnClose = value; }
        }
        public bool CloseOutput
        {
            get { return _jsonWriter.CloseOutput; }
            set { _jsonWriter.CloseOutput = value; }
        }


    }
}
