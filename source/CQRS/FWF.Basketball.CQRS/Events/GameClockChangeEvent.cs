using FWF.ComponentModel;
using FWF.CQRS;
using FWF.Json;
using System;
using System.Collections.Generic;

namespace FWF.Basketball.CQRS.Events
{
    public class GameClockChangeEvent : EventBase
    {
        
        public Guid? GameId { get; set; }
        public string GameName { get; set; }
        public Guid? AwayTeamId { get; set; }
        public Guid? HomeTeamId { get; set; }
        public int? Quarter { get; set; }
        public string GameClock { get; set; }


        public override IEnumerable<ValidationError> Validate()
        {
            foreach (var item in base.Validate())
            {
                yield return item;
            }

            if (this.GameId.IsNull() || this.GameId == Guid.Empty)
            {
                yield return new ValidationError { PropertyName = "GameId", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
            if (this.GameName.IsMissing())
            {
                yield return new ValidationError { PropertyName = "GameName", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
            if (this.AwayTeamId.IsNull() || this.AwayTeamId == Guid.Empty)
            {
                yield return new ValidationError { PropertyName = "AwayTeamId", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
            if (this.HomeTeamId.IsNull() || this.HomeTeamId == Guid.Empty)
            {
                yield return new ValidationError { PropertyName = "HomeTeamId", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
            if (this.Quarter < 1)
            {
                yield return new ValidationError { PropertyName = "Quarter", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }
            if (this.GameClock.IsMissing())
            {
                yield return new ValidationError { PropertyName = "GameClock", ErrorMessage = ValidationMessages.CannotBeNullOrEmpty };
            }


        }

        public override void ToJsonData(IJsonWriter writer)
        {
            writer.WritePropertyName("gameId");
            writer.Write(this.GameId);
            writer.WritePropertyName("gameName");
            writer.Write(this.GameName);
            writer.WritePropertyName("awayTeamId");
            writer.Write(this.AwayTeamId);
            writer.WritePropertyName("homeTeamId");
            writer.Write(this.HomeTeamId);
            writer.WritePropertyName("quarter");
            writer.Write(this.Quarter);
            writer.WritePropertyName("gameClock");
            writer.Write(this.GameClock);
        }

        public override void FromJsonData(IJsonReader reader)
        {
            var propertyName = reader.Value as string;

            switch (propertyName)
            {
                case "gameId":
                    this.GameId = reader.Read<Guid?>(null);
                    break;
                case "gameName":
                    this.GameName = reader.Read<string>(null);
                    break;
                case "awayTeamId":
                    this.AwayTeamId = reader.Read<Guid?>(null);
                    break;
                case "homeTeamId":
                    this.HomeTeamId = reader.Read<Guid?>(null);
                    break;
                case "quarter":
                    this.Quarter = reader.Read<int?>(null);
                    break;
                case "gameClock":
                    this.GameClock = reader.Read<string>(null);
                    break;

                default:
                    reader.Read();
                    break;
            }
        }


    }
}
