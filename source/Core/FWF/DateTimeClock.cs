using System;

namespace FWF
{
    internal class DateTimeClock : Startable, IClock, IAcceleratedClock
    {

        private decimal _accelerationFactor = 1;
        private DateTime _onStartTimestamp;

        private INow _now;

        private class SettableDateTimeNow : INow 
        {
            private readonly TimeSpan _nowDelta;

            public SettableDateTimeNow(DateTime dateTime)
            {
                _nowDelta = DateTime.UtcNow - dateTime;
            }

            public DateTime Now 
            {
                get { return DateTime.UtcNow.Add(_nowDelta); }
            }
        }

        public DateTimeClock(INow now)
        {
            _now = now;
        }

        public DateTimeClock(DateTime dateTime)
        {
            _now = new SettableDateTimeNow(dateTime);
        }

        protected override void OnStart()
        {
            _onStartTimestamp = _now.Now;
        }

        protected override void OnStop()
        {
            _onStartTimestamp = DateTime.MinValue;
        }

        public DateTime StartTimestamp
        {
            get { return _onStartTimestamp; }
        }

        public decimal AccelerationFactor
        {
            get { return _accelerationFactor; }
            set { _accelerationFactor = value; }
        }

        public TimeSpan Elapsed
        {
            get
            {
                if (!IsRunning)
                {
                    return TimeSpan.Zero;
                }

                // NOTE: Take the real amount of time that has elapsed 
                // and multiply it by an acceleration factor

                var startTicks = _onStartTimestamp.Ticks;
                var diff = (_now.Now - _onStartTimestamp).Ticks;

                var accelratedTicks = (long)Math.Round(diff * _accelerationFactor, 0);

                return TimeSpan.FromTicks(accelratedTicks);
            }
        }

        public DateTime Now
        {
            get
            {
                if (!IsRunning)
                {
                    return DateTime.MinValue;
                }

                return _now.Now;
            }
        }


    }
}

