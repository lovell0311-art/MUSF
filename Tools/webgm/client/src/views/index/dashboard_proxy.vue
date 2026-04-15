<template>
  <div class="container">

    <el-row :gutter="16">
      <el-col :span="24">
        <el-card shadow="always">
          <el-row>
            <el-col :span="6">
              <div class="statistic-card">
                <el-statistic title="今日注册" :value="data.todayRegCount" />
                <div class="statistic-footer">
                  <div class="footer-item">
                    <span>昨日注册</span>
                    <template v-if="data.todayRegCount >= data.yesterdayRegCount">
                      <span class="green">
                        {{ data.todayRegCount - data.yesterdayRegCount }}
                        <el-icon>
                          <CaretTop />
                        </el-icon>
                      </span>
                    </template>
                    <template v-else>
                      <span class="red">
                        {{ data.yesterdayRegCount - data.todayRegCount }}
                        <el-icon>
                          <CaretBottom />
                        </el-icon>
                      </span>
                    </template>
                  </div>
                </div>
              </div>
            </el-col>
            <el-col :span="6">
              <div class="statistic-card">
                <el-statistic :value="data.activeCount">
                  <template #title>
                    <div style="display: inline-flex; align-items: center">
                      活跃用户
                      <el-tooltip effect="dark" content="30天内，进入游戏游玩的玩家数" placement="top">
                        <el-icon style="margin-left: 4px" :size="12">
                          <Warning />
                        </el-icon>
                      </el-tooltip>
                    </div>
                  </template>
                </el-statistic>

              </div>
            </el-col>
            <el-col :span="6">
              <div class="statistic-card">
                <el-statistic title="今日充值 (元)" :value="data.todayRecMoney">
                </el-statistic>
                <div class="statistic-footer">
                  <div class="footer-item">
                    <span>昨日充值</span>
                    <template v-if="data.todayRecMoney >= data.yesterdayRecMoney">
                      <span class="green">
                        {{ data.todayRecMoney - data.yesterdayRecMoney }}
                        <el-icon>
                          <CaretTop />
                        </el-icon>
                      </span>
                    </template>
                    <template v-else>
                      <span class="red">
                        {{ data.yesterdayRecMoney - data.todayRecMoney }}
                        <el-icon>
                          <CaretBottom />
                        </el-icon>
                      </span>
                    </template>
                  </div>
                </div>
              </div>
            </el-col>
            <el-col :span="6">
              <div class="statistic-card">
                <el-statistic :value="data.monthRecMoney">
                  <template #title>
                    <div style="display: inline-flex; align-items: center">
                      本月充值 (元)
                      <el-tooltip effect="dark" :content="data.monthRecContectString" placement="top">
                        <el-icon style="margin-left: 4px" :size="12">
                          <Warning />
                        </el-icon>
                      </el-tooltip>
                    </div>
                  </template>
                </el-statistic>
              </div>
            </el-col>
            <el-col :span="6">
              <div class="statistic-card">
                <el-statistic :value="data.lastMonthRecMoney">
                  <template #title>
                    <div style="display: inline-flex; align-items: center">
                      上月充值 (元)
                      <el-tooltip effect="dark" :content="data.lastMonthRecContectString" placement="top">
                        <el-icon style="margin-left: 4px" :size="12">
                          <Warning />
                        </el-icon>
                      </el-tooltip>
                    </div>
                  </template>
                </el-statistic>
              </div>
            </el-col>
            <el-col :span="6">
              <div class="statistic-card">
                <el-statistic :value="data.totalMonthRecMoney">
                  <template #title>
                    <div style="display: inline-flex; align-items: center">
                      总充值 (元)
                    </div>
                  </template>
                </el-statistic>
              </div>
            </el-col>
          </el-row>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="16">
      <el-col :span="24">
        <el-card shadow="always" class="my-card">
          <h2>代理Id: {{ data.channelId }}</h2>
          <el-divider />
          <el-table :data="tableData" border class="table" style="width: 100%">
            <el-table-column align='center' prop="UserId" label="Id" />
            <el-table-column align='center' prop="XYAccountNumber" label="XYUID" />
            <el-table-column align='center' prop="Phone" label="手机号" />
            <el-table-column align='center' prop="RegisterTimeString" label="注册时间" />
            <el-table-column align='center' prop="LastLoginTimeString" label="最后登录时间" />

          </el-table>
          <!-- 分页 -->
          <el-row>
            <el-col :span="24">
              <div class="pagination">
                <el-pagination v-if='paginations.total > 0' v-model:current-page="paginations.page_index"
                  v-model:page-size="paginations.page_size" :page-sizes="paginations.page_sizes"
                  layout="total, sizes, prev, pager, next, jumper" :total="paginations.total"
                  @size-change="handleSizeChange" @current-change="handleCurrentChange" />
              </div>
            </el-col>
          </el-row>

        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script>
