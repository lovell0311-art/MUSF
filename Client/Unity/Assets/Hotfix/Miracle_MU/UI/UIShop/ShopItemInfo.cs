using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ETHotfix
{
    /// <summary>
    /// 商品信息类
    /// </summary>
    public class ShopItemInfo
    {
        public string ItemName { get; set; }//物品名字
       public int ShopId { get; set; }//商品配置表ID
        public int ShopMall { get; set; }//商城类型
        public int ShopItemType { get; set; }//物品类型
        public int Id { get; set; }//商品ID
        public int Price { get; set; }//价格
        public string ItemIcon { get; set; }//物品资源名字
        public string Introduce { get; set; }//物品简介
        public long StartTime { get; set; }//限时起点秒
        public long EndTime { get; set; }//限时终点秒
        public int Discount { get; set; }//折扣
        public int BuyMinLimit { get; set; }//购买限制单次购买最小数量
        public int BuyMaxLimit { get; set; }//购买限制单次购买最大数量
        public int UnitQuantity { get; set; }//购买限制单次购买每组数量
        public int ItemTime { get; set; }//物品使用时间 0 永久
        public int CacheCount { get; set; }//库存
        public int Gemtypes { get; set; }//所需宝石类型
    }
}