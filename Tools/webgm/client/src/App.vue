<template>
  <div id="app">
    <el-config-provider :locale="zhCn">
      <router-view/>
    </el-config-provider>
  </div>
</template>

<script setup>
import zhCn from 'element-plus/es/locale/lang/zh-cn';

</script>

<style>
@import './assets/css/main.css';
@import './assets/css/color-dark.css';
</style>
