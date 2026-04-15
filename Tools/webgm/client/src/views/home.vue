<template>
    <div class="home">
        <div class="container">
          <h1>{{ data.channelId }}</h1>
          <h3>{{ data.identity }}</h3>
        </div>
    </div>
</template>


<script>
import {usePermissStore} from '@/store/permiss'

export default{
  data(){
    return {
      data:{
        channelId:"",
        identity: "user",
      }
    }
  },
  created(){
    const permiss = usePermissStore();
    this.data.channelId = permiss.channelId;
    this.data.identity = permiss.identity;
  }
}
</script>

<style scoped>
.home {
  width: 100%;
  height: 100%;
  background-size: 100% 100%;
}
.container {
  width: 100%;
  height: 100%;
  box-sizing: border-box;
  padding-top: 100px;
  background-color: rgba(0, 0, 0, 0.7);
  text-align: center;
  color: white;
}
.title {
  font-size: 30px;
}
.lead {
  margin-top: 50px;
  font-size: 22px;
}
</style>