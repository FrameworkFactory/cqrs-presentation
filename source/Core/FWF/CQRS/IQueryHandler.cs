using FWF.Security;

namespace FWF.CQRS
{
    public interface IQueryHandler
    {
    }

    public interface IQueryHandler<T> : IQueryHandler where T : IQuery
    {
        IQueryResponse Handle(ISecurityContext securityContext, T query);
    }
}


