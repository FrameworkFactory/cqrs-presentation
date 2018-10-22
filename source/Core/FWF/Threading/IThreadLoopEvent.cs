using System;

namespace FWF.Threading
{
    public interface IThreadLoopEvent
    {
        bool IsCancelled
        {
            get;
        }

        void Cancel();
    }
}


