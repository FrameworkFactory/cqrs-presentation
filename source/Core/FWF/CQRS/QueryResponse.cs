using FWF.Json;


namespace FWF.CQRS
{
    public class QueryResponse : IQueryResponse
    {

        public int? ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

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

