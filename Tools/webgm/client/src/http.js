import axios from 'axios';
import router from './router'
import { ElMessage } from 'element-plus';


// 请求拦截 设置统一header
axios.interceptors.request.use(config=>{
    if(localStorage.token)
    {
        config.headers.Authorization = localStorage.token;
    }
    return config;
},error=>{
    return Promise.reject(error);
});

// 响应拦截 401 token过期处理
axios.interceptors.response.use(response=>{
    return response;
},error=>{
    const {status} = error.response;
    if(status == 401)
    {
        ElMessage.error('token失效，请重新登录');
        // TODO 清除本地token记录
        localStorage.removeItem('token');
    }
    return Promise.reject(error);
})


export default axios