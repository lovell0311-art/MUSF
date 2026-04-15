using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    /// <summary>
    /// 大师技能 1-1000
    /// </summary>
    public enum E_BattleMaster_Id
    {
        #region 法师
        /// <summary>
        /// 减少耐久1
        /// </summary>
        Reduce_Durable1_1 = 1 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 对人防御率提高
        /// </summary>
        PVPDefenseRate2 = 2 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 提高最大SD
        /// </summary>
        IncreaseSDMax3 = 3 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 自动魔力恢复力增加
        /// </summary>
        RecoveryMagic4 = 4 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 减少耐久2
        /// </summary>
        Reduce_Durable2_5 = 5 + BattleMasteryConstData.SpellId,

        /// <summary>
        /// SD恢复速度提高
        /// </summary>
        RecoverySD6 = 6 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 自动生命恢复力增加
        /// </summary>
        RecoveryHp7 = 7 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 防御力增加
        /// </summary>
        IncreaseDefense8 = 8 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 属性防御力增加
        /// </summary>
        IncreaseElementDefense9 = 9 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 自动AG恢复力增加
        /// </summary>
        RecoveryAG10 = 10 + BattleMasteryConstData.SpellId,

        /// <summary>
        /// 减少耐久3
        /// </summary>
        Reduce_Durable3_11 = 11 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 防御成功率提高
        /// </summary>
        DefenseRate12 = 12 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 整套防御力增加
        /// </summary>
        SpecialDefenseRate13 = 13 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 反伤
        /// </summary>
        BackInjureRate14 = 14 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 智力属性增加
        /// </summary>
        IncreaseWillpower15 = 15 + BattleMasteryConstData.SpellId,

        /// <summary>
        /// 体力属性增加
        /// </summary>
        IncreaseBoneGas16 = 16 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 敏捷属性增加
        /// </summary>
        IncreaseAgility17 = 17 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 力量属性增加
        /// </summary>
        IncreaseStrength18 = 18 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 时空之翼防御强化
        /// </summary>
        Equipment_Defense_ShiKongZhiYi_19 = 19 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 守护盾
        /// </summary>
        GuardianShield20 = 20 + BattleMasteryConstData.SpellId,

        /// <summary>
        /// 时空之翼攻击强化
        /// </summary>
        Equipment_Attack_ShiKongZhiYi_21 = 21 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 钢铁铠甲
        /// </summary>
        SteelCarapace_22 = 22 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 防盾
        /// </summary>
        DefensiveShield_23 = 23 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 攻击成功率 （PVE）
        /// </summary>
        IncreaseAtteckSuccessRate24 = 24 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 火龙强化
        /// </summary>
        Skill_HuoLongStrengthen25 = 25 + BattleMasteryConstData.SpellId,


        /// <summary>
        /// 掌心雷强化
        /// </summary>
        Skill_ZhuangXinLeiStrengthen26 = 26 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 法神附体强化
        /// </summary>
        Skill_FaShenFuTiStrengthen27 = 27 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 毁灭烈焰强化
        /// </summary>
        Skill_HuiMieLieYanStrengthen28 = 28 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 爆炎强化
        /// </summary>
        Skill_BaoYanStrengthen29 = 29 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 法神附体精通
        /// </summary>
        Skill_FaShenFuTiMaster30 = 30 + BattleMasteryConstData.SpellId,

        /// <summary>
        /// 黑龙波强化
        /// </summary>
        Skill_HeiLongBoStrengthen31 = 31 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 魔法精通
        /// </summary>
        MagicMaster32 = 32 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 最大生命增加
        /// </summary>
        IncreaseHpMax33 = 33 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 毒炎强化
        /// </summary>
        Skill_DuYanStrengthen34 = 34 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 地狱火强化
        /// </summary>
        Skill_DiYuHuoStrengthen35 = 35 + BattleMasteryConstData.SpellId,


        /// <summary>
        /// 最大魔力增加
        /// </summary>
        IncreaseMagicPowerMax36 = 36 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 冰封强化
        /// </summary>
        Skill_BingFengStrengthen37 = 37 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 陨石强化
        /// </summary>
        Skill_YunShiStrengthen38 = 38 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 最大AG增加
        /// </summary>
        IncreaseAGMax39 = 39 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 暴风雪强化
        /// </summary>
        Skill_BaoFengXueStrengthen40 = 40 + BattleMasteryConstData.SpellId,

        /// <summary>
        /// 星辰一怒强化
        /// </summary>
        Skill_XingCenYiNuStrengthen41 = 41 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 最大生命增加 33,42
        /// </summary>
        IncreaseHpMax42 = 42 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 地牢术
        /// </summary>
        Skill_DiLaoShu43 = 43 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 一露斩
        /// </summary>
        Skill_YiLuZhan44 = 44 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 地牢术强化
        /// </summary>
        Skill_DiLaoShuStrengthen45 = 45 + BattleMasteryConstData.SpellId,


        /// <summary>
        /// PVP攻击成功率
        /// </summary>
        PVPAtteckSuccessRate46 = 46 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 单手杖强化
        /// </summary>
        DanShouJian_Strengthening47 = 47 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 双手杖强化
        /// </summary>
        ShuangShouJian_Strengthening48 = 48 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 盾牌强化
        /// </summary>
        Shield_Strengthening49 = 49 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 单手杖精通
        /// </summary>
        DanShouJian_Master50 = 50 + BattleMasteryConstData.SpellId,

        /// <summary>
        /// 双手杖精通
        /// </summary>
        ShuangShouJian_Master51 = 51 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 盾牌精通
        /// </summary>
        Shield_Master52 = 52 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 守护之魂强化
        /// </summary>
        Skill_ShouHuZhiHunStrengthen53 = 53 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 魔力减少 技能蓝耗减少
        /// </summary>
        ReduceConsumeMp54 = 54 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// Sd 回复   杀怪后
        /// </summary>
        KillRecoverySD55 = 55 + BattleMasteryConstData.SpellId,

        /// <summary>
        /// hp 回复  杀怪后
        /// </summary>
        KillRecoveryHp56 = 56 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 守护之魂熟练
        /// </summary>
        Skill_ShouHuZhiHunSkilled57 = 57 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 最小魔力提高
        /// </summary>
        IncreaseMinMagic58 = 58 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 杀怪时魔力回复
        /// </summary>
        KillRecoveryMp59 = 59 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 守护之魂精通
        /// </summary>
        Skill_ShouHuZhiHunMaster60 = 60 + BattleMasteryConstData.SpellId,

        /// <summary>
        /// 最大魔力提高
        /// </summary>
        IncreaseMaxMagic61 = 61 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 幸运一击几率提高
        /// </summary>
        IncreaseXingYunYiJiRate62 = 62 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 受伤魔力完全回复
        /// </summary>
        Injure_RecoveryAllMp63 = 63 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 受伤生命力完全回复
        /// </summary>
        Injure_RecoveryAllHp64 = 64 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 生命吸收
        /// </summary>
        AttackAbsorbHp65 = 65 + BattleMasteryConstData.SpellId,

        /// <summary>
        /// 卓越一击几率提高
        /// </summary>
        IncreaseZuoYueYiJiRate66 = 66 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 魔法领域
        /// </summary>
        MagicField67 = 67 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// sd 完全回复
        /// </summary>
        Attack_RecoveryAllSD68 = 68 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 双倍一击几率提高
        /// </summary>
        IncreaseShuangBeiYiJiRate69 = 69 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 攻击时概率sd恢复
        /// </summary>
        Attack_RecoverySD70 = 70 + BattleMasteryConstData.SpellId,
        /// <summary>
        /// 无视防御几率提高
        /// </summary>
        IgnoreDefenseRate71 = 71 + BattleMasteryConstData.SpellId,

        #endregion

        #region 剑士
        /// <summary>
        /// 减少耐久1
        /// </summary>
        Reduce_Durable1_101 = 1 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 对人防御率提高
        /// </summary>
        PVPDefenseRate102 = 2 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 提高最大SD
        /// </summary>
        IncreaseSDMax103 = 3 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 自动魔力恢复力增加
        /// </summary>
        RecoveryMagic104 = 4 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 减少耐久2
        /// </summary>
        Reduce_Durable2_105 = 5 + BattleMasteryConstData.SwordsmanId,

        /// <summary>
        /// SD恢复速度提高
        /// </summary>
        RecoverySD106 = 6 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 自动生命恢复力增加
        /// </summary>
        RecoveryHp107 = 7 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 防御力增加
        /// </summary>
        IncreaseDefense108 = 8 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 属性防御力增加
        /// </summary>
        IncreaseElementDefense109 = 9 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 自动AG恢复力增加
        /// </summary>
        RecoveryAG110 = 10 + BattleMasteryConstData.SwordsmanId,

        /// <summary>
        /// 减少耐久3
        /// </summary>
        Reduce_Durable3_111 = 11 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 防御成功率提高
        /// </summary>
        DefenseRate112 = 12 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 整套防御力增加
        /// </summary>
        SpecialDefenseRate113 = 13 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 反伤
        /// </summary>
        BackInjureRate114 = 14 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 智力属性增加
        /// </summary>
        IncreaseWillpower115 = 15 + BattleMasteryConstData.SwordsmanId,

        /// 体力属性增加
        /// </summary>
        IncreaseBoneGas116 = 16 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 敏捷属性增加
        /// </summary>
        IncreaseAgility117 = 17 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 力量属性增加
        /// </summary>
        IncreaseStrength118 = 18 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 暴风之翼防御强化
        /// </summary>
        Equipment_Defense_BaoFengZhiYi_119 = 19 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 暴风之翼攻击强化
        /// </summary>
        Equipment_Attack_BaoFengZhiYi_120 = 20 + BattleMasteryConstData.SwordsmanId,


        /// <summary>
        /// 钢铁铠甲
        /// </summary>
        SteelCarapace_121 = 21 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 攻击成功率 （PVE）
        /// </summary>
        IncreaseAtteckSuccessRate122 = 22 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 剑之愤怒精通
        /// </summary>
        SKill_JianZhiFenNu_master123 = 23 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 霹雳回旋斩强化
        /// </summary>
        Skill_LeiTingHuiXuanZhan124 = 24 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 坚强的信念强化
        /// </summary>
        Skill_JianQiangDeXinNian125 = 25 + BattleMasteryConstData.SwordsmanId,

        /// <summary>
        /// 雷霆裂闪
        /// </summary>
        Skill_LeiTingLieShan126 = 26 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 武器精通
        /// </summary>
        Skill_WeaponMaster127 = 27 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 坚强的庇护
        /// </summary>
        Skill_JianQiangDeBiHu128 = 28 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 袭风刺
        /// </summary>
        Skill_XiFengCi129 = 29 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 坚强的庇护熟练
        /// </summary>
        Skill_JianQiangDeBiHu_skilled130 = 30 + BattleMasteryConstData.SwordsmanId,


        /// <summary>
        /// 最大生命力增加
        /// </summary>
        IncreaseHpMax131 = 31 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 袭风刺熟练
        /// </summary>
        Skill_XiFengCi_skilled132 = 32 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 坚强的庇护精通
        /// </summary>
        Skill_JianQiangDeBiHu_Master133 = 33 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 最大魔力增加
        /// </summary>
        IncreaseMagicPowerMax134 = 34 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 连击强化
        /// </summary>
        ComboStrengthening135 = 35 + BattleMasteryConstData.SwordsmanId,


        /// <summary>
        /// 致命一击强化
        /// </summary>
        ZhiMingYiJi_Strengthening136 = 36 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 最大AG增加
        /// </summary>
        IncreaseAGMax137 = 37 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 突袭技能
        /// </summary>
        Skill_TuXi_138 = 38 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 致命一击精通
        /// </summary>
        ZhiMingYiJi_master139 = 39 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 最大生命增加 131,140
        /// </summary>
        IncreaseHpMax140 = 40 + BattleMasteryConstData.SwordsmanId,


        /// <summary>
        /// 血腥风暴强化
        /// </summary>
        Skill_XueXingFengBao_141 = 41 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 血腥风暴强化
        /// </summary>
        Skill_XueXingFengBao_Strengthening142 = 42 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// PVP攻击成功率
        /// </summary>
        PVPAtteckSuccessRate143 = 43 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 双手剑强化
        /// </summary>
        ShuangShouJian_Strengthening144 = 44 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 单手剑强化
        /// </summary>
        DanShouJian_Strengthening145 = 45 + BattleMasteryConstData.SwordsmanId,



        /// <summary>
        /// 锤类强化
        /// </summary>
        Hammers_Strengthening146 = 46 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 矛类强化
        /// </summary>
        Spear_Strengthening147 = 47 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 双手剑精通 pvp
        /// </summary>
        PVPShuangShouJian_Strengthening148 = 48 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 单手剑精通
        /// </summary>
        DanShouJian_master149 = 49 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 锤类精通
        /// </summary>
        Hammers_master150 = 50 + BattleMasteryConstData.SwordsmanId,


        /// <summary>
        /// 矛类精通
        /// </summary>
        Spear_master151 = 51 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 生命之光强化
        /// </summary>
        ShengMingZhiGuang_Strengthening152 = 52 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 魔力减少 技能蓝耗减少
        /// </summary>
        ReduceConsumeMp153 = 53 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// Sd 回复   杀怪后
        /// </summary>
        KillRecoverySD154 = 54 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// hp 回复  杀怪后
        /// </summary>
        KillRecoveryHp155 = 55 + BattleMasteryConstData.SwordsmanId,


        /// <summary>
        /// 生命之光熟练
        /// </summary>
        Skill_ShengMingZhiGuang_skilled156 = 56 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 最小攻击力提高
        /// </summary>
        IncreaseMinAttack157 = 57 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 杀怪时魔力回复
        /// </summary>
        KillRecoveryMp158 = 58 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 生命之光精通
        /// </summary>
        Skill_ShengMingZhiGuang_master159 = 59 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 最大攻击力提高
        /// </summary>
        IncreaseMaxAttack160 = 60 + BattleMasteryConstData.SwordsmanId,


        /// <summary>
        /// 幸运一击几率提高
        /// </summary>
        IncreaseXingYunYiJiRate161 = 61 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 受伤魔力回复
        /// </summary>
        Injure_RecoveryAllMp162 = 62 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 受伤生命力回复
        /// </summary>
        Injure_RecoveryAllHp163 = 63 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 生命吸收
        /// </summary>
        AttackAbsorbHp164 = 64 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 卓越一击几率提高
        /// </summary>
        IncreaseZuoYueYiJiRate165 = 65 + BattleMasteryConstData.SwordsmanId,


        /// <summary>
        /// 战斗意志
        /// </summary>
        ZhanDouYiZhi166 = 66 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// sd 完全回复
        /// </summary>
        Attack_RecoveryAllSD167 = 67 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 双倍一击几率提高
        /// </summary>
        IncreaseShuangBeiYiJiRate168 = 68 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// sd 防御恢复
        /// </summary>
        Attack_RecoverySD169 = 69 + BattleMasteryConstData.SwordsmanId,
        /// <summary>
        /// 无视防御几率提高
        /// </summary>
        IgnoreDefenseRate170 = 70 + BattleMasteryConstData.SwordsmanId,
        #endregion

        #region 弓箭手
        /// <summary>
        /// 减少耐久1
        /// </summary>
        Reduce_Durable1_201 = 1 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 对人防御率提高
        /// </summary>
        PVPDefenseRate202 = 2 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 提高最大SD
        /// </summary>
        IncreaseSDMax203 = 3 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 自动魔力恢复力增加
        /// </summary>
        RecoveryMagic204 = 4 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 减少耐久2
        /// </summary>
        Reduce_Durable2_205 = 5 + BattleMasteryConstData.ArcherId,

        /// <summary>
        /// SD恢复速度提高
        /// </summary>
        RecoverySD206 = 6 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 自动生命恢复力增加
        /// </summary>
        RecoveryHp207 = 7 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 防御力增加
        /// </summary>
        IncreaseDefense208 = 8 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 属性防御力增加
        /// </summary>
        IncreaseElementDefense209 = 9 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 自动AG恢复力增加
        /// </summary>
        RecoveryAG210 = 10 + BattleMasteryConstData.ArcherId,

        /// <summary>
        /// 减少耐久3
        /// </summary>
        Reduce_Durable3_211 = 11 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 防御成功率提高
        /// </summary>
        DefenseRate212 = 12 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 整套防御力增加
        /// </summary>
        SpecialDefenseRate213 = 13 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 反伤
        /// </summary>
        BackInjureRate214 = 14 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 智力属性增加
        /// </summary>
        IncreaseWillpower215 = 15 + BattleMasteryConstData.ArcherId,

        /// 体力属性增加
        /// </summary>
        IncreaseBoneGas216 = 16 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 敏捷属性增加
        /// </summary>
        IncreaseAgility217 = 17 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 力量属性增加
        /// </summary>
        IncreaseStrength218 = 18 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 时空之翼防御强化
        /// </summary>
        Equipment_Defense_ShiKongZhiYi_219 = 19 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 守护盾
        /// </summary>
        GuardianShield220 = 20 + BattleMasteryConstData.ArcherId,


        /// <summary>
        /// 时空之翼攻击强化
        /// </summary>
        Equipment_Attack_ShiKongZhiYi_221 = 21 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 钢铁铠甲
        /// </summary>
        SteelCarapace_222 = 22 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 防盾
        /// </summary>
        DefensiveShield_223 = 23 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 攻击成功率 （PVE）
        /// </summary>
        IncreaseAtteckSuccessRate224 = 24 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 治疗强化
        /// </summary>
        Skill_ZhiLiaoStrengthen225 = 25 + BattleMasteryConstData.ArcherId,

        /// <summary>
        /// 多重箭强化
        /// </summary>
        Skill_DuoChongJianStrengthen226 = 26 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 召唤兽强化
        /// </summary>
        ZaoHuanShouStrengthen1_227 = 27 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 穿透箭强化
        /// </summary>
        Skill_ChuanTouJianStrengthen228 = 28 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 防御力提高强化
        /// </summary>
        Skill_DefenseIncreaseStrengthen229 = 29 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 吸收伤害
        /// </summary>
        HurtValueAbsorbRate230 = 30 + BattleMasteryConstData.ArcherId,

        /// <summary>
        /// 召唤兽强化2
        /// </summary>
        ZaoHuanShouStrengthen2_231 = 31 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 攻击力提高强化
        /// </summary>
        Skill_AttackIncreaseStrengthen232 = 32 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 武器精通
        /// </summary>
        Skill_WeaponMaster233 = 33 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 最大生命力增加
        /// </summary>
        IncreaseHpMax234 = 34 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 攻击力提高精通
        /// </summary>
        Skill_AttackIncreaseMaster235 = 35 + BattleMasteryConstData.ArcherId,


        /// <summary>
        /// 防御力提高强化
        /// </summary>
        Skill_DefenseIncreaseMaster236 = 36 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 冰箭强化
        /// </summary>
        Skill_BingJianStrengthen_237 = 37 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 最大魔力增加
        /// </summary>
        IncreaseMagicPowerMax238 = 38 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 生命增加
        /// </summary>
        AddHpMax239 = 39 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 五重箭强化
        /// </summary>
        Skill_WuChongJianStrengthen_240 = 40 + BattleMasteryConstData.ArcherId,


        /// <summary>
        /// 最大AG增加
        /// </summary>
        IncreaseAGMax241 = 41 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 三重箭
        /// </summary>
        Skill_SanChongJian_242 = 42 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 最大生命增加 234,243
        /// </summary>
        IncreaseHpMax243 = 43 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 召唤兽强化3
        /// </summary>
        ZaoHuanShouStrengthen3_244 = 44 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 三重箭强化
        /// </summary>
        Skill_SanChongJianStrengthen_245 = 45 + BattleMasteryConstData.ArcherId,


        /// <summary>
        /// 祝福
        /// </summary>
        Skill_ZuFu_246 = 46 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 毒箭
        /// </summary>
        Skill_DuJian_247 = 47 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 萨迪洛斯召唤
        /// </summary>
        Skill_ShaDiLuoShi_248 = 48 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 幻影移行
        /// </summary>
        Skill_HuanYingYiXing_249 = 49 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 祝福强化
        /// </summary>
        Skill_ZuFuStrengthen_250 = 50 + BattleMasteryConstData.ArcherId,

        /// <summary>
        /// 毒箭强化
        /// </summary>
        Skill_DuJianStrengthen_251 = 51 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 闪避
        /// </summary>
        Skill_Dodge_252 = 52 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// PVP攻击成功率
        /// </summary>
        PVPAtteckSuccessRate253 = 53 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 弓类强化
        /// </summary>
        Arch_Strengthening254 = 54 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 弩类强化
        /// </summary>
        Crossbow_Strengthening255 = 55 + BattleMasteryConstData.ArcherId,

        /// <summary>
        /// 盾牌强化
        /// </summary>
        Shield_Strengthening256 = 56 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 弓类精通
        /// </summary>
        Arch_Master257 = 57 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 弩类精通
        /// </summary>
        Crossbow_Master258 = 58 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 盾牌精通
        /// </summary>
        Shield_Master259 = 59 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 魔力减少 技能蓝耗减少
        /// </summary>
        ReduceConsumeMp260 = 60 + BattleMasteryConstData.ArcherId,


        /// <summary>
        /// Sd 回复   杀怪后
        /// </summary>
        KillRecoverySD261 = 61 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// hp 回复  杀怪后
        /// </summary>
        KillRecoveryHp262 = 62 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 无影箭强化
        /// </summary>
        Skill_MpMax_263 = 63 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 最小攻击力提高
        /// </summary>
        IncreaseMinAttack264 = 64 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 杀怪时魔力回复
        /// </summary>
        KillRecoveryMp265 = 65 + BattleMasteryConstData.ArcherId,

        /// <summary>
        /// 最大攻击力提高
        /// </summary>
        IncreaseMaxAttack266 = 66 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 幸运一击几率提高
        /// </summary>
        IncreaseXingYunYiJiRate267 = 67 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 受伤魔力回复
        /// </summary>
        Injure_RecoveryAllMp268 = 68 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 受伤生命力回复
        /// </summary>
        Injure_RecoveryAllHp269 = 69 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 生命吸收
        /// </summary>
        AttackAbsorbHp270 = 70 + BattleMasteryConstData.ArcherId,


        /// <summary>
        /// 卓越一击几率提高
        /// </summary>
        IncreaseZuoYueYiJiRate271 = 71 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 神射手
        /// </summary>
        ShenSheShou272 = 72 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 攻击时sd 完全回复
        /// </summary>
        Attack_RecoveryAllSD273 = 73 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 双倍一击几率提高
        /// </summary>
        IncreaseShuangBeiYiJiRate274 = 74 + BattleMasteryConstData.ArcherId,
        /// <summary>
        /// 攻击时sd 防御恢复
        /// </summary>
        Attack_RecoverySD275 = 75 + BattleMasteryConstData.ArcherId,

        /// <summary>
        /// 无视防御几率提高
        /// </summary>
        IgnoreDefenseRate276 = 76 + BattleMasteryConstData.ArcherId,

        #endregion

        #region 魔剑士
        /// <summary>
        /// 减少耐久1
        /// </summary>
        Reduce_Durable1_301 = 1 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 对人防御率提高
        /// </summary>
        PVPDefenseRate302 = 2 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 提高最大SD
        /// </summary>
        IncreaseSDMax303 = 3 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 自动魔力恢复力增加
        /// </summary>
        RecoveryMagic304 = 4 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 减少耐久2
        /// </summary>
        Reduce_Durable2_305 = 5 + BattleMasteryConstData.SpellswordId,

        /// <summary>
        /// SD恢复速度提高
        /// </summary>
        RecoverySD306 = 6 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 自动生命恢复力增加
        /// </summary>
        RecoveryHp307 = 7 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 防御力增加
        /// </summary>
        IncreaseDefense308 = 8 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 属性防御力增加
        /// </summary>
        IncreaseElementDefense309 = 9 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 自动AG恢复力增加
        /// </summary>
        RecoveryAG310 = 10 + BattleMasteryConstData.SpellswordId,

        /// <summary>
        /// 减少耐久3
        /// </summary>
        Reduce_Durable3_311 = 11 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 防御成功率提高
        /// </summary>
        DefenseRate312 = 12 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 整套防御力增加
        /// </summary>
        SpecialDefenseRate313 = 13 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 反伤几率
        /// </summary>
        BackInjureRate314 = 14 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 智力属性增加
        /// </summary>
        IncreaseWillpower315 = 15 + BattleMasteryConstData.SpellswordId,

        /// 体力属性增加
        /// </summary>
        IncreaseBoneGas316 = 16 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 敏捷属性增加
        /// </summary>
        IncreaseAgility317 = 17 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 力量属性增加
        /// </summary>
        IncreaseStrength318 = 18 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 毁灭之翼防御强化
        /// </summary>
        Equipment_Defense_HuiMieZhiYi_319 = 19 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 武器格挡
        /// </summary>
        WuQiGridBlock320 = 20 + BattleMasteryConstData.SpellswordId,


        /// <summary>
        /// 守护盾
        /// </summary>
        GuardianShield321 = 21 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 毁灭之翼攻击强化
        /// </summary>
        Equipment_Attack_HuiMieZhiYi_322 = 22 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 钢铁铠甲
        /// </summary>
        SteelCarapace_323 = 23 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 防盾
        /// </summary>
        DefensiveShield_324 = 24 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 攻击成功率 （PVE）
        /// </summary>
        IncreaseAtteckSuccessRate325 = 25 + BattleMasteryConstData.SpellswordId,

        /// <summary>
        /// 旋风斩强化
        /// </summary>
        Skill_XuanFengZhanStrengthen326 = 26 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 掌心雷强化
        /// </summary>
        Skill_ZhuangXinLeiStrengthen327 = 27 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 霹雳回旋斩强化
        /// </summary>
        Skill_LeiTingHuiXuanZhanStrengthen328 = 28 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 天雷闪强化
        /// </summary>
        Skill_TianLeiShanStrengthen329 = 29 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 火龙强化
        /// </summary>
        Skill_HuoLongStrengthen330 = 30 + BattleMasteryConstData.SpellswordId,


        /// <summary>
        /// 爆炎强化
        /// </summary>
        Skill_BaoYanStrengthen331 = 31 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 武器精通
        /// </summary>
        WeaponMaster332 = 32 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 玄月斩强化
        /// </summary>
        Skill_XuanYueZhanStrengthen333 = 33 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 黑龙波强化
        /// </summary>
        Skill_HeiLongBoStrengthen334 = 34 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 魔法精通
        /// </summary>
        MagicMaster335 = 35 + BattleMasteryConstData.SpellswordId,


        /// <summary>
        /// 最大生命增加
        /// </summary>
        IncreaseHpMax336 = 36 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 玄月斩精通
        /// </summary>
        Skill_XuanYueZhanMaster337 = 37 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 闪电轰顶强化
        /// </summary>
        Skill_ShanDianHongDingStrengthen338 = 38 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 最大魔力增加
        /// </summary>
        IncreaseMagicPowerMax339 = 39 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 火焰领域强化
        /// </summary>
        Skill_HuoYanLingYuStrengthen340 = 40 + BattleMasteryConstData.SpellswordId,


        /// <summary>
        /// 冰封领域强化
        /// </summary>
        Skill_BingFengLingYuStrengthen341 = 41 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 魔法领域强化
        /// </summary>
        Skill_MoFaLingYuStrengthen342 = 42 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 最大AG增加
        /// </summary>
        IncreaseAGMax343 = 43 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 火焰领域强化
        /// </summary>
        Skill_HuoYanLingYuMaster344 = 44 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 冰封领域强化
        /// </summary>
        Skill_BingFengLingYuMaster345 = 45 + BattleMasteryConstData.SpellswordId,



        /// <summary>
        /// 魔法领域强化
        /// </summary>
        Skill_MoFaLingYuMaster346 = 46 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 最大生命增加 36,47
        /// </summary>
        IncreaseHpMax347 = 47 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 血腥风暴
        /// </summary>
        Skill_XueXingFengBao_348 = 48 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 地牢术
        /// </summary>
        Skill_DiLaoShu349 = 49 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 血腥风暴强化
        /// </summary>
        Skill_XueXingFengBao_Strengthening350 = 50 + BattleMasteryConstData.SpellswordId,


        /// <summary>
        /// 地牢术强化
        /// </summary>
        Skill_DiLaoShuStrengthen351 = 51 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// PVP攻击成功率
        /// </summary>
        PVPAtteckSuccessRate352 = 52 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 双手剑强化
        /// </summary>
        ShuangShouJian_Strengthening353 = 53 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 单手剑强化
        /// </summary>
        DanShouJian_Strengthening354 = 54 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 单手杖强化
        /// </summary>
        DanShouJian_Strengthening355 = 55 + BattleMasteryConstData.SpellswordId,


        /// <summary>
        /// 双手杖强化
        /// </summary>
        ShuangShouJian_Strengthening356 = 56 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 双手剑精通
        /// </summary>
        ShuangShouJian_Strengthening357 = 57 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 单手剑精通
        /// </summary>
        DanShouJian_master358 = 58 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 单手杖精通
        /// </summary>
        DanShouJian_Master359 = 59 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 双手杖精通
        /// </summary>
        ShuangShouJian_Master360 = 60 + BattleMasteryConstData.SpellswordId,


        /// <summary>
        /// 魔力减少 技能蓝耗减少
        /// </summary>
        ReduceConsumeMp361 = 61 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// Sd 回复   杀怪后
        /// </summary>
        KillRecoverySD362 = 62 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// hp 回复  杀怪后
        /// </summary>
        KillRecoveryHp363 = 63 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 最小魔力提高
        /// </summary>
        IncreaseMinMagic364 = 64 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 最小攻击力提高
        /// </summary>
        IncreaseMinAttack365 = 65 + BattleMasteryConstData.SpellswordId,


        /// <summary>
        /// 杀怪时魔力回复
        /// </summary>
        KillRecoveryMp366 = 66 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 最大魔力提高
        /// </summary>
        IncreaseMaxMagic367 = 67 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 最大攻击力提高
        /// </summary>
        IncreaseMaxAttack368 = 68 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 幸运一击几率提高
        /// </summary>
        IncreaseXingYunYiJiRate369 = 69 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 受伤魔力回复
        /// </summary>
        Injure_RecoveryAllMp370 = 70 + BattleMasteryConstData.SpellswordId,


        /// <summary>
        /// 受伤生命力回复
        /// </summary>
        Injure_RecoveryAllHp371 = 71 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 生命吸收
        /// </summary>
        AttackAbsorbHp372 = 72 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 卓越一击几率提高
        /// </summary>
        IncreaseZuoYueYiJiRate373 = 73 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// sd 完全回复
        /// </summary>
        Attack_RecoveryAllSD374 = 74 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 双倍一击几率提高
        /// </summary>
        IncreaseShuangBeiYiJiRate375 = 75 + BattleMasteryConstData.SpellswordId,


        /// <summary>
        /// sd 防御恢复
        /// </summary>
        Attack_RecoverySD376 = 76 + BattleMasteryConstData.SpellswordId,
        /// <summary>
        /// 无视防御几率提高
        /// </summary>
        IgnoreDefenseRate377 = 77 + BattleMasteryConstData.SpellswordId,


        #endregion

        #region 圣导师
        /// <summary>
        /// 减少耐久1
        /// </summary>
        Reduce_Durable1_401 = 1 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 对人防御率提高
        /// </summary>
        PVPDefenseRate402 = 2 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 提高最大SD
        /// </summary>
        IncreaseSDMax403 = 3 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 自动魔力恢复力增加
        /// </summary>
        RecoveryMagic404 = 4 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 减少耐久2
        /// </summary>
        Reduce_Durable2_405 = 5 + BattleMasteryConstData.HolyteacherId,

        /// <summary>
        /// SD恢复速度提高
        /// </summary>
        RecoverySD406 = 6 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 自动生命恢复力增加
        /// </summary>
        RecoveryHp407 = 7 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 防御力增加
        /// </summary>
        IncreaseDefense408 = 8 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 属性防御力增加
        /// </summary>
        IncreaseElementDefense409 = 9 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 自动AG恢复力增加
        /// </summary>
        RecoveryAG410 = 10 + BattleMasteryConstData.HolyteacherId,

        /// <summary>
        /// 减少耐久3
        /// </summary>
        Reduce_Durable3_411 = 11 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 防御成功率提高
        /// </summary>
        DefenseRate412 = 12 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 统率属性增加
        /// </summary>
        IncreaseCommand413 = 13 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 整套防御力增加
        /// </summary>
        SpecialDefenseRate414 = 14 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 反伤几率
        /// </summary>
        BackInjureRate415 = 15 + BattleMasteryConstData.HolyteacherId,

        /// <summary>
        /// 智力属性增加
        /// </summary>
        IncreaseWillpower416 = 16 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 体力属性增加
        /// </summary>
        IncreaseBoneGas417 = 17 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 敏捷属性增加
        /// </summary>
        IncreaseAgility418 = 18 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 力量属性增加
        /// </summary>
        IncreaseStrength419 = 19 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 帝王披风防御强化
        /// </summary>
        Equipment_Defense_DiWangPiFeng420 = 20 + BattleMasteryConstData.HolyteacherId,

        /// <summary>
        /// 守护盾
        /// </summary>
        GuardianShield421 = 21 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 帝王披风攻击强化
        /// </summary>
        Equipment_Attack_DiWangPiFeng422 = 22 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 钢铁铠甲
        /// </summary>
        SteelCarapace_423 = 23 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 防盾
        /// </summary>
        DefensiveShield_424 = 24 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 攻击成功率 （PVE）
        /// </summary>
        IncreaseAtteckSuccessRate425 = 25 + BattleMasteryConstData.HolyteacherId,

        /// <summary>
        /// 星云火链强化
        /// </summary>
        Skill_XingYunHuoLianStrengthen426 = 26 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 冲击波强化
        /// </summary>
        Skill_ChongJiBoStrengthen427 = 27 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 黑王马强化
        /// </summary>
        Equipment_HeiWangMaStrengthen428 = 28 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 致命圣印强化
        /// </summary>
        Skill_ZhiMingShengYinStrengthen429 = 29 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 地裂强化
        /// </summary>
        Skill_DiLieShengYinStrengthen430 = 30 + BattleMasteryConstData.HolyteacherId,


        /// <summary>
        /// 武器精通
        /// </summary>
        Skill_WeaponMaster431 = 31 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 致命圣印强化(2)
        /// </summary>
        Skill_ZhiMingShengYinStrengthen2_432 = 32 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 最大生命力增加
        /// </summary>
        IncreaseHpMax433 = 33 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 致命圣印强化(3)
        /// </summary>
        Skill_ZhiMingShengYinStrengthen3_434 = 34 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 火舞旋风强化
        /// </summary>
        Skill_HuoWuXuanFengStrengthen435 = 35 + BattleMasteryConstData.HolyteacherId,


        /// <summary>
        /// 最大魔力增加
        /// </summary>
        IncreaseMagicPowerMax436 = 36 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 圣极光强化
        /// </summary>
        Skill_ShengJiGuangStrengthen437 = 37 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 火舞旋风精通
        /// </summary>
        Skill_HuoWuXuanFengMaster438 = 38 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 最大AG增加
        /// </summary>
        IncreaseAGMax439 = 39 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 致命一击精通
        /// </summary>
        ZhiMingYiJi_master440 = 40 + BattleMasteryConstData.HolyteacherId,

        /// <summary>
        /// 黑暗之力强化
        /// </summary>
        Skill_HeiAnZhiLiStrengthen441 = 41 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 最大生命增加 433,442
        /// </summary>
        IncreaseHpMax442 = 42 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// PVP攻击成功率
        /// </summary>
        PVPAtteckSuccessRate443 = 43 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 天鹰强化
        /// </summary>
        TianYing_AttackStrengthen444 = 44 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 权杖强化
        /// </summary>
        QuanZhang_Strengthen445 = 45 + BattleMasteryConstData.HolyteacherId,

        /// <summary>
        /// 盾牌强化
        /// </summary>
        Shield_Strengthening446 = 46 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 权杖精通
        /// </summary>
        QuanZhang_Master447 = 47 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 天鹰强化(2)
        /// </summary>
        TianYing_AttackStrengthen2_448 = 48 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 权杖精通PVP
        /// </summary>
        PVPQuanZhang_Master449 = 49 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 盾牌精通
        /// </summary>
        Shield_Master450 = 50 + BattleMasteryConstData.HolyteacherId,

        /// <summary>
        /// 统率攻击力 增加
        /// </summary>
        IncreaseAttackByCommand451 = 51 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 天鹰强化(3)
        /// </summary>
        TianYing_AttackStrengthen3_452 = 52 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 魔力减少 技能蓝耗减少
        /// </summary>
        ReduceConsumeMp453 = 53 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// Sd 回复   杀怪后
        /// </summary>
        KillRecoverySD454 = 54 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// hp 回复  杀怪后
        /// </summary>
        KillRecoveryHp455 = 55 + BattleMasteryConstData.HolyteacherId,


        /// <summary>
        /// 天鹰强化(5)
        /// </summary>
        TianYing_AttackStrengthen5_456 = 56 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 宠物耐久度强化
        /// </summary>
        PetsDurableStrengthen457 = 57 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 最小攻击力提高
        /// </summary>
        IncreaseMinAttack458 = 58 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 杀怪时魔力回复
        /// </summary>
        KillRecoveryMp459 = 59 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 精神之王
        /// </summary>
        JingShenZhiWang460 = 60 + BattleMasteryConstData.HolyteacherId,


        /// <summary>
        /// 天鹰强化(5)
        /// </summary>
        TianYing_AttackStrengthen4_461 = 61 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 最大攻击力提高
        /// </summary>
        IncreaseMaxAttack462 = 62 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 幸运一击几率提高
        /// </summary>
        IncreaseXingYunYiJiRate463 = 63 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 受伤魔力回复
        /// </summary>
        Injure_RecoveryAllMp464 = 64 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 受伤生命力回复
        /// </summary>
        Injure_RecoveryAllHp465 = 65 + BattleMasteryConstData.HolyteacherId,


        /// <summary>
        /// 生命吸收
        /// </summary>
        AttackAbsorbHp466 = 66 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 卓越一击几率提高
        /// </summary>
        IncreaseZuoYueYiJiRate467 = 67 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// sd 完全回复
        /// </summary>
        Attack_RecoveryAllSD468 = 68 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// 双倍一击几率提高
        /// </summary>
        IncreaseShuangBeiYiJiRate469 = 69 + BattleMasteryConstData.HolyteacherId,
        /// <summary>
        /// sd 防御恢复
        /// </summary>
        Attack_RecoverySD470 = 70 + BattleMasteryConstData.HolyteacherId,


        /// <summary>
        /// 无视防御几率提高
        /// </summary>
        IgnoreDefenseRate471 = 71 + BattleMasteryConstData.HolyteacherId,
        #endregion

        #region 召唤术师
        /// <summary>
        /// 减少耐久1
        /// </summary>
        Reduce_Durable1_501 = 1 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 对人防御率提高
        /// </summary>
        PVPDefenseRate502 = 2 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 提高最大SD
        /// </summary>
        IncreaseSDMax503 = 3 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 自动魔力恢复力增加
        /// </summary>
        RecoveryMagic504 = 4 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 减少耐久2
        /// </summary>
        Reduce_Durable2_505 = 5 + BattleMasteryConstData.SummonWarlockId,

        /// <summary>
        /// SD恢复速度提高
        /// </summary>
        RecoverySD506 = 6 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 自动生命恢复力增加
        /// </summary>
        RecoveryHp507 = 7 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 防御力增加
        /// </summary>
        IncreaseDefense508 = 8 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 属性防御力增加
        /// </summary>
        IncreaseElementDefense509 = 9 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 自动AG恢复力增加
        /// </summary>
        RecoveryAG510 = 10 + BattleMasteryConstData.SummonWarlockId,

        /// <summary>
        /// 减少耐久3
        /// </summary>
        Reduce_Durable3_511 = 11 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 防御成功率提高
        /// </summary>
        DefenseRate512 = 12 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 整套防御力增加
        /// </summary>
        SpecialDefenseRate513 = 13 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 反伤
        /// </summary>
        BackInjureRate514 = 14 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 智力属性增加
        /// </summary>
        IncreaseWillpower515 = 15 + BattleMasteryConstData.SummonWarlockId,

        /// <summary>
        /// 体力属性增加
        /// </summary>
        IncreaseBoneGas516 = 16 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 敏捷属性增加
        /// </summary>
        IncreaseAgility517 = 17 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 力量属性增加
        /// </summary>
        IncreaseStrength518 = 18 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 次元之翼防御强化
        /// </summary>
        Equipment_Defense_ChiYuanZhiYi_519 = 19 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 次元之翼攻击强化
        /// </summary>
        Equipment_Attack_ChiYuanZhiYi_520 = 20 + BattleMasteryConstData.SummonWarlockId,

        /// <summary>
        /// 钢铁铠甲
        /// </summary>
        SteelCarapace_521 = 21 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 攻击成功率 （PVE）
        /// </summary>
        IncreaseAtteckSuccessRate522 = 22 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 裂袭强化
        /// </summary>
        Skill_LiXi_Strengthen523 = 23 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 影煞强化
        /// </summary>
        Skill_YingSha_Strengthen524 = 24 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 聚灵强化
        /// </summary>
        Skill_JuLing_Strengthen525 = 25 + BattleMasteryConstData.SummonWarlockId,

        /// <summary>
        /// 链雷咒强化
        /// </summary>
        Skill_LianLeiZou_Strengthen526 = 26 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 聚灵熟悉
        /// </summary>
        Skill_JuLing_Skilled527 = 27 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 昏睡术强化
        /// </summary>
        Skill_HunShuiShu_Strengthen528 = 28 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 烈光闪强化
        /// </summary>
        Skill_LieGuangShan_Strengthen529 = 29 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 聚灵精通
        /// </summary>
        Skill_JuLing_Master530 = 30 + BattleMasteryConstData.SummonWarlockId,

        /// <summary>
        /// 最大生命力增加
        /// </summary>
        IncreaseHpMax531 = 31 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 魔法精通
        /// </summary>
        MagicMaster532 = 32 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 摄魂咒强化
        /// </summary>
        Skill_SheHunZhou_Strengthen533 = 33 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 最大魔力增加
        /// </summary>
        IncreaseMagicPowerMax534 = 34 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 虚弱阵强化
        /// </summary>
        Skill_XuRuoZhen_Strengthen535 = 35 + BattleMasteryConstData.SummonWarlockId,

        /// <summary>
        /// 破御阵强化
        /// </summary>
        Skill_PoYuZhen_Strengthen536 = 36 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 最大AG增加
        /// </summary>
        IncreaseAGMax537 = 37 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 虚弱阵精通
        /// </summary>
        Skill_XuRuoZhen_Master538 = 38 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 破御阵精通
        /// </summary>
        Skill_PoYuZhen_Master539 = 39 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 最大生命增加 531,540
        /// </summary>
        IncreaseHpMax540 = 40 + BattleMasteryConstData.SummonWarlockId,

        /// <summary>
        /// 蒙眼煞
        /// </summary>
        Skill_MengYanSha541 = 41 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// PVP攻击成功率
        /// </summary>
        PVPAtteckSuccessRate542 = 42 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 权杖强化
        /// </summary>
        QuanZhang_Strengthen543 = 43 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 异界之书强化
        /// </summary>
        Equipment_YiJieZhiShu_Strengthen544 = 44 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 权杖精通
        /// </summary>
        QuanZhang_Master545 = 45 + BattleMasteryConstData.SummonWarlockId,

        /// <summary>
        /// 异界之书精通
        /// </summary>
        Equipment_YiJieZhiShu_Master546 = 46 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 魔力减少 技能蓝耗减少
        /// </summary>
        ReduceConsumeMp547 = 47 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// Sd 回复   杀怪后
        /// </summary>
        KillRecoverySD548 = 48 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// hp 回复  杀怪后
        /// </summary>
        KillRecoveryHp549 = 49 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 狂暴术强化
        /// </summary>
        Skill_KuangBaoShu_Strengthen550 = 50 + BattleMasteryConstData.SummonWarlockId,

        /// <summary>
        /// 黑暗镇魂曲强化
        /// </summary>
        Skill_HeiAnZhenHunQu_Strengthen551 = 51 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 最小魔力/诅咒力提高
        /// </summary>
        IncreaseMinMagicAndDamnation552 = 52 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 杀怪时魔力回复
        /// </summary>
        KillRecoveryMp553 = 53 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 狂暴精通
        /// </summary>
        KuangBao_Master554 = 54 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 黑暗镇魂曲精通
        /// </summary>
        Skill_HeiAnZhenHunQu_Master555 = 55 + BattleMasteryConstData.SummonWarlockId,

        /// <summary>
        /// 最大魔力/诅咒力提高
        /// </summary>
        IncreaseMaxMagicAndDamnation556 = 56 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 幸运一击几率提高
        /// </summary>
        IncreaseXingYunYiJiRate557 = 57 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 受伤魔力回复
        /// </summary>
        Injure_RecoveryAllMp558 = 58 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 受伤生命力回复
        /// </summary>
        Injure_RecoveryAllHp559 = 59 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 生命吸收
        /// </summary>
        AttackAbsorbHp560 = 60 + BattleMasteryConstData.SummonWarlockId,

        /// <summary>
        /// 卓越一击几率提高
        /// </summary>
        IncreaseZuoYueYiJiRate561 = 61 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 咒诅的痛苦
        /// </summary>
        Skill_ZuZhouDeTongKu_Master562 = 62 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// sd 完全回复
        /// </summary>
        Attack_RecoveryAllSD563 = 63 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// 双倍一击几率提高
        /// </summary>
        IncreaseShuangBeiYiJiRate564 = 64 + BattleMasteryConstData.SummonWarlockId,
        /// <summary>
        /// sd 防御恢复
        /// </summary>
        Attack_RecoverySD565 = 65 + BattleMasteryConstData.SummonWarlockId,


        /// <summary>
        /// 无视防御几率提高
        /// </summary>
        IgnoreDefenseRate566 = 66 + BattleMasteryConstData.SummonWarlockId,

        #endregion


        #region 格斗家
        /// <summary>
        /// 减少耐久1
        /// </summary>
        MasterAttribute601 = 1 + BattleMasteryConstData.CombatId,
        /// <summary>
        /// PVP防御率提高
        /// </summary>
        MasterAttribute602,
        /// <summary>
        /// 提高最大SD
        /// </summary>
        MasterAttribute603,
        /// <summary>
        /// 自动魔力恢复力增加
        /// </summary>
        MasterAttribute604,
        /// <summary>
        /// 减少耐久（2）
        /// </summary>
        MasterAttribute605,
        /// <summary>
        /// SD恢复速度提高
        /// </summary>
        MasterAttribute606,
        /// <summary>
        /// 自动生命恢复力增加
        /// </summary>
        MasterAttribute607,
        /// <summary>
        /// 防御力增加
        /// </summary>
        MasterAttribute608,
        /// <summary>
        /// 属性防御力增加
        /// </summary>
        MasterAttribute609,
        /// <summary>
        /// 自动AG恢复力增加
        /// </summary>
        MasterAttribute610,
        /// <summary>
        /// 减少耐久（3）
        /// </summary>
        MasterAttribute611,
        /// <summary>
        /// 防御成功率提高
        /// </summary>
        MasterAttribute612,
        /// <summary>
        /// 整套防御力增加
        /// </summary>
        MasterAttribute613,
        /// <summary>
        /// 复仇
        /// </summary>
        MasterAttribute614,
        /// <summary>
        /// 智力属性增加
        /// </summary>
        MasterAttribute615,
        /// <summary>
        /// 体力属性增加
        /// </summary>
        MasterAttribute616,
        /// <summary>
        /// 敏捷属性增加
        /// </summary>
        MasterAttribute617,
        /// <summary>
        /// 力量属性增加
        /// </summary>
        MasterAttribute618,
        /// <summary>
        /// 斗皇披风防御强化
        /// </summary>
        MasterAttribute619,
        /// <summary>
        /// 斗皇披风攻击强化
        /// </summary>
        MasterAttribute620,
        /// <summary>
        /// 钢铁铠甲
        /// </summary>
        MasterAttribute621,
        /// <summary>
        /// 攻击成功率提高(PVE)
        /// </summary>
        MasterAttribute622,
        /// <summary>
        /// 幽冥青狼拳强化
        /// </summary>
        MasterAttribute623,
        /// <summary>
        /// 斗气爆裂强化
        /// </summary>
        MasterAttribute624,
        /// <summary>
        /// 强化神圣气旋
        /// </summary>
        MasterAttribute625,
        /// <summary>
        /// 幽冥青狼拳精通
        /// </summary>
        MasterAttribute626,
        /// <summary>
        /// 斗气爆裂精通
        /// </summary>
        MasterAttribute627,
        /// <summary>
        /// 神圣气旋精通
        /// </summary>
        MasterAttribute628,
        /// <summary>
        /// 最大生命力 增加
        /// </summary>
        MasterAttribute629,
        /// <summary>
        /// 武器精通
        /// </summary>
        MasterAttribute630,
        /// <summary>
        /// 回旋踢强化
        /// </summary>
        MasterAttribute631,
        /// <summary>
        /// 幽冥光速拳强化
        /// </summary>
        MasterAttribute632,
        /// <summary>
        /// 最大魔力增加
        /// </summary>
        MasterAttribute633,
        /// <summary>
        /// 炎龙拳强化
        /// </summary>
        MasterAttribute634,
        /// <summary>
        /// 最大AG 增加
        /// </summary>
        MasterAttribute635,
        /// <summary>
        /// 幽冥光速拳精通
        /// </summary>
        MasterAttribute636,
        /// <summary>
        /// 最大生命力提升
        /// </summary>
        MasterAttribute637,
        /// <summary>
        /// 噬血之龙强化
        /// </summary>
        MasterAttribute638,
        /// <summary>
        /// 鲜血咆哮
        /// </summary>
        MasterAttribute639,
        /// <summary>
        /// 鲜血咆哮强化
        /// </summary>
        MasterAttribute640,
        /// <summary>
        /// PVP攻击率提高
        /// </summary>
        MasterAttribute641,
        /// <summary>
        /// 装备武器强化
        /// </summary>
        MasterAttribute642,
        /// <summary>
        /// 斗神-御强化
        /// </summary>
        MasterAttribute643,
        /// <summary>
        /// 装备武器精通
        /// </summary>
        MasterAttribute644,
        /// <summary>
        /// 防御成功率提高精通
        /// </summary>
        MasterAttribute645,
        /// <summary>
        /// 斗神-命强化
        /// </summary>
        MasterAttribute646,
        /// <summary>
        /// 魔力减少
        /// </summary>
        MasterAttribute647,
        /// <summary>
        /// 杀怪时 SD恢复
        /// </summary>
        MasterAttribute648,
        /// <summary>
        /// 杀怪时生命力恢复
        /// </summary>
        MasterAttribute649,
        /// <summary>
        /// 最小攻击力提高
        /// </summary>
        MasterAttribute650,
        /// <summary>
        /// 杀怪时魔力恢复
        /// </summary>
        MasterAttribute651,
        /// <summary>
        /// 最大攻击力提高
        /// </summary>
        MasterAttribute652,
        /// <summary>
        /// 幸运一击几率提高
        /// </summary>
        MasterAttribute653,
        /// <summary>
        /// 魔力完全恢复
        /// </summary>
        MasterAttribute654,
        /// <summary>
        /// 生命力完全恢复
        /// </summary>
        MasterAttribute655,
        /// <summary>
        /// 生命吸收
        /// </summary>
        MasterAttribute656,
        /// <summary>
        /// 卓越一击几率提高
        /// </summary>
        MasterAttribute657,
        /// <summary>
        /// SD完全恢复
        /// </summary>
        MasterAttribute658,
        /// <summary>
        /// 双倍攻击几率提高
        /// </summary>
        MasterAttribute659,
        /// <summary>
        /// 吸收SD
        /// </summary>
        MasterAttribute660,
        /// <summary>
        /// 无视防御几率提高
        /// </summary>
        MasterAttribute661,
        #endregion

        #region 梦幻骑士
        /// <summary>
        /// 减少耐久（1）
        /// </summary>
        MasterAttribute901 = 1 + BattleMasteryConstData.GrowLancerId,
        /// <summary>
        /// PVP防御率提高
        /// </summary>
        MasterAttribute902,
        /// <summary>
        /// 提高最大SD
        /// </summary>
        MasterAttribute903,
        /// <summary>
        /// 自动魔力恢复力增加
        /// </summary>
        MasterAttribute904,
        /// <summary>
        /// 减少耐久（2）
        /// </summary>
        MasterAttribute905,
        /// <summary>
        /// SD恢复速度提高
        /// </summary>
        MasterAttribute906,
        /// <summary>
        /// 自动生命恢复力增加
        /// </summary>
        MasterAttribute907,
        /// <summary>
        /// 防御力增加
        /// </summary>
        MasterAttribute908,
        /// <summary>
        /// 属性防御力增加
        /// </summary>
        MasterAttribute909,
        /// <summary>
        /// 自动AG恢复力增加
        /// </summary>
        MasterAttribute910,
        /// <summary>
        /// 减少耐久（3）
        /// </summary>
        MasterAttribute911,
        /// <summary>
        /// 防御成功率提高
        /// </summary>
        MasterAttribute912,
        /// <summary>
        /// 整套防御力增加
        /// </summary>
        MasterAttribute913,
        /// <summary>
        /// 复仇
        /// </summary>
        MasterAttribute914,
        /// <summary>
        /// 智力属性增加
        /// </summary>
        MasterAttribute915,
        /// <summary>
        /// 体力属性增加
        /// </summary>
        MasterAttribute916,
        /// <summary>
        /// 敏捷属性增加
        /// </summary>
        MasterAttribute917,
        /// <summary>
        /// 力量属性增加
        /// </summary>
        MasterAttribute918,
        /// <summary>
        /// 超越披风防御强化
        /// </summary>
        MasterAttribute919,
        /// <summary>
        /// 守护盾
        /// </summary>
        MasterAttribute920,
        /// <summary>
        /// 超越披风攻击强化
        /// </summary>
        MasterAttribute921,
        /// <summary>
        /// 钢铁铠甲
        /// </summary>
        MasterAttribute922,
        /// <summary>
        /// 防盾
        /// </summary>
        MasterAttribute923,
        /// <summary>
        /// 攻击成功率提高(PVE)
        /// </summary>
        MasterAttribute924,
        /// <summary>
        /// 牙突刺强化
        /// </summary>
        MasterAttribute925,
        /// <summary>
        /// 旋龙刺 强化
        /// </summary>
        MasterAttribute926,
        /// <summary>
        /// 回旋穿刺 强化
        /// </summary>
        MasterAttribute927,
        /// <summary>
        /// 武器精通
        /// </summary>
        MasterAttribute928,
        /// <summary>
        /// 旋龙刺 精通
        /// </summary>
        MasterAttribute929,
        /// <summary>
        /// 回旋穿刺 精通
        /// </summary>
        MasterAttribute930,
        /// <summary>
        /// 最大生命力 增加
        /// </summary>
        MasterAttribute931,
        /// <summary>
        /// 黑曜石 强化
        /// </summary>
        MasterAttribute932,
        /// <summary>
        /// 幻龙破 强化
        /// </summary>
        MasterAttribute933,
        /// <summary>
        /// 炎舞 强化
        /// </summary>
        MasterAttribute934,
        /// <summary>
        /// 最大魔力增加
        /// </summary>
        MasterAttribute935,
        /// <summary>
        /// 最大AG 增加
        /// </summary>
        MasterAttribute936,
        /// <summary>
        /// 飓风刺 强化
        /// </summary>
        MasterAttribute937,
        /// <summary>
        /// 幻龙破 精通
        /// </summary>
        MasterAttribute938,
        /// <summary>
        /// 炎舞 精通
        /// </summary>
        MasterAttribute939,
        /// <summary>
        /// 最大生命力提升
        /// </summary>
        MasterAttribute940,
        /// <summary>
        /// 破御圣言
        /// </summary>
        MasterAttribute941,
        /// <summary>
        /// 破御圣言 强化
        /// </summary>
        MasterAttribute942,
        /// <summary>
        /// PVP攻击率提高
        /// </summary>
        MasterAttribute943,
        /// <summary>
        /// 长矛强化
        /// </summary>
        MasterAttribute944,
        /// <summary>
        /// 惩戒之盾 强化
        /// </summary>
        MasterAttribute945,
        /// <summary>
        /// 防盾强化
        /// </summary>
        MasterAttribute946,
        /// <summary>
        /// 长矛精通
        /// </summary>
        MasterAttribute947,
        /// <summary>
        /// 惩戒之盾 精通
        /// </summary>
        MasterAttribute948,
        /// <summary>
        /// 防盾精通
        /// </summary>
        MasterAttribute949,
        /// <summary>
        /// 狂怒强化
        /// </summary>
        MasterAttribute950,
        /// <summary>
        /// 魔力减少
        /// </summary>
        MasterAttribute951,
        /// <summary>
        /// 杀怪时 SD恢复
        /// </summary>
        MasterAttribute952,
        /// <summary>
        /// 杀怪时 生命力恢复
        /// </summary>
        MasterAttribute953,
        /// <summary>
        /// 狂怒熟练
        /// </summary>
        MasterAttribute954,
        /// <summary>
        /// 最小攻击力提高
        /// </summary>
        MasterAttribute955,
        /// <summary>
        /// 杀怪时 魔力恢复
        /// </summary>
        MasterAttribute956,
        /// <summary>
        /// 狂怒精通
        /// </summary>
        MasterAttribute957,
        /// <summary>
        /// 最大攻击力提高
        /// </summary>
        MasterAttribute958,
        /// <summary>
        /// 幸运一击几率提高
        /// </summary>
        MasterAttribute959,
        /// <summary>
        /// 魔力完全恢复
        /// </summary>
        MasterAttribute960,
        /// <summary>
        /// 生命力完全恢复
        /// </summary>
        MasterAttribute961,
        /// <summary>
        /// 生命吸收
        /// </summary>
        MasterAttribute962,
        /// <summary>
        /// 卓越一击几率提高
        /// </summary>
        MasterAttribute963,
        /// <summary>
        /// SD完全恢复
        /// </summary>
        MasterAttribute964,
        /// <summary>
        /// 双倍攻击几率提高
        /// </summary>
        MasterAttribute965,
        /// <summary>
        /// 惩戒攻击力提高
        /// </summary>
        MasterAttribute966,
        /// <summary>
        /// 激怒攻击力提高
        /// </summary>
        MasterAttribute967,
        /// <summary>
        /// 吸收SD
        /// </summary>
        MasterAttribute968,
        /// <summary>
        /// 无视防御几率提高
        /// </summary>
        MasterAttribute969,
        #endregion


        #region 共有
        Common2001 = 1 + BattleMasteryConstData.CommonId,
        Common2002 = 2 + BattleMasteryConstData.CommonId,
        Common2003 = 3 + BattleMasteryConstData.CommonId,
        Common2004 = 4 + BattleMasteryConstData.CommonId,
        Common2005 = 5 + BattleMasteryConstData.CommonId,
        Common2006 = 6 + BattleMasteryConstData.CommonId,
        Common2007 = 7 + BattleMasteryConstData.CommonId,
        Common2008 = 8 + BattleMasteryConstData.CommonId,
        Common2009 = 9 + BattleMasteryConstData.CommonId,
        Common2010 = 10 + BattleMasteryConstData.CommonId,
        Common2011 = 11 + BattleMasteryConstData.CommonId,
        Common2012 = 12 + BattleMasteryConstData.CommonId,
        Common2013 = 13 + BattleMasteryConstData.CommonId,
        #endregion
        #region 通用
        BeCommon3001 = 1 + BattleMasteryConstData.BeCommonId,
        BeCommon3002 = 2 + BattleMasteryConstData.BeCommonId,
        BeCommon3003 = 3 + BattleMasteryConstData.BeCommonId,
        BeCommon3004 = 4 + BattleMasteryConstData.BeCommonId,
        BeCommon3005 = 5 + BattleMasteryConstData.BeCommonId,
        BeCommon3006 = 6 + BattleMasteryConstData.BeCommonId,
        BeCommon3007 = 7 + BattleMasteryConstData.BeCommonId,
        BeCommon3008 = 8 + BattleMasteryConstData.BeCommonId,
        BeCommon3009 = 9 + BattleMasteryConstData.BeCommonId,
        BeCommon3010 = 10 + BattleMasteryConstData.BeCommonId,
        BeCommon3011 = 11 + BattleMasteryConstData.BeCommonId,
        BeCommon3012 = 12 + BattleMasteryConstData.BeCommonId,
        BeCommon3013 = 13 + BattleMasteryConstData.BeCommonId,
        BeCommon3014 = 14 + BattleMasteryConstData.BeCommonId,
        BeCommon3015 = 15 + BattleMasteryConstData.BeCommonId,
        BeCommon3016 = 16 + BattleMasteryConstData.BeCommonId,
        BeCommon3017 = 17 + BattleMasteryConstData.BeCommonId,
        BeCommon3018 = 18 + BattleMasteryConstData.BeCommonId,
        BeCommon3019 = 19 + BattleMasteryConstData.BeCommonId,
        BeCommon3020 = 20 + BattleMasteryConstData.BeCommonId,
        BeCommon3021 = 21 + BattleMasteryConstData.BeCommonId,
        BeCommon3022 = 22 + BattleMasteryConstData.BeCommonId,
        BeCommon3023 = 23 + BattleMasteryConstData.BeCommonId,
        BeCommon3024 = 24 + BattleMasteryConstData.BeCommonId,
        BeCommon3025 = 25 + BattleMasteryConstData.BeCommonId,
        BeCommon3026 = 26 + BattleMasteryConstData.BeCommonId,
        BeCommon3027 = 27 + BattleMasteryConstData.BeCommonId,
        BeCommon3028 = 28 + BattleMasteryConstData.BeCommonId,
        BeCommon3029 = 29 + BattleMasteryConstData.BeCommonId,
        BeCommon3030 = 30 + BattleMasteryConstData.BeCommonId,
        BeCommon3031 = 31 + BattleMasteryConstData.BeCommonId,
        BeCommon3032 = 32 + BattleMasteryConstData.BeCommonId,
        BeCommon3033 = 33 + BattleMasteryConstData.BeCommonId,
        BeCommon3034 = 34 + BattleMasteryConstData.BeCommonId,
        BeCommon3035 = 35 + BattleMasteryConstData.BeCommonId,
        BeCommon3036 = 36 + BattleMasteryConstData.BeCommonId,
        #endregion
        /// <summary>
        /// MAX
        /// </summary>
        MAX = 1000,
    }
}
