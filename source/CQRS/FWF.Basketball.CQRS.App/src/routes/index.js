import Vue from 'vue'
import Router from 'vue-router'

import Index from '@/views/index'
import Game from '@/views/game'

export default new Router({
  routes: [
    { path: '/', name: 'index', component: Index },
    { path: '/game/:id', name: 'game', component: Game },
  ]
})
