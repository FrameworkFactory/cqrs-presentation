using FWF.ComponentModel;
using FWF.CQRS;
using FWF.Json;
using System;
using System.Collections.Generic;

namespace FWF.Basketball.CQRS.Queries
{
    public class GetPlayerQuery : IQuery
    {

        public IEnumerable<ValidationError> Validate()
        {
            if ((Id.IsNull() || Id == Guid.Empty))
            {
                yield return new ValidationError { PropertyName = "Id", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
        }

        public Guid? Id { get; set; }

        public void ToJson(IJsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("id");
            writer.Write(this.Id);
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
                    case "id":
                        this.Id = reader.Read<Guid?>(null);
                        break;

                    default:
                        reader.Read();
                        break;
                }

                var didRead = reader.Read();

                if (!didRead)
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


    public class GetPlayerQueryResponse : QueryResponseSingle<Player>
    {
    }
}


