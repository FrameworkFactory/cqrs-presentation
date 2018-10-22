using FWF.ComponentModel;
using FWF.CQRS;
using FWF.Json;
using System;
using System.Collections.Generic;

namespace FWF.Basketball.CQRS.Events
{
    public class PlayerDetailChangeEvent : EventBase
    {

        public Guid? Id { get; set; }
        public string Name { get; set; }
        public Guid? TeamId { get; set; }
        public string Position { get; set; }
        public int? TotalPoints { get; set; }


        public override IEnumerable<ValidationError> Validate()
        {
            foreach (var item in base.Validate())
            {
                yield return item;
            }

            if (this.Id.IsNull() || this.Id == Guid.Empty)
            {
                yield return new ValidationError { PropertyName = "Id", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
            if (this.Name.IsMissing())
            {
                yield return new ValidationError { PropertyName = "Name", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
            if (this.TeamId.IsNull() || this.TeamId == Guid.Empty)
            {
                yield return new ValidationError { PropertyName = "TeamId", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
            if (this.Position.IsMissing())
            {
                yield return new ValidationError { PropertyName = "Position", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
            if (this.TotalPoints < 0)
            {
                yield return new ValidationError { PropertyName = "TotalPoints", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
        }

        public override void ToJsonData(IJsonWriter writer)
        {
            writer.WritePropertyName("id");
            writer.Write(this.Id);
            writer.WritePropertyName("name");
            writer.Write(this.Name);
            writer.WritePropertyName("teamId");
            writer.Write(this.TeamId);
            writer.WritePropertyName("position");
            writer.Write(this.Position);
            writer.WritePropertyName("totalPoints");
            writer.Write(this.TotalPoints);
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
                case "teamId":
                    this.TeamId = reader.Read<Guid?>(null);
                    break;
                case "position":
                    this.Position = reader.Read<string>(null);
                    break;
                case "totalPoints":
                    this.TotalPoints = reader.Read<int?>(null);
                    break;

                default:
                    reader.Read();
                    break;
            }
        }


    }
}
