using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// H活动-点卡2.0.xlsx-在线签到
    /// </summary>
    public partial class OnlineDuration_RewardConfig : C_ConfigInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 在线时长
        /// </summary>
        public int Time { get; set; }
        /// <summary>
        /// 带[主宰无双]称号
        /// </summary>
        public int Title { get; set; }
        /// <summary>
        /// 奖励金币
        /// </summary>
        public int RewardCoins { get; set; }
        /// <summary>
        /// 奖励U币
        /// </summary>
        public int RewardMiracleCoin { get; set; }
        /// <summary>
        /// 奖励道具ID
        /// </summary>
        public List<int> ItemID { get; set; }
    }
    /// <summary>
    /// 配置数据:H活动-点卡2.0.xlsx-在线签到
    /// </summary>
    [ReadConfigAttribute(typeof(OnlineDuration_RewardConfig), new AppType[] { AppType.Game })]
    public class OnlineDuration_RewardConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, OnlineDuration_RewardConfig> JsonDic = new Dictionary<int, OnlineDuration_RewardConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public OnlineDuration_RewardConfigJson(string b_ReadStr)
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
            List<OnlineDuration_RewardConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OnlineDuration_RewardConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(OnlineDuration_RewardConfigJson);
    }
}