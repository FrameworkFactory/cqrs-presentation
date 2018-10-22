import Vue from 'vue'
import Vuex from 'vuex'
import Router from 'vue-router'
import injector from 'vue-inject'
import buefy from 'buefy'
import VueBus from 'vue-bus'
import VueNativeSock from 'vue-native-websocket'

import App from './app'
import router from './routes'
import store from './storage'

Vue.config.productionTip = process.env.NODE_ENV == 'production'

require('./font_awesome')

Vue.use(injector)   // Dependency injection
Vue.use(Router)     // SPA routing 
Vue.use(Vuex)       // App state management 
Vue.use(buefy)      // Used for Bulma-based components
Vue.use(VueBus)     // Used for bus-pattern handling

// 
Vue.use(VueNativeSock, 'ws://localhost:24000/ws', {
  store: store,
  format: 'json',
  reconnection: true, // (Boolean) whether to reconnect automatically (false)
  reconnectionDelay: 3000, // (Number) how long to initially wait before attempting a new (1000)
})


new Vue({
  el: '#app',

  router,   // Provide the routes to VUE
  store,    // Provide the specific app stores to VUE

  render: h => h(App)
})
