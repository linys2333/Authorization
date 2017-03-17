<template>
    <transition :name="transitionName">
        <router-view></router-view>
    </transition>
</template>
<script>
    export default {
        data() {
            return {
                transitionName: 'slide-left'
            }
        },
        created() {},
        methods: {},
        mounted() {
            console.log('Index.vue')
        },
        watch: {
            '$route' (to, from) {
                let toDepth = to.path.split('/').length
                let fromDepth = from.path.split('/').length

                if (to.path[to.path.length - 1] === '/') {
                    toDepth--
                }
                if (from.path[from.path.length - 1] === '/') {
                    fromDepth--
                }
                this.transitionName = toDepth < fromDepth ? 'slide-right' : 'slide-left'
                    // console.log('Route To: ' + to.path)
                    // console.log('Transition: ' + this.transitionName)
            }
        }
    }
</script>
<style src='ASSET/less/global.less' lang='less'></style>