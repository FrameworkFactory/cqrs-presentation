using FWF.ComponentModel;
using FWF.CQRS;
using FWF.Json;
using System;
using System.Collections.Generic;

namespace FWF.Basketball.CQRS.Commands
{
    public class UpdatePlayerCommand : ICommand
    {

        public IEnumerable<ValidationError> Validate()
        {
            if (Id.IsNull() || Id == Guid.Empty)
            {
                yield return new ValidationError { PropertyName = "Id", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
            if (Name.IsMissing())
            {
                yield return new ValidationError { PropertyName = "Name", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
            if (TeamId.IsNull() || TeamId == Guid.Empty)
            {
                yield return new ValidationError { PropertyName = "TeamId", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
            if (Version.IsNull() || Version.Length == 0)
            {
                yield return new ValidationError { PropertyName = "Version", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
        }


        public Guid? Id { get; set; }

        public Guid? TeamId { get; set; }

        public string Name { get; set; }


        public byte[] Version { get; set; }


        public void ToJson(IJsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("id");
            writer.Write(this.Id);
            writer.WritePropertyName("teamId");
            writer.Write(this.TeamId);
            writer.WritePropertyName("name");
            writer.Write(this.Name);

            writer.WritePropertyName("version");
            writer.Write(this.Version);

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
                    case "teamId":
                        this.TeamId = reader.Read<Guid?>(null);
                        break;
                    case "name":
                        this.Name = reader.Read<string>(null);
                        break;
                    

                    case "version":
                        this.Version = reader.Read<byte[]>(null);
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
}

