<template>
    <div>
        <navbar title='用户中心' show-back='true'>
        </navbar>

        <div class="container">
            <ul>
                <li id='userInfo' class="row">
                    <div>
                        <div class="user-icon">
                            <i class=" glyphicon glyphicon-user"></i>
                        </div>
                    </div>
                    <div class="valign-bottom">
                        <div>
                            <span>账号：</span><span>{{userName}}</span>
                        </div>
                        <div>
                            <span>企业代码：</span><span>{{tenantName}}</span>
                        </div>
                    </div>
                    <div>
                    </div>
                </li>
                <li class="row">
                    <div class="line">
                        <i class="glyphicon glyphicon-globe"></i>
                        <span>团队</span>
                        <router-link class="align-right" to='/'>
                            <i class="glyphicon glyphicon-chevron-right"></i>
                        </router-link>
                    </div>
                    <div class="hr"></div>
                    <div class="line">
                        <i class="glyphicon glyphicon-th"></i>
                        <span>服务</span>
                        <router-link class="align-right" to='/'>
                            <i class="glyphicon glyphicon-chevron-right"></i>
                        </router-link>
                    </div>
                    <div class="hr"></div>
                    <div class="line" @click="go">
                        <i class="glyphicon glyphicon-cog"></i>
                        <span>设置</span>
                        <router-link class="align-right" to='/'>
                            <i class="glyphicon glyphicon-chevron-right"></i>
                        </router-link>
                    </div>
                </li>
                <li class="row">
                    <div class="line" @click="logout">
                        <i class="glyphicon glyphicon-log-out"></i>
                        <span>退出</span>
                    </div>
                </li>
                
            </ul>
        </div>
    </div>
</template>

<script>
    import Navbar from 'COMPONENT/Navbar/'
    import {
        mapGetters
    } from 'vuex'

    export default {
        components: {
            Navbar
        },
        created() {},
        computed: {
            ...mapGetters([
                'userName',
                'tenantName'
            ])
        },
        methods: {
            go() {
                this.$store.dispatch('getSecret', {
                    userName: this.userName
                })
            },
            logout() {
                this.$store.dispatch('logout')
                    .then(() => this.$router.push('/Login'))
            }
        }
    }
</script>

<style scoped>
    #userInfo {
        display: flex;
        height: 80px;
        justify-content: flex-start;
        align-items: center;
        padding-left: 30px;
    }
    
    .user-icon {
        display: flex;
        font-size: 22px;
        color: chocolate;
        border: solid 1px #ddd;
        border-radius: 30px;
        height: 50px;
        width: 50px;
        justify-content: center;
        align-items: center;
        margin-right: 20px;
    }
    
    .align-right {
        float: right;
    }
    
    .valign-bottom {
        display: flex;
        flex-direction: column-reverse;
        align-items: flex-start;
    }
    
    .row {
        margin: 5px 0 10px;
        background: #fff;
        padding: 5px 20px;
    }
    
    .line {
        padding: 3px;
    }
    
    .line span {
        padding-left: 10px;
    }
    
    ul {
        list-style: none;
        padding: 0;
    }
    
    .hr {
        border-color: #ddd;
        margin: 3px;
    }
</style>