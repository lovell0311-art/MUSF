<template>
    <div class="login-wrap">
        <div class="ms-login">
            <div class="ms-title">后台管理系统</div>
            <el-form :model="loginUser" :rules="rules" ref="login" label-width="0px" class="ms-content">
                <el-form-item prop="name">
                    <el-input v-model="loginUser.name" placeholder="用户名">
                        <template #prepend>
                            <el-icon><User /></el-icon>
                        </template>
                    </el-input>
                </el-form-item>
                <el-form-item prop="password">
                    <el-input
                        type="password"
                        placeholder="密码"
                        v-model="loginUser.password"
                        @keyup.enter="submitForm('login')"
                        >
                        <template #prepend>
                            <el-icon><Lock /></el-icon>
                        </template>
                    </el-input>
                </el-form-item>
                <div class="login-btn">
                    <el-button type="primary" @click="submitForm('login')">登录</el-button>
                </div>

            </el-form>

        </div>
    </div>

</template>


<script>
import { getCurrentInstance } from 'vue';
import { Lock, User } from '@element-plus/icons-vue';
import { ElMessage } from 'element-plus';
import jwt_decode from "jwt-decode";
import {usePermissStore} from '@/store/permiss'

const context = getCurrentInstance()?.appContext.config.globalProperties;

export default {
    name: "login",
    components:{
        Lock,
        User
    },
    data()
    {
        return {
            loginUser:{
                name:"",
                password:""
            },
            rules:{
                name:[
                    {
                        required: true,
                        message: '请输入用户名',
                        trigger: 'blur'
                    }
                ],
                password:[{required: true,message: '请输入密码',trigger:'blur'}]
            }
        }
    },
    methods:{
        submitForm(formName){
            this.$refs[formName].validate(valid=>{
                if(valid)
                {
                    console.log(this.loginUser);
                    this.$axios.post("/api/users/login",this.loginUser)
                                .then(res =>{
                                    const {success,msg}=res.data;
                                    if(success)
                                    {
                                        ElMessage.success("登录成功");
                                        const {token} = res.data;
                                        localStorage.setItem("token",token);
                                        this.$router.push("/index");
                                    }else{
                                        ElMessage.error(msg);
                                    }
                                    
                                })
                                .catch(
                                    err=>{
                                        console.log(err);
                                    }
                                )
                }
            });
        }
    }


}



</script>



<style scoped>

.login-wrap{
    position:relative;
    width: 100%;
    height: 100%;
    background:cadetblue;
    background-size: 100%;
}

.ms-login{
    position:absolute;
    left:50%;
    top:50%;
    width:350px;
    margin:-190px 0 0 -175px;
    border-radius: 5px;
    background:rgba(255,255,255,0.3);
    overflow: hidden;
}

.ms-title{
    width:100%;
    line-height: 50px;
    text-align:center;
    font-size:20px;
    color:#fff;
    border-bottom: 1px solid #ddd;
}

.ms-content{
    padding: 30px 30px;
}

.login-btn{
    text-align:center;
}

.login-btn button{
    width:100%;
    height: 36px;
    margin-bottom:10px;
}

</style>