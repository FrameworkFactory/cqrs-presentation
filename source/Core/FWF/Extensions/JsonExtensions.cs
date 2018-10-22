using System;
using System.Collections.Generic;
using System.IO;
using FWF.Json;
using FWF.Logging;

using System.Text;

namespace FWF
{
    public static class JsonExtensions
    {
        /*
         * JSON serialization is everywhere.  By default, any object can be serialized/deserialized 
         * using Newtonsoft's JsonSerializer.  This works well as long as both sides of the workflow
         * adhere to the specific JSON setting defaults shown in JSON.cs.
         * 
         * JSON serialization can be slow when repeated in high transaction volume scenarios.  To 
         * compensate for this, the IJsonSerializable and IJsonDeserializable interfaces provide
         * methods that serialize directly without the need of reflection or difficult object 
         * graph scenarios.
         * 
         * These custom methods shine when attached to streaming components that provide
         * a single stream of serialized data that can be extended with compression, encryption,
         * and multiple components with a single serialization call, rather than one hit per
         * component.
        */

        private static readonly IJsonSerializer _jsonSerializer = JSON.GetSerializer();

        public static ILog Log
        {
            get;
            set;
        }

        public static string ToJsonString(this object jsonObject)
        {
            if (ReferenceEquals(jsonObject, null))
            {
                return string.Empty;
            }

            var jsonSerializable = jsonObject as IJsonSerializable;

            if (!ReferenceEquals(jsonSerializable, null))
            {
                return ToJsonString(jsonSerializable);
            }

            var stringBuilder = new StringBuilder();

            using (var writer = JSON.GetWriter(new StringWriter(stringBuilder)))
            {
                try
                {
                    _jsonSerializer.Serialize(writer, jsonObject);
                }
                catch (Exception ex)
                {
                    if (Log != null)
                    {
                        Log.WarnFormat(
                            "Unable to serialize component: {0} - {1}",
                            jsonObject.GetType().FullName,
                            ex.Message
                            );
                    }
                    throw;
                }
            }

            return stringBuilder.ToString();
        }

        public static string ToJsonString(this IJsonSerializable jsonSerializable)
        {
            if (ReferenceEquals(jsonSerializable, null))
            {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder();

            using (var writer = JSON.GetWriter(new StringWriter(stringBuilder)))
            {
                try
                {
                    jsonSerializable.ToJson(writer);
                }
                catch (Exception ex)
                {
                    if (Log != null)
                    {
                        Log.WarnFormat(
                            "Unable to serialize component: {0} - {1}",
                            jsonSerializable.GetType().Name,
                            ex.Message
                            );
                    }
                    throw;
                }
            }

            return stringBuilder.ToString();
        }

        public static string ToJsonString(this IEnumerable<IJsonSerializable> jsonSerialiables)
        {
            if (ReferenceEquals(jsonSerialiables, null))
            {
                throw new ArgumentNullException("jsonSerialiables");
            }

            var stringBuilder = new StringBuilder();

            using (var writer = JSON.GetWriter(new StringWriter(stringBuilder)))
            {
                writer.WriteStartArray();

                foreach (var jsonSerialiable in jsonSerialiables)
                {
                    try
                    {
                        jsonSerialiable.ToJson(writer);
                    }
                    catch (Exception ex)
                    {
                        if (Log != null)
                        {
                            Log.WarnFormat(
                                "Unable to serialize component: {0} - {1}",
                                jsonSerialiable.GetType().Name,
                                ex.Message
                                );
                        }
                        throw;
                    }
                }

                writer.WriteEndArray();
            }

            return stringBuilder.ToString();
        }

        public static bool TryJsonDeserialize<T>(this string jsonString, out T returnObject) where T : class
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                throw new ArgumentNullException("jsonString");
            }

            object returnObj;
            var success = TryJsonDeserialize(jsonString, typeof(T), out returnObj);

