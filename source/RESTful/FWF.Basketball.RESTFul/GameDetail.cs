using System;

namespace FWF.Basketball.RESTFul
{
    public class GameDetail
    {

        public Guid Id
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public int Quarter
        {
            get; set;
        }

        public string GameClock
        {
            get; set;
        }

        public Guid AwayTeamId
        {
            get; set;
        }

        public int AwayScore
        {
            get; set;
        }

        public Guid HomeTeamId
        {
            get; set;
        }

        public int HomeScore
        {
            get; set;
        }


    }
}
