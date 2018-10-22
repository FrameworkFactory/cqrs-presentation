using System;

namespace FWF
{
    public interface IAcceleratedClock : IClock
    {


        decimal AccelerationFactor
        {
            get;
            set;
        }

    }
}

