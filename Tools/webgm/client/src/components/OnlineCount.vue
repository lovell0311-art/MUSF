<template>
    <el-form-item label="时间段">
        <el-date-picker v-model="view.timeLimit" type="daterange" range-separator="至" start-placeholder="开始时间"
            end-placeholder="结束时间" :size="size" />
        <el-button @click="OnClickFind()">查询</el-button>
    </el-form-item>
    <div :style="{ height: height, width: width }" :id="id"></div>
</template>


<script>
import { ref, reactive } from 'vue';
import { ElMessage, ElLoading } from 'element-plus';

const echarts = require('echarts/lib/echarts');
require('echarts/lib/component/title');
require('echarts/lib/component/tooltip');
require('echarts/lib/component/grid');
require('echarts/lib/chart/line');

let myChart = null;

export default {
    name: "echarts",
    props: {
        height: {
            type: String,
            default: "500px",
        },
        width: {
            type: String,
            default: "500px",
        },
        id: {
            type: String,
            default: "demo",
        },
    },
    data() {
        return {
            view: {
                timeLimit: []
            },
            eChart: null,
        };
    },
    mounted() {
        this.InitAsync();
    },
    methods: {
        async InitAsync() {
            this.drawLine();
            this.InitTimeLimit();
            await this.FindOnlineCountAsync();
        },
        drawLine() {
            // 基于准备好的dom，初始化echarts实例
            myChart = echarts.init(document.getElementById(this.id));
            window.addEventListener('resize', function () {
                myChart.resize();
            });

            let data = [];

            let option = {
                title: {
                    text: '在线人数'
                },
                tooltip: {
                    trigger: 'axis',
                    formatter: function (params) {
                        const param = params[0];
                        var date = new Date(Number(param.name));
                        return (
                            date.toLocaleString()+
                            ' 在线:' +
                            param.value[1]
                        );
                    },
                    axisPointer: {
                        animation: false
                    }
                },
                xAxis: {
                    type: 'time',
                    splitLine: {
                        show: false
                    }
                },
                yAxis: {
                    type: 'value',
                    boundaryGap: [0, '100%'],
                    splitLine: {
                        show: false
                    }
                },
                series: [
                    {
                        name: 'Fake Data',
                        type: 'line',
                        showSymbol: false,
                        data: data,
                        smooth: true
                    }
                ]
            };

            // 绘制图表
            myChart.setOption(option);
        },
        InitTimeLimit() {
            // 获取当天的日期
            const today = new Date();
            // 获取当天的开始时间，即 00:00:00
            const startOfToday = new Date(today.getFullYear(), today.getMonth(), today.getDate());
            // 获取当天的结束时间，即 23:59:59
            const endOfToday = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 23, 59, 59, 999);

            this.view.timeLimit = [startOfToday, endOfToday];
        },
        async FindOnlineCountAsync() {
            const startTime = this.view.timeLimit[0];
            const endTime2 = this.view.timeLimit[1];
            const endTime = new Date(endTime2.getFullYear(), endTime2.getMonth(), endTime2.getDate(), 23, 59, 59, 999)

            let res = await this.$axios.post("/api/game/proxy/find/online_count", {
                beginTime: startTime.getTime(),
                endTime: endTime.getTime()
            });

            if (res.data.success == false) {
                ElMessage.error(res.data.msg);
                return;
            }


            let data = [];
            function addData(time, count) {
                data.push({
                    name: time,
                    value: [
                        time,
                        count
                    ]
                });
            };
            for (let i = 0; i < res.data.data.length; i++) {
                const element = res.data.data[i];
                addData(element.CreateTime, element.Count);
            }

            myChart.setOption({
                series: [
                    {
                        data: data
                    }
                ]
            });
        },


        OnClickFind()
        {
            this.FindOnlineCountAsync();
        }


    },
};
</script>