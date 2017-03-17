<template>
    <div class="nav-header">
        <div class="left">
            <slot name='left'>
                <a class="btn-link glyphicon glyphicon-chevron-left" href='javascript:window.history.back()' v-if="showBack"></a>
            </slot>
        </div>
        <div class="center">
            {{title}}
        </div>
        <div class="right">
            <slot></slot>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['title', 'showBack'],
        components: {},
        created() {},
        data() {
            return {}
        },
        methods: {}
    }
</script>