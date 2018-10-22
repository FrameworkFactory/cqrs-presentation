using FWF.Security;

namespace FWF.CQRS
{
    public interface IEventPublisherSubscription
    {
        void Forward(IEvent entityEvent);
    }
}

