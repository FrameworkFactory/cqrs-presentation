using Autofac;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace FWF.Json
{
    public class JSON
    {

        internal static ConstructorHandling ConstructorHandlingDefault = ConstructorHandling.AllowNonPublicDefaultConstructor;
        internal static NullValueHandling NullValueHandlingDefault = NullValueHandling.Ignore;
        internal static TypeNameAssemblyFormatHandling TypeNameAssemblyFormatHandlingDefault = TypeNameAssemblyFormatHandling.Simple;
        internal static TypeNameHandling TypeNameHandlingDefault = TypeNameHandling.None;

        internal static FloatFormatHandling FloatFormatHandlingDefault = FloatFormatHandling.Symbol;
        internal static FloatParseHandling FloatParseHandlingDefault = FloatParseHandling.Double;

        internal static string DateFormatStringDefault = "o";
        internal static DateFormatHandling DateFormatHandlingDefault = DateFormatHandling.IsoDateFormat;
        internal static DateTimeZoneHandling DateTimeZoneHandlingDefault = DateTimeZoneHandling.Utc;
        internal static DateParseHandling DateParseHandlingDefault = DateParseHandling.DateTime;

        internal static Formatting FormattingDefault = Formatting.None;
        internal static char IndentCharDefault = '\0';
        internal static int IndentationDefault = 0;

        public static JsonEmpty Empty = new JsonEmpty();

        public class JsonEmpty : IJsonConvertable
        {

            public void ToJson(IJsonWriter writer)
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
            }

            public void FromJson(IJsonReader reader)
            {
                reader.Read();
                reader.Read();
            }

        }

        internal static IComponentContext ComponentContext
        {
            get; set;
        }
        
        public static IJsonSerializer GetSerializer()
        {
            return ComponentContext.Resolve<IJsonSerializer>();
        }

        public static IJsonWriter GetWriter(Stream stream)
        {
            var streamWriter = new StreamWriter(stream, Encoding.UTF8, 8192, true);

            return GetWriter(streamWriter);
        }
        public static IJsonWriter GetWriter(TextWriter textWriter)
        {
            var jsonTextWriter = new JsonTextWriter(textWriter)
            {
                DateFormatString = DateFormatStringDefault,
                DateFormatHandling = DateFormatHandlingDefault,
                DateTimeZoneHandling = DateTimeZoneHandlingDefault,
                FloatFormatHandling = FloatFormatHandlingDefault,
                Formatting = FormattingDefault,
                IndentChar = IndentCharDefault,
                Indentation = IndentationDefault,
            };

            return ComponentContext.Resolve<IJsonWriter>(
                new TypedParameter(typeof(Newtonsoft.Json.JsonWriter), jsonTextWriter)
                );
        }

        public static IJsonReader GetReader(Stream stream)
        {
            var streamReader = new StreamReader(stream, Encoding.UTF8, false, 8192, true);
            return GetReader(streamReader);
        }
        public static IJsonReader GetReader(TextReader textReader)
        {
            var jsonTextReader = new JsonTextReader(textReader)
            {
                DateParseHandling = DateParseHandlingDefault,
                DateTimeZoneHandling = DateTimeZoneHandlingDefault,
                FloatParseHandling = FloatParseHandlingDefault,
            };

            return ComponentContext.Resolve<IJsonReader>(
                new TypedParameter(typeof(Newtonsoft.Json.JsonReader), jsonTextReader)
                );
        }


        public static IJsonWriter GetBsonWriter(Stream stream)
        {
            var writer = new BinaryWriter(stream, Encoding.UTF8, true);
            var dataWriter = new Newtonsoft.Json.Bson.BsonDataWriter(writer);

            return ComponentContext.Resolve<IJsonWriter>(
                new TypedParameter(typeof(Newtonsoft.Json.JsonWriter), dataWriter)
                );
        }
        public static IJsonReader GetBsonReader(Stream stream)
        {
            var reader = new BinaryReader(stream, Encoding.UTF8, true);
            var dataReader = new Newtonsoft.Json.Bson.BsonDataReader(reader, false, DateTimeKind.Utc);

            return ComponentContext.Resolve<IJsonReader>(
                new TypedParameter(typeof(Newtonsoft.Json.JsonReader), dataReader)
                );
        }

    }
}

