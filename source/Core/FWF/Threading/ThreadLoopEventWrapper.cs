using System;
using System.Threading;

namespace FWF.Threading
{
    internal class ThreadLoopEventWrapper : IThreadLoopEvent
    {
        public ManualResetEventSlim ResetEvent
        {
            get;
            set;
        }

        public bool IsCancelled
        {
            get { return this.ResetEvent.IsSet; }
        }

        public void Cancel()
        {
            this.ResetEvent.Set();
        }

    }
}


