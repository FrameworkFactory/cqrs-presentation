<script>

export default {
  name: 'App',
  components: {
  },
  data () {
    return {
      title: 'CQRS Basketball Game'
    }
  },
  computed: {
  },
  methods: {
  }
}
</script>

<template>
  
  <div id="app">
    <div class="app-wrapper">

      <section class="hero is-primary">
        <div class="hero-body">
          <div class="container">
            <h1 class="title">
              {{title}}
            </h1>
          </div>
        </div>
      </section>
      
      <router-view/>

    </div>
  </div>

</template>

<style lang="scss">
@import './style/global.scss';
</style>
