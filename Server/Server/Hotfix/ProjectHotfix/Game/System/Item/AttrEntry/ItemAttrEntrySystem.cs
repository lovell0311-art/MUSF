using System;
using System.Collections.Generic;
using System.Text;
using ETModel;

namespace ETHotfix
{
    public static class ItemAttrEntrySystem
    {
        /// <summary>
        /// 应用属性到 GamePlayer
        /// </summary>
        /// <param name="self"></param>
        /// <param name="player"></param>
        public static void ApplyPropTo(this ItemAttrEntry self,GamePlayer gamePlayer,float pct = 1f,bool IsExcellent = false)
        {
            int value = 0;
            if(self.WillWeaken == false)
            {
                // 属性不会衰减
                pct = 1f;
            }
            if (IsExcellent)
                value = self.Value + self.AppendValue;
            else 
                value = self.Value;

            switch (self.PropId)
            {
                case EItemAttrEntryPropId.AddExcellentAttackRate:
                    gamePlayer.AddEquipProperty(E_GameProperty.ExcellentAttackRate, (int)(value * 100 * pct));
                    break;
                case EItemAttrEntryPropId.AddAttackDamageByLevel_20:
                    value = gamePlayer.Data.Level / 20;
                    gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, value);
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, value);
                    gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, value);
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, value);
                    break;
                case EItemAttrEntryPropId.AddAttackDamagePct:
                    gamePlayer.AddEquipProperty(E_GameProperty.MinAtteckPct, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteckPct, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteckPct, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteckPct, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddAttackSpeed:
                    gamePlayer.AddEquipProperty(E_GameProperty.AttackSpeed, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.KillMonsterReplyHp_8:
                    gamePlayer.AddEquipProperty(E_GameProperty.KillMonsterReplyHp_8, 1);
                    break;
                case EItemAttrEntryPropId.KillMonsterReplyMp_8:
                    gamePlayer.AddEquipProperty(E_GameProperty.KillMonsterReplyMp_8, 1);
                    break;
                case EItemAttrEntryPropId.AddMaxHpPct:
                    gamePlayer.AddEquipProperty(E_GameProperty.PROP_HP_MAXPct, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddMaxMpPct:
                    gamePlayer.AddEquipProperty(E_GameProperty.PROP_MP_MAXPct, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddDamageMinusPct:
                    gamePlayer.AddEquipProperty(E_GameProperty.InjuryValueRate_Reduce, (int)(value * 100 * pct));
                    break;
                case EItemAttrEntryPropId.AddDamageReflectPct:
                    gamePlayer.AddEquipProperty(E_GameProperty.BackInjuryRate, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddDefenseSuccessRatePct:
                    gamePlayer.AddEquipProperty(E_GameProperty.DefenseRatePct, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddkillMonsterReveiceGoldPct:
                    gamePlayer.AddEquipProperty(E_GameProperty.AddGoldCoinRate_Increase, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddMinAttackDamage:
                    gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddMaxAttackDamage:
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.ReducedStrengthRequire:
                case EItemAttrEntryPropId.ReducedDexterityRequire:
                    break;
                case EItemAttrEntryPropId.AddAttackDamage:
                    gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddCriticalDamage:
                    gamePlayer.AddEquipProperty(E_GameProperty.LucklyAttackHurtValueIncrease, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.ExcellentAttackHurtValueIncrease, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddMagicPower:
                case EItemAttrEntryPropId.AddSkillAttack:
                    gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddAttackSuccessRatePVP:
                    gamePlayer.AddEquipProperty(E_GameProperty.PVPAtteckSuccessRate, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.DecreaseSDRate:
                    gamePlayer.AddEquipProperty(E_GameProperty.AttackSdRate, (int)(value * 100 * pct));
                    break;
                case EItemAttrEntryPropId.AddIgnoreSDRate:
                    gamePlayer.AddEquipProperty(E_GameProperty.SDAttackIgnoreRate, (int)(value * 100 * pct));
                    break;
                case EItemAttrEntryPropId.AddDefense:
                    gamePlayer.AddEquipProperty(E_GameProperty.Defense, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddDefensePct:
                    gamePlayer.AddEquipProperty(E_GameProperty.DefensePct, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddMaxAGPct:
                    gamePlayer.AddEquipProperty(E_GameProperty.PROP_AG_MAXPct, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddRefillHP:
                    gamePlayer.AddEquipProperty(E_GameProperty.ReplyHp, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddRefillMP:
                    gamePlayer.AddEquipProperty(E_GameProperty.ReplyMp, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddDefenseSuccessRatePvP:
                    gamePlayer.AddEquipProperty(E_GameProperty.PVPDefenseRate, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddSDRate:
                    gamePlayer.AddEquipProperty(E_GameProperty.HitSdRate, (int)(value * 100 * pct));
                    break;
                case EItemAttrEntryPropId.AddDefenseSuccessRate:
                    gamePlayer.AddEquipProperty(E_GameProperty.DefenseRate, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddRefillHPPct:
                    gamePlayer.AddEquipProperty(E_GameProperty.ReplyHpRate, (int)(value * 100 * pct));
                    break;
                case EItemAttrEntryPropId.AddAllAttackDamage:
                    gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, (int)(value * pct));

                    gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, (int)(value * pct));

                    gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteck, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteck, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddAllAttackDamagePct:
                    gamePlayer.AddEquipProperty(E_GameProperty.MinAtteckPct, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteckPct, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteckPct, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteckPct, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteckPct, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteckPct, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddMaxHp:
                    gamePlayer.AddEquipProperty(E_GameProperty.PROP_HP_MAX, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.DamageAbsPct_Guard:
                    gamePlayer.AddEquipProperty(E_GameProperty.DamageAbsPct_Guard, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.DamageAbsPct_Mounts:
                    gamePlayer.AddEquipProperty(E_GameProperty.DamageAbsPct_Mounts, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.DamageIncreasePct:
                    gamePlayer.AddEquipProperty(E_GameProperty.InjuryValueRate_Increase, (int)(value * 100 * pct));
                    break;
                case EItemAttrEntryPropId.DamageAbsPct_Wing:
                    gamePlayer.AddEquipProperty(E_GameProperty.DamageAbsPct_Wing, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.DamageAbsPct_Pets:
                    gamePlayer.AddEquipProperty(E_GameProperty.DamageAbsPct_Pets, (int)(self.Value * pct));
                    break;
                case EItemAttrEntryPropId.AddReallyDefense:
                    gamePlayer.AddEquipProperty(E_GameProperty.ReallyDefense, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddAttackIgnoreDefenseRate:
                    gamePlayer.AddEquipProperty(E_GameProperty.AttackIgnoreDefenseRate, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddDamageMinus:
                    gamePlayer.AddEquipProperty(E_GameProperty.InjuryValue_Reduce, (int)(value * pct));
                    break;

                case EItemAttrEntryPropId.AddMaxMp:
                    gamePlayer.AddEquipProperty(E_GameProperty.PROP_HP_MAX, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddMaxAg:
                    gamePlayer.AddEquipProperty(E_GameProperty.PROP_AG_MAX, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddMaxSd:
                    gamePlayer.AddEquipProperty(E_GameProperty.PROP_SD_MAX, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddLuckyAttackRate:
                    gamePlayer.AddEquipProperty(E_GameProperty.LucklyAttackRate, (int)(value * 100 * pct));
                    break;
                case EItemAttrEntryPropId.AddDoubleAttackRate:
                    gamePlayer.AddEquipProperty(E_GameProperty.InjuryValueRate_2, (int)(value * 100 * pct));
                    break;
                case EItemAttrEntryPropId.AddTripleAttackRate:
                    gamePlayer.AddEquipProperty(E_GameProperty.InjuryValueRate_3, (int)(value * 100 * pct));
                    break;
                case EItemAttrEntryPropId.AddExcellentAttackDamage:
                    gamePlayer.AddEquipProperty(E_GameProperty.ExcellentAttackHurtValueIncrease, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddLuckyAttackDamage:
                    gamePlayer.AddEquipProperty(E_GameProperty.LucklyAttackHurtValueIncrease, (int)(value * pct));
                    break;

                case EItemAttrEntryPropId.AddMagicPct:
                    gamePlayer.AddEquipProperty(E_GameProperty.MagicRate_Increase, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddAttackMagicDamage:
                    gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, (int)(value * pct));

                    gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddMagicCurseDamage:
                    gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, (int)(value * pct));

                    gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteck, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteck, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddAttackSuccessRate:
                    gamePlayer.AddEquipProperty(E_GameProperty.AtteckSuccessRate, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddReboundAttackRate:
                    gamePlayer.AddEquipProperty(E_GameProperty.ReboundRate, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddInjury_ReplyAllHpRate:
                    gamePlayer.AddEquipProperty(E_GameProperty.Injury_ReplyAllHpRate, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddInjury_ReplyAllMpRate:
                    gamePlayer.AddEquipProperty(E_GameProperty.Injury_ReplyAllMpRate, (int)(value * pct));
                    break;

                case EItemAttrEntryPropId.AddStrength:
                    gamePlayer.AddEquipProperty(E_GameProperty.Property_Strength, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddWillpower:
                    gamePlayer.AddEquipProperty(E_GameProperty.Property_Willpower, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddAgility:
                    gamePlayer.AddEquipProperty(E_GameProperty.Property_Agility, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddBoneGas:
                    gamePlayer.AddEquipProperty(E_GameProperty.Property_BoneGas, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddCommand:
                    gamePlayer.AddEquipProperty(E_GameProperty.Property_Command, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddAllAttr:
                    gamePlayer.AddEquipProperty(E_GameProperty.Property_Strength, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.Property_Willpower, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.Property_Agility, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.Property_BoneGas, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.Property_Command, (int)(value * pct));
                    break;

                case EItemAttrEntryPropId.AddIceResistance:
                    gamePlayer.AddEquipProperty(E_GameProperty.IceResistance, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddPoisonResistance:
                    gamePlayer.AddEquipProperty(E_GameProperty.CurseResistance, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddFireResistance:
                    gamePlayer.AddEquipProperty(E_GameProperty.FireResistance, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.AddLightningResistance:
                    gamePlayer.AddEquipProperty(E_GameProperty.ThunderResistance, (int)(value * pct));
                    break;

                case EItemAttrEntryPropId.AddKillMonsterExpPct:
                    gamePlayer.AddEquipProperty(E_GameProperty.ExperienceBonus, (int)(value * pct));
                    break;

                case EItemAttrEntryPropId.DamageIncreasePctWhenEquipTwoHandSword:
                    {
                        EquipmentComponent equipmentCom = gamePlayer.Player.GetCustomComponent<EquipmentComponent>();
                        Item weapon = equipmentCom.GetEquipItemByPosition(EquipPosition.Weapon);
                        if (weapon == null) break;
                        if (weapon.ConfigData.TwoHand != 1) break;
                        gamePlayer.AddEquipProperty(E_GameProperty.InjuryValueRate_Increase, (int)(value * 100 * pct));
                    }
                    break;
                case EItemAttrEntryPropId.IgnoreAbsorbRatePct:
                    gamePlayer.AddEquipProperty(E_GameProperty.IgnoreAbsorbRate, (int)(value * 100 * pct));
                    break;
                case EItemAttrEntryPropId.AttackIgnoreAbsorbRatePct:
                    gamePlayer.AddEquipProperty(E_GameProperty.AttackIgnoreAbsorbRate, (int)(value * 100 * pct));
                    break;
                case EItemAttrEntryPropId.RestorePlayerHealth:
                    gamePlayer.AddEquipProperty(E_GameProperty.PROP_HP_MAX, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.RestorePlayerHealthPct:
                    gamePlayer.AddEquipProperty(E_GameProperty.PROP_HP_MAXPct, (int)(value * 100 * pct));
                    break;
                case EItemAttrEntryPropId.DisregardHarmReduction:
                    gamePlayer.AddEquipProperty(E_GameProperty.DisregardHarmReductionPct, (int)(value * 100 * pct));
                    break;
                //血脉属性关系
                case EItemAttrEntryPropId.BloodMinATK:
                    gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteck, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.MinDamnationAtteck, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.BloodMaxATK:
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteck, (int)(value * pct));
                    gamePlayer.AddEquipProperty(E_GameProperty.MaxDamnationAtteck, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.BloodGoldIncrease:
                    gamePlayer.AddEquipProperty(E_GameProperty.AddGoldCoinRate_Increase, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.BloodDoubleDamage:
                    gamePlayer.AddEquipProperty(E_GameProperty.InjuryValueRate_2, (int)(value * 100 * pct));
                    break;
                case EItemAttrEntryPropId.BloodMaxMagic:
                    gamePlayer.AddEquipProperty(E_GameProperty.PROP_MP_MAX, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.BloodSkill:
                    gamePlayer.AddEquipProperty(E_GameProperty.EmbedSkillAttack, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.BloodDefensePct:
                    gamePlayer.AddEquipProperty(E_GameProperty.DefenseRate, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.BloodHpAutoRecover:
                    gamePlayer.AddEquipProperty(E_GameProperty.ReplyHpRate, (int)(value * 100 * pct));
                    break;
                case EItemAttrEntryPropId.BloodTripleInjury:
                    gamePlayer.AddEquipProperty(E_GameProperty.InjuryValueRate_3, (int)(value * 100 * pct));
                    break;
                case EItemAttrEntryPropId.BloodRebound:
                    gamePlayer.AddEquipProperty(E_GameProperty.ReboundRate, (int)(value * 100 * pct));
                    break;
                case EItemAttrEntryPropId.BloddLuckyHit:
                    gamePlayer.AddEquipProperty(E_GameProperty.LucklyAttackRate, (int)(value * 100 * pct));
                    break;
                case EItemAttrEntryPropId.BloodGreatShot:
                    gamePlayer.AddEquipProperty(E_GameProperty.ExcellentAttackRate, (int)(value * 100 * pct));
                    break;
                case EItemAttrEntryPropId.BloodLuckyHitAdd:
                    gamePlayer.AddEquipProperty(E_GameProperty.LucklyAttackHurtValueIncrease, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.BloodGreatShotAdd:
                    gamePlayer.AddEquipProperty(E_GameProperty.ExcellentAttackHurtValueIncrease, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.BloodMPAutoRecover:
                    gamePlayer.AddEquipProperty(E_GameProperty.ReplyMp, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.BloodMagicAutoPct:
                    gamePlayer.AddEquipProperty(E_GameProperty.ReplyMpRate, (int)(value *100* pct));
                    break;
                case EItemAttrEntryPropId.BloodAGAutoAdd:
                    gamePlayer.AddEquipProperty(E_GameProperty.EmbedAGRecoveryUp, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.BloodAgAutoPct:
                    gamePlayer.AddEquipProperty(E_GameProperty.ReplyAGRate, (int)(value * 100 * pct));
                    break;
                case EItemAttrEntryPropId.BloodSDAutoAdd:
                    gamePlayer.AddEquipProperty(E_GameProperty.ReplySD, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.BloodSDAutoPct:
                    gamePlayer.AddEquipProperty(E_GameProperty.ReplySDRate, (int)(value *100 * pct));
                    break;
                case EItemAttrEntryPropId.BloodAtAttackTimeSDPct:
                    gamePlayer.AddEquipProperty(E_GameProperty.AttackSdRate, (int)(value * 100 * pct));
                    break;
                case EItemAttrEntryPropId.BloodBoundProbability:
                    gamePlayer.AddEquipProperty(E_GameProperty.ShacklesRate, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.BloodDKBoundProbability:
                    gamePlayer.AddEquipProperty(E_GameProperty.ShacklesResistanceRate, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.BloodShieldHurtAbsorb:
                    gamePlayer.AddEquipProperty(E_GameProperty.ShieldHurtAbsorb, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.BloodDefenseShieldRate:
                    gamePlayer.AddEquipProperty(E_GameProperty.DefenseShieldRate, (int)(value *100* pct));
                    break;
                case EItemAttrEntryPropId.BloodDisregardHarmReduction:
                    gamePlayer.AddEquipProperty(E_GameProperty.DisregardHarmReductionPct, (int)(value * 100 * pct));
                    break;
                case EItemAttrEntryPropId.BloodKillAutoHp:
                    gamePlayer.AddEquipProperty(E_GameProperty.KillEnemyReplyHpRate, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.BloodKillAutoMp:
                    gamePlayer.AddEquipProperty(E_GameProperty.KillEnemyReplyMpRate, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.BloodKillAutoAg:
                    gamePlayer.AddEquipProperty(E_GameProperty.KillEnemyReplyAGRate, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.BloodKillAutoSD:
                    gamePlayer.AddEquipProperty(E_GameProperty.KillEnemyReplySDRate, (int)(value * pct));
                    break;
                case EItemAttrEntryPropId.BloodAutoHpOver:
                    gamePlayer.AddEquipProperty(E_GameProperty.Injury_ReplyAllHpRate, (int)(value * pct));
                    break;
            }

        }
    }
}
