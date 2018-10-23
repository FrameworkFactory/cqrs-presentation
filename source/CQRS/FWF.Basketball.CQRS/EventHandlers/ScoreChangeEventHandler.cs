using FWF.Basketball.CQRS.Data;
using FWF.Basketball.CQRS.Events;
using FWF.CQRS;
using FWF.Logging;
using FWF.Security;

namespace FWF.Basketball.CQRS.EventHandlers
{
    internal class ScoreChangeEventHandler : IEventHandler<ScoreChangeEvent>
    {

        private readonly IReadCacheDataRepository _readCacheDataRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILog _log;

        public ScoreChangeEventHandler(
            IReadCacheDataRepository readCacheDataRepository,
            IEventPublisher eventPublisher,
            ILogFactory logFactory
            )
        {
            _readCacheDataRepository = readCacheDataRepository;
            _eventPublisher = eventPublisher;

            _log = logFactory.CreateForType(this);
        }

        public void Handle(ISecurityContext securityContext, ScoreChangeEvent eventInstance)
        {
            // Whenever a score changes, this effects other calculations
            // Using the event-driven framework - ensure that each calculation(aggregate) is updated as well

            var playerDetail = _readCacheDataRepository.FirstOrDefault<PlayerDetail>(x => x.Id == eventInstance.PlayerId);

            if (playerDetail.IsNull())
            {
                playerDetail = new PlayerDetail
                {
                    Id = eventInstance.PlayerId.GetValueOrDefault(),
                    Name = eventInstance.PlayerName,
                    TeamId = eventInstance.TeamId.GetValueOrDefault(),
                    Position = eventInstance.PlayerPosition,
                    TotalPoints = 0,
                };
                using (var writeContext = _readCacheDataRepository.BeginWrite())
                {
                    writeContext.Insert(playerDetail);
                }
            }

            // For each score, increment the total points

            playerDetail.TotalPoints += eventInstance.Points.GetValueOrDefault();

            // Save in local repository 

            using (var writeContext = _readCacheDataRepository.BeginWrite())
            {
                writeContext.Update(playerDetail);
            }

            // Publish new event of player detail update

            _eventPublisher.Publish(
                new PlayerDetailChangeEvent
                {
                    Id = playerDetail.Id,
                    Name = playerDetail.Name,
                    TeamId = playerDetail.TeamId,
                    Position = playerDetail.Position,
                    TotalPoints = playerDetail.TotalPoints,
                }
                );

        }
    }
}
