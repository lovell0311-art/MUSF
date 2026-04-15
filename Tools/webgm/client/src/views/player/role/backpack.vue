<template>
    <div class="handle-box" v-permiss="EPermiss.edit_item">
        <el-button type="primary" @click="OnAddItem">新增</el-button>
    </div>

    <el-table :data="tableData" border class="table" style="width: 100%">
        <el-table-column align="center" prop="ConfigId" label="ID" width="120" />
        <el-table-column align="center" prop="Name" label="名称" />
        <el-table-column align="center" prop="Quantity" label="数量" width="120" />
        <el-table-column align="center" prop="UID" label="UID" />
        <el-table-column
            align="center"
            fixed="right"
            label="操作"
            width="150"
            v-permiss="EPermiss.edit_item"
        >
            <template #default="scope">
                <el-button
                    v-if="scope.row.UID !== ''"
                    link
                    type="primary"
                    size="small"
                    @click="handleWatch(scope.$index)"
                >
                    编辑
                </el-button>
                <el-button
                    v-if="scope.row.UID !== ''"
                    link
                    type="primary"
                    size="small"
                    @click="handleDelete(scope.$index)"
                >
                    删除
                </el-button>
            </template>
        </el-table-column>
    </el-table>

    <ItemEditor
        ref="ItemEditor"
        :drawer="drawer"
        :IsEdit="IsEdit"
        @add-click="OnClickAddItem"
        @modify-click="OnClickModifyItem"
    />

    <el-dialog
        v-model="dialogVisible"
        :title="'删除物品 (' + deletedItem.UID + ')'"
        width="30%"
    >
        <span>确定删除 {{ deletedItem.Name }} 吗？</span>
        <template #footer>
            <span class="dialog-footer">
                <el-button @click="dialogVisible = false">取消</el-button>
                <el-button type="danger" @click="OnClickDeleteItem(deletedItem.Index)">
                    删除
                </el-button>
            </span>
        </template>
    </el-dialog>
</template>

<script>
import { ref } from 'vue';
import { ElMessage } from 'element-plus';
import { Item } from '@/game/item';
import ItemEditor from '@/components/ItemEditor';
import item_config from '@/config/item_config';
import { EPermiss } from '../../../../../common/permiss';

export default {
    components: {
        ItemEditor,
    },
    setup() {
        const IsEdit = ref(false);
        return {
            IsEdit,
        };
    },
    data() {
        return {
            dialogVisible: false,
            deletedItem: {
                Index: 0,
                Name: '',
                UID: '',
            },
            tableData: [],
            drawer: { value: false },
            EPermiss,
        };
    },
    methods: {
        OnAddItem() {
            this.$refs.ItemEditor.Init();
            this.IsEdit = false;
            this.drawer.value = true;
        },
        async handleWatch(index) {
            const res = await this.$axios.post('/api/game/item/get', {
                zoneId: this.$route.params.zone_id,
                itemUid: this.tableData[index].UID,
            });
            if (res.data.success === false) {
                ElMessage.error(res.data.msg);
                return;
            }

            const item = new Item(res.data.data);
            this.$refs.ItemEditor.Init();
            this.$refs.ItemEditor.Item = item;
            this.$refs.ItemEditor.Item2Form(item);
            this.IsEdit = true;
            this.drawer.value = true;
        },
        handleDelete(index) {
            this.deletedItem.Index = index;
            this.deletedItem.Name = this.tableData[index].Name;
            this.deletedItem.UID = this.tableData[index].UID;
            this.dialogVisible = true;
        },
        async RefreshtableData() {
            const res = await this.$axios.post('/api/game/player/backpack', {
                zoneId: this.$route.params.zone_id,
                gameUserId: this.$route.params.role_uid,
            });

            this.tableData = [];
            if (res.data.success === false) {
                ElMessage.error(res.data.msg);
                return;
            }

            res.data.data.forEach(item => {
                let name = item_config[String(item.ConfigId)]
                    ? item_config[String(item.ConfigId)].Name
                    : 'Unknown';
                if (item.Level > 0) {
                    name = `${name} +${item.Level}`;
                }
                this.tableData.push({
                    ConfigId: String(item.ConfigId),
                    Name: name,
                    Quantity: item.Quantity,
                    UID: item.ItemUid,
                });
            });
        },
        async OnClickAddItem() {
            const createAttr = this.$refs.ItemEditor.Form2ItemCreateAttr();
            const configId = this.$refs.ItemEditor.form.Item.ConfigId;
            const res = await this.$axios.post('/api/game/item/backpack/add', {
                zoneId: this.$route.params.zone_id,
                userId: this.$route.params.account_uid,
                gameUserId: this.$route.params.role_uid,
                configId,
                createAttr,
            });

            if (res.data.success === false) {
                ElMessage.error(res.data.msg);
                return;
            }

            ElMessage.success('新增成功');
            this.drawer.value = false;
            await this.RefreshtableData();
        },
        async OnClickModifyItem() {
            const item = this.$refs.ItemEditor.Form2Item();
            const res = await this.$axios.post('/api/game/item/modify', {
                zoneId: this.$route.params.zone_id,
                itemUid: item.Uid,
                data: item.ToUpdateDBData(),
            });

            if (res.data.success === false) {
                ElMessage.error(res.data.msg);
                return;
            }

            ElMessage.success('修改成功');
            this.drawer.value = false;
            await this.RefreshtableData();
        },
        async OnClickDeleteItem(index) {
            const res = await this.$axios.post('/api/game/item/delete', {
                zoneId: this.$route.params.zone_id,
                itemUid: this.tableData[index].UID,
            });

            if (res.data.success === false) {
                ElMessage.error(res.data.msg);
                return;
            }

            this.tableData.splice(index, 1);
            this.dialogVisible = false;
            ElMessage.success('删除成功');
        },
    },
    created() {
        this.RefreshtableData();
    },
};
</script>

<style scoped>
.handle-box {
    margin-bottom: 12px;
}
</style>
