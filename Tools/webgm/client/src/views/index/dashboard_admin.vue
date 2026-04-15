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
                <el-statistic :value="data.totalRegCount">
                  <template #title>
                    <div style="display: inline-flex; align-items: center">
                      注册人数
                      <el-tooltip effect="dark" content="通过实名认证的玩家数" placement="top">
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
                <el-statistic :value="data.sevenDayActiveCount">
                  <template #title>
                    <div style="display: inline-flex; align-items: center">
                      活跃用户
                      <el-tooltip effect="dark" content="七天内，进入游戏游玩的玩家数" placement="top">
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
                <el-statistic title="今日活跃用户" :value="data.todayActiveCount">
                </el-statistic>
              </div>
            </el-col>
          </el-row>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="16">
      <el-col :span="24">
        <el-card shadow="always">
          <el-row>
            <!-- 次日留存 -->
            <el-col :span="4">
              <div class="statistic-card">
                <el-statistic title="次日留存" :value="data.oneDayRetain.retainPct.toFixed(2) + '%'">
                </el-statistic>
                <div class="statistic-footer">
                  <div class="footer-item">
                    <span>{{ '登录:' + data.oneDayRetain.loginCount + ' ' + '注册:' + data.oneDayRetain.regCount }}</span>
                  </div>
                </div>
              </div>
            </el-col>
            <el-col :span="4">
              <div class="statistic-card">
                <el-statistic title="二日留存" :value="data.day2Retain.retainPct.toFixed(2) + '%'">
                </el-statistic>
                <div class="statistic-footer">
                  <div class="footer-item">
                    <span>{{ '登录:' + data.day2Retain.loginCount + ' ' + '注册:' + data.day2Retain.regCount }}</span>
                  </div>
                </div>
              </div>
            </el-col>
            <el-col :span="4">
              <div class="statistic-card">
                <el-statistic title="三日留存" :value="data.day3Retain.retainPct.toFixed(2) + '%'">
                </el-statistic>
                <div class="statistic-footer">
                  <div class="footer-item">
                    <span>{{ '登录:' + data.day3Retain.loginCount + ' ' + '注册:' + data.day3Retain.regCount }}</span>
                  </div>
                </div>
              </div>
            </el-col>
            <el-col :span="4">
              <div class="statistic-card">
                <el-statistic title="四日留存" :value="data.day4Retain.retainPct.toFixed(2) + '%'">
                </el-statistic>
                <div class="statistic-footer">
                  <div class="footer-item">
                    <span>{{ '登录:' + data.day4Retain.loginCount + ' ' + '注册:' + data.day4Retain.regCount }}</span>
                  </div>
                </div>
              </div>
            </el-col>
            <el-col :span="4">
              <div class="statistic-card">
                <el-statistic title="五日留存" :value="data.day5Retain.retainPct.toFixed(2) + '%'">
                </el-statistic>
                <div class="statistic-footer">
                  <div class="footer-item">
                    <span>{{ '登录:' + data.day5Retain.loginCount + ' ' + '注册:' + data.day5Retain.regCount }}</span>
                  </div>
                </div>
              </div>
            </el-col>
            <el-col :span="4">
              <div class="statistic-card">
                <el-statistic title="六日留存" :value="data.day6Retain.retainPct.toFixed(2) + '%'">
                </el-statistic>
                <div class="statistic-footer">
                  <div class="footer-item">
                    <span>{{ '登录:' + data.day6Retain.loginCount + ' ' + '注册:' + data.day6Retain.regCount }}</span>
                  </div>
                </div>
              </div>
            </el-col>

            <!-- 七日留存 -->
            <el-col :span="4">
              <div class="statistic-card">
                <el-statistic title="七日留存" :value="data.sevenDayRetain.retainPct.toFixed(2) + '%'">
                </el-statistic>
                <div class="statistic-footer">
                  <div class="footer-item">
                    <span>{{ '登录:' + data.sevenDayRetain.loginCount + ' ' + '注册:' + data.sevenDayRetain.regCount }}</span>
                  </div>
                </div>
              </div>
            </el-col>
            <!-- 十五日留存 -->
            <el-col :span="4">
              <div class="statistic-card">
                <el-statistic title="十五日留存" :value="data.fifteenDayRetain.retainPct.toFixed(2) + '%'">
                </el-statistic>
                <div class="statistic-footer">
                  <div class="footer-item">
                    <span>{{ '登录:' + data.fifteenDayRetain.loginCount + ' ' + '注册:' + data.fifteenDayRetain.regCount }}</span>
                  </div>
                </div>
              </div>
            </el-col>
            <!-- 三十日留存 -->
            <el-col :span="4">
              <div class="statistic-card">
                <el-statistic title="三十日留存" :value="data.thirtyDayRetain.retainPct.toFixed(2) + '%'">
                </el-statistic>
                <div class="statistic-footer">
                  <div class="footer-item">
                    <span>{{ '登录:' + data.thirtyDayRetain.loginCount + ' ' + '注册:' + data.thirtyDayRetain.regCount }}</span>
                  </div>
                </div>
              </div>
            </el-col>
          </el-row>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="16">
      <el-col :span="12">
        <el-card shadow="always" class="my-card2">
          <OnlineCount width="100%" height="512px" id="Test" />
        </el-card>
      </el-col>
      <el-col :span="12">
        <el-card shadow="always" class="my-card">
          <h2>{{ view.totalRecMoneyString }}</h2>
          <el-divider />
          <el-form :model="AccountData" label-width="64px">
            <el-form-item label="渠道id">
              <el-input v-model="dialog.data.channelId" placeholder="代理Id"></el-input>
            </el-form-item>
            <el-form-item label="时间段">
              <el-date-picker v-model="view.inquiryTime" type="daterange" range-separator="至" start-placeholder="开始时间"
                end-placeholder="结束时间" :size="size" />
            </el-form-item>
            <el-button @click="OnClickInquiryTotalRecMoney()">查询</el-button>
          </el-form>
        </el-card>
      </el-col>
    </el-row>
  </div>

  <el-dialog v-model="dialog.totalRecMoneyResult">
    <el-descriptions title="代理累计充值查询结果" :column="1" border>
      <el-descriptions-item label="代理Id" label-align="right" align="center">{{ dialog.data.channelId
      }}</el-descriptions-item>
      <el-descriptions-item label="开始时间" label-align="right" align="center">{{ dialog.data.startTimeString
      }}</el-descriptions-item>
      <el-descriptions-item label="结束时间" label-align="right" align="center">{{ dialog.data.endTimeString
      }}</el-descriptions-item>
      <el-descriptions-item label="充值金额(元)" label-align="right" align="center">{{ dialog.data.recMoney
      }}</el-descriptions-item>
    </el-descriptions>
  </el-dialog>
