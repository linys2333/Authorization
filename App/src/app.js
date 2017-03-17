/* 启动文件 */
import Vue from 'vue'
import './directives/'
import './filters/'
import store from './store/'
import router from './routes/router'
import Index from 'COMPONENT/Index'

new Vue({
    el: '#app',
    store,
    router,
    render: h => h(Index)
})

/**
 * 根据 https://github.com/vuejs/vue-devtools#NOTES
 * Vue 1.0.18 以上版本需要如下配置 devtools
 */
if (__DEV__) {
    console.info('[当前环境] 开发环境')
    Vue.config.devtools = true
}
if (__PROD__) {
    console.info('[当前环境] 生产环境')
    Vue.config.devtools = false
}


// === Webpack 处理 assets，取消注释即可进行测试 === //
/* 处理 less / sass */
// import 'ASSET/less/normalize.less'
// import 'ASSET/scss/normalize.scss'

/* 处理 img，小于 10KB 的转为 base64，否则使用 URL */
// import base64 from 'ASSET/img/smaller.png'
// import url from 'ASSET/img/larger.png'

// function appendImgToBody(content) {
//   const img = document.createElement('img')
//   img.src = content
//   document.body.appendChild(img)
// }

// appendImgToBody(base64)
// appendImgToBody(url)