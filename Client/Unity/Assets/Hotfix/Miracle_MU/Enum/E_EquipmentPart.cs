using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{   
    /// <summary>
    /// 角色部位
    /// </summary>
    public enum E_EquipmentPart : byte
    {
        /// <summary> 武器 </summary>
        Weapon = 1,
        /// <summary> 盾牌 </summary>
        Shield = 2,
        /// <summary> 头盔 </summary>
        Helmet = 3,
        /// <summary> 铠甲 </summary>
        Armor = 4,
        /// <summary> 护腿 </summary>
        Leggings = 5,
        /// <summary> 护手 </summary>
        HandGuard = 6,
        /// <summary> 靴子 </summary>
        Boots = 7,
        /// <summary> 翅膀 </summary>
        Wing =8,
        /// <summary> 守护（小恶魔） </summary>
        Guard = 9,
        /// <summary> 项链 </summary>
        Necklace = 10,
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
       

        None = 200,
    }
}