using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// H活动-点卡2.0.xlsx-恐惧之地怪物
    /// </summary>
    public partial class TrialTower_MonsterConfig : C_ConfigInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 怪物ID
        /// </summary>
        public int MobId { get; set; }
        /// <summary>
        /// 怪物
        /// </summary>
        public string Monster { get; set; }
        /// <summary>
        /// 怪物数量
        /// </summary>
        public int Number { get; set; }
    }
    /// <summary>
    /// 配置数据:H活动-点卡2.0.xlsx-恐惧之地怪物
    /// </summary>
    [ReadConfigAttribute(typeof(TrialTower_MonsterConfig), new AppType[] { AppType.Game })]
    public class TrialTower_MonsterConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, TrialTower_MonsterConfig> JsonDic = new Dictionary<int, TrialTower_MonsterConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public TrialTower_MonsterConfigJson(string b_ReadStr)
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
            List<TrialTower_MonsterConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TrialTower_MonsterConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(TrialTower_MonsterConfigJson);
    }
}