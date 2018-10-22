
using FWF.Json;
using System;

namespace FWF.CQRS
{
    public abstract class QueryResponseSingle<T> : IQueryResponse where T : class 
    {
        public int? ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public T Item
        {
            get; set;
        }

        public void ToJson(IJsonWriter writer)
        {
            writer.WriteStartObject();

            if (this.ErrorCode.HasValue)
            {
                writer.WritePropertyName("errorCode");
                writer.Write(this.ErrorCode);
            }

            if (this.ErrorMessage.IsPresent())
            {
                writer.WritePropertyName("errorMessage");
                writer.Write(this.ErrorMessage);
            }

            writer.WritePropertyName("item");

            JSON.GetSerializer().Serialize(writer, this.Item);
            
            writer.WriteEndObject();
        }

        public void FromJson(IJsonReader reader)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                reader.Read();
            }

            while (true)
            {
                if (reader.TokenType != JsonToken.PropertyName)
                {
                    break;
                }

                var propertyName = reader.Value as string;

                switch (propertyName)
                {
                    case "errorCode":
                        this.ErrorCode = reader.Read<int?>(null);
                        break;
                    case "errorMessage":
                        this.ErrorMessage = reader.Read<string>(null);
                        break;
                    case "item":
                        reader.Read();

                        this.Item = JSON.GetSerializer().Deserialize(reader, typeof(T)) as T;
                        break;

                    default:
                        reader.Read();
                        break;
                }

                if (!reader.Read())
                {
                    break;
                }
            }

            if (reader.TokenType != JsonToken.EndObject)
            {
                reader.Read();
            }
        }

    }
}

