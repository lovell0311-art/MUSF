using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// H活动-点卡2.0.xlsx-国庆签到
    /// </summary>
    public partial class SevenDay_RewardPropsConfig : C_ConfigInfo
    {
        /// <summary>
        /// 奖励ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 活动ID
        /// </summary>
        public int ActivityID { get; set; }
        /// <summary>
        /// 奖励魔晶
        /// </summary>
        public int MagicCrystal { get; set; }
    }
    /// <summary>
    /// 配置数据:H活动-点卡2.0.xlsx-国庆签到
    /// </summary>
    [ReadConfigAttribute(typeof(SevenDay_RewardPropsConfig), new AppType[] { AppType.Game })]
    public class SevenDay_RewardPropsConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, SevenDay_RewardPropsConfig> JsonDic = new Dictionary<int, SevenDay_RewardPropsConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public SevenDay_RewardPropsConfigJson(string b_ReadStr)
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
            List<SevenDay_RewardPropsConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SevenDay_RewardPropsConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(SevenDay_RewardPropsConfigJson);
    }
}