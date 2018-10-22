

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
            state.games.push(game);
          }
          else {
            localGame.name = game.name;
            localGame.quarter = game.quarter;
            localGame.gameClock = game.gameClock;
            localGame.awayScore = game.awayScore;
            localGame.homeScore = game.homeScore;
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
/*
          var localGame = state.games.find(function (x) { return x.id === score.gameId });

          if (localGame) {

            // Determine if the score increments the home or away team
            if (score.teamId == localGame.awayTeamId) {
              localGame.awayScore = localGame.awayScore + score.points;
            }
            if (score.teamId == localGame.homeTeamId) {
              localGame.homeScore = localGame.homeScore + score.points;
            }
          }
*/
          state.scores.push(score);
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
  