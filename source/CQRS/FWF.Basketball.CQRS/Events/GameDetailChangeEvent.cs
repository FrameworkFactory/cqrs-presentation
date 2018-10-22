using FWF.ComponentModel;
using FWF.CQRS;
using FWF.Json;
using System;
using System.Collections.Generic;

namespace FWF.Basketball.CQRS.Events
{
    public class GameDetailChangeEvent : EventBase
    {

        public Guid? Id { get; set; }
        public string Name { get; set; }
        public int? Quarter { get; set; }
        public string GameClock { get; set; }
        public Guid? AwayTeamId { get; set; }
        public int? AwayScore { get; set; }
        public Guid? HomeTeamId { get; set; }
        public int? HomeScore { get; set; }


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
            if (this.Quarter.IsNull() || this.Quarter < 1)
            {
                yield return new ValidationError { PropertyName = "Quarter", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
            if (this.GameClock.IsMissing())
            {
                yield return new ValidationError { PropertyName = "GameClock", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
            if (this.AwayTeamId.IsNull() || this.AwayTeamId == Guid.Empty)
            {
                yield return new ValidationError { PropertyName = "AwayTeamId", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
            if (this.AwayScore < 0)
            {
                yield return new ValidationError { PropertyName = "AwayScore", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
            if (this.HomeTeamId.IsNull() || this.HomeTeamId == Guid.Empty)
            {
                yield return new ValidationError { PropertyName = "HomeTeamId", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
            if (this.HomeScore < 0)
            {
                yield return new ValidationError { PropertyName = "HomeScore", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
        }

        public override void ToJsonData(IJsonWriter writer)
        {
            writer.WritePropertyName("id");
            writer.Write(this.Id);
            writer.WritePropertyName("name");
            writer.Write(this.Name);
            writer.WritePropertyName("quarter");
            writer.Write(this.Quarter);
            writer.WritePropertyName("gameClock");
            writer.Write(this.GameClock);
            writer.WritePropertyName("awayTeamId");
            writer.Write(this.AwayTeamId);
            writer.WritePropertyName("awayScore");
            writer.Write(this.AwayScore);
            writer.WritePropertyName("homeTeamId");
            writer.Write(this.HomeTeamId);
            writer.WritePropertyName("homeScore");
            writer.Write(this.HomeScore);
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
                case "quarter":
                    this.Quarter = reader.Read<int?>(null);
                    break;
                case "gameClock":
                    this.GameClock = reader.Read<string>(null);
                    break;
                case "awayTeamId":
                    this.AwayTeamId = reader.Read<Guid?>(null);
                    break;
                case "awayScore":
                    this.AwayScore = reader.Read<int?>(null);
                    break;
                case "homeTeamId":
                    this.HomeTeamId = reader.Read<Guid?>(null);
                    break;
                case "homeScore":
                    this.HomeScore = reader.Read<int?>(null);
                    break;
                default:
                    reader.Read();
                    break;
            }
        }


    }
}
