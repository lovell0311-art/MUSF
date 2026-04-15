using System.Collections.Generic;
using System.Diagnostics;

namespace ETModel
{
    public static class E_GamePropertySystem
    {
        public static bool IsChangeTypeEnemy(this E_GameProperty b_GameProperty)
        {
            if (b_GameProperty == E_GameProperty.PROP_HP
                || b_GameProperty == E_GameProperty.PROP_MP
                || b_GameProperty == E_GameProperty.PROP_SD
                || b_GameProperty == E_GameProperty.PROP_AG
                || b_GameProperty == E_GameProperty.Level)
            {
                return true;
            }

            return false;
        }
        public static bool IsChangeTypePets(this E_GameProperty b_GameProperty)
        {
            if (b_GameProperty == E_GameProperty.PROP_HP
                || b_GameProperty == E_GameProperty.PROP_MP
                || b_GameProperty == E_GameProperty.PROP_SD
                || b_GameProperty == E_GameProperty.PROP_AG
                || b_GameProperty == E_GameProperty.Level)
            {
                return true;
            }

            return false;
        }
        public static bool IsChangeTypePlayer(this E_GameProperty b_GameProperty)
        {
            if (b_GameProperty == E_GameProperty.PROP_HP
                || b_GameProperty == E_GameProperty.PROP_MP
                || b_GameProperty == E_GameProperty.PROP_SD
                || b_GameProperty == E_GameProperty.PROP_AG
                || b_GameProperty == E_GameProperty.AttackDistance
                || b_GameProperty == E_GameProperty.Level
                || b_GameProperty == E_GameProperty.OccupationLevel
                || b_GameProperty == E_GameProperty.FreePoint
                || b_GameProperty == E_GameProperty.Exprience
                || b_GameProperty == E_GameProperty.GoldCoin
                || b_GameProperty == E_GameProperty.MiracleCoin
                || b_GameProperty == E_GameProperty.YuanbaoCoin
                || b_GameProperty == E_GameProperty.PkNumber
               )
            {
                return true;
            }

            return false;
        }

    }
    public enum E_GameProperty
    {
        /// <summary>
        /// 力量
        /// </summary>
        Property_Strength = 1,
        /// <summary>
        /// 意志 智力
        /// </summary>
        Property_Willpower = 2,
        /// <summary>
        /// 敏捷
        /// </summary>
        Property_Agility = 3,
        /// <summary>
        /// 体力
        /// </summary>
        Property_BoneGas = 4,
        /// <summary>
        /// 统帅
        /// </summary>
        Property_Command = 5,


        PROP_HP = 10,
        PROP_MP = 11,
        PROP_SD = 12,
        PROP_AG = 13,
        PROP_HP_MAX = 14,
        PROP_MP_MAX = 15,
        PROP_SD_MAX = 16,
        PROP_AG_MAX = 17,
        Injury_HP = 18,
        Injury_SD = 19,


        /// <summary>
        /// 最大攻击
        /// </summary>
        MaxAtteck = 21,
        /// <summary>
        /// 最小攻击
        /// </summary>
        MinAtteck = 22,
        /// <summary>
        /// 最大魔法攻击(最大魔力)
        /// </summary>
        MaxMagicAtteck = 23,
        /// <summary>
        /// 最小魔法攻击(最大魔力)
        /// </summary>
        MinMagicAtteck = 24,
        /// <summary>
        /// 最大元素攻击(留空)
        /// </summary>
        MaxElementAtteck = 25,
        /// <summary>
        /// 最小元素攻击(留空)
        /// </summary>
        MinElementAtteck = 26,
        /// <summary>
        /// 攻击成功率
        /// </summary>
        AtteckSuccessRate = 27,
        /// <summary>
        /// PVP攻击成功率
        /// </summary>
        PVPAtteckSuccessRate = 28,
        /// <summary>
        /// 元素攻击成功率
        /// </summary>
        ElementAtteckSuccessRate = 29,
        /// <summary>
        /// PVP元素攻击成功率
        /// </summary>
        PVPElementAtteckSuccessRate = 30,
        /// <summary>
        /// 防御率
        /// </summary>
        DefenseRate = 31,
        /// <summary>
        /// 元素防御率
        /// </summary>
        ElementDefenseRate = 32,
        /// <summary>
        /// PVP防御率
        /// </summary>
        PVPDefenseRate = 33,
        /// <summary>PVP元素防御率</summary>
        PVPElementDefenseRate = 34,
        /// <summary>防御</summary>
        Defense = 35,
        /// <summary>元素防御</summary>
        ElementDefense = 36,
        /// <summary>攻速</summary>
        AttackSpeed = 37,
        /// <summary>移速</summary>
        MoveSpeed = 38,
        /// <summary>移速增量</summary>
        MoveSpeed_Increase = 39,
        /// <summary>移速减量</summary>
        MoveSpeed_Reduce = 40,

