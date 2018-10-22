using System;
using System.Collections.Generic;

namespace FWF.Data.Local
{
    public interface ILocalDataContext : IRunnable
    {
        
        IEnumerable<T> Get<T>(Func<T, bool> predicate) where T : class;

        ILocalWriteDataContext BeginWrite();

    }
}

