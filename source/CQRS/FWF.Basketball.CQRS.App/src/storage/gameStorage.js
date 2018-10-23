

const gameStorage = {
  namespaced: true,

  state: {
      message: "",
      games: [],
      players: [],
      scores: []
    },
  
    getters: {
      getGameById: state => (id) => {
        return state.games.find(x => x.id === id)
      },
      getPlayerById: state => (id) => {
        return state.players.find(x => x.id === id)
      }
    },
   
    mutations:{
      addOrUpdateGameDetail(state, game) {

        var localGame = state.games.find(function (x) { return x.id === game.id });

        if (!localGame) {
          game.awayScore = 0;
          game.homeScore = 0;
          state.games.push(game);
        }
        else {
          localGame.name = game.name;
          localGame.quarter = game.quarter;
          localGame.gameClock = game.gameClock;

          if (game.awayScore) {
              localGame.awayScore = game.awayScore;
          }

          if (game.homeScore) {
            localGame.homeScore = game.homeScore;
          }            
        }
      },

      addOrUpdatePlayerDetail(state, player) {

        var localPlayer = state.players.find(function (x) { return x.id === player.id });

        if (!localPlayer) {
          state.players.push(player);
        }
        else {
          localPlayer.totalPoints = player.totalPoints;
        }
      },

      addScore(state, score) {
        state.scores.push(score);
      },

      addOrUpdatePlayerFantasy(state, playerFantasy) {

        var localPlayer = state.players.find(function (x) { return x.id === playerFantasy.id });

        if (localPlayer) {
          localPlayer.fantasyPoints = playerFantasy.fantasyPoints;
        }
      }
    },

    actions: {
      gameDetailChangeEvent: function(context, message) {

        var game = {
          id: message.id,
          name: message.name,
          quarter: message.quarter,
          gameClock: message.gameClock,
          awayTeamId : message.awayTeamId,
          awayScore: message.awayScore,
          homeTeamId : message.homeTeamId,
          homeScore: message.homeScore
        };

        context.commit('addOrUpdateGameDetail', game);
      },

      gameClockChangeEvent: function(context, message) {

        var game = {
          id: message.gameId,
          name: message.gameName,
          quarter: message.quarter,
          gameClock: message.gameClock,
          awayTeamId : message.awayTeamId,
          homeTeamId : message.homeTeamId,
        };

        context.commit('addOrUpdateGameDetail', game);
      },

      playerDetailChangeEvent: function(context, message) {

        var player = {
            id: message.id,
            name: message.name,
            teamId: message.teamId,
            position: message.position,
            totalPoints: message.totalPoints 
        };
        context.commit('addOrUpdatePlayerDetail', player);
      },

      scoreChangeEvent: function(context, message) {

        var score = {
          gameId: message.gameId,
          teamId: message.teamId,
          playerId: message.playerId,
          points: message.points
        };
        context.commit('addScore', score);
      },

      playerFantasyChangeEvent: function(context, message) {

        var playerFantasy = {
          id: message.id,
          name: message.name,
          teamId: message.teamId,
          fantasyPoints: message.fantasyPoints
        };
        context.commit('addOrUpdatePlayerFantasy', playerFantasy);
      },

    }

    /*
    actions: {
      sendMessage: function(context, message) {
        Vue.prototype.$socket.send(message)
      }
    }
    */
}

export default gameStorage
