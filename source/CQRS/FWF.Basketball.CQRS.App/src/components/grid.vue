<script>
export default {
    name: 'grid',
    props: {
      teamId: {
        type: String
      },
    },

    data () {
      return {
        columns: [
            {
                field: 'name',
                label: 'Name',
            },
            {
                field: 'position',
                label: 'Position',
                width: '70',
            },
            {
                field: 'totalPoints',
                label: 'Points',
                width: '40',
                numeric: true,
                sortable: true,
            }
        ]
      }
    },

    computed: {
        players() {
            var allPlayers = this.$store.state.game.players;

            if (allPlayers) {
                var teamPlayers = allPlayers.filter(x => x.teamId === this.teamId);
                return teamPlayers;
            }

            return null;
        }
    },
  }
</script>

<template>

  <b-table 
    :data="players" 
    :columns="columns"
    :narrowed="true"
    :bordered="true"
    ></b-table>

</template>

<style scoped>
</style>
