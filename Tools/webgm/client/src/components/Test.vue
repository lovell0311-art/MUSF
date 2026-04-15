<template>
    <el-button type="primary" v-on:click="$emit('test-click')">{{title}}</el-button>
</template>


<script>

export default{
    methods:{
        OnClick(){
            console.log("test");
        }
    },
    props:{
        title: {type: String},
        value:{type:String},
    }
}


</script>