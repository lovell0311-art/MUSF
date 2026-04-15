using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// C充值礼包-点卡2.0.xlsx-甄选礼包信息
    /// </summary>
    public partial class SelectionGiftBundle_ItemInfoConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 甄选类型id
        /// </summary>
        public int TypeId { get; set; }
        /// <summary>
        /// 职业类型(0.全部职业)
        /// </summary>
        public int RoleType { get; set; }
        /// <summary>
        /// 是否可选
        /// </summary>
        public int IsSelectable { get; set; }
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
        /// 追加等级
        /// </summary>
        public int OptLevel { get; set; }
        /// <summary>
        /// 锻造等级
        /// </summary>
        public int SmithLevel { get; set; }
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
        /// 卓越属性
        /// </summary>
        public List<int> OptionExcellent { get; set; }
        /// <summary>
        /// 套装id
        /// </summary>
        public int SetId { get; set; }
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
    /// 配置数据:C充值礼包-点卡2.0.xlsx-甄选礼包信息
    /// </summary>
    [ReadConfigAttribute(typeof(SelectionGiftBundle_ItemInfoConfig), new AppType[] { AppType.Game })]
    public class SelectionGiftBundle_ItemInfoConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, SelectionGiftBundle_ItemInfoConfig> JsonDic = new Dictionary<int, SelectionGiftBundle_ItemInfoConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public SelectionGiftBundle_ItemInfoConfigJson(string b_ReadStr)
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
            List<SelectionGiftBundle_ItemInfoConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SelectionGiftBundle_ItemInfoConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(SelectionGiftBundle_ItemInfoConfigJson);
    }
}