using System;
using FWF.Logging;

namespace FWF.Test.Logging
{
    public class UnitTestLogFactory : Startable, ILogFactory
    {
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        public string LogDirectory
        {
            get;
            set;
        }

        public ILog CreateForType<T>()
        {
            return new UnitTestLog(typeof(T).FullName);
        }

        public ILog CreateForType(Type type)
        {
            return new UnitTestLog(type.FullName);
        }

        public ILog CreateForType(object instance)
        {
            if (ReferenceEquals(instance, null))
            {
                throw new ArgumentNullException("instance");
            }

            return new UnitTestLog(instance.GetType().FullName);
        }

        public ILog Create(string logFullName)
        {
            return new UnitTestLog(logFullName);
        }
    }
}



