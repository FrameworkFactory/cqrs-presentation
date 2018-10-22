using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FWF.Data.Local
{
    internal class LocalDataContext : Startable, ILocalDataContext
    {
        private readonly IDictionary<Type, IList> _items = new ConcurrentDictionary<Type, IList>();
        private volatile object _lockObject = new object();

        public LocalDataContext(
            )
        {
        }

        protected override void OnStart()
        {
            lock (_lockObject)
            {
                _items.Clear();
            }
        }

        protected override void OnStop()
        {
            lock (_lockObject)
            {
                _items.Clear();
            }
        }


        public virtual IEnumerable<T> Get<T>(Func<T, bool> predicate) where T : class
        {
            if (!IsRunning)
            {
                throw new Exception("Component not started - ReadOnlyLocalDataContext");
            }

            lock (_lockObject)
            {
                if (!_items.ContainsKey(typeof(T)))
                {
                    _items.Add(typeof(T), new ArrayList());
                }
            }

            var typedItems = _items[typeof(T)];

            var returnItems = new List<T>();

            lock (_lockObject)
            {
                var enumerator = typedItems.GetEnumerator();

                while (true)
                {
                    if (!enumerator.MoveNext())
                    {
                        break;
                    }

                    if (predicate(enumerator.Current as T))
                    {
                        returnItems.Add(enumerator.Current as T);
                    }
                }
            }

            return returnItems;
        }

        public IList GetListForType(Type itemType)
        {
            if (!IsRunning)
            {
                throw new Exception("Component not started - ReadOnlyLocalDataContext");
            }

            lock (_lockObject)
            {
                if (!_items.ContainsKey(itemType))
                {
                    _items.Add(itemType, new ArrayList());
                }
            }

            var typedItems = _items[itemType];

            return typedItems;
        }

        public ILocalWriteDataContext BeginWrite()
        {
            if (!IsRunning)
            {
                throw new Exception("Component not started - LocalDataContext");
            }

            return new LocalWriteDataContext(this);
        }


    }
}





