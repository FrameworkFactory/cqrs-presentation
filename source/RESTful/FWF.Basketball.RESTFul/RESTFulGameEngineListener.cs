using System;
using System.Collections.Generic;

namespace FWF.Basketball.RESTFul
{
    internal class RESTFulGameEngineListener : IRESTFulGameEngineListener
    {
        private IDictionary<Guid, GameClock> _gameClocks = new Dictionary<Guid, GameClock>();

        public void GameClockChange(Game game, GameClock gameClock)
        {
            _gameClocks[game.Id] = gameClock;
        }

        public void ScoreChange(Game game, GameClock gameClock, Score score)
        {

        }

        public GameClock GetGameClockById(Guid gameId)
        {
            if ( _gameClocks.ContainsKey(gameId))
            {
                return _gameClocks[gameId];
            }

            return null;
        }

    }
}
