using FWF.Data.Local;
using System;
using System.Collections.Generic;

namespace FWF.Basketball.Logic.Data
{
    public interface IGameDataRepository : ILocalDataContext
    {

        IEnumerable<T> GetAll<T>() where T : class;

        T FirstOrDefault<T>(Func<T, bool> predicate) where T : class;

        T GetRandom<T>() where T : class;
        T GetRandom<T>(Func<T, bool> predicate) where T : class;

    }
}
