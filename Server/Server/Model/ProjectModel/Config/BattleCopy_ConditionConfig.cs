using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// F副本-点卡2.0.xlsx-副本条件
    /// </summary>
    public partial class BattleCopy_ConditionConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 挑战次数
        /// </summary>
        public int Challenge { get; set; }
        /// <summary>
        /// 经验倍数
        /// </summary>
        public int EXPMultiple { get; set; }
        /// <summary>
        /// 分数倍数
        /// </summary>
        public int ScoreRate { get; set; }
        /// <summary>
        /// 经验系数
        /// </summary>
        public int EXPRate { get; set; }
        /// <summary>
        /// 金币系数
        /// </summary>
        public int CoinRate { get; set; }
    }
    /// <summary>
    /// 配置数据:F副本-点卡2.0.xlsx-副本条件
    /// </summary>
    [ReadConfigAttribute(typeof(BattleCopy_ConditionConfig), new AppType[] { AppType.Game })]
    public class BattleCopy_ConditionConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, BattleCopy_ConditionConfig> JsonDic = new Dictionary<int, BattleCopy_ConditionConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public BattleCopy_ConditionConfigJson(string b_ReadStr)
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
            List<BattleCopy_ConditionConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BattleCopy_ConditionConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(BattleCopy_ConditionConfigJson);
    }
}