        /// <summary>攻击距离</summary>
        AttackDistance = 41,
        /// <summary>技能伤害增幅</summary>
        SkillAddition = 42,

        /// <summary>pvp 附加攻击</summary>
        PVPAttack = 43,
        /// <summary>pvp 附加防御</summary>
        PVPDefense = 44,
        /// <summary>pve 附加攻击</summary>
        PVEAttack = 45,
        /// <summary>pve 附加防御</summary>
        PVEDefense = 46,

        /// <summary>真实防御</summary>
        ReallyDefense = 47,

        /// <summary>
        /// 生命自动恢复量 7s
        /// 生命值自动增加量提高
        /// 脱离战斗后，额外增加的恢复量
        /// </summary>
        ReplyHp = 50,
        /// <summary>
        /// 魔力自动恢复量 3s
        /// 魔法值自动增加量提高
        /// 脱离战斗后，额外增加的恢复量
        /// </summary>
        ReplyMp = 51,
        /// <summary>AG自动恢复量 3s</summary>
        ReplyAG = 52,
        /// <summary>SD自动恢复量 7s</summary>
        ReplySD = 53,

        /// <summary>生命恢复万分比 脱离战斗7s</summary>
        ReplyHpRate = 54,
        /// <summary>魔力恢复万分比 脱离战斗3s</summary>
        ReplyMpRate = 55,
        /// <summary>AG恢复万分比 脱离战斗3s</summary>
        ReplyAGRate = 56,
        /// <summary>SD恢复万分比 脱离战斗7s</summary>
        ReplySDRate = 57,


        /// <summary>击杀怪物生命自动恢复量</summary>
        KillEnemyReplyHpRate = 58,
        /// <summary>击杀怪物魔法值自动恢复量</summary>
        KillEnemyReplyMpRate = 59,
        /// <summary>击杀怪物AG自动恢复量</summary>
        KillEnemyReplyAGRate = 60,
        /// <summary>击杀怪物SD自动恢复量</summary>
        KillEnemyReplySDRate = 61,


        /// <summary>生命完全恢复几率</summary>
        ReplyAllHpRate = 62,
        /// <summary>魔力完全恢复几率</summary>
        ReplyAllMpRate = 63,
        /// <summary>AG完全恢复几率</summary>
        ReplyAllAGRate = 64,
        /// <summary>SD完全恢复几率</summary>
        ReplyAllSdRate = 65,

        /// <summary>受到伤害时,Hp完全恢复几率</summary>
        Injury_ReplyAllHpRate = 66,
        /// <summary>受到伤害时,Hp完全恢复几率</summary>
        Injury_ReplyAllMpRate = 67,
        /// <summary>攻击时,SD完全恢复几率</summary>
        Attack_ReplyAllSdRate = 68,

        /// <summary>
        /// 生命力吸收量
        /// 攻击敌人时每次攻击成功，以50%的概率生命力恢复{0:F}。
        /// </summary>
        HpAbsorbRate = 80,
        /// <summary>
        /// sd吸收量
        /// 攻击敌人时每次攻击成功，以50%的概率sd恢复{0:F}。
        /// </summary>
        SdAbsorbRate = 81,

        /// <summary>魔法值使用减少率 万分比</summary>
        MpConsumeRate_Reduce = 82,
        /// <summary>AG使用减少率</summary>
        AgConsumeRate_Reduce = 83,


        /// <summary>攻击时无视防御概率 万分比</summary>
        AttackIgnoreDefenseRate = 84,
        /// <summary>反弹几率 万分比</summary>
        ReboundRate = 85,
        /// <summary>幸运一击伤害增加量</summary>
        LucklyAttackHurtValueIncrease = 86,
        /// <summary>卓越一击伤害增加量</summary>
        ExcellentAttackHurtValueIncrease = 87,
        /// <summary>伤害提高率 万分比</summary>
        InjuryValueRate_Increase = 88,
        /// <summary>伤害减少率 万分比</summary>
        InjuryValueRate_Reduce = 89,
        /// <summary>伤害减少量</summary>
        InjuryValue_Reduce = 90,
        /// <summary>伤害反射率</summary>
        BackInjuryRate = 91,
        /// <summary>伤害吸收率</summary>
        HurtValueAbsorbRate = 92,

