<template>
    <h3>{{ msg }}</h3>
</template>

<script>
export default {
    name: 'sample',
    data () {
      return {
        msg: 'Hello world'
      }
    }
  }
</script>

<style scoped>
h3 {
  font-weight: normal;
}
</style>
