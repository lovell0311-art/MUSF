using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{

    /// <summary>
    /// 格子类型枚举
    /// </summary>
    public enum E_Grid_Type : byte
    {
        /// <summary>不可装备</summary>
        None = 0,
        /// <summary> 武器 </summary>
        Weapon = 1,
        /// <summary> 盾牌 </summary>
        Shield = 2,
        /// <summary> 头盔 </summary>
        Helmet = 3,
        /// <summary> 铠甲 </summary>
        Armor = 4,
        /// <summary> 护腿 </summary>
        Leggings =5,
        /// <summary> 护手 </summary>
        HandGuard = 6,
        /// <summary> 靴子 </summary>
        Boots = 7,
        /// <summary> 翅膀 </summary>
        Wing = 8,
        /// <summary> 守护（小恶魔） </summary>
        Guard = 9,
        /// <summary> 项链 </summary>
        Necklace =10,
        /// <summary> 左戒指 </summary>
        LeftRing = 11,
        /// <summary> 右戒指 </summary>
        RightRing = 12,
        /// <summary> 旗帜 </summary>
        Flag = 13,
        /// <summary> 宠物 </summary>
        Pet = 14,
        /// <summary> 手环 </summary>
        WristBand = 15,
        /// <summary> 天鹰</summary>
        TianYing = 16,
        /// <summary> 背部武器 左边节点 </summary>
        LeftBackPos = 25,
        /// <summary> 背部武器 右边节点 </summary>
        RightBackPos = 26,



        /// <summary>背包</summary>
        Knapsack = 17,
        /// <summary>合成</summary>
        Gem_Merge,
        /// <summary>仓库 </summary>
        Ware_House,
        /// <summary>商城 </summary>
        Shop,
        /// <summary>摆摊</summary>
        Stallup,
        /// <summary>其他玩家的摊位</summary>
        Stallup_OtherPlayer,
        /// <summary>交易</summary>
        Trade,
        /// <summary>交易时其他玩家的物品面板</summary>
        Trade_Other,
        /// <summary>赠送金币</summary>
        GiveCoin,
        /// <summary>赠送物品</summary>
        GiveGoods,
        /// <summary>物品寄售</summary>
        Consignment,
        /// <summary>属性还原</summary>
        Reduction,
        /// <summary>镶嵌</summary>
        Inlay,
        /// <summary>删除</summary>
        Delete,


        /// <summary>坐骑</summary>
        Mounts = 101,

        
    }
}
