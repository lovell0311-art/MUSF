using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// W物品自定义掉落-点卡2.0.xlsx-自定义掉落类型
    /// </summary>
    public partial class ItemCustomDrop_TypeConfig : C_ConfigInfo
    {
        /// <summary>
        /// 配置ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 权重
        /// </summary>
        public int DropRate { get; set; }
    }
    /// <summary>
    /// 配置数据:W物品自定义掉落-点卡2.0.xlsx-自定义掉落类型
    /// </summary>
    [ReadConfigAttribute(typeof(ItemCustomDrop_TypeConfig), new AppType[] { AppType.Game })]
    public class ItemCustomDrop_TypeConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, ItemCustomDrop_TypeConfig> JsonDic = new Dictionary<int, ItemCustomDrop_TypeConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public ItemCustomDrop_TypeConfigJson(string b_ReadStr)
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
            List<ItemCustomDrop_TypeConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemCustomDrop_TypeConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(ItemCustomDrop_TypeConfigJson);
    }
}