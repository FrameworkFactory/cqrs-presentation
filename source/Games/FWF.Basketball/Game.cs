using System;

namespace FWF.Basketball
{
    public partial class Game : Entity<Game>
    {
        
        public Guid HomeTeamId
        {
            get; set;
        }

        public Guid AwayTeamId
        {
            get; set;
        }

        public DateTime LastScoreTimestamp
        {
            get; set;
        }

    }
}