        /// <summary>被击时Sd比率 万分比</summary>
        HitSdRate = 93,
        /// <summary>攻击时Sd比率 万分比</summary>
        AttackSdRate = 94,
        /// <summary>攻击时无视Sd概率 万分比</summary>
        SDAttackIgnoreRate = 95,
        /// <summary>束缚几率</summary>
        ShacklesRate = 96,
        /// <summary>束缚抵抗几率</summary>
        ShacklesResistanceRate = 97,
        /// <summary>盾牌伤害吸收量</summary>
        ShieldHurtAbsorb = 98,
        /// <summary>防盾几率 万分比</summary>
        DefenseShieldRate = 99,
        /// <summary>获得金币增加率</summary>   
        AddGoldCoinRate_Increase = 100,
        /// <summary>魔杖魔力提升百分比</summary>
        MagicRate_Increase = 101,
        /// <summary>格挡几率 万分比</summary>
        GridBlockRate = 102,
        /// <summary>守护盾几率 万分比</summary>
        GuardShieldRate = 103,
        /// <summary>
        /// 宠物部分属性 攻击加成百分比
        /// </summary>
        AttackBonus = 104,
        /// <summary>
        /// 防御加成百分比
        /// </summary>
        DefenseBonus = 105,
        /// <summary>
        /// 攻速加成百分比
        /// </summary>
        AttackSpeedBonus = 106,
        /// <summary>
        /// 防御率加成百分比
        /// </summary>
        DefenseRateBonus = 107,
        /// <summary>
        /// 生命值加成百分比
        /// </summary>
        HealthBonus = 108,
        /// <summary>
        /// 宠物蓝量加成百分比
        /// </summary>
        MagicBonus = 109,
        /// <summary>
        /// 技能攻击力
        /// </summary>
        SkillAttack = 110,
        /// <summary>
        /// 宠物伤害吸收，百分比
        /// </summary>
        PetsDamageAbsorption,
        /// <summary>
        /// 最大诅咒攻击
        /// </summary>
        MaxDamnationAtteck,
        /// <summary>
        /// 最小诅咒攻击
        /// </summary>
        MinDamnationAtteck,
        /// <summary>书诅咒提升百分比</summary>
        DamnationRate_Increase,

        ///////////////////////////百分比属性 2级属性/////////////////////
        PROP_HP_MAXPct,
        PROP_MP_MAXPct,
        PROP_SD_MAXPct,
        PROP_AG_MAXPct,
        MaxAtteckPct,
        MinAtteckPct,
        MaxMagicAtteckPct,
        MinMagicAtteckPct,
        MaxElementAtteckPct,
        MinElementAtteckPct,
        AtteckSuccessRatePct,
        PVPAtteckSuccessRatePct,
        ElementAtteckSuccessRatePct,
        PVPElementAtteckSuccessRatePct,
        DefenseRatePct,
        ElementDefenseRatePct,
        PVPDefenseRatePct,
        PVPElementDefenseRatePct,
        DefensePct,
        ElementDefensePct,
        MaxDamnationAtteckPct,
        MinDamnationAtteckPct,
        ///////////////////////////百分比属性/////////////////////


        ///////////////////////////物品词条属性/////////////////////
        /// <summary>击杀怪物恢复自己最大生命/8 HpMax/8*Value</summary>
        KillMonsterReplyHp_8,
        /// <summary>击杀怪物恢复自己最大魔法值/8 MpMax/8*Value</summary>
        KillMonsterReplyMp_8,
        /// <summary>
        /// 伤害吸收百分比 守护
        /// </summary>
        DamageAbsPct_Guard,
        /// <summary>
        /// 伤害吸收百分比 坐骑
        /// </summary>
        DamageAbsPct_Mounts,
        /// <summary>
        /// 伤害吸收百分比 翅膀
        /// </summary>
        DamageAbsPct_Wing,
        ///////////////////////////物品词条属性/////////////////////


