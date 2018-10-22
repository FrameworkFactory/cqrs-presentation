using Autofac;
using FWF.Basketball.Logic;
using FWF.CQRS;
using FWF.Logging;

namespace FWF.Basketball.Console
{
    internal class ConsoleService : AbstractService
    {

        private readonly IGamePlayEngine _gamePlayEngine;
        private readonly IGamePlayListener _gamePlayListener;

        private readonly ICqrsLogicHandler _logicHandler;
        private readonly ILog _log;

        public ConsoleService(
            IGamePlayEngine gamePlayEngine,
            IGamePlayListener gamePlayListener,
            ICqrsLogicHandler logicHandler,
            IComponentContext componentContext,
            ILogFactory logFactory
            ) : base(componentContext, logFactory)
        {
            _gamePlayEngine = gamePlayEngine;
            _gamePlayListener = gamePlayListener;

            _logicHandler = logicHandler;

            _log = logFactory.CreateForType(this);
        }

        protected override void OnStart()
        {
            base.OnStart();

            _logicHandler.Start();


            _gamePlayEngine.Subscribe(_gamePlayListener);

            _gamePlayEngine.Start();

        }

        protected override void OnStop()
        {
            _gamePlayEngine.Stop();

            _logicHandler.Stop();

            base.OnStop();
        }


    }
}
