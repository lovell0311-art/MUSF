using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// W宝箱-点卡2.0.xlsx-宝箱类型
    /// </summary>
    public partial class TreasureChest_TypeConfig : C_ConfigInfo
    {
        /// <summary>
        /// 物品id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 不重复
        /// </summary>
        public int NotRepeat { get; set; }
        /// <summary>
        /// 必掉道具
        /// </summary>
        public Dictionary<int,int> MustFall { get; set; }
        /// <summary>
        /// 数量权重
        /// </summary>
        public Dictionary<int,int> CountRate { get; set; }
    }
    /// <summary>
    /// 配置数据:W宝箱-点卡2.0.xlsx-宝箱类型
    /// </summary>
    [ReadConfigAttribute(typeof(TreasureChest_TypeConfig), new AppType[] { AppType.Game })]
    public class TreasureChest_TypeConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, TreasureChest_TypeConfig> JsonDic = new Dictionary<int, TreasureChest_TypeConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public TreasureChest_TypeConfigJson(string b_ReadStr)
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
            List<TreasureChest_TypeConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TreasureChest_TypeConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(TreasureChest_TypeConfigJson);
    }
}