using FWF.ComponentModel;
using System;
using System.Collections.Generic;
using FWF.Json;

namespace FWF.CQRS
{
    public abstract class EventBase : IEvent
    {
        public Guid? EventId { get; set; }

        public DateTime? EventTimestamp { get; set; }

        public OperationMask Operation
        {
            get; set;
        }

        public virtual IEnumerable<ValidationError> Validate()
        {
            if (this.EventId.IsNull() || this.EventId == Guid.Empty)
            {
                yield return new ValidationError { PropertyName = "eventId", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
            if (this.EventTimestamp.IsNull() || this.EventTimestamp == DateTime.MinValue)
            {
                yield return new ValidationError { PropertyName = "eventTimestamp", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
        }

        public abstract void ToJsonData(IJsonWriter jsonWriter);
        public abstract void FromJsonData(IJsonReader reader);

        public void ToJson(IJsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("eventId");
            writer.Write(this.EventId);
            writer.WritePropertyName("eventTimestamp");
            writer.Write(this.EventTimestamp);

            ToJsonData(writer);

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
                    case "eventId":
                        this.EventId = reader.Read<Guid?>(null);
                        break;
                    case "eventTimestamp":
                        this.EventTimestamp = reader.Read<DateTime?>(null);
                        break;

                    default:
                        FromJsonData(reader);
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
}

