
using FWF.ComponentModel;
using FWF.CQRS;
using FWF.Json;
using System;
using System.Collections.Generic;

namespace FWF.Basketball.CQRS.Commands
{
    public class AddPlayersCommand : ICommand
    {

        private IEnumerable<AddPlayersCommandItem> _items = new List<AddPlayersCommandItem>();

        public class AddPlayersCommandItem
        {
            public Guid? Id { get; set; }

            public string Name { get; set; }


            public void ToJson(IJsonWriter writer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("id");
                writer.Write(this.Id);
                writer.WritePropertyName("name");
                writer.Write(this.Name);
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
                        case "name":
                            this.Name = reader.Read<string>(null);
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

        public Guid? TeamId { get; set; }

        public IEnumerable<AddPlayersCommandItem> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public IEnumerable<ValidationError> Validate()
        {
            if (TeamId.IsNull() || TeamId == Guid.Empty)
            {
                yield return new ValidationError { PropertyName = "TeamId", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
        }

        public void ToJson(IJsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("teamId");
            writer.Write(this.TeamId);
            writer.WritePropertyName("items");

            writer.WriteStartArray();
            foreach (var item in _items)
            {
                item.ToJson(writer);
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
                    case "tenantId":
                        this.TeamId = reader.Read<Guid?>(null);
                        break;
                    case "items":
                        reader.Read();
                        reader.Read();

                        var items = new List<AddPlayersCommandItem>();

                        while (reader.TokenType == JsonToken.StartObject)
                        {
                            var item = new AddPlayersCommandItem();

                            item.FromJson(reader);

                            items.Add(item);
                        }

                        this.Items = items;
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


