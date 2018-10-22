using Autofac;
using FWF.Logging;

namespace FWF.Basketball.RESTFul.App
{
    internal class RESTFulApp : AbstractApp
    {

        private readonly ILog _log;

        public RESTFulApp(
            IComponentContext componentContext,
            ILogFactory logFactory
            ) : base(componentContext, logFactory)
        {

            _log = logFactory.CreateForType(this);
        }

        protected override void OnStart()
        {
            base.OnStart();

        }

        protected override void OnStop()
        {
            base.OnStop();
        }


    }
}
