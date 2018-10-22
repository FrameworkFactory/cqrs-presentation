using FWF.Basketball.CQRS.Events;
using FWF.Basketball.Logic;
using FWF.CQRS;
using FWF.Logging;

namespace FWF.Basketball.CQRS
{
    internal class CQRSGamePlayerListener : IGamePlayListener
    {

        private readonly IEventPublisher _eventPublisher;
        private readonly ILog _log;

        public CQRSGamePlayerListener(
            IEventPublisher eventPublisher,
            ILogFactory logFactory
            )
        {
            _eventPublisher = eventPublisher;

            _log = logFactory.CreateForType(this);
        }

        /*
         * Events are a huge part of CQRS.  With any change from the game play engine, publish
         * these as events...
        */

        public void GameClockChange(Game game, GameClock gameClock)
        {
            _eventPublisher.Publish(
                new GameClockChangeEvent
                { 
                    GameId = game.Id,
                    GameName = game.Name,
                    AwayTeamId = game.AwayTeamId,
                    HomeTeamId = game.HomeTeamId,

                    Quarter = gameClock.Quarter,
                    GameClock = gameClock.Time.ToCustomString("mm:ss.f") // Render as a game clock
                }
                );

        }

        public void ScoreChange(Game game, GameClock gameClock, Score score)
        {
            _eventPublisher.Publish(
                new ScoreChangeEvent
                {
                    GameId = game.Id,
                    GameName = game.Name,
                    AwayTeamId = game.AwayTeamId,
                    HomeTeamId = game.HomeTeamId,
                    Quarter = gameClock.Quarter,
                    GameClock = gameClock.Time.ToCustomString("mm:ss.f"), // Render as a game clock
                    TeamId = score.TeamId,
                    PlayerId = score.PlayerId,
                    PlayerName = score.PlayerName,
                    PlayerPosition = score.PlayerPosition,
                    Points = score.Points
                }
                );
        }

    }
}
