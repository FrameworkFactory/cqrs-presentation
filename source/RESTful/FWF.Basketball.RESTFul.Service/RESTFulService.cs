using Autofac;
using FWF.Basketball.Logic;
using FWF.CQRS;
using FWF.Logging;

namespace FWF.Basketball.RESTFul.Service
{
    internal class RESTFulService : AbstractService
    {

        private readonly IRESTFulGameEngineListener _gameEngineListener;
        private readonly IGamePlayEngine _gamePlayEngine;

        private readonly ICqrsLogicHandler _logicHandler;
        private readonly ILog _log;

        public RESTFulService(
            IRESTFulGameEngineListener gameEngineListener,
            IGamePlayEngine gamePlayEngine,
            ICqrsLogicHandler logicHandler,
            IComponentContext componentContext,
            ILogFactory logFactory
            ) : base(componentContext, logFactory)
        {
            _gameEngineListener = gameEngineListener;
            _gamePlayEngine = gamePlayEngine;

            _logicHandler = logicHandler;

            _log = logFactory.CreateForType(this);
        }

        protected override void OnStart()
        {
            base.OnStart();

            _logicHandler.Start();

            _gamePlayEngine.Subscribe(_gameEngineListener);

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
