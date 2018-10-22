<script>
import GamePreview from '@/components/gamePreview.vue'

export default {
    name: 'index',
    components: {
      GamePreview
    },
    data () {
        return {
      }
    },
    mounted: function() {
    },
    computed: {
        games() {
            return this.$store.state.game.games;
        }
    }
  }
</script>

<template>
    <div class="container">
        <section class="section clearfix">

        <h3 class="title is-3">Current Games</h3>

        <div class="columns">
            <div v-for="game in games" class="column is-one-third">
                <GamePreview :id="game.id" :name="game.name"/>
            </div>
        </div>

        <!--
        <div class="columns">
            <div class="column is-one-third">
                <GamePreview id="e6eab218-b52f-4d5b-a96f-c98836a9c401" name="Game #1"/>
            </div>
            <div class="column is-one-third">
                <GamePreview id="2" name="Game #2"/>
            </div>
            <div class="column is-one-third">
                <GamePreview id="3" name="Game #3"/>
            </div>
        </div>
        -->

        </section>
    </div>
</template>

<style scoped>
</style>
