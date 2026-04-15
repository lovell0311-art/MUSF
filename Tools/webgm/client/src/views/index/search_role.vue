<template>
    <div>
		<div class="container">
            
            <div class="handle-box">
				<el-select v-model="query.zoneId" placeholder="区服" class="handle-select mr10">
					<el-option 
                        v-for="item in gmConfig.AllZone"
                        :key="item.Id"
                        :label="item.Name"
                        :value="item.Id">
                    </el-option>
				</el-select>
				<el-input v-model="query.name" placeholder="角色名 / 角色ID / 账号ID" class="handle-input mr10" @keyup.enter="handleSearch"></el-input>
				<el-button type="primary" :icon="Search" @click="handleSearch">
                    <el-icon><Search /></el-icon>&nbsp 搜索</el-button>
			</div>
            

            <el-table :data="tableData" border class="table" style="width: 100%">
            <el-table-column 
                align='center'
                prop="GameUserId" 
                label="Id"
                width="200" />
            <el-table-column 
                align='center'
                prop="NickName" 
                label="昵称" />
            <el-table-column 
                align='center'
                prop="ClassName" 
                label="职业" 
                width="120" />
            <el-table-column 
                align='center'
                prop="Level" 
                label="等级" 
                width="120" />
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
import {GetClassName} from '../../game/role';
import { ElMessage,ElLoading } from 'element-plus';

  export default{
    name:"search_player",
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
                name:'',    // 搜索的用户名
            },
            lastQuery:{ // 上次查询的信息
                zoneId:'',
                name:'',
            }

        };

    },
    methods:{
        handleWatch(index){
            // 打开角色页面
            let routeUrl = this.$router.resolve({
                path: `/player/z${this.lastQuery.zoneId}/a${this.tableData[index].UserId}/r${this.tableData[index].GameUserId}`,
            });
            window.open(routeUrl.href, '_blank');
            console.log(`click=${this.tableData[index].NickName}`);
        },
        handleSizeChange(page_size){
            this.paginations.page_size = page_size;
            this.handleCurrentChange(1);
        },
        handleCurrentChange(page_index){
            this.paginations.page_index = page_index;
            const searchData = {
                zoneId: this.query.zoneId,
                roleName: this.query.name,
                skip: this.paginations.page_size * (this.paginations.page_index - 1),
                limit: this.paginations.page_size
            };

            this.$axios.post("/api/game/player/search",searchData).then(res=>{
                if(res.data.success == false)
                {
                    ElMessage.error(res.data.msg);
                    return;
                }
                this.tableData = res.data.data;
                this.tableData.forEach(res=>{
                    res.ClassName = GetClassName(res.PlayerTypeId,res.OccupationLevel);
                });
                this.paginations.total = res.data.total;
                console.log(this.tableData);
                console.log(this.paginations.total);
            })
            .catch(err=>{
                ElMessage.error(err?.response?.data?.msg || err?.message || '搜索失败');
            });
        },
        handleSearch(){
            this.handleCurrentChange(1);
            this.lastQuery = this.query;
            console.log(`zoneId ${this.query.zoneId} name ${this.query.name}`);
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
