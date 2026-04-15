using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// W物品属性-点卡2.0.xlsx-额外属性
    /// </summary>
    public partial class ItemAttrEntry_ExtraConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 属性名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 万分比
        /// </summary>
        public int IsBP { get; set; }
        /// <summary>
        /// 属性id
        /// </summary>
        public int PropId { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public int Value0 { get; set; }
        /// <summary>
        /// 出现权重
        /// </summary>
        public int Rate0 { get; set; }
        /// <summary>
        /// 值2
        /// </summary>
        public int Value1 { get; set; }
        /// <summary>
        /// 出现权重2
        /// </summary>
        public int Rate1 { get; set; }
        /// <summary>
        /// 值3
        /// </summary>
        public int Value2 { get; set; }
        /// <summary>
        /// 出现权重3
        /// </summary>
        public int Rate2 { get; set; }
    }
    /// <summary>
    /// 配置数据:W物品属性-点卡2.0.xlsx-额外属性
    /// </summary>
    [ReadConfigAttribute(typeof(ItemAttrEntry_ExtraConfig), new AppType[] { AppType.Game,AppType.GM })]
    public class ItemAttrEntry_ExtraConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, ItemAttrEntry_ExtraConfig> JsonDic = new Dictionary<int, ItemAttrEntry_ExtraConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public ItemAttrEntry_ExtraConfigJson(string b_ReadStr)
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
            List<ItemAttrEntry_ExtraConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemAttrEntry_ExtraConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(ItemAttrEntry_ExtraConfigJson);
    }
}