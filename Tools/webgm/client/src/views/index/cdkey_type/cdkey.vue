<template>
    <div class="container">
      <el-button type="primary" @click="OnClickGenCDKey()">生成兑换码</el-button>
      <el-table :data="tableData" border class="table" style="width: 100%">
          <el-table-column
              align='center'
              prop="CodeStr"
              label="兑换码" />
          <el-table-column
              align='center'
              prop="IsUseStr"
              label="是否使用"
              width="100" />
          <el-table-column
              align='center'
              prop="UseIds"
              label="使用角色"
              width="200" />
          <el-table-column
              align='center'
              fixed="right"
              label="操作"
              width="140">
              <!--
              <template #default="scope">
              <el-button link type="primary" size="small" @click="handleWatch(scope.$index)">复制</el-button>
              <el-button link type="primary" size="small" @click="handleWatch(scope.$index)">删除</el-button>
              </template>
              -->
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

    <!--生成兑换码-->
    <el-dialog v-model="genCDKey.dialogFormVisible" title="生成兑换码">
    <el-form :model="genCDKey.form">
      <el-form-item label="生成数量" label-width="140px">
        <el-input-number v-model="genCDKey.form.count" :min="1" :max="10000" controls-position="right" />
      </el-form-item>

    </el-form>
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="genCDKey.dialogFormVisible = false">取消</el-button>
        <el-button type="primary" @click="OnClickSendAddCDKey()">
          生成并下载
        </el-button>
      </span>
    </template>
  </el-dialog>

  </template>


<script>
import { ElMessage,ElLoading } from 'element-plus';


  export default{
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
                search:'', // 搜索内容
              },
              lastQuery:{ // 上次查询的信息
                search:'',
              },
              genCDKey:{
                dialogFormVisible: false,
                form:{
                    count: 0
                }
              }

          };

      },
      methods:{
        OnClickGenCDKey(){
            this.genCDKey.form.count = 1;
            this.genCDKey.dialogFormVisible = true;
        },

        async OnClickSendAddCDKey(){
            var res = await this.$axios.post('/api/game/cdkey/add_code',{
              zoneId:this.$route.params.zone_id,
              rewardType:this.$route.params.reward_type,
              count:this.genCDKey.form.count,
            });
            if(res.data.success == false)
            {
                ElMessage.error(res.data.msg);
                return;
            }
            ElMessage.success("生成成功");
            let firstId = res.data.data.firstId;
            let lastId = res.data.data.lastId;
            this.genCDKey.dialogFormVisible = false;
            res = await this.$axios.post('/api/game/cdkey/download_cdkey',{
              zoneId:this.$route.params.zone_id,
              firstId:firstId,
              lastId:lastId,
            });
            if(res.data.success == false)
            {
                ElMessage.error(res.data.msg);
                return;
            }
            ElMessage.success("正在下载...");
            this.download(res,`cdkey_${firstId}_${lastId}.txt`);
        },


        download (data,fileName) {
          if (!data) {
            return
          }
          let url = window.URL.createObjectURL(new Blob([data.data]));
          let link = document.createElement('a');
          link.style.display = 'none';
          link.href = url;
          link.setAttribute('download', fileName);
          document.body.appendChild(link);
          link.click();
          document.body.removeChild(link);
        },
        handleSizeChange(page_size){
            this.paginations.page_size = page_size;
            this.handleCurrentChange(1);
        },
        handleCurrentChange(page_index){
            this.paginations.page_index = page_index;
            const searchData = {
                zoneId: this.$route.params.zone_id,
                skip: this.paginations.page_size * (this.paginations.page_index - 1),
                limit: this.paginations.page_size,
                rewardType: this.$route.params.reward_type,
                viewUsed:1,
            };
            console.log(JSON.stringify(searchData));
            this.$axios.post("/api/game/cdkey/get_code",searchData).then(res=>{
                if(res.data.success == false)
                {
                    ElMessage.error(res.data.msg);
                    return;
                }
                this.tableData = res.data.data;
                console.log(JSON.stringify(this.tableData ));
                this.tableData.forEach(res=>{
                  res.UseIdsList = JSON.parse(res.UseIds);
                  if(res.UseIdsList.length > 0)
                  {
                    res.IsUseStr = "已使用";
                  }else{
                    res.IsUseStr = "";
                  }

                });

                this.paginations.total = res.data.total;
            })
            .catch(err=>{
                ElMessage.error(err);
            });
        },
      },
      created()
      {
        this.handleCurrentChange(1);
      }
  }

  </script>