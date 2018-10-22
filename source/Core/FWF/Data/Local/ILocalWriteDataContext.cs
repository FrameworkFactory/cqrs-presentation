using System;

namespace FWF.Data.Local
{
    public interface ILocalWriteDataContext : IDisposable
    {
        void Insert<T>(T dataObject);
        void Update<T>(T dataObject);
        void Delete<T>(T dataObject);
    }
}

