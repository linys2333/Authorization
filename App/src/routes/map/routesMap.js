// 不同模块应代码分离
export default [{
    path: '/Login',
    name: 'Login',
    meta: { noCheckToken: true },
    component(resolve) {
        require(['VIEW/UserCenter/Login'], resolve)
    }
}, {
    path: '/',
    name: 'Index',
    component(resolve) {
        require(['VIEW/UserCenter/'], resolve)
    }
}]