import { ref, reactive } from 'vue';
import gmConfig from '../../config/gm_config';
import { ElMessage, ElLoading } from 'element-plus';
import moment from 'moment';
import { usePermissStore } from '@/store/permiss'

export default {
  name: "dashboard",
  setup() {
    return {
      gmConfig,
    }
  },
  data() {
    return {
      tableData: [],
      paginations: {
        page_index: 1,  // 当前位于哪页
        total: 0,   // 总数
        page_size: 5,   // 1页显示多少条
        page_sizes: [5, 10, 15, 20],   // 每页显示多少条
        layout: "total, sizes, prev, pager, next, jumper"   // 翻页属性
      },
      data: {
        todayRegCount: 0,
        yesterdayRegCount: 0,
        activeCount: 0,
        todayRecMoney: 0, // 今日充值金额
        yesterdayRecMoney: 0, // 昨日充值金额
        sevenDayRecMoney: 0,  // 七天充值金额
        monthRecMoney: 0, // 本月充值金额
        monthRecStartTimeString: "",  // 本月充值开始时间
        monthRecEndTimeString: "",  // 本月充值结束时间
        monthRecContectString: "",  // 本月充值提示内容
        lastMonthRecMoney: 0, // 上月充值金额
        lastMonthRecContectString: "",  // 上月充值提示内容
        totalMonthRecMoney: 0, // 总充值金额
        channelId: "",
      }

    };

  },
  methods: {
    handleSizeChange(page_size) {
      this.paginations.page_size = page_size;
      this.handleCurrentChange(1);
    },
    async handleCurrentChange(page_index) {
      this.paginations.page_index = page_index;
      const postData = {
        skip: this.paginations.page_size * (this.paginations.page_index - 1),
        limit: this.paginations.page_size
      };

      let res = await this.$axios.post("/api/game/proxy/account_info", postData);
      if (res.data.success == false) {
        ElMessage.error(res.data.msg);
        return;
      }

      this.tableData = res.data.data;
      let date = new Date();
      this.tableData.forEach(item => {
        date.setTime(item.LastLoginTime);
        item.LastLoginTimeString = moment(date).format('yyyy年MM月DD日 HH时mm分ss秒');
        date.setTime(item.RegisterTime);
        item.RegisterTimeString = moment(date).format('yyyy年MM月DD日 HH时mm分ss秒');
      });
      this.paginations.total = res.data.total;
      console.log(this.tableData);
      console.log(this.paginations.total);
    },
    async InitRegisterCount() {
      // 获取当天的日期
      const today = new Date();
      // 获取当天的开始时间，即 00:00:00
      const startOfToday = new Date(today.getFullYear(), today.getMonth(), today.getDate());
      // 获取当天的结束时间，即 23:59:59
      const endOfToday = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 23, 59, 59, 999);

      let resToday = await this.$axios.post("/api/game/proxy/account_reg_count", {
        beginTime: startOfToday.getTime(),
        endTime: endOfToday.getTime()
      });
      if (resToday.data.success == false) {
        ElMessage.error(resToday.data.msg);
        return;
      }
      this.data.todayRegCount = resToday.data.count;

      const yesterday = new Date();
      yesterday.setDate(yesterday.getDate() - 1);
      // 获取前一天的开始时间，即 00:00:00
      const startOfYesterday = new Date(yesterday.getFullYear(), yesterday.getMonth(), yesterday.getDate());
      // 获取前一天的结束时间，即 23:59:59
      const endOfYesterday = new Date(yesterday.getFullYear(), yesterday.getMonth(), yesterday.getDate(), 23, 59, 59, 999);
      let resYesterday = await this.$axios.post("/api/game/proxy/account_reg_count", {
        beginTime: startOfYesterday.getTime(),
        endTime: endOfYesterday.getTime()
      });
      if (resYesterday.data.success == false) {
        ElMessage.error(resYesterday.data.msg);
        return;
      }
      this.data.yesterdayRegCount = resYesterday.data.count;
    },
    async InitActiveCount() {
      // 获取当天的日期
      const today = new Date();
      const SevenDayAgo = new Date();
      SevenDayAgo.setDate(today.getDate() - 30);
      // 获取七天前的开始时间，即 00:00:00
      const startOfSevenDayAgo = new Date(SevenDayAgo.getFullYear(), SevenDayAgo.getMonth(), SevenDayAgo.getDate());
      const endOfToday = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 23, 59, 59, 999);
      let res = await this.$axios.post("/api/game/proxy/account_login_count", {
        beginTime: startOfSevenDayAgo.getTime(),
        endTime: endOfToday.getTime()
      });
      if (res.data.success == false) {
        ElMessage.error(res.data.msg);
        return;
      }
      this.data.activeCount = res.data.count;
    },
    // 初始化 今日充值金额
    async InitTodayRecMoney() {
      // 获取当天的日期
      const today = new Date();
      // 获取当天的开始时间，即 00:00:00
      const startOfToday = new Date(today.getFullYear(), today.getMonth(), today.getDate());
      // 获取当天的结束时间，即 23:59:59
      const endOfToday = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 23, 59, 59, 999);

      let resToday = await this.$axios.post("/api/game/proxy/total_rec_amount", {
        beginTime: startOfToday.getTime(),
        endTime: endOfToday.getTime()
      });
      if (resToday.data.success == false) {
        ElMessage.error(resToday.data.msg);
        return;
      }
      this.data.todayRecMoney = resToday.data.total;

      const yesterday = new Date();
      yesterday.setDate(yesterday.getDate() - 1);
      // 获取前一天的开始时间，即 00:00:00
      const startOfYesterday = new Date(yesterday.getFullYear(), yesterday.getMonth(), yesterday.getDate());
      // 获取前一天的结束时间，即 23:59:59
      const endOfYesterday = new Date(yesterday.getFullYear(), yesterday.getMonth(), yesterday.getDate(), 23, 59, 59, 999);
      let resYesterday = await this.$axios.post("/api/game/proxy/total_rec_amount", {
        beginTime: startOfYesterday.getTime(),
        endTime: endOfYesterday.getTime()
      });
      if (resYesterday.data.success == false) {
        ElMessage.error(resYesterday.data.msg);
        return;
      }
      this.data.yesterdayRecMoney = resYesterday.data.total;
    },
    // 初始化 七日充值金额
    async InitSevenDayRecMoney() {
      // 获取当天的日期
      const today = new Date();
      const SevenDayAgo = new Date();
      SevenDayAgo.setDate(today.getDate() - 7);
      // 获取七天前的开始时间，即 00:00:00
      const startOfSevenDayAgo = new Date(SevenDayAgo.getFullYear(), SevenDayAgo.getMonth(), SevenDayAgo.getDate());
      const endOfToday = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 23, 59, 59, 999);
      let res = await this.$axios.post("/api/game/proxy/total_rec_amount", {
        beginTime: startOfSevenDayAgo.getTime(),
        endTime: endOfToday.getTime()
      });
      if (res.data.success == false) {
        ElMessage.error(res.data.msg);
        return;
      }
      this.data.sevenDayRecMoney = res.data.total;
    },
    // 初始化 本月充值金额
    async InitMonthRecMoney() {
      // 获取当前日期
      const today = new Date();
      // 获取本月的开始时间
      const startOfMonth = new Date(today.getFullYear(), today.getMonth(), 1);
      // 获取下个月的开始时间
      const nextMonth = new Date(today.getFullYear(), today.getMonth() + 1, 1);
      // 获取本月的结束时间
      const endOfMonth = new Date(nextMonth.getTime() - 1);

      let res = await this.$axios.post("/api/game/proxy/total_rec_amount", {
        beginTime: startOfMonth.getTime(),
        endTime: endOfMonth.getTime()
      });
      if (res.data.success == false) {
        ElMessage.error(res.data.msg);
        return;
      }
      this.data.monthRecMoney = res.data.total;
      this.data.monthRecStartTimeString = moment(startOfMonth).format('yyyy-MM-DD HH:mm:ss');
      this.data.monthRecEndTimeString = moment(endOfMonth).format('yyyy-MM-DD HH:mm:ss');
      this.data.monthRecContectString = `${this.data.monthRecStartTimeString} 至 ${this.data.monthRecEndTimeString} 的充值金额`;
    },
    // 初始化 本月充值金额
    async InitLastMonthRecMoney() {
      // 获取当前日期
      const today = new Date();

      // 获取上个月的开始时间
      const startOfLastMonth = new Date(today.getFullYear(), today.getMonth() - 1, 1);

      // 获取本月的开始时间
      const startOfThisMonth = new Date(today.getFullYear(), today.getMonth(), 1);

      // 获取上个月的结束时间
      const endOfLastMonth = new Date(startOfThisMonth.getTime() - 1);

      let res = await this.$axios.post("/api/game/proxy/total_rec_amount", {
        beginTime: startOfLastMonth.getTime(),
        endTime: endOfLastMonth.getTime()
      });
      if (res.data.success == false) {
        ElMessage.error(res.data.msg);
        return;
      }
      this.data.lastMonthRecMoney = res.data.total;
      let monthRecStartTimeString = moment(startOfLastMonth).format('yyyy-MM-DD HH:mm:ss');
      let monthRecEndTimeString = moment(endOfLastMonth).format('yyyy-MM-DD HH:mm:ss');
      this.data.lastMonthRecContectString = `${monthRecStartTimeString} 至 ${monthRecEndTimeString} 的充值金额`;
    },
    // 初始化 总充值金额
    async InitTotalMonthRecMoney() {
      // 获取当前日期
      const today = new Date();
      let res = await this.$axios.post("/api/game/proxy/total_rec_amount", {
        beginTime: 0,
        endTime: today.getTime()
      });
      if (res.data.success == false) {
        ElMessage.error(res.data.msg);
        return;
      }
      this.data.totalMonthRecMoney = res.data.total;
    },
    async Awake() {
      this.InitRegisterCount();
      this.InitActiveCount();
      this.InitTodayRecMoney();
      this.InitMonthRecMoney();
      this.InitLastMonthRecMoney();
      this.InitTotalMonthRecMoney();

      const permiss = usePermissStore();
      this.data.channelId = permiss.channelId;
    }
  },
  created() {
    this.handleCurrentChange(1);
    this.Awake();
  },
  beforeUpdate() {
    this.handleCurrentChange(1);
    this.Awake();
  }
};
</script>

<style scoped>
.el-col {
  text-align: center;
}

.el-card {
  margin-bottom: 12px;
}

.el-statistic {
  --el-statistic-content-font-size: 28px;
}

.statistic-card {
  height: 100%;
  padding: 20px;
  border-radius: 4px;
  background-color: var(--el-bg-color-overlay);
}

.statistic-footer {
  align-items: center;
  font-size: 12px;
  color: var(--el-text-color-regular);
  margin-top: 16px;
}

.statistic-footer .footer-item span:last-child {
  display: inline-flex;
  align-items: center;
  margin-left: 4px;
}

.green {
  color: var(--el-color-success);
}

.red {
  color: var(--el-color-error);
}
</style>