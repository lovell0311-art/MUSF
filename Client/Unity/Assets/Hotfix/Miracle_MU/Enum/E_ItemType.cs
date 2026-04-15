using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 物品类型
    /// </summary>
    public enum E_ItemType
    {
        None = 0,
        /// <summary>剑</summary>
        Swords = 1,
        /// <summary>斧头</summary>
        Axes = 2,
        /// <summary>棒/槌</summary>
        Maces = 3,
        /// <summary>弓</summary>
        Bows = 4,
        /// <summary>弩</summary>
        Crossbows = 5,
        /// <summary>箭筒/箭</summary>
        Arrow = 6,
        /// <summary>矛/长矛</summary>
        Spears = 7,
        /// <summary>魔杖</summary>
        Staffs = 8,
        /// <summary>异界之书/魔法书</summary>
        MagicBook = 9,
        /// <summary> 权杖 </summary>
        Scepter = 10,
        /// <summary>符文魔棒</summary>
        RuneWand = 11,
        /// <summary>拳刃</summary>
        FistBlade = 12,
        /// <summary>魔剑</summary>
        MagicSword = 13,
        /// <summary>短剑</summary>
        ShortSword = 14,
        /// <summary>魔法枪</summary>
        MagicGun = 15,
        /// <summary>盾</summary>
        Shields = 16,
        /// <summary>头盔</summary>
        Helms = 17,
        /// <summary>盔甲</summary>
        Armors = 18,
        /// <summary>护腿</summary>
        Pants = 19,
        /// <summary>护手</summary>
        Gloves = 20,
        /// <summary>靴子</summary>
        Boots = 21,
        /// <summary>翅膀/披风</summary>
        Wing = 22,
        /// <summary>项链</summary>
        Necklace = 23,
        /// <summary>戒指</summary>
        Rings = 24,
        /// <summary>耳环</summary>
        Dangler = 25,
        /// <summary>坐骑</summary>
        Mounts = 26,
        /// <summary>荧光宝石</summary>
        FGemstone = 27,
        /// <summary>宝石</summary>
        Gemstone = 28,
        /// <summary>技能书</summary>
        SkillBooks = 29,
        /// <summary>守护</summary>
        Guard = 30,
        /// <summary>消耗品(血瓶|药水|实力提升卷轴)</summary>
        Consumables = 31,
        /// <summary> 其他 </summary>
        Other = 32,
        /// <summary> 任务物品 </summary>
        Task = 33,
        /// <summary> 旗帜 </summary>
        QiZhi = 34,
        /// <summary> 宠物 </summary>
        Pet = 35,
        /// <summary>手环</summary>
        WristBand = 36
    };
    public static class ItemType
    {
        public static string GetItenOneType(this int index) => index switch
        {
            1 => "武器",
            2 => "防具",
            3 => "坐骑",
            4 => "翅膀",
            5 => "宠物",
            6 => "旗帜",
            7 => "其他",
            _ => string.Empty
        };
        public static int GetItenType(this int index) => index switch
        {
            1 => 1,
            2 => 1,
            3 => 1,
            4 => 1,
            5 => 1,
            6 => 1,
            7 => 1,
            8 => 1,
            9 => 1,
            10 => 1,
            11 => 1,
            12 => 1,
            13 => 1,
            14 => 1,
            15 => 1,
            16 => 2,
            17 => 2,
            18 => 2,
            19 => 2,
            20 => 2,
            21 => 2,
            22 => 4,
            23 => 2,
            24 => 2,
            25 => 2,
            26 => 3,
            27 => 7,
            28 => 7,
            29 => 7,
            30 => 7,
            31 => 7,
            32 => 7,
            33 => 7,
            34 => 6,
            35 => 5,
            36 => 2,
            _ => 0
        };
        public static string GetItenTwoType(this int index) => index switch
        {
            1 => "剑",
            2 => "斧头",
            3 => "棒/槌",
            4 => "弓",
            5 => "弩",
            6 => "箭筒/箭",
            7 => "矛/长矛",
            8 => "魔杖",
            9 => "魔法书",
            10 => "权杖",
            11 => "符文魔棒",
            12 => "拳刃",
            13 => "魔剑",
            14 => "短剑",
            15 => "魔法枪",
            16 => "盾",
            17 => "头盔",
            18 => "盔甲",
            19 => "护腿",
            20 => "护手",
            21 => "靴子",
            23 => "项链",
            24 => "戒指",
            25 => "耳环",
            36 => "手环",
            _=> string.Empty
        };
        
    }

}