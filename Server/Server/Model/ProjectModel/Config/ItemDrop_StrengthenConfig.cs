using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// W物品-点卡2.0.xlsx-强化等级概率
    /// </summary>
    public partial class ItemDrop_StrengthenConfig : C_ConfigInfo
    {
        /// <summary>
        /// 索引
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 强化等级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
    /// <summary>
    /// 配置数据:W物品-点卡2.0.xlsx-强化等级概率
    /// </summary>
    [ReadConfigAttribute(typeof(ItemDrop_StrengthenConfig), new AppType[] { AppType.Game,AppType.Robot,AppType.GM })]
    public class ItemDrop_StrengthenConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, ItemDrop_StrengthenConfig> JsonDic = new Dictionary<int, ItemDrop_StrengthenConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public ItemDrop_StrengthenConfigJson(string b_ReadStr)
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
            List<ItemDrop_StrengthenConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemDrop_StrengthenConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(ItemDrop_StrengthenConfigJson);
    }
}