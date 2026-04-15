using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// F副本-点卡2.0.xlsx-恶魔广场排名
    /// </summary>
    public partial class BattleCopy_RankConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 排名倍数
        /// </summary>
        public int Multiple { get; set; }
    }
    /// <summary>
    /// 配置数据:F副本-点卡2.0.xlsx-恶魔广场排名
    /// </summary>
    [ReadConfigAttribute(typeof(BattleCopy_RankConfig), new AppType[] { AppType.Game })]
    public class BattleCopy_RankConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, BattleCopy_RankConfig> JsonDic = new Dictionary<int, BattleCopy_RankConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public BattleCopy_RankConfigJson(string b_ReadStr)
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
            List<BattleCopy_RankConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BattleCopy_RankConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(BattleCopy_RankConfigJson);
    }
}