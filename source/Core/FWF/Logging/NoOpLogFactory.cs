using System;

namespace FWF.Logging
{
    public class NoOpLogFactory : Startable, ILogFactory
    {

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        public ILog CreateForType(object instance)
        {
            return new NoOpLog();
        }

    }
}


