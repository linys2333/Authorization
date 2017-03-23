import VueRouter from 'vue-router'
import routesMap from './map/routesMap'

const router = new VueRouter({
    routes: routesMap,
    mode: 'history',
    scrollBehavior: function(to, from, savedPosition) {
        return savedPosition || { x: 0, y: 0 }
    }
})

// 权限拦截
router.beforeEach((to, from, next) => {
    let token = sessionStorage.token || ''
    let noCheckToken = to.matched.some(record => record.meta.noCheckToken)
    if (!noCheckToken && !token) {
        alert('需要登录后才能访问')
        next('/Login')
    } else {
        next()
    }
})

export default router