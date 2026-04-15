using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// H活动-点卡2.0.xlsx-等级奖励
    /// </summary>
    public partial class LevelUpRewards_RewardConfig : C_ConfigInfo
    {
        /// <summary>
        /// 奖励ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 领取限制
        /// </summary>
        public int Limit { get; set; }
        /// <summary>
        /// 活动ID
        /// </summary>
        public int ActivityID { get; set; }
        /// <summary>
        /// 奖励金币
        /// </summary>
        public int GoldCoin { get; set; }
        /// <summary>
        /// 奖励U币
        /// </summary>
        public int MiracleCoin { get; set; }
        /// <summary>
        /// 奖励道具ID
        /// </summary>
        public List<int> ItemID { get; set; }
    }
    /// <summary>
    /// 配置数据:H活动-点卡2.0.xlsx-等级奖励
    /// </summary>
    [ReadConfigAttribute(typeof(LevelUpRewards_RewardConfig), new AppType[] { AppType.Game })]
    public class LevelUpRewards_RewardConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, LevelUpRewards_RewardConfig> JsonDic = new Dictionary<int, LevelUpRewards_RewardConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public LevelUpRewards_RewardConfigJson(string b_ReadStr)
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
            List<LevelUpRewards_RewardConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LevelUpRewards_RewardConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(LevelUpRewards_RewardConfigJson);
    }
}