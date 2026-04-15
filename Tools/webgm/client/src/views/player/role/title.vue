<template>
    <el-button type="primary" @click="dialogAddTitleVisible = true">添加称号</el-button>
    <el-table :data="tableData" border class="table" style="width: 100%">
        <el-table-column align='center' prop="TitleId" label="ID" width="120" />
        <el-table-column align='center' prop="Name" label="称号名" width="180" />
        <el-table-column align='center' prop="Type" label="类型" width="180" />
        <el-table-column align='center' prop="BeginTime" label="开始时间" />
        <el-table-column align='center' prop="EndTime" label="到期时间" />
        <el-table-column align='center' fixed="right" label="操作" width="150" v-permiss="EPermiss.edit_item">
            <template #default="scope">
                <el-button link type="primary" size="small" @click="handleDelTitle(scope.$index)">删除</el-button>
            </template>
        </el-table-column>
    </el-table>


    <!--对话框-->
    <el-dialog v-model="dialogAddTitleVisible" title="添加称号">
        <el-form :model="form.newTitle">
            <el-form-item label="称号" :label-width="form.newTitleLabelWidth">
                <el-select v-model="form.newTitle.titleId" class="m-2" placeholder="Select"
                    @change="OnSelectChangeRefreshItemOptions">
                    <el-option v-for="item in TitleInfo" :key="item.value" :label="item.label" :value="item.value" />
                </el-select>
            </el-form-item>
            <el-form-item label="类型" :label-width="form.newTitleLabelWidth">
                <el-select v-model="form.newTitle.titleType" class="m-2" placeholder="Select"
                    @change="OnSelectChangeRefreshItemOptions">
                    <el-option v-for="item in titleType" :key="item.value" :label="item.label" :value="item.value" />
                </el-select>
            </el-form-item>
            <el-form-item label="有效时间" :label-width="form.newTitleLabelWidth">
                <el-space :size="4">
                    <el-input-number v-model="form.newTitle.ValidTime.Day" size="small" style="width:64px" :min="0"
                        :max="3650" :controls="false" />
                    <el-text class="mx-1">天</el-text>
                    <el-input-number v-model="form.newTitle.ValidTime.Hour" size="small" style="width:64px" :min="0"
                        :max="24" :controls="false" />
                    <el-text class="mx-1">时</el-text>
                    <el-input-number v-model="form.newTitle.ValidTime.Minute" size="small" style="width:64px" :min="0"
                        :max="60" :controls="false" />
                    <el-text class="mx-1">分</el-text>
                </el-space>
            </el-form-item>
        </el-form>
        <template #footer>
            <span class="dialog-footer">
                <el-button @click="dialogAddTitleVisible = false">取消</el-button>
                <el-button type="primary" @click="OnClickAddTitle()">
                    添加
                </el-button>
            </span>
        </template>
    </el-dialog>

    <!-- 删除物品对话框 -->
    <el-dialog v-model="dialogDelTitleVisible" :title="'删除称号(' + deleteTitle.TitleId + ')'" width="30%">
        <span>是否要删除 {{ deleteTitle.Name }}</span>
        <template #footer>
            <span class="dialog-footer">
                <el-button @click="dialogVisible = false">取消</el-button>
                <el-button type="danger" @click="OnClickDelTitle(deleteTitle.Index)">
                    删除
                </el-button>
            </span>
        </template>
    </el-dialog>
    <!-- End 删除物品对话框 -->
</template>

<script>
import { EPermiss } from '../../../../../common/permiss';
import { ElMessage, ElLoading } from 'element-plus';
import title_config from '@/config/title_config';
import moment from 'moment';

export default {
    data() {
        return {
            EPermiss: EPermiss,
            tableData: [],
            titleType: [
                {
                    label: "绑定账号",
                    value: 0,
                },
                {
                    label: "绑定角色",
                    value: 1,
                }
            ],
            dialogAddTitleVisible: false,
            dialogDelTitleVisible: false,
            form: {
                newTitle: {
                    titleId: 0,
                    titleType: 1,
                    Name: '',
                    EndTime: '',
                    ValidTime:
                    {
                        Day: 7,
                        Hour: 0,
                        Minute: 0,
                    }
                },
                newTitleLabelWidth: 124,
            },
            TitleInfo: [],
            deleteTitle:{
                Id:0,
                TitleId:0,
                Name:'',
                Index:0,
            }
        }
    },
    mounted() {
        this.Init();
    },
    methods: {
        Init() {
            this.InitTitleInfo();
            this.InitTitle();
        },
        InitTitleInfo() {
            this.TitleInfo = [];
            for (const [key, value] of Object.entries(title_config)) {
                this.TitleInfo.push({ label: value.Name, value: key });
            }
            this.form.newTitle.titleId = this.TitleInfo[0].value;
        },
        async InitTitle() {
            let res = await this.$axios.post('/api/game/player/title',
                {
                    zoneId: this.$route.params.zone_id,
                    userId: this.$route.params.account_uid,
                    gameUserId: this.$route.params.role_uid,
                });
            if (res.data.success == false) {
                ElMessage.error(res.data.msg);
                return;
            }
            console.log(JSON.stringify(res.data.data));
            let formatTime = 'yyyy-MM-DD HH:mm:ss';
            this.tableData = [];
            res.data.data.forEach((item, index) => {
                let BeginTimeDate = new Date(Number(item.BeginTime) * 1000);
                let EndTimeDate = new Date(Number(item.EndTime) * 1000);
                this.tableData.push({
                    TitleId: item.TitleId,
                    Id: item.Id,
                    Type: item.Type == 0 ? '绑定账号' : '绑定角色',
                    Name: title_config[item.TitleId].Name,
                    BeginTime: moment(BeginTimeDate).format(formatTime),
                    EndTime: moment(EndTimeDate).format(formatTime),
                });
            });

        },
        async OnClickAddTitle() {
            let data = new Date();
            const validTime = this.form.newTitle.ValidTime;
            data.setTime(data.getTime() + validTime.Day * 1000 * 60 * 60 * 24 + validTime.Hour * 1000 * 60 * 60 + validTime.Minute * 1000 * 60);
            var request = {
                zoneId: this.$route.params.zone_id,
                userId: this.$route.params.account_uid,
                gameUserId: this.$route.params.role_uid,
                titleId: this.form.newTitle.titleId,
                type: this.form.newTitle.titleType,
                endTime: Math.floor(data.getTime() / 1000),
            };
            var res = await this.$axios.post('/api/game/player/add/title', request);
            if (res.data.success == false) {
                ElMessage.error(res.data.msg);
                return;
            }
            ElMessage.success("添加成功");

            this.dialogAddTitleVisible = false;
        },
        handleDelTitle(index) {
            this.deleteTitle.Id = this.tableData[index].Id;
            this.deleteTitle.TitleId = this.tableData[index].TitleId;
            this.deleteTitle.Name = this.tableData[index].Name;
            this.deleteTitle.Index = index;

            this.dialogDelTitleVisible = true;
        },
        async OnClickDelTitle(index) {
            var request = {
                zoneId: this.$route.params.zone_id,
                userId: this.$route.params.account_uid,
                gameUserId: this.$route.params.role_uid,
                id: this.tableData[index].Id,
                titleId: this.tableData[index].TitleId,
            };
            var res = await this.$axios.post('/api/game/player/del/title', request);
            if (res.data.success == false) {
                ElMessage.error(res.data.msg);
                this.dialogDelTitleVisible = false;
                return;
            }
            this.tableData.splice(index, 1);
            this.dialogDelTitleVisible = false;
            ElMessage.success("删除成功");
        }
    }
}


</script>