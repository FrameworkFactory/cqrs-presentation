
using FWF.ComponentModel;
using System;
using System.Collections.Generic;
using FWF.Json;

namespace FWF.CQRS
{
    public abstract class EntityEventBase : EventBase
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }

        public override IEnumerable<ValidationError> Validate()
        {
            foreach (var item in base.Validate())
            {
                yield return item;
            }

            if (this.Id.IsNull() || this.Id == Guid.Empty)
            {
                yield return new ValidationError { PropertyName = "id", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
            if (this.Name.IsMissing())
            {
                yield return new ValidationError { PropertyName = "name", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
        }

        public override void ToJsonData(IJsonWriter writer)
        {
            writer.WritePropertyName("id");
            writer.Write(this.Id);
            writer.WritePropertyName("name");
            writer.Write(this.Name);
        }

        public override void FromJsonData(IJsonReader reader)
        {
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
        }

    }
}

