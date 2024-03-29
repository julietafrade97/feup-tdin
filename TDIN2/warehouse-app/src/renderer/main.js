import Vue from 'vue'
import axiosApi from 'axios'

import App from './App'
import router from './router'
import Vuetify from 'vuetify'

Vue.use(Vuetify, {
  theme: {
    primary: '#45818e',
    secondary: '#cc0000',
    accent: '#76a5af',
    error: '#e06666'
  }
})

if (!process.env.IS_WEB) Vue.use(require('vue-electron'))
Vue.http = Vue.prototype.$http = axios
Vue.config.productionTip = false

const axios = axiosApi.create({
  baseURL:'http://localhost:5001/api/warehouse'
});
window.axios = axios;

/* eslint-disable no-new */
new Vue({
  components: { App },
  router,
  template: '<App/>'
}).$mount('#app')
