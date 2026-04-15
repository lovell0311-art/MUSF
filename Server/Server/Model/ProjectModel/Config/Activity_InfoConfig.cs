using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// H活动-点卡2.0.xlsx-活动
    /// </summary>
    public partial class Activity_InfoConfig : C_ConfigInfo
    {
        /// <summary>
        /// 活动ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 活动介绍
        /// </summary>
        public string ActivityIntroduce { get; set; }
        /// <summary>
        /// 奖励介绍
        /// </summary>
        public string RewardIntroduce { get; set; }
        /// <summary>
        /// 开启时间
        /// </summary>
        public string OpenTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }
    }
    /// <summary>
    /// 配置数据:H活动-点卡2.0.xlsx-活动
    /// </summary>
    [ReadConfigAttribute(typeof(Activity_InfoConfig), new AppType[] { AppType.Game })]
    public class Activity_InfoConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Activity_InfoConfig> JsonDic = new Dictionary<int, Activity_InfoConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Activity_InfoConfigJson(string b_ReadStr)
        {
            ReadData(b_ReadStr);
        }
        /// <summary>
        /// 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public override void ReadData(string b_ReadStr)
        {
            JsonDic.Clear();
            List<Activity_InfoConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Activity_InfoConfig>>(b_ReadStr);
            for (int i = 0; i < jsonData.Count; i++)
            {
                var mConfig = jsonData[i];
                mConfig.InitExpand();
                JsonDic[mConfig.Id] = mConfig;
            }
        }
        /// <summary>
        /// ConfigType
        /// </summary>
        public override Type ConfigType => typeof(Activity_InfoConfigJson);
    }
}