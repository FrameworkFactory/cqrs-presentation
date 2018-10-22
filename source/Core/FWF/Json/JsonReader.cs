using FWF.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FWF.Json
{
    internal class JsonReader : DisposableObject, IJsonReader
    {

        private readonly List<IJsonConverter> _converters = new List<IJsonConverter>();

        private readonly Newtonsoft.Json.JsonReader _jsonReader;
        private readonly ILog _log;

        public JsonReader(
            Newtonsoft.Json.JsonReader jsonReader,
            IEnumerable<IJsonConverter> converters,
            ILogFactory logFactory
            )
        {
            _jsonReader = jsonReader;

            _log = logFactory.CreateForType(this);

            _converters.AddRange(converters);
        }

        public override void Dispose(bool disposing)
        {
            _jsonReader.Close();

            base.Dispose(disposing);
        }

        internal Newtonsoft.Json.JsonReader Reader
        {
            get { return _jsonReader; }
        }

        public JsonToken TokenType
        {
            get
            {
                return (JsonToken) Enum.Parse(typeof(JsonToken), _jsonReader.TokenType.ToString());
            }
        }

        public int Depth
        {
            get { return _jsonReader.Depth; }
        }

        public object Value
        {
            get { return _jsonReader.Value; }
        }


        public bool Read()
        {
            return _jsonReader.Read();
        }

        public T Read<T>(T defaultValue)
        {
            _jsonReader.Read();

            if (_jsonReader.Value.IsNull())
            {
                return defaultValue;
            }

            var converter = _converters.FirstOrDefault(x => x.CanConvert(typeof(T)));

            if (converter.IsNotNull())
            {
                return converter.Read(_jsonReader.Value).Cast(defaultValue);
            }

            // C# has difficulty converting from nullable to non-nullable.  Lookup the type
            // and determine if we can convert first to the underlying type, then nullable type.

            var underlyingType = Nullable.GetUnderlyingType(typeof(T));

            if (underlyingType != null)
            {
                var convertedValue = Convert.ChangeType(_jsonReader.Value, underlyingType);

                return convertedValue.Cast(defaultValue);
            }
            
            return _jsonReader.Value.Cast(defaultValue);
        }



        public bool CloseInput
        {
            get { return _jsonReader.CloseInput; }
            set { _jsonReader.CloseInput = value; }
        }

    }
}
