using System;

namespace FWF.Basketball
{
    public partial class Player : Entity<Player>
    {

        public Guid TeamId
        {
            get; set;
        }

        public string Number // NOTE: Need to show some non-numerical numbers, i.e. "00"
        {
            get; set;
        }

        public string Position
        {
            get; set;
        }

    }
}

