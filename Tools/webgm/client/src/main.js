import { createApp } from 'vue'
import { createPinia } from 'pinia';
import piniaPersist from 'pinia-plugin-persist-uni'
import App from './App.vue'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import router from './router'
import { usePermissStore } from './store/permiss';
import axios from './http'
import * as ElementPlusIconsVue from '@element-plus/icons-vue';
import * as echarts from 'echarts';
//Vue.prototype.$echarts = echarts

const app = createApp(App);
const pinia = createPinia();
pinia.use(piniaPersist);
app.use(pinia);

// 注册elementplus图标
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
    app.component(key, component);
}

app.config.globalProperties.$axios = axios;
app.config.globalProperties.$echarts = echarts;

// 自定义权限指令
const permiss = usePermissStore();


app.directive('permiss',{
    mounted(el,binding){

        if(!permiss.Ok(String(binding.value)))
        {
            //el['hidden'] = true;
            el.parentNode.removeChild(el)
        }
    }
})
app.config.globalProperties.$permissOk = val =>{
    return permiss.Ok(String(val));
}

app.use(router);
app.use(ElementPlus);
app.mount('#app');
