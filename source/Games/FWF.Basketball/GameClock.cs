using System;

namespace FWF.Basketball
{
    public class GameClock
    {

        public static int NumberOfQuarters = 4;
        public static TimeSpan TimeEachQuarter = TimeSpan.FromMinutes(10);

        public GameClock()
        {
            Quarter = 1;
            Time = TimeEachQuarter;
        }

        public int Quarter
        {
            get; set;
        }

        public TimeSpan Time
        {
            get; set;
        }

        public bool IsComplete
        {
            get { return this.Quarter >= NumberOfQuarters && this.Time <= TimeSpan.Zero; }
        }

    }
}
