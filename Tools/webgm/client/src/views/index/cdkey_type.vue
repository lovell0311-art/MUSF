<template>
  <div class="container">
    <div class="handle-box">
        <el-button type="primary" @click="OnClickAddCodeRewardType()">添加兑换码组</el-button>
    </div>
    <el-table :data="tableData" border class="table" style="width: 100%">
        <el-table-column
            align='center'
            prop="Id"
            label="Id" />
        <el-table-column
            align='center'
            prop="CodeType"
            label="兑换码类型" />
        <el-table-column
            align='center'
            prop="RewardType"
            label="奖励类型" />
        <el-table-column
            align='center'
            label="道具数"
            width="60">
            <template #default="scope">
                <el-button link type="primary" size="small" click="#">{{ scope.row.RewardStrCount }}</el-button>
            </template>
        </el-table-column>
        <el-table-column
            align='center'
            prop="Remark"
            label="备注" />
        <el-table-column
            align='center'
            fixed="right"
            label="操作"
            width="140">
            <template #default="scope">
            <el-button link type="primary" size="small" @click="handleModifyRemark(scope.$index)">修改备注</el-button>
            <el-button link type="primary" size="small" @click="handleWatch(scope.$index)">查看兑换码</el-button>
            </template>
        </el-table-column>
        </el-table>
        <!-- 分页 -->
        <el-row>
            <el-col :span="24">
                <div class="pagination">
                    <el-pagination
                        v-if='paginations.total > 0'
                        v-model:current-page="paginations.page_index"
                        v-model:page-size="paginations.page_size"
                        :page-sizes="paginations.page_sizes"
                        layout="total, sizes, prev, pager, next, jumper"
                        :total="paginations.total"
                        @size-change="handleSizeChange"
                        @current-change="handleCurrentChange"
                    />
                </div>
            </el-col>
        </el-row>
  </div>

  <RCodeType
  ref="RCodeType"
  v-bind:dialogFormVisible="rcodeType.dialogFormVisible"
  v-bind:ZoneId="rcodeType.zoneId"
  v-bind:IsView="rcodeType.isVeiw"
   />

   <el-dialog v-model="modifyRemarkVisible" :title="modifyRemarkTitle">
    <el-form :model="modifyRemarkForm">
      <el-form-item label="备注" label-width="140px">
        <el-input v-model="modifyRemarkForm.Remark" />
      </el-form-item>
    </el-form>
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="modifyRemarkVisible = false">取消</el-button>
        <el-button type="primary" @click="ModifyRemark">
          修改
        </el-button>
      </span>
    </template>
  </el-dialog>

</template>


<script>
import { ElMessage,ElLoading } from 'element-plus';
import gmConfig from '../../config/gm_config';
import RCodeType from '@/components/RCodeType';


export default{
    components:{
        RCodeType
    },
    setup(){
        return {
            gmConfig,
        }
    },
    data(){
        return {
            tableData: [],
            paginations:{
                page_index: 1,  // 当前位于哪页
                total: 0,   // 总数
                page_size: 5,   // 1页显示多少条
                page_sizes: [5,10,15,20],   // 每页显示多少条
                layout: "total, sizes, prev, pager, next, jumper"   // 翻页属性
            },
            query:{
                zoneId:1, // 区服id
            },
            lastQuery:{ // 上次查询的信息
                zoneId:'1',
            },
            rcodeType:{
                dialogFormVisible:{
                    value:false
                },
                zoneId:{
                    value:1,
                },
                isVeiw: {
                    value:false
                }
            },
            selectedCodeType:{
              Id: "",
              index:0,
            },
            modifyRemarkVisible: false,
            modifyRemarkTitle:"",
            modifyRemarkForm:{
              Remark:"",
            }

        };

    },
    methods:{
        handleWatch(index){
            // 打开角色页面
            let routeUrl = this.$router.resolve({
                path: `/cdkey_type/z${this.lastQuery.zoneId}/${this.tableData[index].RewardType}`,
            });
            window.open(routeUrl.href, '_blank');
        },
        handleModifyRemark(index)
        {
          this.selectedCodeType.Id = this.tableData[index].Id;
          this.selectedCodeType.Index = index;
          this.modifyRemarkTitle = `修改备注 (${this.selectedCodeType.Id})`;

          this.modifyRemarkForm.Remark = "";

          this.modifyRemarkVisible = true;
        },
        async ModifyRemark()
        {
          var res = await this.$axios.post('/api/game/cdkey/set_remark',{
            id:this.selectedCodeType.Id,
            remark:this.modifyRemarkForm.Remark
          });
          if(res.data.success == false)
          {
              ElMessage.error(res.data.msg);
              return;
          }
          ElMessage.success("修改成功");
          this.modifyRemarkVisible = false;
          this.tableData[this.selectedCodeType.Index].Remark = this.modifyRemarkForm.Remark;
        },
        handleSizeChange(page_size){
            this.paginations.page_size = page_size;
            this.handleCurrentChange(1);
        },
        handleCurrentChange(page_index){
            this.paginations.page_index = page_index;
            const searchData = {
                zoneId: this.query.zoneId,
                skip: this.paginations.page_size * (this.paginations.page_index - 1),
                limit: this.paginations.page_size
            };
            this.lastQuery.zoneId = this.query.zoneId;
            this.$axios.post("/api/game/cdkey/get_type",searchData).then(res=>{
                if(res.data.success == false)
                {
                    ElMessage.error(res.data.msg);
                    return;
                }
                this.tableData = res.data.data;
                this.tableData.forEach(res=>{

                    res.RewardStrObj = JSON.parse(res.RewardStr);
                    res.RewardStrCount = res.RewardStrObj.length;
                });
                this.paginations.total = res.data.total;
                console.log(this.tableData);
                console.log(this.paginations.total);
            })
            .catch(err=>{
                ElMessage.error(err);
            });
        },
        OnClickAddCodeRewardType()
        {
            this.rcodeType.isVeiw.value = false;
            this.rcodeType.zoneId.value = this.query.zoneId;
            this.rcodeType.dialogFormVisible.value = true;
        },
        OnClickWatchCodeRewardType(index)
        {
            this.$refs.RCodeType.form.itemList = this.tableData[index].RewardStrObj;
            this.rcodeType.isVeiw.value = true;
            this.rcodeType.zoneId.value = this.query.zoneId;
            this.rcodeType.dialogFormVisible.value = true;
        }
    },
    created(){
        this.handleCurrentChange(1);
    }
}

</script>