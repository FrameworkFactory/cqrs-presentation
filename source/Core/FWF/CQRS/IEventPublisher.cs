using System.Collections.Generic;

namespace FWF.CQRS
{
    public interface IEventPublisher : IRunnable
    {

        void Publish(IEvent @event);
        void Publish(IEnumerable<IEvent> @event);

        void Attach(IEventPublisherSubscription eventSubscription);

        void Detach(IEventPublisherSubscription eventSubscription);

    }
}