            returnObject = returnObj as T;

            return success;
        }

        public static bool TryJsonDeserialize(this string jsonString, Type typeToSerializeTo, out object returnObject)
        {
            returnObject = null;

            if (string.IsNullOrEmpty(jsonString))
            {
                throw new ArgumentNullException("jsonString");
            }

            // If the object has ToJson() and FromJson(), then leverage that
            if (typeof(IJsonDeserializable).IsAssignableFrom(typeToSerializeTo))
            {
                try
                {
                    returnObject = Activator.CreateInstance(typeToSerializeTo);

                    ((IJsonDeserializable)returnObject).FromJsonString(jsonString);

                    return true;
                }
                catch (Exception ex)
                {
                    if (Log != null)
                    {
                        Log.WarnFormat(
                            "Unable to serialize component: {0} - {1}",
                            typeToSerializeTo.Name,
                            ex.Message
                            );
                    }

                    throw;
                }
            }

            var success = false;

            using (var reader = JSON.GetReader(new StringReader(jsonString)))
            {
                try
                {
                    while (reader.TokenType != JsonToken.StartObject)
                    {
                        if (!reader.Read())
                        {
                            return false;
                        }
                    }

                    returnObject = _jsonSerializer.Deserialize(reader, typeToSerializeTo);

                    success = true;
                }
                catch (Exception ex)
                {
                    if (Log != null)
                    {
                        Log.WarnFormat(
                            "Unable to serialize component: {0} - {1}",
                            typeToSerializeTo.Name,
                            ex.Message
                            );
                    }

                    throw;
                }
            }

            return success;
        }

        public static void FromJsonString(this IJsonDeserializable jsonObject, string jsonString)
        {
            if (ReferenceEquals(jsonObject, null))
            {
                throw new ArgumentNullException("jsonObject");
            }

            using (var reader = JSON.GetReader(new StringReader(jsonString)))
            {
                while(reader.TokenType != JsonToken.StartObject)
                {
                    if (!reader.Read())
                    {
                        return;
                    }
                }

                try
                {
                    jsonObject.FromJson(reader);
                }
                catch (Exception ex)
                {
                    if (Log != null)
                    {
                        Log.WarnFormat(
                            "Unable to serialize component: {0} - {1}",
                            jsonObject.GetType().FullName,
                            ex.Message
                            );
                    }

                    throw;
                }
            }

        }

        public static T JsonDeserialize<T>(this Stream stream)
        {
            T item = default(T);

            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            using (var jsonReader = JSON.GetReader(streamReader))
            {
                item = _jsonSerializer.Deserialize<T>(jsonReader);
            }

            return item;
        }

        public static T JsonClone<T>(this T jsonObject) where T : class, new()
        {
            if (ReferenceEquals(jsonObject, null))
            {
                return default(T);
            }

            // IJsonConvertable objects can serialize very quickly to JSON
            var convertableJsonObject = jsonObject as IJsonConvertable;
            var isConvertableObject = convertableJsonObject.IsNotNull();

            T result = default(T);

            using (var memoryStream = new MemoryStream())
            using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            using (var jsonReader = JSON.GetReader(streamReader))
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var jsonWriter = JSON.GetWriter(streamWriter))
            {
                if (isConvertableObject)
                {
                    convertableJsonObject.ToJson(jsonWriter);
                }
                else
                {
                    _jsonSerializer.Serialize(jsonWriter, jsonObject);
                }

                // Ensure that all bits have been written to the stream
                jsonWriter.Flush();

                memoryStream.Position = 0;

                if (isConvertableObject)
                {
                    jsonReader.Read();

                    var newConvertableObject = new T() as IJsonConvertable;
                    newConvertableObject.FromJson(jsonReader);

                    result = newConvertableObject as T;
                }
                else
                {
                    result = _jsonSerializer.Deserialize<T>(jsonReader);
                }
            }

            return result;
        }


    }
}


