using FWF.Security;

namespace FWF.CQRS
{
    public interface ICqrsLogicHandler : IRunnable
    {
        IQueryResponse Handle(ISecurityContext securityContext, IQuery query);

        ICommandResponse Handle(ISecurityContext securityContext, ICommand query);

        void Handle(ISecurityContext securityContext, IEvent @event);

    }
}