        ///////////////////////////大师属性/////////////////////
        /// <summary>
        /// 装备中的武器和防具耐久下降速度减慢 百分比
        /// </summary>
        MpsDownDur1,
        /// <summary>
        /// 装备中的首饰（项链，戒指）耐久下降速度减慢 百分比
        /// </summary>
        MpsDownDur2,
        /// <summary>
        /// （小恶魔、小天使、兽角、彩云兽、炎狼兽之角）耐久下降速度减慢 百分比
        /// </summary>
        MpsDownDur3,
        /// <summary>
        /// 宠物(黑王马、天鹰)的生命力减少速度 百分比
        /// </summary>
        MpsPetDurDownSpeed,
        /// <summary>
        /// 整套防御力增加
        /// </summary>
        SpecialDefenseRate,
        ///////////////////////////大师属性/////////////////////



        ///////////////////////////效果及属性/////////////////////
        /// <summary>啥都没触发</summary>
        NullAttack,
        /// <summary>双倍伤害概率 万分比</summary>
        InjuryValueRate_2,
        /// <summary>三倍伤害概率 万分比</summary>
        InjuryValueRate_3,
        /// <summary>幸运一击概率  最大伤害 万分比</summary>
        LucklyAttackRate,
        /// <summary>卓越一击概率  1.3倍 万分比</summary>
        ExcellentAttackRate,
        ///////////////////////////效果及属性/////////////////////


        /// <summary>
        /// 冰抗性
        /// </summary>
        IceResistance,
        /// <summary>
        /// 毒抗性
        /// </summary>
        CurseResistance,
        /// <summary>
        /// 火抗性
        /// </summary>
        FireResistance,
        /// <summary>
        /// 雷抗性
        /// </summary>
        ThunderResistance,

        /// <summary>
        /// 等级
        /// </summary>
        Level = 200,
        /// <summary>
        /// 职业等级
        /// </summary>
        OccupationLevel = 201,
        /// <summary>
        /// 联盟名字
        /// </summary>
        UnionName = 202,
        /// <summary>
        /// 自由点数
        /// </summary>
        FreePoint = 203,
        /// <summary>
        /// 当前经验
        /// </summary>
        Exprience = 204,
        /// <summary>
        /// 获取经验
        /// </summary>
        ExprienceDrop = 205,

        /// <summary>
        /// 服务器时间戳
        /// </summary>
        ServerTime = 206,

        /// <summary> 
        /// 金币
        /// </summary>
        GoldCoin = 207,
        /// <summary>
        /// 金币变化值
        /// </summary>
        GoldCoinChange = 208,
        /// <summary>
        /// 经验加成百分比
        /// </summary>
        ExperienceBonus = 209,
        /// <summary>
        /// 爆率加成百分比
        /// </summary>
        ExplosionRate = 230,
        /// <summary>
        /// 道具回收金币加成百分比
        /// </summary>
        GoldCoinMarkup = 231,
        /// <summary>
        /// 奇迹币
        /// </summary>
        MiracleCoin = 232,
        /// <summary>
        /// 奇迹币变化值
        /// </summary>
        MiracleChange = 233,
        /// <summary>
        /// 元宝
        /// </summary>
        YuanbaoCoin = 234,
        /// <summary>
        /// 元宝变化值
        /// </summary>
        YuanbaoChange = 235,
        /// <summary>
        /// pk模式 0:和平 1:全体 2:友方
        /// </summary>
        PlayerKillingMedel = 236,
        /// <summary>
        /// pk 点数
        /// </summary>
        PkNumber = 237,

        /// <summary>
        /// 天鹰最大攻击
        /// </summary>
        TianYingMaxAtteck = 238,
        /// <summary>
        /// 天鹰最小攻击
        /// </summary>
        TianYingMinAtteck = 239,
        /// <summary>
        /// 宠物提供的防护罩
        /// </summary>
        FangYuHuZhao =240,



        /// <summary>
        /// 宠物部分属性 攻击加成百分比 天鹰专用
        /// </summary>
        AttackBonus2,



