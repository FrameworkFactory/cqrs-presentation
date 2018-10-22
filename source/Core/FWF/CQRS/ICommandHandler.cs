using FWF.Security;

namespace FWF.CQRS
{
    public interface ICommandHandler
    {
    }

    public interface ICommandHandler<T> : ICommandHandler where T : ICommand
    {
        ICommandResponse Handle(ISecurityContext securityContext, T command);
    }
}



