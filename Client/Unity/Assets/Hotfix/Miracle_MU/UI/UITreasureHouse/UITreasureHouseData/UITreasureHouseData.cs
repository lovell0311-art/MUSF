using NPOI.POIFS.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    public static class UITreasureHouseData
    {
        /// <summary>
        /// 一类
        /// </summary>
        public static List<ItemSort> itemOneList = new List<ItemSort>();
        /// <summary>
        /// 二类
        /// </summary>
        public static List<ItemSort> itemTwoList = new List<ItemSort>();
        /// <summary>
        /// 物品列表
        /// </summary>
        public static List<TreasureHouseItemInfo> treasureHouseItems = new List<TreasureHouseItemInfo>();
        /// <summary>
        /// 我的物品列表
        /// </summary>
        public static List<TreasureHouseItemInfo> MyItems = new List<TreasureHouseItemInfo>();
    }
    public class ItemSort
    {
        /// <summary>
        /// id
        /// </summary>
        public int Id;
        /// <summary>
        /// 名字
        /// </summary>
        public string Name;
        /// <summary>
        /// 层级
        /// </summary>
        public int Order;
        /// <summary>
        /// 父级Id
        /// </summary>
        public int ParentId;
    }
    public class FiltrateData
    {
        public int Page;//大类
        public string Name;//道具名称
        public string RoleClass = "";//职业类型
        public int Excellent = 0;//卓越条数
        public int Enhance = 0;//强化等级
        public int Readdition = 0;//套装，1是套装，2是洞装，3是一起
        public int MaxType = 1;//道具所在大类
        public int SortType = 0;//排序方式0降序1升序默认降序
    }
}
