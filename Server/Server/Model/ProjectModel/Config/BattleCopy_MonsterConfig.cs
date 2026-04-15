using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// F副本-点卡2.0.xlsx-恶魔广场怪物
    /// </summary>
    public partial class BattleCopy_MonsterConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// #波数1
        /// </summary>
        public List<int> Number1 { get; set; }
        /// <summary>
        /// #波数2
        /// </summary>
        public List<int> Number2 { get; set; }
        /// <summary>
        /// #波数3
        /// </summary>
        public List<int> Number3 { get; set; }
        /// <summary>
        /// #波数4
        /// </summary>
        public List<int> Number4 { get; set; }
        /// <summary>
        /// #波数5
        /// </summary>
        public List<int> Number5 { get; set; }
        /// <summary>
        /// #波数6
        /// </summary>
        public List<int> Number6 { get; set; }
        /// <summary>
        /// #波数7
        /// </summary>
        public List<int> Number7 { get; set; }
        /// <summary>
        /// #波数8
        /// </summary>
        public List<int> Number8 { get; set; }
        /// <summary>
        /// #波数9
        /// </summary>
        public List<int> Number9 { get; set; }
        /// <summary>
        /// #波数10
        /// </summary>
        public List<int> Number10 { get; set; }
    }
    /// <summary>
    /// 配置数据:F副本-点卡2.0.xlsx-恶魔广场怪物
    /// </summary>
    [ReadConfigAttribute(typeof(BattleCopy_MonsterConfig), new AppType[] { AppType.Game })]
    public class BattleCopy_MonsterConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, BattleCopy_MonsterConfig> JsonDic = new Dictionary<int, BattleCopy_MonsterConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public BattleCopy_MonsterConfigJson(string b_ReadStr)
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
            List<BattleCopy_MonsterConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BattleCopy_MonsterConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(BattleCopy_MonsterConfigJson);
    }
}