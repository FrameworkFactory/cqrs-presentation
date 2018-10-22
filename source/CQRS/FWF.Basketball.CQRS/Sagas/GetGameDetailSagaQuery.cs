using FWF.ComponentModel;
using FWF.CQRS;
using FWF.Json;
using System;
using System.Collections.Generic;

namespace FWF.Basketball.CQRS.Sagas
{
    public class GetGameDetailSagaQuery : IQuery
    {
        public IEnumerable<ValidationError> Validate()
        {
            if (TeamId.IsNull() || TeamId == Guid.Empty)
            {
                yield return new ValidationError { PropertyName = "TeamId", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
        }

        public Guid? TeamId { get; set; }

        public void ToJson(IJsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("teamId");
            writer.Write(this.TeamId);
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
                    case "teamId":
                        this.TeamId = reader.Read<Guid?>(null);
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


    public class GetGameDetailSagaQueryResponse : QueryResponseMultiple<GameDetail>
    {
    }
}


