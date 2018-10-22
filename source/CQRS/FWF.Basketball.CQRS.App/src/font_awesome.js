import Vue from 'vue'

import { library } from '@fortawesome/fontawesome-svg-core'
import { faHome, faBuilding, faUsers, faEdit, faTrashAlt, faUser, faSearch, faQuestionCircle, faSignOutAlt, faCoffee } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

// Specify font awesome icons used in the application
library.add(
  faHome, faBuilding, faUsers, faEdit, faTrashAlt,
  faUser, faQuestionCircle, faSignOutAlt, faSearch, faCoffee
  )
Vue.component('font-awesome-icon', FontAwesomeIcon)
