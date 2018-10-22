using System;

namespace FWF.Data.Local
{
    internal class LocalWriteDataContext : ILocalWriteDataContext
    {
        private readonly LocalDataContext _dataContext;

        public LocalWriteDataContext(LocalDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        ~LocalWriteDataContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing)
                {
                    GC.SuppressFinalize(this);
                }
            }
        }

        public void Insert<T>(T dataObject)
        {
            var list = _dataContext.GetListForType(typeof(T));
            list.Add(dataObject);
        }

        public void Update<T>(T dataObject)
        {
            var list = _dataContext.GetListForType(typeof(T));

            lock (list)
            {
                var indexAt = list.IndexOf(dataObject);

                if (indexAt != -1)
                {
                    list.RemoveAt(indexAt);
                    list.Add(dataObject);
                }
            }
        }

        public void Delete<T>(T dataObject)
        {
            var list = _dataContext.GetListForType(typeof(T));

            list.Remove(dataObject);
        }
    }
}

