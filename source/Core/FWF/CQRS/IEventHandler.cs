
using FWF.Security;

namespace FWF.CQRS
{
    public interface IEventHandler
    {
    }

    public interface IEventHandler<T> : IEventHandler where T : IEvent
    {
        void Handle(ISecurityContext securityContext, T eventInstance);
    }
}


