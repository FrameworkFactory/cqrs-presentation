using FWF.Configuration;
using FWF.Data.Local;
using FWF.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FWF.Basketball.Logic.Data
{
    internal class GameDataRepository : Startable, ILocalDataContext, IGameDataRepository
    {
        private readonly ILocalDataContext _dataContext;
        private readonly IAppSettings _appSettings;
        private readonly IRandom _random;
        private readonly ILog _log;

        public GameDataRepository(
            ILocalDataContext localDataContext,
            IAppSettings appSettings,
            IRandom random,
            ILogFactory logFactory
            )
        {
            _dataContext = localDataContext;
            _appSettings = appSettings;
            _random = random;

            _log = logFactory.CreateForType(this);
        }

        protected override void OnStart()
        {
            _dataContext.Start();
        }

        protected override void OnStop()
        {
            _dataContext.Stop();
        }

        public IEnumerable<T> Get<T>(Func<T, bool> predicate) where T : class
        {
            return _dataContext.Get<T>(predicate);
        }

        public IEnumerable<T> GetAll<T>() where T : class
        {
            return _dataContext.Get<T>(x => true);
        }

        public T FirstOrDefault<T>(Func<T, bool> predicate) where T : class
        {
            return Get(predicate).FirstOrDefault();
        }

        public T GetRandom<T>() where T : class
        {
            var allItems = GetAll<T>().ToList().AsReadOnly();

            return _random.Any(allItems);
        }

        public T GetRandom<T>(Func<T, bool> predicate) where T : class
        {
            var allItems = Get<T>(predicate).ToList().AsReadOnly();

            return _random.Any(allItems);
        }


        public ILocalWriteDataContext BeginWrite()
        {
            return _dataContext.BeginWrite();
        }


    }
}
