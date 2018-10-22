<script>
import GameDisplay from '@/components/gameDisplay.vue'

export default {
    name: 'game',
    components: {
      GameDisplay
    },
    data () {
        return {
      }
    },
    mounted: function() {
    },
    methods: {
        getGameData() {
            var gameId = this.$route.params.id;
            return this.$store.getters['game/getGameById'](gameId);
        }
    },
    computed: {
        gameId() {
            return this.$route.params.id;
        },
        gameName() {
            var game = this.getGameData();
            return game ? game.name : "";
        },
    }
  }
</script>

<template>
    <div class="container">
        <section class="section clearfix">

            <nav class="breadcrumb">
                <ul>
                    <li><router-link to="/">Games</router-link></li>
                    <li class="is-active"><a href="#" aria-current="page">Game Detail</a></li>
                </ul>
            </nav>

            <GameDisplay :id="gameId" :name="gameName"/>

        </section>
    </div>
</template>

<style scoped>
</style>
