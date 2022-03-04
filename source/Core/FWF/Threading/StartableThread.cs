using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FWF.Configuration;
using FWF.Logging;

namespace FWF.Threading
{

    public class StartableThread : Startable
    {
        private string _name;
        private bool _isBackground;
        private StartableThreadLoop _threadLoop;
        private TimeSpan _threadLatency;

        private readonly TimeSpan _loopLatency = TimeSpan.FromMilliseconds(50);

        private readonly ManualResetEventSlim _manualResetEvent = new ManualResetEventSlim(false);
        private Thread _thread;

        private readonly IAppSettings _appSettings;
        private readonly ILog _log;
        
        public StartableThread(
            IAppSettings appSettings,
            ILogFactory logFactory
            )
        {
            _appSettings = appSettings;

            _log = logFactory.CreateForType(this);
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public TimeSpan ThreadLatency
        {
            get { return _threadLatency; }
            set { _threadLatency = value; }
        }

        public StartableThreadLoop ThreadLoop
        {
            get { return _threadLoop; }
            set {
                if (IsRunning)
                {
                    throw new InvalidOperationException("Unable to change ThreadLoop while thread is running");
                }
                _threadLoop = value;
            }
        }

        public bool IsBackground
        {
            get { return _isBackground; }
            set {
                if (IsRunning)
                {
                    throw new InvalidOperationException("Unable to change IsBackground while thread is running");
                }
                _isBackground = value;
            }
        }

        protected override void OnStart()
        {
            if (ReferenceEquals(_threadLoop, null))
            {
                throw new InvalidOperationException("Unable to start thread - missing thread loop delegate");
            }

            _thread = new Thread(ThreadStart)
            {
                Name = _name,
                IsBackground = _isBackground
            };

            _manualResetEvent.Reset();

            _thread.Start();
        }

        protected override void OnStop()
        {
            _manualResetEvent.Set();

            if (!ReferenceEquals(_thread, null))
            {
                if (_thread.IsAlive)
                {
                    try
                    {
                        _thread.Join(TimeSpan.FromSeconds(5));
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex, ex.Message);
                    }
                }
            }
        }

        private void ThreadStart(object state)
        {
            if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
            {
                Thread.CurrentThread.Name = _name;
            }

            var wrapper = new ThreadLoopEventWrapper
            {
                ResetEvent = _manualResetEvent
            };

            var stopwatch = new Stopwatch();

            // Thread loop
            while (true)
            {
                stopwatch.Restart();

                // Do work
                Task task = null;

                // Record the "exact" time before we do work.  The thread latency is the amount
                // of time between tasks - not work time + latency.
                var ticksBeforeLoop = stopwatch.Elapsed.Ticks;

                try
                {
                    task = _threadLoop(wrapper);
                }
                catch(Exception ex)
                {
                    _log.Error(ex, ex.Message);
                }

                /*
                 * Thread Latency
                 *  - The amount of time from BEFORE the work starts to the next available time to start it again
                 *  - This means that we are "waiting" for both the work to finish and a signal from the reset event
                 *  - This will ensure that if we have a task running every second:
                 *    - It will start on the start of every second
                 *    - The amount of time the task runs doesn't matter...
                */

                // Look for either the completed thread work, or the stop request
                while (true)
                {
                    // Has the reset event been set (stopped)?
                    var didSignal = _manualResetEvent.IsSet || _manualResetEvent.Wait(_loopLatency);

                    // Thread has been requested to be stopped - wait until the loop is done then exit
                    if (didSignal)
                    {
                        // This is the last thread loop, exit once the task is complete
                        if (!ReferenceEquals(task, null))
                        {
                            try
                            {
                                task.Wait(); // TODO: Timeout here?  With exception?
                            }
                            catch(Exception ex)
                            {
                                _log.Error(ex, ex.Message);
                            }
                        }
                        return;
                    }
                                        
                    // If the task is completed, then wait
                    if (ReferenceEquals(task, null) || task.IsCompleted)
                    {
                        /*
                         * How long to wait
                         *  - If the work task took 1 second
                         *  - And the Thread Latency is 5 seconds
                         *  - The wait time here should be 4 seconds
                        */
                        var timeToWait = _threadLatency.Ticks - ticksBeforeLoop;

                        // Make sure that we wait before starting another thread work task again
                        if (timeToWait > 0)
                        {
                            var time = TimeSpan.FromTicks(timeToWait);
                            didSignal = _manualResetEvent.Wait(time);
                        }

                        // If we did get a signal then stop processing, otherwise 
                        // return back to the work method
                        if (didSignal)
                        {
                            return;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

        }

    }
}




