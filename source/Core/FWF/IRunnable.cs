using System;

namespace FWF
{
    public interface IRunnable
    {
        bool IsRunning
        {
            get;
        }

        void Start();

        void Stop();
    }
}



