
using FWF.Basketball.CQRS.Data;
using FWF.Basketball.CQRS.Queries;
using FWF.CQRS;
using FWF.Logging;
using FWF.Security;
using System.Linq;

namespace FWF.Basketball.CQRS.QueryHandlers
{
    internal class GetPlayerDetailsQueryHandler : IQueryHandler<GetPlayerDetailsQuery>
    {

        private readonly IReadCacheDataRepository _readCacheDataRepository;
        private readonly ILog _log;

        public GetPlayerDetailsQueryHandler(
            IReadCacheDataRepository readCacheDataRepository,
            ILogFactory logFactory
            )
        {
            _readCacheDataRepository = readCacheDataRepository;

            _log = logFactory.CreateForType(this);
        }

        public IQueryResponse Handle(ISecurityContext securityContext, GetPlayerDetailsQuery query)
        {

            // TODO: Should check ISecurityContext if the user is allowed to run the query

            var results = _readCacheDataRepository
                .Get<PlayerDetail>(x => x.TeamId == query.TeamId)
                .ToList();
                
            return new GetPlayerDetailsQueryResponse
            {
                TotalItems = results.Count,
                Items = results
            };
        }

    }
}
