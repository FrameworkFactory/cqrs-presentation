using System;

namespace FWF
{
    internal class DateTimeNow : INow 
    {

        public DateTime Now
        {
            get { return DateTime.UtcNow; }
        }

    }
}

