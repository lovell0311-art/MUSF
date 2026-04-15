using System;
using System.Collections.Generic;

using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    /// <summary>
    /// 商城分类
    /// </summary>
    public enum ShopMallType
    {
        /// <summary>
        /// U币商城
        /// </summary>
        None,
        /// <summary>
        /// 魔晶商城
        /// </summary>
        CRYSTAL,
        /// <summary>
        /// 称号商城
        /// </summary>
        TITLE,
        /// <summary>
        /// 宝石兑商城
        /// </summary>
        GEM,
        /// <summary>
        /// 战盟积分商城
        /// </summary>
        ALLIANCESCORE,
        /// <summary>
        /// 商城类型上限
        /// </summary>
        ShopMax,
    }
    /// <summary>
    /// 物品分类
    /// </summary>
    public enum ItemSortType
    {
        /// <summary>
        /// buff类
        /// </summary>
        Buff = 1,
        /// <summary>
        /// 消耗类
        /// </summary>
        Consume,
        /// <summary>
        /// 宠物坐骑类
        /// </summary>
        PetMount,
        /// <summary>
        /// 特殊类
        /// </summary>
        Special,
        /// <summary>
        /// 装备类型
        /// </summary>
        Item,
        /// <summary>
        /// 套装类
        /// </summary>
        Set,
        /// <summary>
        /// 宝箱
        /// </summary>
        BaoXiang = 8,
        Max,
    }
    public class ShopItemDB : DBBase
    {
        /// <summary>
        /// 商城ID
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 道具ID
        /// </summary>
        public int ItemId { get; set; }
        /// <summary>
        /// 需要存数据库，道具的库存数量
        /// </summary>
        public int limitCnt { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public int IsDisabled { get; set; }
    }

    public class ShopItem
    {
        /// <summary>
        /// 商城ID
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 道具ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public int Price { get; set; }
        /// <summary>
        /// 物品名称
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 物品标识
        /// </summary>
        public string ItemIcon  { get; set; }
        /// <summary>
        /// 物品介绍
        /// </summary>
        public string Introduce { get; set; }
        /// <summary>
        /// 道具使用期限
        /// </summary>
        public int ItemTime { get; set; }
        /// <summary>
        /// 限时起点
        /// </summary>
        public long StartTime { get; set; }
        /// <summary>
        /// 限时终点
        /// </summary>
        public long EndTime { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public int Discount { get; set; }
        /// <summary>
        /// 购买限制：填入数值为单次购买最大组数量，默认填入1单次购买必须有1个
        /// </summary>
        public int BuyMaxlimit { get; set; }
        /// <summary>
        /// 购买限制：填入数值为单次购买最小组数量，默认填入1单次购买必须有1个
        /// </summary>
        public int BuyMinlimit { get; set; }
        /// <summary>
        /// 填入数值为单次购买每组数量
        /// </summary>
        public int UnitQuantity { get; set; }
        /// <summary>
        /// 物品自定义属性方法
        /// </summary>
        public List<string> CustomAttrMathod = new List<string>();
        /// <summary>
        /// 需要存数据库，道具的库存数量
        /// </summary>
        public int limitCnt { get; set; }
        /// <summary>
        /// 折扣起点
        /// </summary>
        public long DiscountStartTime { get; set; }
        /// <summary>
        /// 折扣终点
        /// </summary>
        public long DiscountEndTime { get; set; }
        public int SetId { get; set; }
        /// <summary>
        ///需要的宝石类型
        /// </summary>
        public int Gemtypes { get; set; }
    }
}