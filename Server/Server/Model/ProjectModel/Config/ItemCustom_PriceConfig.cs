using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// W物品自定义属性-点卡2.0.xlsx-物品价值
    /// </summary>
    public partial class ItemCustom_PriceConfig : C_ConfigInfo
    {
        /// <summary>
        /// 配置ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// #名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 使用公式
        /// </summary>
        public int UseFormula { get; set; }
        /// <summary>
        /// 价值公式
        /// </summary>
        public string Formula { get; set; }
        /// <summary>
        /// 0级
        /// </summary>
        public int Value0 { get; set; }
        /// <summary>
        /// 1级
        /// </summary>
        public int Value1 { get; set; }
        /// <summary>
        /// 2级
        /// </summary>
        public int Value2 { get; set; }
        /// <summary>
        /// 3级
        /// </summary>
        public int Value3 { get; set; }
        /// <summary>
        /// 4级
        /// </summary>
        public int Value4 { get; set; }
        /// <summary>
        /// 5级
        /// </summary>
        public int Value5 { get; set; }
        /// <summary>
        /// 6级
        /// </summary>
        public int Value6 { get; set; }
        /// <summary>
        /// 7级
        /// </summary>
        public int Value7 { get; set; }
        /// <summary>
        /// 8级
        /// </summary>
        public int Value8 { get; set; }
        /// <summary>
        /// 9级
        /// </summary>
        public int Value9 { get; set; }
        /// <summary>
        /// 10级
        /// </summary>
        public int Value10 { get; set; }
        /// <summary>
        /// 11级
        /// </summary>
        public int Value11 { get; set; }
        /// <summary>
        /// 12级
        /// </summary>
        public int Value12 { get; set; }
        /// <summary>
        /// 13级
        /// </summary>
        public int Value13 { get; set; }
        /// <summary>
        /// 14级
        /// </summary>
        public int Value14 { get; set; }
        /// <summary>
        /// 15级
        /// </summary>
        public int Value15 { get; set; }
    }
    /// <summary>
    /// 配置数据:W物品自定义属性-点卡2.0.xlsx-物品价值
    /// </summary>
    [ReadConfigAttribute(typeof(ItemCustom_PriceConfig), new AppType[] { AppType.Game })]
    public class ItemCustom_PriceConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, ItemCustom_PriceConfig> JsonDic = new Dictionary<int, ItemCustom_PriceConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public ItemCustom_PriceConfigJson(string b_ReadStr)
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
            List<ItemCustom_PriceConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemCustom_PriceConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(ItemCustom_PriceConfigJson);
    }
}