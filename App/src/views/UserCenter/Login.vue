<template>
    <div class="container no-bar">
        <div class="text-center logo">
            <img :src="logoUrl" alt="" class="img-circle logo-icon" />
        </div>
        <div class="input-group">
            <span class="input-group-addon">
                <i class="glyphicon glyphicon-home"></i>
            </span>
            <input v-model="userInfo.tenantName" type="url" class="form-control" placeholder="企业代码">
        </div>
        <br/>
        <div class="input-group">
            <span class="input-group-addon">
                <i class="glyphicon glyphicon-user"></i>
            </span>
            <input v-model="userInfo.userName" type="url" class="form-control" placeholder="账号">
        </div>
        <br/>
        <div class="input-group">
            <span class="input-group-addon">
                <i class="glyphicon glyphicon-lock"></i>
            </span>
            <input v-model="userInfo.password" type="password" class="form-control" placeholder="密码">
        </div>
        <br/>    
        <div>
            <button class="btn btn-info btn-block" v-saving="saving" @click="login">登录</button>
        </div>
    </div>
</template>

<script>
    import logo from 'ASSET/img/logo.png'
    import crypto from 'crypto'

    export default {
        data() {
            return {
                logoUrl: logo,
                userInfo: {},
                saving: false
            }
        },
        created() {
            let getter = this.$store.getters
            this.userInfo = {
                tenantName: getter.tenantName,
                userName: getter.userName,
                password: ''
            }
        },
        computed: {},
        methods: {
            login() {
                this.saving = true

                this.$store.dispatch('getSecret', this.userInfo)
                    .then(secret => {
                        // 先对密码本身加密（数据库存储格式）
                        this.userInfo.password = crypto.createHash('md5')
                            .update(this.userInfo.password)
                            .digest('hex')

                        // 再按服务端颁发的随机串加密
                        this.userInfo.password = crypto.createHash('md5')
                            .update(`${this.userInfo.password}.${secret}`)
                            .digest('hex')

                        this.$store.dispatch('login', this.userInfo)
                            .then(() => this.$router.push('/'))
                            .catch(msg => {
                                this.userInfo.password = ''
                                alert(msg)
                                this.saving = false
                            })
                    })
                    .catch(msg => {
                        alert(msg)
                        this.saving = false
                    })

            }
        }
    }
</script>

<style lang="less" src='./Login.less' scoped/>