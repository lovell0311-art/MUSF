<template>
	<div class="container">
        <el-button type="primary" @click="addUserFormVisible = true">添加新用户</el-button>
        <el-table :data="tableData" border class="table" style="width: 100%">
            <el-table-column 
                align='center'
                prop="Id" 
                label="Id" />
            <el-table-column 
                align='center'
                prop="Name" 
                label="用户名/代理Id" />
            <el-table-column 
                align='center'
                prop="Identity" 
                label="权限" />
            <el-table-column 
                align='center'
                fixed="right" 
                label="操作" 
                width="240">
                <template #default="scope">
                <el-button type="danger" size="small" @click="OnClickDelete(scope)">删除</el-button>
                </template>
            </el-table-column>
        </el-table>
    </div>





    <el-dialog v-model="addUserFormVisible" title="添加用户">
        <el-form :model="newUser">
            <el-form-item label="权限" :label-width="newUserFormLabelWidth">
                <el-select v-model="newUser.identity" placeholder="请选择权限">
                <el-option label="管理员" value="admin" />
                <el-option label="GM" value="gm" />
                <el-option label="代理" value="proxy" />
                </el-select>
            </el-form-item>
            <el-form-item label="用户名/代理Id" :label-width="newUserFormLabelWidth">
                <el-input v-model="newUser.name" />
            </el-form-item>
            <el-form-item label="邮箱" :label-width="newUserFormLabelWidth">
                <el-input v-model="newUser.email" />
            </el-form-item>
            <el-form-item label="密码" :label-width="newUserFormLabelWidth">
                <el-input v-model="newUser.password" type="password" />
            </el-form-item>
        </el-form>
        <template #footer>
        <span class="dialog-footer">
            <el-button type="primary" @click="OnClickAddUser()">
            添加
            </el-button>
            <el-button @click="addUserFormVisible = false">取消</el-button>
        </span>
        </template>
    </el-dialog>
</template>


<script>
import { ref,reactive } from 'vue';
import gmConfig from '../../config/gm_config';
import { ElMessage,ElLoading } from 'element-plus';
import moment from 'moment';


export default{
    name:"user_manager",
    data(){
        return {
            addUserFormVisible:false,
            tableData:[],
            newUser:{
                name:"",
                email:"",
                password:"",
                identity:""
            },
            newUserFormLabelWidth: 124,
          
        }
    },
    methods:{
        Awake()
        {
            this.InitAllUsers();
        },
        async OnClickAddUser()
        {
            let res = await this.$axios.post("/api/users/add",{
                name:this.newUser.name,
                email:this.newUser.email,
                password:this.newUser.password,
                identity:this.newUser.identity,
            });
            if(res.data.success == false)
            {
                ElMessage.error(res.data.msg);
                return;
            }
            this.addUserFormVisible = false;
            ElMessage.success("添加成功");
            this.InitAllUsers();
        },
        async OnClickEdit(scope)
        {

        },
        async OnClickDelete(scope)
        {
            console.log(`scope.row.Id=${scope.row.Id}`);
            let res = await this.$axios.post("/api/users/remove",{
                userId:scope.row.Id,
            });
            if(res.data.success == false)
            {
                ElMessage.error(res.data.msg);
                return;
            }
            this.addUserFormVisible = false;
            ElMessage.success("删除成功");
            this.InitAllUsers();
        },
        async InitAllUsers()
        {
            let res = await this.$axios.post("/api/users/all_users",{});
            if(res.data.success == false)
            {
                ElMessage.error(res.data.msg);
                return;
            }

            this.tableData = res.data.data;
        }


    },
    created(){
        this.Awake();
    },
    beforeUpdate(){
        //this.Awake();
    }
}

</script>
