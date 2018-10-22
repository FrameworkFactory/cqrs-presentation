using FWF.Basketball.CQRS.Events;
using FWF.Basketball.Logic.Data;
using FWF.CQRS;
using FWF.Logging;
using FWF.Security;

namespace FWF.Basketball.CQRS.EventHandlers
{
    internal class ScoreChangeEventHandler2 : IEventHandler<ScoreChangeEvent>
    {

        private readonly IGameDataRepository _gameDataRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILog _log;

        public ScoreChangeEventHandler2(
            IGameDataRepository gameDataRepository,
            IEventPublisher eventPublisher,
            ILogFactory logFactory
            )
        {
            _gameDataRepository = gameDataRepository;
            _eventPublisher = eventPublisher;

            _log = logFactory.CreateForType(this);
        }

        public void Handle(ISecurityContext securityContext, ScoreChangeEvent eventInstance)
        {
            // Whenever a score changes, this effects other calculations
            // Using the event-driven framework - ensure that each calculation(aggregate) is updated as well

            var gameDetail = _gameDataRepository.FirstOrDefault<GameDetail>(x => x.Id == eventInstance.GameId);

            if (gameDetail.IsNull())
            {
                gameDetail = new GameDetail 
                {
                    Id = eventInstance.GameId.GetValueOrDefault(),
                    Name = eventInstance.GameName,
                    Quarter  = eventInstance.Quarter.GetValueOrDefault(),
                    GameClock = eventInstance.GameClock,
                    AwayTeamId = eventInstance.AwayTeamId.GetValueOrDefault(),
                    AwayScore = 0,
                    HomeTeamId = eventInstance.HomeTeamId.GetValueOrDefault(),
                    HomeScore = 0,
                };
                using (var writeContext = _gameDataRepository.BeginWrite())
                {
                    writeContext.Insert(gameDetail);
                }
            }

            // Ensure the time is updated
            gameDetail.GameClock = eventInstance.GameClock;

            // For each score, increment the total points to the correct away/home team

            var isHomeTeamScore = eventInstance.TeamId.Equals(gameDetail.HomeTeamId);

            if (isHomeTeamScore)
            {
                gameDetail.HomeScore += eventInstance.Points.GetValueOrDefault();
            }
            else
            {
                gameDetail.AwayScore += eventInstance.Points.GetValueOrDefault();
            }

            // Save in local repository 

            using (var writeContext = _gameDataRepository.BeginWrite())
            {
                writeContext.Update(gameDetail);
            }

            // Publish new event of player detail update

            _eventPublisher.Publish(
                new GameDetailChangeEvent
                {
                    Id = gameDetail.Id,
                    Name = gameDetail.Name,
                    Quarter = gameDetail.Quarter,
                    GameClock = gameDetail.GameClock,
                    AwayTeamId = gameDetail.AwayTeamId,
                    AwayScore = gameDetail.AwayScore,
                    HomeTeamId = gameDetail.HomeTeamId,
                    HomeScore = gameDetail.HomeScore,
                }
                );

        }
    }
}
