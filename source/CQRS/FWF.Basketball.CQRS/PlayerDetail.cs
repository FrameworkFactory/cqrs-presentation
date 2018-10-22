
using System;

namespace FWF.Basketball.CQRS
{
    public class PlayerDetail
    {
        public Guid Id
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public Guid TeamId
        {
            get; set;
        }

        public string Position
        {
            get; set;
        }

        public int TotalPoints
        {
            get; set;
        }

    }
}
