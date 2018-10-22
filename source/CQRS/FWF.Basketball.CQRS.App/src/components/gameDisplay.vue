<script>
  import Grid from '@/components/grid.vue'

  export default {
    name: 'game-display',
    components: {
      Grid
    },

    props: {
      id: {
        type: String
      },
      name: {
        type: String,
        default: ''
      }
    },

    methods: {
        getGameData() {
            return this.$store.getters['game/getGameById'](this.id);
        }
    },

    computed: {
        gameClock() {
            var game = this.getGameData();
            return game ? game.gameClock : "00:00";
        },
        gameQuarter() {
            var game = this.getGameData();
            return game ? game.quarter : "1";
        },
        
        awayScore() {
            var game = this.getGameData();
            return game ? game.awayScore : 0;
        },
        homeScore() {
            var game = this.getGameData();
            return game ? game.homeScore : 0;
        },

        homeTeamId() {
            var game = this.getGameData();
            return game ? game.homeTeamId : "";
        },
        awayTeamId() {
            var game = this.getGameData();
            return game ? game.awayTeamId : "";
        }
    },

  };
</script>

<template>

    <div>
        <h3 class="title is-3">{{name}}</h3>

        <div class="columns">
          <div class="column is-half">
            <div class="level">
                <div class="level-item has-text-centered">
                        <div>
                        <p class="heading">Away Team</p>
                        <p class="title">{{awayScore}}</p>
                        </div>
                    </div>
                    <div class="level-item has-text-centered">
                        <div>
                            <p class="heading">Quarter #{{gameQuarter}}</p>
                            <p class="title">{{gameClock}}</p>
                        </div>
                    </div>
                    <div class="level-item has-text-centered">
                        <div>
                            <p class="heading">Home Team</p>
                            <p class="title">{{homeScore}}</p>
                        </div>
                    </div>
            </div>
          </div>
        </div>

        <div class="columns">
            <div class="column is-one-quarter">
                <grid :teamId="awayTeamId">
                </grid>
            </div>
            <div class="column is-one-quarter">
                <grid :teamId="homeTeamId">
                </grid>
            </div>
        </div>
        
    </div>
  
</template>
