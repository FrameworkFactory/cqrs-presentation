using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FWF.Basketball.RESTFul.App.Models;
using FWF.Configuration;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FWF.Basketball.RESTFul.App.Controllers
{
    public class HomeController : Controller
    {

        private readonly IAppSettings _appSettings;

        public HomeController(
            IAppSettings appSettings
            )
        {
            _appSettings = appSettings;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            var games = await GetGamesFromApi();

            var model = new GamesModel();

            if (games.IsNotNull())
            {
                model.Games.AddRange(games);
            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GameDetail(Guid id)
        {
            var game = await GetGameFromApi(id);
            var players = await GetPlayersFromApi(id);

            var model = new SingleGameModel();

            if (game.IsNotNull())
            {
                model.Game = game;

                if (players.IsNotNull())
                {
                    var awayPlayers = players.Where(x => x.TeamId == game.AwayTeamId);
                    model.AwayPlayers.AddRange(awayPlayers);

                    var homePlayers = players.Where(x => x.TeamId == game.HomeTeamId);
                    model.HomePlayers.AddRange(homePlayers);
                }
            }

            return View(model);
        }
        
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<IEnumerable<GameDetail>> GetGamesFromApi()
        {
            var apiUrl = _appSettings.Get("ApiUrl");

            var gamesUrl = apiUrl + "games";

            IEnumerable<GameDetail> games = null;

            using (var http = new HttpClient())
            {
                var response = await http.GetAsync(gamesUrl);
                var json = await response.Content.ReadAsStringAsync();

                games = JsonConvert.DeserializeObject<IEnumerable<GameDetail>>(json);
            }

            return games;
        }

        private async Task<GameDetail> GetGameFromApi(Guid gameId)
        {
            var apiUrl = _appSettings.Get("ApiUrl");

            var gamesUrl = apiUrl + "games/" + gameId.ToString("N");

            GameDetail game = null;

            using (var http = new HttpClient())
            {
                var response = await http.GetAsync(gamesUrl);
                var json = await response.Content.ReadAsStringAsync();

                game = JsonConvert.DeserializeObject<GameDetail>(json);
            }

            return game;
        }

        private async Task<IEnumerable<PlayerDetail>> GetPlayersFromApi(Guid gameId)
        {
            var apiUrl = _appSettings.Get("ApiUrl");

            var gamesUrl = apiUrl + "players?gameId=" + gameId.ToString("N");

            IEnumerable<PlayerDetail> players = null;

            using (var http = new HttpClient())
            {
                var response = await http.GetAsync(gamesUrl);
                var json = await response.Content.ReadAsStringAsync();

                players = JsonConvert.DeserializeObject<IEnumerable<PlayerDetail>>(json);
            }

            return players;
        }
    }
}