        #region 镶嵌属性词条万分比 Embed
        /// <summary>
        /// 每20等级攻击力/魔力增加{0:G}
        /// </summary>
        EmbedAttack20,
        /// <summary>
        /// 攻击速度提升{0:G}
        /// </summary>
        EmbedAttackSpeed,
        /// <summary>
        /// 最大攻击力/魔法攻击力提升{0:G}
        /// </summary>
        EmbedAttackMaxUp,
        /// <summary>
        /// 最小攻击力/魔法攻击力提升{0:G}
        /// </summary>
        EmbedAttackMinUp,
        /// <summary>
        /// 攻击力/魔法攻击力增加{0:G}
        /// </summary>
        EmbedAttack,
        /// <summary>
        /// AG消耗量减少{0:P}
        /// </summary>
        EmbedAGDecrease,
        /// <summary>
        /// 防御成功率提升{0:P}
        /// </summary>
        EmbedDefRateUp,
        /// <summary>
        /// 防御力提升{0:G}
        /// </summary>
        EmbedDefUp,
        /// <summary>
        /// 盾牌防御力提升{0:P}
        /// </summary>
        EmbedShieldDefUp,
        /// <summary>
        /// 伤害减少{0:P}
        /// </summary>
        EmbedHarmReduction,
        /// <summary>
        /// 伤害反射{0:P}
        /// </summary>
        EmbedNociceptiveReflex,
        /// <summary>
        /// 杀怪增加生命值{0:G}
        /// </summary>
        EmbedIncreaseLife,
        /// <summary>
        /// 杀怪增加魔法值{0:G}
        /// </summary>
        EmbedIncreaseMagic,
        /// <summary>
        /// 技能攻击力提升{0:G}
        /// </summary>
        EmbedSkillAttack,
        /// <summary>
        /// 攻击成功率提升{0:G}
        /// </summary>
        EmbedAttackRateUp,
        /// <summary>
        /// 物品耐久度强化{0:P}
        /// </summary>
        EmbedDurabilityUp,
        /// <summary>
        /// 生命值自动恢复提升{0:G}
        /// </summary>
        EmbedHpSelfRecovery,
        /// <summary>
        /// 最大生命值提升{0:G}
        /// </summary>
        EmbedMaxHPUp,
        /// <summary>
        /// 最大魔法值提升{0:G}
        /// </summary>
        EmbedMaxHMUp,
        /// <summary>
        /// 魔法值自动恢复提升{0:G}
        /// </summary>
        EmbedHmSelfRecovery,
        /// <summary>
        /// 最大AG提升{0:G}
        /// </summary>
        EmbedMaxAGUp,
        /// <summary>
        /// AG恢复量提升{0:G}
        /// </summary>
        EmbedAGRecoveryUp,
        /// <summary>
        /// 卓越一击攻击力提升{0:G}
        /// </summary>
        EmbedGreatShotAttack,
        /// <summary>
        /// 卓越一击概率提升{0:P}
        /// </summary>
        EmbedGreatShotRateUp,
        /// <summary>
        /// 幸运一击攻击力提升{0:G}
        /// </summary>
        EmbedLuckyStrokeAttack,
        /// <summary>
        /// 幸运一击概率提升{0:P}
        /// </summary>
        EmbedLuckyStrokeRateUp,
        /// <summary>
        /// 力量提升{0:G}
        /// </summary>
        EmbedStrengthUP,
        /// <summary>
        /// 敏捷提升{0:G}
        /// </summary>
        EmbedAgilityUP,
        /// <summary>
        /// 体力提升{0:G}
        /// </summary>
        EmbedStaminaUP,
        /// <summary>
        /// 智力提升{0:G}
        /// </summary>
        EmbedIntelligenceUP,
        #endregion

        /// <summary>
        /// 减少吸收
        /// </summary>
        IgnoreAbsorbRate,
        /// <summary>攻击时无视吸收概率 万分比</summary>
        AttackIgnoreAbsorbRate,

        /// <summary>
        /// 格斗家近战攻击力
        /// </summary>
        AdvanceAttackPower,
        /// <summary>
        /// 范围攻击力
        /// </summary>
        RangeAttack,
        /// <summary>
        /// 格斗家神兽攻击力
        /// </summary>
        SacredBeast,
        /// <summary>
        /// 梦幻骑士惩处攻击力
        /// </summary>
        DreamRiderPenalize,
        /// <summary>
        /// 梦幻骑士激怒攻击力
        /// </summary>
        DreamRiderIrritate,
        /// <summary>
        /// 宠物伤害吸收
        /// </summary>
        DamageAbsPct_Pets,
        /// <summary>
        /// 减少对方的伤害减少属性万分比
        /// </summary>
        DisregardHarmReductionPct,
        /// <summary>
        /// 战盟积分
        /// </summary>
        AllianceScoreChange,
        /// <summary>
        /// 属性最大数量
        /// </summary>
        GamePropertyMax,

    }
}