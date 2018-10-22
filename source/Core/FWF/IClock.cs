using System;

namespace FWF
{
    public interface IClock : IRunnable, INow 
    {

        DateTime StartTimestamp
        {
            get;
        }

        TimeSpan Elapsed
        {
            get;
        }
        
    }
}

