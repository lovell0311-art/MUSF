using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// S商城-点卡2.0.xlsx-宝石兑换
    /// </summary>
    public partial class ShopMall_PropConfig : C_ConfigInfo
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 商城类型
        /// </summary>
        public int ShopMall { get; set; }
        /// <summary>
        /// 商品分类
        /// </summary>
        public int ShopType { get; set; }
        /// <summary>
        /// 道具ID
        /// </summary>
        public int ItemID { get; set; }
        /// <summary>
        /// 物品名称
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 物品标识
        /// </summary>
        public string ItemIcon { get; set; }
        /// <summary>
        /// 物品介绍
        /// </summary>
        public string Introduce { get; set; }
        /// <summary>
        /// 物品到期时间
        /// </summary>
        public int ItemTime { get; set; }
        /// <summary>
        /// 宝石种类
        /// </summary>
        public int Gemtypes { get; set; }
        /// <summary>
        /// 初始单价
        /// </summary>
        public int Price { get; set; }
        /// <summary>
        /// 限时起点
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 限时时长
        /// </summary>
        public long Duration { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public int Discount { get; set; }
        /// <summary>
        /// 折扣限时起点
        /// </summary>
        public string DiscountstartTime { get; set; }
        /// <summary>
        /// 折扣限时时长
        /// </summary>
        public long Discountduration { get; set; }
        /// <summary>
        /// 最大组数限制
        /// </summary>
        public int BuyMaxlimit { get; set; }
        /// <summary>
        /// 最小组数限制
        /// </summary>
        public int BuyMinlimit { get; set; }
        /// <summary>
        /// 单组数量
        /// </summary>
        public int UnitQuantity { get; set; }
        /// <summary>
        /// 套装ID
        /// </summary>
        public int SetID { get; set; }
        /// <summary>
        /// 限量
        /// </summary>
        public int Limit { get; set; }
        /// <summary>
        /// 限量时间(秒)
        /// </summary>
        public int LimitTime { get; set; }
        /// <summary>
        /// 自定义属性方法
        /// </summary>
        public string CustomAttrMathod { get; set; }
        /// <summary>
        /// 库存数量
        /// </summary>
        public int InventoryMax { get; set; }
        /// <summary>
        /// 绑定物品
        /// </summary>
        public int IsBind { get; set; }
    }
    /// <summary>
    /// 配置数据:S商城-点卡2.0.xlsx-宝石兑换
    /// </summary>
    [ReadConfigAttribute(typeof(ShopMall_PropConfig), new AppType[] { AppType.Game })]
    public class ShopMall_PropConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, ShopMall_PropConfig> JsonDic = new Dictionary<int, ShopMall_PropConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public ShopMall_PropConfigJson(string b_ReadStr)
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
            List<ShopMall_PropConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ShopMall_PropConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(ShopMall_PropConfigJson);
    }
}