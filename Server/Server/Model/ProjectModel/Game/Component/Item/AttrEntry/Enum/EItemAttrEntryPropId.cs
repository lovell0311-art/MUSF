using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    // 物品词条属性
    public enum EItemAttrEntryPropId
    {
        /// <summary>
        /// 卓越一击概率 增加
        /// </summary>
        AddExcellentAttackRate = 1,
        /// <summary>
        /// 攻击力(魔法)增加 +角色等级/?
        /// </summary>
        AddAttackDamageByLevel_20 = 2,
        /// <summary>
        /// 攻击力(魔法)增加 百分比
        /// </summary>
        AddAttackDamagePct = 3,
        /// <summary>
        /// 攻击(魔法)速度增加
        /// </summary>
        AddAttackSpeed = 4,
        /// <summary>
        /// 杀死怪物时所获（生命值/8）
        /// </summary>
        KillMonsterReplyHp_8 = 5,
        /// <summary>
        /// 杀死怪物时所获（魔法值/8）
        /// </summary>
        KillMonsterReplyMp_8 = 6,
        /// <summary>
        /// 最大生命值增加 百分比
        /// </summary>
        AddMaxHpPct = 7,
        /// <summary>
        /// 最大魔法值增加 百分比
        /// </summary>
        AddMaxMpPct = 8,
        /// <summary>
        /// 伤害减少 百分比
        /// </summary>
        AddDamageMinusPct = 9,
        /// <summary>
        /// 伤害反射 百分比
        /// </summary>
        AddDamageReflectPct = 10,
        /// <summary>
        /// 防御成功率 百分比
        /// </summary>
        AddDefenseSuccessRatePct = 11,
        /// <summary>
        /// 杀死怪物时所获金增加 百分比
        /// </summary>
        AddkillMonsterReveiceGoldPct = 12,
        /// <summary>
        /// 最小攻击力提高
        /// </summary>
        AddMinAttackDamage = 13,
        /// <summary>
        /// 最大攻击力提高
        /// </summary>
        AddMaxAttackDamage = 14,
        /// <summary>
        /// 装备这件装备需要的力量减少
        /// </summary>
        ReducedStrengthRequire = 15,
        /// <summary>
        /// 装备这件装备需要的敏捷减少
        /// </summary>
        ReducedDexterityRequire = 16,
        /// <summary>
        /// 最小/最大攻击力提高
        /// </summary>
        AddAttackDamage = 17,
        /// <summary>
        /// 加重伤害,幸运一击/卓越一击 触发后增加的伤害 （扣除防御前添加）
        /// </summary>
        AddCriticalDamage = 18,
        /// <summary>
        /// 技能攻击力提高 （扣除防御前添加）
        /// </summary>
        AddSkillAttack = 19,
        /// <summary>
        /// pvp攻击成功率提高
        /// </summary>
        AddAttackSuccessRatePVP = 20,
        /// <summary>
        /// 扣除目标sdRate 让目标少扣点sd
        /// sdRate -= self.DecreaseSDRate
        /// </summary>
        DecreaseSDRate = 21,
        /// <summary>
        /// 攻击时无视 SD 概率提高
        /// pvp时有多少概况无视对方sd
        /// </summary>
        AddIgnoreSDRate = 22,
        /// <summary>
        /// 魔法伤害提高
        /// </summary>
        AddMagicPower = 23,
        /// <summary>
        /// 防御力提高
        /// </summary>
        AddDefense = 24,
        /// <summary>
        /// 最大AG提高 百分比
        /// </summary>
        AddMaxAGPct = 25,
        /// <summary>
        /// 生命值自动增加量提高
        /// 脱离战斗后，额外增加的恢复量
        /// </summary>
        AddRefillHP = 27,
        /// <summary>
        /// 魔法值自动增加量提高
        /// 脱离战斗后，额外增加的恢复量
        /// </summary>
        AddRefillMP = 28,
        /// <summary>
        /// (PvP)防御成功率提高
        /// </summary>
        AddDefenseSuccessRatePvP = 29,
        /// <summary>
        /// 增加自己sdRate 让自己多扣点sd
        /// sdRate += self.AddSDRate
        /// </summary>
        AddSDRate = 31,
        /// <summary>
        /// 防御成功率
        /// </summary>
        AddDefenseSuccessRate = 32,
        /// <summary>
        /// 生命值自动增加百分比 提高
        /// 脱离战斗后，恢复的百分比提高
        /// </summary>
        AddRefillHPPct = 33,
        /// <summary>
        /// 攻击力/魔法攻击力/诅咒能力增加
        /// </summary>
        AddAllAttackDamage = 34,
        /// <summary>
        /// 攻击力，魔力，诅咒提高40%
        /// </summary>
        AddAllAttackDamagePct = 35,
        /// <summary>
        /// 最大生命值增加
        /// </summary>
        AddMaxHp = 36,
        /// <summary>
        /// 伤害吸收百分比 守护
        /// </summary>
        DamageAbsPct_Guard = 37,
        /// <summary>
        /// 伤害吸收百分比 坐骑
        /// </summary>
        DamageAbsPct_Mounts = 38,
        /// <summary>
        /// 伤害提高百分比
        /// </summary>
        DamageIncreasePct = 39,
        /// <summary>
        /// 伤害吸收百分比 翅膀
        /// </summary>
        DamageAbsPct_Wing = 40,
        /// <summary>
        /// 添加真实防御(基础防御，不会被负面buff影响)
        /// </summary>
        AddReallyDefense = 41,
        /// <summary>
        /// 无视目标防御概率(攻击时无视防御概率)
        /// </summary>
        AddAttackIgnoreDefenseRate = 42,
        /// <summary>
        /// 伤害减少量
        /// </summary>
        AddDamageMinus = 43,

        /// <summary>
        /// 最大魔法值增加
        /// </summary>
        AddMaxMp = 45,
        /// <summary>
        /// 最大Ag值增加
        /// </summary>
        AddMaxAg = 46,
        /// <summary>
        /// 最大Sd值增加
        /// </summary>
        AddMaxSd = 47,


        /// <summary>
        /// 幸运一击概率 增加
        /// </summary>
        AddLuckyAttackRate = 55,
        /// <summary>
        /// 双倍伤害概率 增加
        /// </summary>
        AddDoubleAttackRate = 56,
        /// <summary>
        /// 三倍伤害概率 增加
        /// </summary>
        AddTripleAttackRate = 57,
        /// <summary>
        /// 宠物伤害吸收
        /// </summary>
        DamageAbsPct_Pets = 58,


        /// <summary>
        /// 卓越一击 触发后增加的伤害 （扣除防御前添加）
        /// </summary>
        AddExcellentAttackDamage = 60,
        /// <summary>
        /// 幸运一击 触发后增加的伤害 （扣除防御前添加）
        /// </summary>
        AddLuckyAttackDamage = 61,
        /// <summary>
        /// [未实现]双倍伤害 触发后增加的伤害 （扣除防御前添加）
        /// </summary>
        AddDoubleAttackDamage = 62,
        /// <summary>
        /// [未实现]三倍伤害 触发后增加的伤害 （扣除防御前添加）
        /// </summary>
        AddTripleAttackDamage = 63,
        /// <summary>
        /// 属性伤害增加 卓越一击、幸运一击、双倍伤害、三倍伤害
        /// </summary>
        AddAllEffectAttackDamage = 65,


        /// <summary>
        /// 魔力增加百分比(和法杖的魔力相同)
        /// </summary>
        AddMagicPct = 75,
        /// <summary>
        /// 攻击力/魔力 增加
        /// </summary>
        AddAttackMagicDamage = 76,
        /// <summary>
        /// 魔力/诅咒力 增加
        /// </summary>
        AddMagicCurseDamage = 77,
        /// <summary>
        /// 攻击成功率增加
        /// </summary>
        AddAttackSuccessRate = 80,
        /// <summary>
        /// 反弹攻击力概率 增加
        /// </summary>
        AddReboundAttackRate = 81,
        /// <summary>
        /// 完全回复生命概率 增加
        /// </summary>
        AddInjury_ReplyAllHpRate = 82,
        /// <summary>
        /// 完全回复魔法概率 增加
        /// </summary>
        AddInjury_ReplyAllMpRate = 83,
        

        /// <summary>
        /// 力量
        /// </summary>
        AddStrength = 101,
        /// <summary>
        /// 意志 智力
        /// </summary>
        AddWillpower = 102,
        /// <summary>
        /// 敏捷
        /// </summary>
        AddAgility = 103,
        /// <summary>
        /// 体力
        /// </summary>
        AddBoneGas = 104,
        /// <summary>
        /// 统帅
        /// </summary>
        AddCommand = 105,
        /// <summary>
        /// 提高所有属性
        /// </summary>
        AddAllAttr = 106,

        /// <summary>
        /// 冰抗
        /// </summary>
        AddIceResistance = 200,
        /// <summary>
        /// 毒抗
        /// </summary>
        AddPoisonResistance = 201,
        /// <summary>
        /// 雷抗
        /// </summary>
        AddLightningResistance = 202,
        /// <summary>
        /// 火抗
        /// </summary>
        AddFireResistance = 203,
        /// <summary>
        /// [没实现]土抗
        /// </summary>
        AddEarthResistance = 204,
        /// <summary>
        /// [没实现]风抗
        /// </summary>
        AddWindResistance = 205,
        /// <summary>
        /// [没实现]水抗
        /// </summary>
        AddWaterResistance = 206,


        /// <summary>
        /// 增加杀怪经验百分比
        /// </summary>
        AddKillMonsterExpPct = 300,


        /// <summary>
        /// 双手剑佩戴时伤害增加百分比
        /// </summary>
        DamageIncreasePctWhenEquipTwoHandSword = 400,


        /// <summary>
        /// 减少吸收百分比
        /// </summary>
        IgnoreAbsorbRatePct = 500,
        /// <summary>
        /// 无视吸收百分比
        /// </summary>
        AttackIgnoreAbsorbRatePct = 501,
        /// <summary>
        /// 恢复玩家生命
        /// </summary>
        RestorePlayerHealth = 502,
        /// <summary>
        /// 恢复玩家生命百分比
        /// </summary>
        RestorePlayerHealthPct = 503,
        /// <summary>
        /// 减少对方的伤害减少属性
        /// </summary>
        DisregardHarmReduction = 504,
        /// <summary>
        /// 防御力提高百分比
        /// </summary>
        AddDefensePct = 2401,

        /// <summary>
        /// 血脉最小攻击力/魔力/诅咒
        /// </summary>
        BloodMinATK = 600,
        /// <summary>
        /// 击杀怪物增加金币掉落
        /// </summary>
        BloodGoldIncrease = 601,
        /// <summary>
        /// 双倍伤害
        /// </summary>
        BloodDoubleDamage = 602,
        /// <summary>
        /// 最大魔法值增加
        /// </summary>
        BloodMaxMagic = 603,
        /// <summary>
        /// 技能攻击力提高
        /// </summary>
        BloodSkill = 604,
        /// <summary>
        /// 防御成功率增加
        /// </summary>
        BloodDefensePct = 605,
        /// <summary>
        /// 生命力自动恢复提高
        /// </summary>
        BloodHpAutoRecover = 606,
        /// <summary>
        /// 三倍伤害几率
        /// </summary>
        BloodTripleInjury = 607,
        /// <summary>
        /// 反弹几率
        /// </summary>
        BloodRebound = 608,
        /// <summary>
        /// 幸运一击伤害几率
        /// </summary>
        BloddLuckyHit = 609,
        /// <summary>
        /// 卓越一击伤害几率
        /// </summary>
        BloodGreatShot = 610,
        /// <summary>
        /// 幸运一击伤害增加量
        /// </summary>
        BloodLuckyHitAdd = 611,
        /// <summary>
        /// 卓越一击伤害增加量
        /// </summary>
        BloodGreatShotAdd = 612,
        /// <summary>
        /// 魔力自动恢复量
        /// </summary>
        BloodMPAutoRecover = 613,
        /// <summary>
        /// 魔力恢复几率
        /// </summary>
        BloodMagicAutoPct = 614,
        /// <summary>
        /// AG自动恢复量
        /// </summary>
        BloodAGAutoAdd = 615,
        /// <summary>
        /// AG恢复几率
        /// </summary>
        BloodAgAutoPct = 616,
        /// <summary>
        /// SD自动恢复量
        /// </summary>
        BloodSDAutoAdd = 617,
        /// <summary>
        /// SD恢复几率
        /// </summary>
        BloodSDAutoPct = 618,
        /// <summary>
        /// 攻击时SD比率
        /// </summary>
        BloodAtAttackTimeSDPct = 619,
        /// <summary>
        /// 束缚几率
        /// </summary>
        BloodBoundProbability = 620,
        /// <summary>
        /// 抵抗束缚几率
        /// </summary>
        BloodDKBoundProbability = 621,
        /// <summary>
        /// 盾牌伤害吸收量
        /// </summary>
        BloodShieldHurtAbsorb = 622,
        /// <summary>
        /// 防盾几率
        /// </summary>
        BloodDefenseShieldRate = 623,
        /// <summary>
        /// 削弱对方减伤率
        /// </summary>
        BloodDisregardHarmReduction = 624,
        /// <summary>
        /// 击杀怪物生命值自动恢复量
        /// </summary>
        BloodKillAutoHp = 625,
        /// <summary>
        /// 击杀怪物魔法值自动恢复量
        /// </summary>
        BloodKillAutoMp = 626,
        /// <summary>
        /// 击杀怪物AG自动恢复量
        /// </summary>
        BloodKillAutoAg = 627,
        /// <summary>
        /// 击杀怪物SD自动恢复量
        /// </summary>
        BloodKillAutoSD = 628,
        /// <summary>
        /// 受到伤害时,HP完全恢复几率
        /// </summary>
        BloodAutoHpOver = 629,
        /// <summary>
        /// 血脉最大攻击力/魔力/诅咒
        /// </summary>
        BloodMaxATK = 630,
    }
}
