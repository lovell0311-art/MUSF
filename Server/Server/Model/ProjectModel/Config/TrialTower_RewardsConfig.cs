using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// H活动-点卡2.0.xlsx-恐惧之地奖励
    /// </summary>
    public partial class TrialTower_RewardsConfig : C_ConfigInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 奖励
        /// </summary>
        public string Reward { get; set; }
    }
    /// <summary>
    /// 配置数据:H活动-点卡2.0.xlsx-恐惧之地奖励
    /// </summary>
    [ReadConfigAttribute(typeof(TrialTower_RewardsConfig), new AppType[] { AppType.Game })]
    public class TrialTower_RewardsConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, TrialTower_RewardsConfig> JsonDic = new Dictionary<int, TrialTower_RewardsConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public TrialTower_RewardsConfigJson(string b_ReadStr)
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
            List<TrialTower_RewardsConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TrialTower_RewardsConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(TrialTower_RewardsConfigJson);
    }
}