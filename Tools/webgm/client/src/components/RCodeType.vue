<template>
      <el-dialog  v-model="dialogFormVisible.value" title="兑换码类型生成">
    <el-form :model="form">
        <el-form-item label="道具">
            <el-table :data="form.itemList" border class="table" style="width: 100%">
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
                    fixed="right" 
                    label="操作" 
                    width="150">
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
            <el-button type="primary" @click="OnAddItem"><el-icon><Plus /></el-icon>&nbsp新增</el-button>
        </el-form-item>

    </el-form>
    <template #footer>
      <span v-if="IsView.value == false" class="dialog-footer">
        <el-button @click="dialogFormVisible.value = false">取消</el-button>
        <el-button type="primary" @click="OnGenCodeRewardType()" :disabled="form.itemList.length == 0">
          创建
        </el-button>
      </span>
    </template>
  </el-dialog>
    <ItemEditor ref="ItemEditor" v-bind:drawer="drawer" :IsEdit="IsEdit" v-on:add-click="OnClickAddItem" v-on:modify-click="OnClickModifyItem" />
</template>

<script>
import { ref,reactive } from 'vue';
import ItemEditor from '@/components/ItemEditor';
import {ItemCreateAttr,MailItem} from '@/game/item';
import item_config from '@/config/item_config';
import { ElMessage,ElLoading } from 'element-plus';


export default{
    components:{
        ItemEditor
    },
    props:{
        dialogFormVisible: {value:false},
        IsView:{value:false},
        ZoneId:{value:1}
    },
    data(){
        return{
            drawer:{
                value:false
            },
            IsEdit:false,
            EditIndex: 0,
            form:{
                name:'',
                content:'',
                itemList:[],
            }
        }
    },
    methods:{
        async handleWatch(index){
            this.$refs.ItemEditor.Init();
            this.$refs.ItemEditor.form.Item.ConfigId = this.form.itemList[index].MailItem.ItemConfigID;
            this.$refs.ItemEditor.ItemCreateAttr2Form(this.form.itemList[index].MailItem.CreateAttr);
            this.IsEdit = true;
            this.drawer.value = true;
            this.EditIndex = index;
        },
        async handleDelete(index){
            this.form.itemList.splice(index,1);
        },
        OnAddItem(){
            if(this.form.itemList.length >= 6)
            {
                ElMessage.warning("单个邮件，最多只能添加 6 个附件");
                return;
            }
            this.$refs.ItemEditor.Init();
            this.IsEdit = false;
            this.drawer.value = true;
        },
        OnClickAddItem(){
            var itemCreateAttr = this.$refs.ItemEditor.Form2ItemCreateAttr();
            var mailItem = new MailItem();
            mailItem.CreateAttr = itemCreateAttr;
            mailItem.ItemConfigID = this.$refs.ItemEditor.form.Item.ConfigId;
            console.log(JSON.stringify(itemCreateAttr));
            var name = item_config[String(mailItem.ItemConfigID)]?item_config[String(mailItem.ItemConfigID)]["Name"]:"Unknown";
            if(mailItem.CreateAttr.Level > 0)
            {
                name = name + ` +${mailItem.CreateAttr.Level}`;
            }
            this.form.itemList.push({
                ConfigId: mailItem.ItemConfigID,
                Name:name,
                Quantity:mailItem.CreateAttr.Quantity,
                MailItem:mailItem,
            });
            this.drawer.value = false;
        },
        OnClickModifyItem()
        {
            var itemCreateAttr = this.$refs.ItemEditor.Form2ItemCreateAttr();
            var mailItem = new MailItem();
            mailItem.CreateAttr = itemCreateAttr;
            mailItem.ItemConfigID = this.$refs.ItemEditor.form.Item.ConfigId;
            console.log(JSON.stringify(itemCreateAttr));
            var name = item_config[String(mailItem.ItemConfigID)]?item_config[String(mailItem.ItemConfigID)]["Name"]:"Unknown";
            if(mailItem.CreateAttr.Level > 0)
            {
                name = name + ` +${mailItem.CreateAttr.Level}`;
            }
            this.form.itemList[this.EditIndex] = {
                ConfigId: mailItem.ItemConfigID,
                Name:name,
                Quantity:mailItem.CreateAttr.Quantity,
                MailItem:mailItem,
            };
            this.drawer.value = false;
        },
        async OnGenCodeRewardType()
        {
            if(this.form.itemList.length == 0)
            {
                ElMessage.warning("至少要添加一件道具");
                return;
            }

            var mailItems = [];
            this.form.itemList.forEach((item,index)=>{
                mailItems.push(item.MailItem);
            });
            var request = {
                zoneId:this.ZoneId.value,
                rewardList:mailItems,
            };
            console.log(JSON.stringify(request));
            var res = await this.$axios.post('/api/game/cdkey/add_type',request);
            console.log(JSON.stringify(res.data));
            if(res.data.success == false)
            {
                ElMessage.error(res.data.msg);
                return;
            }
            ElMessage.success("创建成功");


            this.dialogFormVisible.value = false;
        },
        Clear(){
            this.form.name='';
            this.form.content='';
            this.form.itemList = [];
        }
    }
}


</script>