
using FWF.Json;
using System.Collections.Generic;

namespace FWF.CQRS
{
    public abstract class QueryResponseMultiple<T> : IQueryResponse where T : class
    {

        protected QueryResponseMultiple()
        {
            // Should start with empty collection rather than NULL
            this.Items = new List<T>();
        }

        public int? ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public IEnumerable<T> Items
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

            writer.WritePropertyName("items");
            writer.WriteStartArray();

            foreach (var item in this.Items)
            {
                JSON.GetSerializer().Serialize(writer, item);
            }

            writer.WriteEndArray();

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
                    case "items":
                        reader.Read();
                        reader.Read();

                        var items = new List<T>();

                        while (reader.TokenType == JsonToken.StartObject)
                        {
                            var item = JSON.GetSerializer().Deserialize(reader, typeof(T)) as T;

                            items.Add(item);

                            if (!reader.Read())
                            {
                                break;
                            }
                        }

                        this.Items = items;
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

