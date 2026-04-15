using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// W物品自定义掉落-点卡2.0.xlsx-自定义掉落信息
    /// </summary>
    public partial class ItemCustomDrop_InfoConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 掉落类型
        /// </summary>
        public int DropType { get; set; }
        /// <summary>
        /// 怪物id
        /// </summary>
        public int MonsterId { get; set; }
        /// <summary>
        /// 怪物名
        /// </summary>
        public string MonsterName { get; set; }
        /// <summary>
        /// 怪物id范围
        /// </summary>
        public List<int> MonsterIdRange { get; set; }
        /// <summary>
        /// 怪物等级范围
        /// </summary>
        public List<int> MonsterLevel { get; set; }
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
        /// 权重
        /// </summary>
        public int DropRate { get; set; }
    }
    /// <summary>
    /// 配置数据:W物品自定义掉落-点卡2.0.xlsx-自定义掉落信息
    /// </summary>
    [ReadConfigAttribute(typeof(ItemCustomDrop_InfoConfig), new AppType[] { AppType.Game })]
    public class ItemCustomDrop_InfoConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, ItemCustomDrop_InfoConfig> JsonDic = new Dictionary<int, ItemCustomDrop_InfoConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public ItemCustomDrop_InfoConfigJson(string b_ReadStr)
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
            List<ItemCustomDrop_InfoConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemCustomDrop_InfoConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(ItemCustomDrop_InfoConfigJson);
    }
}