
using System;

namespace FWF.Basketball
{
    public class Score : Entity<Score>
    {

        public Guid GameId
        {
            get; set;
        }

        public string GameName
        {
            get; set;
        }

        public Guid TeamId
        {
            get; set;
        }

        public string TeamName
        {
            get; set;
        }

        public Guid PlayerId
        {
            get; set;
        }

        public string PlayerName
        {
            get; set;
        }

        public string PlayerPosition
        {
            get; set;
        }

        public int Points
        {
            get; set;
        }


    }
}
