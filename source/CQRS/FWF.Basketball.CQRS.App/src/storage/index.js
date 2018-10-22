import Vue from 'vue'
import Vuex from 'vuex'

import gameStorage from './gameStorage'

// NOTE: Need to call this first before creating a store
Vue.use(Vuex)

export default new Vuex.Store({
  
  modules: {
    game : gameStorage,
  },

  state: {
    socket: {
      isConnected: false,
      message: '',
      reconnectError: false,
    }
  },

  mutations:{
    SOCKET_ONOPEN (state, event)  {
      Vue.prototype.$socket = event.currentTarget
      state.socket.isConnected = true
    },
    SOCKET_ONCLOSE (state, event)  {
      state.socket.isConnected = false
    },
    SOCKET_ONERROR (state, event)  {
      console.error(state, event)
    },
    SOCKET_RECONNECT(state, count) {
      console.info(state, count)
    },
    SOCKET_RECONNECT_ERROR(state) {
      state.socket.reconnectError = true;
    },
    // default handler called for all methods
    SOCKET_ONMESSAGE (state, message)  {
      state.socket.message = message
    },
  },
  
})

