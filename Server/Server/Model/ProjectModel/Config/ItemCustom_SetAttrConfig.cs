using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// W物品自定义属性-点卡2.0.xlsx-套装属性
    /// </summary>
    public partial class ItemCustom_SetAttrConfig : C_ConfigInfo
    {
        /// <summary>
        /// 配置ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 最小伤害
        /// </summary>
        public int DamageMin { get; set; }
        /// <summary>
        /// 最大伤害
        /// </summary>
        public int DamageMax { get; set; }
        /// <summary>
        /// 诅咒
        /// </summary>
        public int Curse { get; set; }
        /// <summary>
        /// 魔力百分比
        /// </summary>
        public int MagicPct { get; set; }
        /// <summary>
        /// 攻击速度
        /// </summary>
        public int AttackSpeed { get; set; }
        /// <summary>
        /// 防御
        /// </summary>
        public int Defense { get; set; }
        /// <summary>
        /// 防御率
        /// </summary>
        public int DefenseRate { get; set; }
        /// <summary>
        /// 耐久
        /// </summary>
        public int Durable { get; set; }
        /// <summary>
        /// 需求等级
        /// </summary>
        public int ReqLvl { get; set; }
        /// <summary>
        /// 力量
        /// </summary>
        public int ReqStr { get; set; }
        /// <summary>
        /// 敏捷
        /// </summary>
        public int ReqAgi { get; set; }
        /// <summary>
        /// 体力
        /// </summary>
        public int ReqVit { get; set; }
        /// <summary>
        /// 智力
        /// </summary>
        public int ReqEne { get; set; }
        /// <summary>
        /// 统率
        /// </summary>
        public int ReqCom { get; set; }
    }
    /// <summary>
    /// 配置数据:W物品自定义属性-点卡2.0.xlsx-套装属性
    /// </summary>
    [ReadConfigAttribute(typeof(ItemCustom_SetAttrConfig), new AppType[] { AppType.Game })]
    public class ItemCustom_SetAttrConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, ItemCustom_SetAttrConfig> JsonDic = new Dictionary<int, ItemCustom_SetAttrConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public ItemCustom_SetAttrConfigJson(string b_ReadStr)
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
            List<ItemCustom_SetAttrConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemCustom_SetAttrConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(ItemCustom_SetAttrConfigJson);
    }
}