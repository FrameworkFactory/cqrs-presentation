using FWF.ComponentModel;
using FWF.CQRS;
using FWF.Json;
using System;
using System.Collections.Generic;

namespace FWF.Basketball.CQRS.Events
{
    public class PlayerFantasyChangeEvent : EventBase
    {

        public Guid? Id { get; set; }
        public string Name { get; set; }
        public Guid? TeamId { get; set; }
        public int? FantasyPoints { get; set; }


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
            if (this.FantasyPoints < 0)
            {
                yield return new ValidationError { PropertyName = "FantasyPoints", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
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
            writer.WritePropertyName("fantasyPoints");
            writer.Write(this.FantasyPoints);
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
                case "fantasyPoints":
                    this.FantasyPoints = reader.Read<int?>(null);
                    break;

                default:
                    reader.Read();
                    break;
            }
        }


    }
}


