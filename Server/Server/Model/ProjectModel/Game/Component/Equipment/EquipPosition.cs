using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    /// <summary>
    /// 装备部位
    /// </summary>
    public enum EquipPosition
    {
        /// <summary>不可装备</summary>
        None = 0,
        /// <summary>武器</summary>
        Weapon = 1,
        /// <summary>盾牌 </summary>
        Shield = 2,
        /// <summary>头盔</summary>
        Helmet = 3,
        /// <summary>铠甲</summary>
        Armor = 4,
        /// <summary>护腿</summary>
        Leggings = 5,
        /// <summary>护手</summary>
        HandGuard = 6,
        /// <summary>靴子</summary>
        Boots = 7,
        /// <summary>翅膀</summary>
        Wing = 8,
        /// <summary>守护</summary>
        Guard = 9,
        /// <summary>项链</summary>
        Necklace = 10,
        /// <summary>左戒指</summary>
        LeftRing = 11,
        /// <summary>右戒指</summary>
        RightRing = 12,
        /// <summary>旗帜</summary>
        Flag = 13,
        /// <summary>宠物</summary>
        Pet = 14,
        /// <summary> 手环 </summary>
        Bracelet = 15,
        /// <summary> 天鹰 </summary>
        TianYing = 16,

        /// 临时卡槽，不会保存到数据库
        TempSlot = 100,
        /// <summary>坐骑</summary>
        Mounts = 101,

    }
}
