<template>
    <div>User {{ id }}</div>
    <div>User {{ $route.params.id }}</div>
</template>

<script>
export default{
    name: "test",
    props:{
        id:{type: String,required:true}
    },
}

</script>