import router from 'ROUTER/router'

export const httpHandle = (req, next) => {
    // 以form形式提交post时间，默认是payload形式
    //req.headers.set('Content-Type', 'application/x-www-form-urlencoded')

    // 身份验证
    req.headers.set('Authorization', sessionStorage.token || '')

    next(function(res) {
        switch (res.status) {
            case 200:
                let token = res.headers.get('Authorization') || ''
                if (token) {
                    sessionStorage.setItem('token', token)
                } else {
                    sessionStorage.removeItem('token')
                }
                break
            case 401:
                sessionStorage.removeItem('token')
                break
        }

        if (!res.ok) {
            let token = sessionStorage.token || ''
            if (!token) {
                alert('身份验证失败，请重新登陆')
                router.push('/Login')
            }
        }
    })
}