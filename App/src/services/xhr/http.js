import Vue from 'vue'
import { rootPath, errHandler } from './config'

const success = (res) => {
    let data = res.body
    if (data && data.success) {
        return data.data
    }
    return fail(res)
}

const fail = (res) => {
    errHandler(res)

    let data = res.body
    if (data && !data.success) {
        return Promise.reject(data.message)
    }
    return Promise.reject(data)
}

const xhr = ({ url, data = null, method = 'get' }) => {
    url = url.indexOf('http') === 0 ? url : rootPath + url

    switch (method) {
        case 'get':
            return Vue.http.get(url)
                .then(success, fail)
        case 'post':
            return Vue.http.post(url, data)
                .then(success, fail)
    }
}

export default xhr