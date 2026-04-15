using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// W物品属性-点卡2.0.xlsx-卓越属性条数概率
    /// </summary>
    public partial class ItemAttrEntryRate_ExcConfig : C_ConfigInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 条数
        /// </summary>
        public int StripCnt { get; set; }
        /// <summary>
        /// 装备概率(万分比)
        /// </summary>
        public int Rate { get; set; }
        /// <summary>
        /// 宠物概率(万分比)
        /// </summary>
        public int PetsRate { get; set; }
        /// <summary>
        /// 旗帜概率(万分比)
        /// </summary>
        public int FlagRate { get; set; }
    }
    /// <summary>
    /// 配置数据:W物品属性-点卡2.0.xlsx-卓越属性条数概率
    /// </summary>
    [ReadConfigAttribute(typeof(ItemAttrEntryRate_ExcConfig), new AppType[] { AppType.Game,AppType.GM })]
    public class ItemAttrEntryRate_ExcConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, ItemAttrEntryRate_ExcConfig> JsonDic = new Dictionary<int, ItemAttrEntryRate_ExcConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public ItemAttrEntryRate_ExcConfigJson(string b_ReadStr)
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
            List<ItemAttrEntryRate_ExcConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemAttrEntryRate_ExcConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(ItemAttrEntryRate_ExcConfigJson);
    }
}