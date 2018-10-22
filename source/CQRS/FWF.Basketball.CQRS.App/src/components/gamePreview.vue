<script>

  export default {
    name: 'game-preview',
    components: {
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
            return game.awayScore;
        },
        homeScore() {
            var game = this.getGameData();
            return game.homeScore;
        }
    },

  };
</script>

<template>
<div>
    <div class="card">
        <header class="card-header">
            <router-link :to="{ name: 'game', params: { id: id }}" class="card-header-title">{{name}}</router-link>
            <a href="#" class="card-header-icon" aria-label="more options">
            <span class="icon">
                <i class="fas fa-angle-down" aria-hidden="true"></i>
            </span>
            </a>
        </header>
        <div class="card-content">
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
</div>
</template>
