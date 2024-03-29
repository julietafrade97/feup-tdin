import Vue from 'vue'
import Router from 'vue-router'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'default',
      component: require('@/components/Login').default
    },
    {
      path: '/login',
      name: 'login',
      component: require('@/components/Login').default
    },
    {
      path: '/register',
      name: 'register',
      component: require('@/components/Register').default
    },
    {
      path: '/books',
      name: 'books',
      component: require('@/components/Books').default,
    },
    {
      path: '/orders',
      name: 'orders',
      component: require('@/components/Orders').default,
    },
    {
      path: '*',
      redirect: '/'
    }
  ]
})
