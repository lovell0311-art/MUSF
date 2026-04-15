using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 翅膀附加属性
    /// </summary>
    public enum E_ItemWingsOpt
    {
        ATTACK_IGNORES_DEFENCE_PROBABILITY_3 = 1 << 0,  // 无视敌人防御力的概率增加 3 5 6 7%
        ATTACK_IGNORES_DEFENCE_PROBABILITY_5 = 1 << 1,  // 无视敌人防御力的概率增加 5%
        HP_50_LEVEL_5 = 1 << 2, // 最高生命值 +50(+翅膀等级*5)
        MP_50_LEVEL_5 = 1 << 3, // 最高魔法值 +50(+翅膀等级*5)
        ATTR_COM_10_LEVEL_5 = 1 << 4,   // 统率 +10(+翅膀等级*5)
        CHANCE_TO_FULLY_RECOVER_MANA_PCT_3 = 1 << 5, // 魔法值完全恢复概率增加	3 5 6 7%
        CHANCE_OF_FULL_RECOVERY_OF_LIFE_PCT_3 = 1 << 6, // 生命值完全恢复概率增加	3 5 6 7%
        REBOUND_ATTACK_PCT_3 = 1 << 7, // 反弹攻击力概率增加		3 5 6 7%
        EXCELLENT_HIT_PCT_3 = 1 << 8, // 卓越伤害概率增加		3 5 7 10%
        DOUBLE_DAMAGE_PCT_3 = 1 << 9, // 双倍伤害概率增加		3 5 6 7%
        ATTACK_SPEED_5 = 1 << 10,// 攻击(魔法)速度增加		5 7 9 12
        STR_10 = 1 << 11,// 力量增加				10 25 40 65
        VIT_10 = 1 << 12,// 体力增加				10 25 40 65
        ENE_10 = 1 << 13,// 智力增加				10 25 40 65
        AGI_10 = 1 << 14,// 敏捷增加				10 25 40 65
    };
}