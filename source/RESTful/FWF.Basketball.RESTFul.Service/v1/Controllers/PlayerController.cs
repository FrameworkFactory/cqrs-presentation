using FWF.Basketball.Logic.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FWF.Basketball.RESTFul.Service.v1.Controllers
{
    [Route("players")]
    public class PlayerController : ControllerBase
    {
        private readonly IRESTFulGameEngineListener _gameEngineListener;
        private readonly IGameDataRepository _gameDataRepository;

        public PlayerController(
            IRESTFulGameEngineListener gameEngineListener,
            IGameDataRepository gameDataRepository
            )
        {
            _gameEngineListener = gameEngineListener;
            _gameDataRepository = gameDataRepository;
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [ProducesResponseType(typeof(IEnumerable<PlayerDetail>), (int)HttpStatusCode.OK)]
        public async Task<IEnumerable<PlayerDetail>> GetAll()
        {
            var results = new List<PlayerDetail>();

            // Has to be a better way than to re-calculate the game/player score
            var allPlayers = _gameDataRepository.GetAll<Player>();
            var allScores = _gameDataRepository.GetAll<Score>();

            foreach (var player in allPlayers)
            {
                var playerScore = allScores
                    .Where(x => x.PlayerId == player.Id)
                    .Sum(x => x.Points);

                var playerDetail = new PlayerDetail
                {
                    Id = player.Id,
                    Name = player.Name,
                    Position = player.Position,
                    TeamId = player.TeamId,

                    TotalPoints = playerScore
                };

                results.Add(playerDetail);
            }

            return await Task.FromResult(results);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(typeof(PlayerDetail), (int)HttpStatusCode.OK)]
        [ProducesResponseType(404)]
        public async Task<PlayerDetail> Get(Guid id)
        {
            return await Task.FromResult(
                    _gameDataRepository.FirstOrDefault<PlayerDetail>(x => x.Id == id)
                        );

        }

    }
}
