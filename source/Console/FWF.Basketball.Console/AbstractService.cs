using Autofac;
using FWF.Configuration;
using FWF.CQRS;
using FWF.Logging;

namespace FWF.Basketball.Console
{
    public abstract class AbstractService : Startable, IEventPublisherSubscription
    {

        private readonly IEventPublisher _eventPublisher;
        private readonly IAppSettings _appSettings;
        private readonly ILog _log;

        public AbstractService(
            IComponentContext componentContext,
            ILogFactory logFactory
            )
        {
            _eventPublisher = componentContext.Resolve<IEventPublisher>();
            _appSettings = componentContext.Resolve<IAppSettings>();

            // Subscribe myself to any event
            _eventPublisher.Attach(this);

            _log = logFactory.CreateForType(this);
        }

        protected override void OnStart()
        {
            // Start dependent components
            _eventPublisher.Start();
        }

        protected override void OnStop()
        {
            _eventPublisher.Stop();
        }
        
        public void Forward(IEvent entityEvent)
        {
            // We have received an event that we want to forward to all clients

        }

    }
}

