using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// W宝箱-点卡2.0.xlsx-宝箱物品信息
    /// </summary>
    public partial class TreasureChest_ItemInfoConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 宝箱id
        /// </summary>
        public int TreasureChestId { get; set; }
        /// <summary>
        /// 物品id
        /// </summary>
        public int ItemId { get; set; }
        /// <summary>
        /// 强化等级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 追加列表id
        /// </summary>
        public int OptListId { get; set; }
        /// <summary>
        /// 追加等级
        /// </summary>
        public int OptLevel { get; set; }
        /// <summary>
        /// 拥有技能
        /// </summary>
        public int HasSkill { get; set; }
        /// <summary>
        /// 套装id
        /// </summary>
        public int SetId { get; set; }
        /// <summary>
        /// 绑定物品
        /// </summary>
        public int IsBind { get; set; }
        /// <summary>
        /// 卓越属性
        /// </summary>
        public List<int> OptionExcellent { get; set; }
        /// <summary>
        /// 自定义属性方法
        /// </summary>
        public string CustomAttrMathod { get; set; }
        /// <summary>
        /// 掉落权重
        /// </summary>
        public int DropRate { get; set; }
    }
    /// <summary>
    /// 配置数据:W宝箱-点卡2.0.xlsx-宝箱物品信息
    /// </summary>
    [ReadConfigAttribute(typeof(TreasureChest_ItemInfoConfig), new AppType[] { AppType.Game })]
    public class TreasureChest_ItemInfoConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, TreasureChest_ItemInfoConfig> JsonDic = new Dictionary<int, TreasureChest_ItemInfoConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public TreasureChest_ItemInfoConfigJson(string b_ReadStr)
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
            List<TreasureChest_ItemInfoConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TreasureChest_ItemInfoConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(TreasureChest_ItemInfoConfigJson);
    }
}