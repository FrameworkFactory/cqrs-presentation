using FWF.Basketball;
using FWF.Basketball.Logic.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace FWF.Basketball.RESTFul.Service.v1.Controllers
{
    [Route("scores")]
    public class ScoreController : ControllerBase
    {
        private readonly IGameDataRepository _gameDataRepository;

        public ScoreController(
            IGameDataRepository gameDataRepository
            )
        {
            _gameDataRepository = gameDataRepository;
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [ProducesResponseType(typeof(IEnumerable<Score>), (int)HttpStatusCode.OK)]
        public async Task<IEnumerable<Score>> GetAll()
        {
            return await Task.FromResult(
                    _gameDataRepository.GetAll<Score>()
                        );
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(typeof(Score), (int)HttpStatusCode.OK)]
        [ProducesResponseType(404)]
        public async Task<Score> Get(Guid id)
        {
            return await Task.FromResult(
                    _gameDataRepository.FirstOrDefault<Score>(x => x.Id == id)
                        );

        }


    }
}
