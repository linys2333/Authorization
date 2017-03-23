import userService from 'SERVICE/userService'

const state = {
    tenantName: 'cloud',
    userName: 'admin'
}

const getters = {
    tenantName: state => state.tenantName,
    userName: state => state.userName
}

const actions = {
    getSecret({ commit }, data) {
        return userService.getSecret(data)
    },
    login({ commit }, data) {
        return userService.login(data)
            .then(() => commit('login', data))
    },
    logout({ commit }) {
        return userService.logout()
            .then(() => commit('logout'))
    }
}

const mutations = {
    login(state, data) {
        state.userName = data.userName
        state.tenantName = data.tenantName
    },
    logout(state) {
        state.userName = ''
        state.tenantName = ''
    }
}

export default {
    state,
    getters,
    actions,
    mutations
}