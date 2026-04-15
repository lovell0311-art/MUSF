<template>
	<div class="container">
        <h2>服务器管理</h2>
        <el-divider />
        <el-table :data="table.serverData" border class="table" style="width: 100%">
            <el-table-column 
                align='center'
                prop="AppId" 
                label="ID" 
                width="50" />
            <el-table-column 
                align='center'
                prop="AppType" 
                label="服务器类型" />
            <el-table-column 
                align='center'
                prop="Status" 
                label="状态"
                width="120" >
                <template #default="scope">
                    <el-tag v-if="scope.row.Status == 1" class="ml-2" type="success">运行中</el-tag>
                    <el-tag v-if="scope.row.Status == 0" class="ml-2" type="danger">已停止</el-tag>
                    <el-tag v-if="scope.row.Status == -1" class="ml-2" type="warning">获取中</el-tag>
                </template>
            </el-table-column>
            <el-table-column 
                align='center'
                prop="OnlineCount" 
                label="在线人数" 
                width="120" />
            <el-table-column 
                align='center'
                prop="EnterGameCount" 
                label="游戏人数"
                width="120" />
            <el-table-column 
                align='center'
                fixed="right" 
                label="操作" 
                width="200">
                <template #default="scope">
                <el-button 
                link type="primary" size="small" @click="handleRefresh(scope.$index)">刷新</el-button>
                <el-button 
                link type="primary" size="small" @click="handleRunCodeEditor(scope.$index)">Repl</el-button>
                <el-button
                link type="primary" size="small" @click="handleDelete(scope.$index)" :disabled="true">热重载</el-button>
                <el-button
                link type="primary" size="small" @click="handleDelete(scope.$index)" :disabled="true">停服</el-button>
                </template>
            </el-table-column>
        </el-table>
        <el-dialog class="code-editor" v-model="codeEditor" :title="codeEditorTitle">
            <CodeEditor 
                v-model="csharpCodeValue" 
                width="984px"
                height="256px"

                border_radius="4px" 
                :language_selector="false" 
                :languages="[['csharp', 'C#']]">
            </CodeEditor>
            <br>
            <el-button type="primary" @click="OnClickRunCode()">运行代码</el-button>
            <br>
            <br>
            <CodeEditor 
                :value="consoleCodeValue" 
                width="984px"
                height="256px"
                :hide_header="true"
                border_radius="4px" 
                :language_selector="false" 
                :read_only="true"
                :languages="[['console', 'Console']]">
            </CodeEditor>
        </el-dialog>
        
    </div>
</template>

<script>

import { computed,ref,reactive } from 'vue';
import { ElMessage,ElLoading } from 'element-plus';
import CodeEditor from 'simple-code-editor';

export default{
    components:{
        CodeEditor
    },
    data(){
        return {
            table:{
                serverData:[
                    {
                        AppId:12,
                        AppType:"Game",
                        Status:-1,
                        OnlineCount:0,
                        EnterGameCount:0,
                    },
                    {
                        AppId:13,
                        AppType:"Game",
                        Status:-1,
                        OnlineCount:0,
                        EnterGameCount:0,
                    },
                    {
                        AppId:14,
                        AppType:"Game",
                        Status:-1,
                        OnlineCount:0,
                        EnterGameCount:0,
                    },
                    {
                        AppId:25,
                        AppType:"LoginCenter",
                        Status:-1,
                        OnlineCount:0,
                        EnterGameCount:0,
                    }

                ]
            },
            csharpCodeValue: '',
            consoleCodeValue: '',
            codeEditorTitle: '',
            targetAppId: 0,
            targetAppType: '',
            codeEditor: false,
        }
    },
    methods:{
        async handleRefresh(index){
            try{
                var res = await this.$axios.post("/api/game/server/game_status",{serverId:this.table.serverData[index].AppId});
                if(res.data.success == false)
                {
                    ElMessage.error(res.data.msg);
                    return;
                }
                this.table.serverData[index].Status = res.data.data.ServerStatus;
                this.table.serverData[index].OnlineCount = res.data.data.OnlineCount;
                this.table.serverData[index].EnterGameCount = res.data.data.EnterGameCount;
            }
            catch(e)
            {
                ElMessage.error(e);
            }
        },
        handleRunCodeEditor(index)
        {
            this.targetAppId = this.table.serverData[index].AppId;
            this.targetAppType = this.table.serverData[index].AppType;
            this.codeEditorTitle = `Repl (${this.targetAppType}:${this.targetAppId})`;
            this.codeEditor = true;
        },
        async OnClickRunCode()
        {
            try{
                this.consoleCodeValue = "请求中...";
                var res = await this.$axios.post("/api/game/server/run_code",{serverId:this.targetAppId,code:this.csharpCodeValue});
                if(res.data.success == false)
                {
                    ElMessage.error(res.data.msg);
                    this.consoleCodeValue = res.data.msg;
                    return;
                }
                this.consoleCodeValue = res.data.data.Return;
            }
            catch(e)
            {
                ElMessage.error(e);
                this.consoleCodeValue = e;
            }
        }

    },

    created(){
        this.table.serverData.forEach((item,index)=>{
            this.handleRefresh(index);
        })
    }
}

</script>

<style>
.code-editor{
    width: 1024px !important;
}
</style>