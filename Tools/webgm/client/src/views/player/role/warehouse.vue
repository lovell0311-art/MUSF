<template>
    <el-table :data="tableData" border class="table" style="width: 100%">
    <el-table-column 
        align='center'
        prop="ConfigId" 
        label="ID" 
        width="120" />
    <el-table-column 
        align='center'
        prop="Name" 
        label="物品名"/>
    <el-table-column 
        align='center'
        prop="Quantity" 
        label="数量" 
        width="120" />
    <el-table-column 
        align='center'
        prop="UID" 
        label="UID" />
    <el-table-column 
        align='center'
        fixed="right" 
        label="操作" 
        width="150"
        v-permiss="EPermiss.edit_item">
        <template #default="scope">
        <el-button 
        v-if='scope.row.UID != ""'
        link type="primary" size="small" @click="handleWatch(scope.$index)">编辑</el-button>
        <el-button 
        v-if='scope.row.UID != ""'
        link type="primary" size="small" @click="handleDelete(scope.$index)">删除</el-button>
        </template>
    </el-table-column>
    </el-table>
    <ItemEditor ref="ItemEditor" v-bind:drawer="drawer" :IsEdit="IsEdit" v-on:modify-click="OnClickModifyItem" />
    <!-- 删除物品对话框 -->
    <el-dialog
        v-model="dialogVisible"
        :title="'删除物品('+ deletedItem.UID + ')'"
        width="30%"
    >
        <span>是否要删除 {{ deletedItem.Name }}</span>
        <template #footer>
        <span class="dialog-footer">
            <el-button @click="dialogVisible = false">取消</el-button>
            <el-button type="danger" @click="OnClickDeleteItem(deletedItem.Index)">
            删除
            </el-button>
        </span>
        </template>
    </el-dialog>
    <!-- End 删除物品对话框 -->
</template>

<script>

import { ref,reactive } from 'vue';
import { ElMessage,ElLoading } from 'element-plus';
import {Item} from '@/game/item';
import ItemEditor from '@/components/ItemEditor';
import item_config from '@/config/item_config';
import { EPermiss } from '../../../../../common/permiss';

export default{
    components:{
        ItemEditor,
    },
    setup(){
        const IsEdit = ref(false);

        return {
            IsEdit,
        }
    },
    data(){
        return {
            EPermiss:EPermiss,
            dialogVisible: false,
            deletedItem:{
                Index: 0,
                Name: '',
                UID: '',
            },
            tableData:[],
            drawer: {value:false},
        }
    },
    methods:{
        async handleWatch(index){
            var res = await this.$axios.post('/api/game/item/get',{zoneId:this.$route.params.zone_id,itemUid:this.tableData[index].UID});
            if(res.data.success == false)
            {
                ElMessage.error(res.data.msg);
                return;
            }
            
            var item = new Item(res.data.data);
            console.log(JSON.stringify(item));
            
            console.log(typeof(this.$refs.ItemEditor));
            this.$refs.ItemEditor.Init();
            this.$refs.ItemEditor.Item = item;
            this.$refs.ItemEditor.Item2Form(item);
            this.IsEdit = true;
            this.drawer.value = true;
        },
        async handleDelete(index){
            this.deletedItem.Index = index;
            this.deletedItem.Name = this.tableData[index].Name;
            this.deletedItem.UID = this.tableData[index].UID;

            this.dialogVisible = true;
        },
        RefreshtableData()
        {
            // 刷新装备信息
            this.$axios.post('/api/game/player/warehouse',{zoneId:this.$route.params.zone_id,userId:this.$route.params.account_uid}).then(
                res=>{
                    this.tableData = [];
                    console.log(JSON.stringify(res.data.data));
                    if(res.data.success == false)
                    {
                        ElMessage.error(res.data.msg);
                    }else{
                        res.data.data.forEach((item,index)=>{

                            var name = item_config[String(item.ConfigId)]?item_config[String(item.ConfigId)]["Name"]:"Unknown";
                            if(item.Level > 0)
                            {
                                name = name + ` +${item.Level}`;
                            }
                            this.tableData.push({
                                ConfigId: String(item.ConfigId),
                                Name: name,
                                Quantity: item.Quantity,
                                UID: item.ItemUid,
                            });
                        });
                        console.log(JSON.stringify(this.tableData));

                    }
                }
            );
        },

        async OnClickModifyItem()
        {
            var item = this.$refs.ItemEditor.Form2Item();
            console.log(JSON.stringify(item.ToUpdateDBData()))
            var res = await this.$axios.post('/api/game/item/modify',{zoneId:this.$route.params.zone_id,itemUid:item.Uid,data:item.ToUpdateDBData()});
            if(res.data.success == false)
            {
                ElMessage.error(res.data.msg);
                return;
            }
            ElMessage.success('修改成功');
            this.drawer.value = false;
        },
        async OnClickDeleteItem(index)
        {
            var res = await this.$axios.post('/api/game/item/delete',{zoneId:this.$route.params.zone_id,itemUid:this.tableData[index].UID});
            if(res.data.success == false)
            {
                ElMessage.error(res.data.msg);
                return;
            }
            this.dialogVisible = false;
            this.tableData.splice(index,1);

        }
    },
    created(){
        this.RefreshtableData();
    },
    beforeUpdate(){
        //this.RefreshtableData();
    }
}


</script>

