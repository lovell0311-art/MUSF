using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ETHotfix
{

    /// <summary>
    ///物品附加属性（数值定义好后 禁止更改）
    /// </summary>
    public enum E_ItemValue : int
    {
        Begin,
        AttackSpeed = 1,// 攻击速度
        WalkSpeed = 2,// 移动速度
        DamageMin = 3,// 最小伤害
        DamageMax = 4,// 最大伤害
        DefenseRate = 5,// 防御成功率
        Defense = 6,// 防御
        MagicDefense = 7,// 魔法防御
        MagicDamage = 8,// 魔法伤害
        Curse = 9,// 诅咒
        Durability = 10,// 剩余耐久
        DurabilityMax = 11,// 最大耐久
        Level = 12, // 强化等级
        SkillId = 13,// 物品附带的技能
        IsBind = 14, // 绑定账号 0.未绑定 1.绑定的装备(账号)（无法交易，丢弃，摆摊） 2.绑定的装备(角色)（无法交易，丢弃，摆摊，移到仓库）
        IsTask = 15, // 0.其他普通物品 1.任务物品（无法交易，丢弃，摆摊，移到仓库,出售）
        ValidTime = 16, // 有效时间 时间戳 秒 还能用15年 (无法交易，摆摊)

        UpPetAttackPct = 17, // 宠物攻击力提升百分比

        RequireLevel = 20,        // 需要等级
        RequireStrength = 21,     // 需要力量
        RequireAgile = 22,    // 需要敏捷
        RequireEnergy = 23,       // 需要智力
        RequireVitality = 24,     // 需要体力
        RequireCommand = 25,   // 需要统率

        MountsLevel = 30,  // (坐骑/宠物)等级
        MountsExp = 31,    // (坐骑/宠物)经验

        BuyMoney = 40,  // 购买价格
        SellMoney = 41, // 出售价格
        RepairMoney = 42,   // 维修价格
        Quantity = 43,  // 数量


        OptValue = 48,  // 物品开启追加属性ID
        OptLevel = 49,  // 追加属性等级 0-4
        SetId = 50, // 套装id
        LuckyEquip = 51, // 是幸运装备

        OrecycledID = 56,   //再生属性ID 对应配置表
        OrecycledLevel = 57,//再生属性等级

        IsUsing = 60,  // 正在使用中 1.使用中... (无法交易，移动，出售，丢弃，摆摊)
        IsValidItem = 61,// 是有效果的物品
        IsLocking = 62,    // 锁定的物品 1.锁定中... 无法进行任何操作(如:移动，出售，丢弃，摆摊，使用)

        //荧光镶嵌        id:value/100  level:value%100
        FluoreAttr = 70,//荧光宝石附带属性ID和Value，荧光宝石特有
        FluoreSlotCount = 71,   //装备拥有的插槽数
        FluoreSlot1 = 72,//插槽1  
        FluoreSlot2 = 73,//插槽2
        FluoreSlot3 = 74,//插槽3
        FluoreSlot4 = 75,//插槽4
        FluoreSlot5 = 76,//插槽5


        // 宝藏、藏宝图字段
        TimeLimit = 80,//藏宝图的到期时间
        TreasureMapId = 81,
        TreasureKeyIdA = 82,
        TreasureKeyIdB = 83,
        // 区id
        TreasureZoneId = 84,
        TreasureNpcConfigId = 85,

        TreasurePosX = 90,
        TreasurePosY = 91,


        Stall_BuyPrice = 10000,//摆摊购买价格
        Stall_SellPrice=10001,//摆摊出售价格
        Stall_BuyMoJingPrice =10002,//摆摊购买魔晶价格
        Stall_SellMoJingPrice = 10003,//摆摊出售魔晶价格
        End,
    };
}