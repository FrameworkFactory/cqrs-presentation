using Autofac;
using FWF.Basketball.CQRS;
using FWF.Basketball.Logic;
using FWF.Basketball.Logic.Data;
using FWF.CQRS;
using FWF.Logging;

namespace FWF.Basketball.Console
{
    internal class ConsoleService : AbstractService
    {

        private readonly IGamePlayEngine _gamePlayEngine;
        private readonly IGamePlayListener _gamePlayListener;

        private readonly IReadCacheDataRepository _readCacheDataRepository;
        private readonly IGameDataRepository _gameDataRepository;

        private readonly ICqrsLogicHandler _logicHandler;
        private readonly ILog _log;

        public ConsoleService(
            IGamePlayEngine gamePlayEngine,
            IGamePlayListener gamePlayListener,
            IReadCacheDataRepository readCacheDataRepository,
            IGameDataRepository gameDataRepository,
            ICqrsLogicHandler logicHandler,
            IComponentContext componentContext,
            ILogFactory logFactory
            ) : base(componentContext, logFactory)
        {
            _gamePlayEngine = gamePlayEngine;
            _gamePlayListener = gamePlayListener;
            _readCacheDataRepository = readCacheDataRepository;
            _gameDataRepository = gameDataRepository;

            _logicHandler = logicHandler;

            _log = logFactory.CreateForType(this);
        }

        protected override void OnStart()
        {
            base.OnStart();

            _readCacheDataRepository.Start();
            _gameDataRepository.Start();

            _logicHandler.Start();


            _gamePlayEngine.Subscribe(_gamePlayListener);

            _gamePlayEngine.Start();

        }

        protected override void OnStop()
        {
            _gamePlayEngine.Stop();

            _logicHandler.Stop();

            _readCacheDataRepository.Stop();
            _gameDataRepository.Stop();


            base.OnStop();
        }


    }
}