</template>

<script>
import { ref, reactive } from 'vue';
import gmConfig from '../../config/gm_config';
import { ElMessage, ElLoading } from 'element-plus';
import moment from 'moment';
import OnlineCount from '@/components/OnlineCount';


export default {
  name: "dashboard",
  components: {
    OnlineCount,
  },
  setup() {
    return {
      gmConfig,
    }
  },
  data() {
    return {
      data: {
        todayRegCount: 0,
        yesterdayRegCount: 0,
        onlineCount: "-",
        totalRegCount: 0,
        todayActiveCount: 0,
        sevenDayActiveCount: 0,

        oneDayRetain:
        {
          regCount: 0,
          loginCount: 0,
          retainPct: 0.0,
        },
        day2Retain:
        {
          regCount: 0,
          loginCount: 0,
          retainPct: 0.0,
        },
        day3Retain:
        {
          regCount: 0,
          loginCount: 0,
          retainPct: 0.0,
        },
        day4Retain:
        {
          regCount: 0,
          loginCount: 0,
          retainPct: 0.0,
        },
        day5Retain:
        {
          regCount: 0,
          loginCount: 0,
          retainPct: 0.0,
        },
        day6Retain:
        {
          regCount: 0,
          loginCount: 0,
          retainPct: 0.0,
        },
        sevenDayRetain:
        {
          regCount: 0,
          loginCount: 0,
          retainPct: 0.0,
        },
        fifteenDayRetain:
        {
          regCount: 0,
          loginCount: 0,
          retainPct: 0.0,
        },
        thirtyDayRetain:
        {
          regCount: 0,
          loginCount: 0,
          retainPct: 0.0,
        },
      },
      view: {
        totalRecMoneyString: "代理累计充值查询",
        inquiryTime: []
      },
      dialog: {
        totalRecMoneyResult: false,
        data: {
          channelId: "",
          startTimeString: "",
          endTimeString: "",
          recMoney: 0,
        }
      }

    };

  },
  created() {
    this.Awake();
  },
  beforeUpdate() {
    //this.Awake();
    console.log("beforeUpdate");
  },
  methods: {
    async OnClickInquiryTotalRecMoney() {
      const startTime = this.view.inquiryTime[0];
      const endTime = this.view.inquiryTime[1];
      const startOfToday = new Date(startTime.getFullYear(), startTime.getMonth(), startTime.getDate());
      const endOfToday = new Date(endTime.getFullYear(), endTime.getMonth(), endTime.getDate(), 23, 59, 59, 999);

      let res = await this.$axios.post("/api/game/proxy/find/total_rec_amount", {
        channelId: this.dialog.data.channelId,
        beginTime: startOfToday.getTime(),
        endTime: endOfToday.getTime()
      });
      if (res.data.success == false) {
        ElMessage.error(res.data.msg);
        return;
      }

      this.dialog.totalRecMoneyResult = true;
      this.dialog.data.startTimeString = moment(startOfToday).format('yyyy年MM月DD日 HH时mm分ss秒')
      this.dialog.data.endTimeString = moment(endOfToday).format('yyyy年MM月DD日 HH时mm分ss秒')
      this.dialog.data.recMoney = res.data.total;
    },
    async InitRegisterCount() {
      // 获取当天的日期
      const today = new Date();
      // 获取当天的开始时间，即 00:00:00
      const startOfToday = new Date(today.getFullYear(), today.getMonth(), today.getDate());
      // 获取当天的结束时间，即 23:59:59
      const endOfToday = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 23, 59, 59, 999);

      let resToday = await this.$axios.post("/api/game/proxy/all/account_reg_count", {
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
      let res = await this.$axios.post("/api/game/proxy/all/account_reg_count", {
        beginTime: startOfYesterday.getTime(),
        endTime: endOfYesterday.getTime()
      });
      if (res.data.success == false) {
        ElMessage.error(res.data.msg);
        return;
      }
      this.data.yesterdayRegCount = res.data.count;
    },
    async InitTotelRegisterCount() {
      let res = await this.$axios.post("/api/game/proxy/all/total_reg_count", {});
      if (res.data.success == false) {
        ElMessage.error(res.data.msg);
        return;
      }
      this.data.totalRegCount = res.data.count;
    },
    async InitSevenDayActiveCount() {
      // 获取当天的日期
      const today = new Date();
      const SevenDayAgo = new Date();
      SevenDayAgo.setDate(today.getDate() - 7);
      // 获取七天前的开始时间，即 00:00:00
      const startOfSevenDayAgo = new Date(SevenDayAgo.getFullYear(), SevenDayAgo.getMonth(), SevenDayAgo.getDate());
      const endOfToday = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 23, 59, 59, 999);
      let res = await this.$axios.post("/api/game/proxy/all/account_login_count", {
        beginTime: startOfSevenDayAgo.getTime(),
        endTime: endOfToday.getTime()
      });
      if (res.data.success == false) {
        ElMessage.error(res.data.msg);
        return;
      }
      this.data.sevenDayActiveCount = res.data.count;
    },
    async InitTodayActiveCount() {
      // 获取当天的日期
      const today = new Date();
      // 获取当天的开始时间，即 00:00:00
      const startOfToday = new Date(today.getFullYear(), today.getMonth(), today.getDate());
      // 获取当天的结束时间，即 23:59:59
      const endOfToday = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 23, 59, 59, 999);
      let res = await this.$axios.post("/api/game/proxy/all/account_login_count", {
        beginTime: startOfToday.getTime(),
        endTime: endOfToday.getTime()
      });
      if (res.data.success == false) {
        ElMessage.error(res.data.msg);
        return;
      }
      this.data.todayActiveCount = res.data.count;
    },
    async GetRetain(day)
    {
      const startday = new Date();
      //startday.setDate(startday.getDate() - 1);
      // 获取前一天的开始时间，即 00:00:00
      const startOfYesterday = startday;
      // 获取前一天的结束时间，即 23:59:59
      const endOfYesterday = new Date();
      endOfYesterday.setDate(startday.getDate() - 1);

      const startOfoneday = new Date();
      const endOfoneday = new Date();
      startOfoneday.setDate(startday.getDate() - day);
      endOfoneday.setDate(startday.getDate() - day - 1);

      let res = await this.$axios.post("/api/game/proxy/retain", {
        channelId: "",
        loginBeginTime: endOfYesterday.getTime(),
        loginEndTime: startOfYesterday.getTime(),
        regBeginTime: endOfoneday.getTime(),
        regEndTime: startOfoneday.getTime()
      });
      if (res.data.success == false) {
        ElMessage.error(res.data.msg);
        return;
      }
      if(res.data.regCount == 0)
      {
        return {
          regCount: res.data.regCount,
          loginCount: res.data.loginCount,
          retainPct: 0.0,
        };
      }else{
        return {
          regCount: res.data.regCount,
          loginCount: res.data.loginCount,
          retainPct: (res.data.loginCount / res.data.regCount) * 100,
        };
      }
    },
    async InitRetain()
    {
      this.data.oneDayRetain = await this.GetRetain(1);
      this.data.day2Retain = await this.GetRetain(2);
      this.data.day3Retain = await this.GetRetain(3);
      this.data.day4Retain = await this.GetRetain(4);
      this.data.day5Retain = await this.GetRetain(5);
      this.data.day6Retain = await this.GetRetain(6);
      this.data.sevenDayRetain = await this.GetRetain(7);
      this.data.fifteenDayRetain = await this.GetRetain(15);
      this.data.thirtyDayRetain = await this.GetRetain(30);

    },
    async Awake() {
      this.InitRegisterCount();
      this.InitTotelRegisterCount();
      this.InitSevenDayActiveCount();
      this.InitTodayActiveCount();
      this.InitRetain();
    }
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