using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// H活动-点卡2.0.xlsx-每日抽奖
    /// </summary>
    public partial class GoldLottery_ItemInfoConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 活动ID
        /// </summary>
        public int ActivityID { get; set; }
        /// <summary>
        /// 图标名
        /// </summary>
        public string IconName { get; set; }
        /// <summary>
        /// 物品id
        /// </summary>
        public int ItemId { get; set; }
        /// <summary>
        /// 物品名
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 强化等级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 拥有技能
        /// </summary>
        public int HasSkill { get; set; }
        /// <summary>
        /// 有幸运属性
        /// </summary>
        public int HasLucky { get; set; }
        /// <summary>
        /// 自定义属性方法
        /// </summary>
        public string CustomAttrMathod { get; set; }
        /// <summary>
        /// 命中权重
        /// </summary>
        public int Weight { get; set; }
    }
    /// <summary>
    /// 配置数据:H活动-点卡2.0.xlsx-每日抽奖
    /// </summary>
    [ReadConfigAttribute(typeof(GoldLottery_ItemInfoConfig), new AppType[] { AppType.Game })]
    public class GoldLottery_ItemInfoConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, GoldLottery_ItemInfoConfig> JsonDic = new Dictionary<int, GoldLottery_ItemInfoConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public GoldLottery_ItemInfoConfigJson(string b_ReadStr)
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
            List<GoldLottery_ItemInfoConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GoldLottery_ItemInfoConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(GoldLottery_ItemInfoConfigJson);
    }
}