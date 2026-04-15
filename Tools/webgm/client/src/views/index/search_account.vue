<template>
    <div>
		<div class="container">
            
            <div class="handle-box">
				<el-select v-model="query.zoneId" placeholder="区服" class="handle-select mr10" v-permiss="1">
					<el-option 
                        v-for="item in gmConfig.AllZone"
                        :key="item.Id"
                        :label="item.Name"
                        :value="item.Id">
                    </el-option>
				</el-select>
				<el-input v-model="query.search" placeholder="账号ID / 手机号 / XYUID / 渠道ID" class="handle-input mr10" @keyup.enter="handleSearch"></el-input>
				<el-button type="primary" :icon="Search" @click="handleSearch">
                    <el-icon><Search /></el-icon>&nbsp 搜索</el-button>
			</div>
            

            <el-table :data="tableData" border class="table" style="width: 100%">
            <el-table-column 
                align='center'
                prop="UserId" 
                label="Id" />
            <el-table-column 
                align='center'
                prop="XYAccountNumber" 
                label="XYUID" />
            <el-table-column 
                align='center'
                prop="Phone" 
                label="手机号" />
            <el-table-column 
                align='center'
                prop="ChannelId" 
                label="渠道Id" />
            <el-table-column 
                align='center'
                prop="LastLoginTimeString" 
                label="上次登录时间" />
            <el-table-column 
                align='center'
                fixed="right" 
                label="操作" 
                width="70">
                <template #default="scope">
                <el-button link type="primary" size="small" @click="handleWatch(scope.$index)">查看</el-button>
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
    </div>
</template>
  
<script>
import { ref,reactive } from 'vue';
import gmConfig from '../../config/gm_config';
import { ElMessage,ElLoading } from 'element-plus';
import moment from 'moment';

  export default{
    name:"search_account",
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
                search:'',    // 搜索的用户名
            },
            lastQuery:{ // 上次查询的信息
                zoneId:'',
                search:'',
            }

        };

    },
    methods:{
        handleWatch(index){
            // 打开角色页面
            let routeUrl = this.$router.resolve({
                path: `/player/z${this.lastQuery.zoneId}/a${this.tableData[index].UserId}/account`,
            });
            window.open(routeUrl.href, '_blank');
        },
        handleSizeChange(page_size){
            this.paginations.page_size = page_size;
            this.handleCurrentChange(1);
        },
        async handleCurrentChange(page_index){
            this.paginations.page_index = page_index;
            const searchData = {
                search: this.query.search,
                skip: this.paginations.page_size * (this.paginations.page_index - 1),
                limit: this.paginations.page_size
            };

            try {
                let res = await this.$axios.post("/api/game/account/search",searchData);
            if(res.data.success == false)
            {
                ElMessage.error(res.data.msg);
                return;
            }

            this.tableData = res.data.data;
            let date = new Date();
            this.tableData.forEach(item=>{
                date.setTime(item.LastLoginTime);
                item.LastLoginTimeString = moment(date).format('yyyy年MM月DD日 HH时mm分ss秒');
            });
            this.paginations.total = res.data.total;
            console.log(this.tableData);
                console.log(this.paginations.total);
            } catch (err) {
                ElMessage.error(err?.response?.data?.msg || err?.message || '搜索失败');
            }
        },
        handleSearch(){
            this.handleCurrentChange(1);
            this.lastQuery = this.query;
            console.log(`zoneId ${this.query.zoneId} search ${this.query.search}`);
        }
    },
  };
</script>

<style scoped>
.handle-box {
	margin-bottom: 20px;
}

.handle-select {
	width: 120px;
}

.handle-input {
	width: 300px;
}
.table {
	width: 100%;
	font-size: 14px;
}
.mr10 {
	margin-right: 10px;
}

.pagination {
  text-align: right;
  margin-top: 10px;
}
</style>
