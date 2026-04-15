using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// H活动-点卡2.0.xlsx-限时购买
    /// </summary>
    public partial class LimitedPurchase_RewardPropsConfig : C_ConfigInfo
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
        /// 充值金额
        /// </summary>
        public int TypeId { get; set; }
        /// <summary>
        /// 职业类型(0.全部职业)
        /// </summary>
        public int RoleType { get; set; }
        /// <summary>
        /// 物品id
        /// </summary>
        public int ItemId { get; set; }
        /// <summary>
        /// 物品名
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 物品资源名
        /// </summary>
        public string ResourceName { get; set; }
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
        /// 绑定物品
        /// </summary>
        public int IsBind { get; set; }
        /// <summary>
        /// 自定义属性方法
        /// </summary>
        public string CustomAttrMathod { get; set; }
    }
    /// <summary>
    /// 配置数据:H活动-点卡2.0.xlsx-限时购买
    /// </summary>
    [ReadConfigAttribute(typeof(LimitedPurchase_RewardPropsConfig), new AppType[] { AppType.Game })]
    public class LimitedPurchase_RewardPropsConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, LimitedPurchase_RewardPropsConfig> JsonDic = new Dictionary<int, LimitedPurchase_RewardPropsConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public LimitedPurchase_RewardPropsConfigJson(string b_ReadStr)
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
            List<LimitedPurchase_RewardPropsConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LimitedPurchase_RewardPropsConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(LimitedPurchase_RewardPropsConfigJson);
    }
}