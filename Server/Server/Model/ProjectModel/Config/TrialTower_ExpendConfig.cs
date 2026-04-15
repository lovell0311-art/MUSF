using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// H活动-点卡2.0.xlsx-试炼之塔消耗
    /// </summary>
    public partial class TrialTower_ExpendConfig : C_ConfigInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 消耗(魔晶)
        /// </summary>
        public int Expend { get; set; }
    }
    /// <summary>
    /// 配置数据:H活动-点卡2.0.xlsx-试炼之塔消耗
    /// </summary>
    [ReadConfigAttribute(typeof(TrialTower_ExpendConfig), new AppType[] { AppType.Game })]
    public class TrialTower_ExpendConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, TrialTower_ExpendConfig> JsonDic = new Dictionary<int, TrialTower_ExpendConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public TrialTower_ExpendConfigJson(string b_ReadStr)
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
            List<TrialTower_ExpendConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TrialTower_ExpendConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(TrialTower_ExpendConfigJson);
    }
}