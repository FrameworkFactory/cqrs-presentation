using FWF.Basketball.Logic.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FWF.Basketball.RESTFul.Service.v1.Controllers
{
    [Route("games")]
    public class GameController : ControllerBase
    {
        private readonly IRESTFulGameEngineListener _gameEngineListener;
        private readonly IGameDataRepository _gameDataRepository;

        public GameController(
            IRESTFulGameEngineListener gameEngineListener,
            IGameDataRepository gameDataRepository
            )
        {
            _gameEngineListener = gameEngineListener;
            _gameDataRepository = gameDataRepository;
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [ProducesResponseType(typeof(IEnumerable<GameDetail>), (int)HttpStatusCode.OK)]
        public async Task<IEnumerable<GameDetail>> GetAll()
        {
            var allGames = _gameDataRepository.GetAll<Game>()
                .OrderBy(x => x.Name);

            // Has to be a better way than to re-calculate the game score

            var allScores = _gameDataRepository.GetAll<Score>();

            var results = new List<GameDetail>();

            foreach (var item in allGames)
            {
                var awayScore = allScores
                    .Where(x => x.TeamId == item.AwayTeamId)
                    .Sum(x => x.Points);

                var homeScore = allScores
                    .Where(x => x.TeamId == item.HomeTeamId)
                    .Sum(x => x.Points);

                var gameClock = _gameEngineListener.GetGameClockById(item.Id);

                if (gameClock.IsNull())
                {
                    gameClock = new GameClock();
                }

                var gameDetail = new GameDetail
                {
                    Id = item.Id,
                    Name = item.Name,
                    Quarter = gameClock.Quarter,
                    GameClock = gameClock.Time.ToCustomString("mm:ss.f"), // Render as a game clock,
                    AwayTeamId = item.AwayTeamId,
                    AwayScore = awayScore,
                    HomeTeamId = item.HomeTeamId,
                    HomeScore = homeScore
                };

                results.Add(gameDetail);
            }

            return await Task.FromResult(results);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(typeof(GameDetail), (int)HttpStatusCode.OK)]
        [ProducesResponseType(404)]
        public async Task<GameDetail> Get(Guid id)
        {
            return await Task.FromResult(
                    _gameDataRepository.FirstOrDefault<GameDetail>(x => x.Id == id)
                        );

        }

    }
}
