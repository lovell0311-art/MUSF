<template>
    <el-table :data="tableData" border class="table" style="width: 100%">
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
</template>


<script>
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
        };
    },
    methods:{

    }
}
</script>