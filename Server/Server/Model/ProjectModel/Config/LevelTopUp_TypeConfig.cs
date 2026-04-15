using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// C充值礼包-点卡2.0.xlsx-等级充值
    /// </summary>
    public partial class LevelTopUp_TypeConfig : C_ConfigInfo
    {
        /// <summary>
        /// 捆绑包id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 充值金额
        /// </summary>
        public int RechargeAmount { get; set; }
        /// <summary>
        /// 职业类型
        /// </summary>
        public int RoleType { get; set; }
        /// <summary>
        /// 赠送物品ID
        /// </summary>
        public int ConfigId { get; set; }
        /// <summary>
        /// 物品名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 强化等级
        /// </summary>
        public int EnhanceLevel { get; set; }
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
    /// 配置数据:C充值礼包-点卡2.0.xlsx-等级充值
    /// </summary>
    [ReadConfigAttribute(typeof(LevelTopUp_TypeConfig), new AppType[] { AppType.Game })]
    public class LevelTopUp_TypeConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, LevelTopUp_TypeConfig> JsonDic = new Dictionary<int, LevelTopUp_TypeConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public LevelTopUp_TypeConfigJson(string b_ReadStr)
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
            List<LevelTopUp_TypeConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LevelTopUp_TypeConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(LevelTopUp_TypeConfigJson);
    }
}