using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{



    public static partial class GamePlayerSystem
    {
        /// <summary>
        /// 接口不在获取金币
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="b_GameProperty"></param>
        /// <param name="b_HasTemporary"></param>
        /// <returns></returns>
        public static int GetNumerial(this GamePlayer b_Component, E_GameProperty b_GameProperty, bool b_HasTemporary = true)
        {
            if (b_GameProperty.IsChangeTypePlayer())
            {
                return b_Component.GetNumerialSpecial(b_GameProperty);
            }
            return b_Component.ResetNumerial(b_GameProperty, b_HasTemporary);

            if (b_Component.ChangePropertyDic.TryGetValue(b_GameProperty, out var mChangeProperty))
            {
                if (mChangeProperty.Item1)
                {
                    return mChangeProperty.Item2;
                }
            }
            else
            {
                mChangeProperty = b_Component.ChangePropertyDic[b_GameProperty] = (false, 0);
            }

            int mResult = b_Component.ResetNumerial(b_GameProperty, b_HasTemporary);

            mChangeProperty.Item1 = true;
            mChangeProperty.Item2 = mResult;
            return mResult;
        }



        public static int ResetNumerial(this GamePlayer b_Component, E_GameProperty b_GameProperty, bool b_HasTemporary = true)
        {
            int mResult = 0;

            //基础
            if (b_Component.GamePropertyDic.TryGetValue(b_GameProperty, out var mGamePropertyValue))
            {
                mResult += mGamePropertyValue;
            }
            // 装备属性应用
            if (b_Component.EquipPropertyDic.TryGetValue(b_GameProperty, out int mEquipPropertyValue))
            {
                mResult += mEquipPropertyValue;
            }

            switch (b_GameProperty)
            {
                case E_GameProperty.Property_Strength:
                    {
                        mResult += b_Component.Config.Strength + b_Component.Data.Strength + b_Component.Data.Strength2;
                    }
                    break;
                case E_GameProperty.Property_Willpower:
                    {
                        mResult += b_Component.Config.Willpower + b_Component.Data.Willpower + b_Component.Data.Willpower2;
                    }
                    break;
                case E_GameProperty.Property_Agility:
                    {
                        mResult += b_Component.Config.Agility + b_Component.Data.Agility + b_Component.Data.Agility2;
                    }
                    break;
                case E_GameProperty.Property_BoneGas:
                    {
                        mResult += b_Component.Config.BoneGas + b_Component.Data.BoneGas + b_Component.Data.BoneGas2;
                    }
                    break;
                case E_GameProperty.Property_Command:
                    {
                        mResult += b_Component.Config.Command + b_Component.Data.Command + b_Component.Data.Command2;
                    }
                    break;
                default:
                    break;
            }

            if (b_Component.GamePropertyNumerialDic.TryGetValue(b_GameProperty, out var GamePropertyNumerial))
            {
                mResult += GamePropertyNumerial.Run(b_Component, b_HasTemporary);
            }


            //if (b_Component.Data.Level >= 400 && b_Component.Data.OccupationLevel >= 3)
            //{// 大师
            //    mResult += Numerial_master(b_Component, b_GameProperty, b_HasTemporary);
            //}

            //// buff
            //if (b_HasTemporary)
            //{
            //    mResult += Numerial_skill(b_Component, b_GameProperty);
            //}

            void ApplyPropPct(ref int value, E_GameProperty propId)
            {
                int pct = b_Component.GetNumerial(propId);
                if (value != 0 && pct != 0)
                {
                    pct += 100;
                    value = (int)(mResult * pct * 0.01f);
                }
            }

            // 二级属性应用
            switch (b_GameProperty)
            {
                case E_GameProperty.PROP_HP_MAX:
                    ApplyPropPct(ref mResult, E_GameProperty.PROP_HP_MAXPct);
                    break;
                case E_GameProperty.PROP_MP_MAX:
                    ApplyPropPct(ref mResult, E_GameProperty.PROP_MP_MAXPct);
                    break;
                case E_GameProperty.PROP_SD_MAX:
                    ApplyPropPct(ref mResult, E_GameProperty.PROP_SD_MAXPct);
                    break;
                case E_GameProperty.PROP_AG_MAX:
                    ApplyPropPct(ref mResult, E_GameProperty.PROP_AG_MAXPct);
                    break;
                case E_GameProperty.MaxAtteck:
                    ApplyPropPct(ref mResult, E_GameProperty.MaxAtteckPct);
                    break;
                case E_GameProperty.MinAtteck:
                    ApplyPropPct(ref mResult, E_GameProperty.MinAtteckPct);
                    break;
                case E_GameProperty.MaxMagicAtteck:
                    ApplyPropPct(ref mResult, E_GameProperty.MaxMagicAtteckPct);
                    break;
                case E_GameProperty.MinMagicAtteck:
                    ApplyPropPct(ref mResult, E_GameProperty.MinMagicAtteckPct);
                    break;
                case E_GameProperty.MaxElementAtteck:
                    ApplyPropPct(ref mResult, E_GameProperty.MaxElementAtteckPct);
                    break;
                case E_GameProperty.MinElementAtteck:
                    ApplyPropPct(ref mResult, E_GameProperty.MinElementAtteckPct);
                    break;
                case E_GameProperty.AtteckSuccessRate:
                    ApplyPropPct(ref mResult, E_GameProperty.AtteckSuccessRatePct);
                    break;
                case E_GameProperty.PVPAtteckSuccessRate:
                    ApplyPropPct(ref mResult, E_GameProperty.PVPAtteckSuccessRatePct);
                    break;
                case E_GameProperty.ElementAtteckSuccessRate:
                    ApplyPropPct(ref mResult, E_GameProperty.ElementAtteckSuccessRatePct);
                    break;
                case E_GameProperty.PVPElementAtteckSuccessRate:
                    ApplyPropPct(ref mResult, E_GameProperty.PVPElementAtteckSuccessRatePct);
                    break;
                case E_GameProperty.DefenseRate:
                    ApplyPropPct(ref mResult, E_GameProperty.DefenseRatePct);
                    break;
                case E_GameProperty.ElementDefenseRate:
                    ApplyPropPct(ref mResult, E_GameProperty.ElementDefenseRatePct);
                    break;
                case E_GameProperty.PVPDefenseRate:
                    ApplyPropPct(ref mResult, E_GameProperty.PVPDefenseRatePct);
                    break;
                case E_GameProperty.PVPElementDefenseRate:
                    ApplyPropPct(ref mResult, E_GameProperty.PVPElementDefenseRatePct);
                    break;
                case E_GameProperty.Defense:
                    ApplyPropPct(ref mResult, E_GameProperty.DefensePct);
                    break;
                case E_GameProperty.ElementDefense:
                    ApplyPropPct(ref mResult, E_GameProperty.ElementDefensePct);
                    break;
                case E_GameProperty.MaxDamnationAtteck:
                    ApplyPropPct(ref mResult, E_GameProperty.MaxDamnationAtteckPct);
                    break;
                case E_GameProperty.MinDamnationAtteck:
                    ApplyPropPct(ref mResult, E_GameProperty.MinDamnationAtteckPct);
                    break;
            }

            // 限制 上限或者后计算
            // 修正
            switch (b_GameProperty)
            {
                case E_GameProperty.MinAtteck:
                    {
                        var mMaxAtteck = b_Component.GetNumerialFunc(E_GameProperty.MaxAtteck);
                        if (mResult > mMaxAtteck)
                        {
                            mResult = mMaxAtteck;
                        }
                    }
                    break;
                case E_GameProperty.MinMagicAtteck:
                    {
                        var mMaxMagicAtteck = b_Component.GetNumerialFunc(E_GameProperty.MaxMagicAtteck);
                        if (mResult > mMaxMagicAtteck)
                        {
                            mResult = mMaxMagicAtteck;
                        }
                    }
                    break;
                case E_GameProperty.MinElementAtteck:
                    {
                        var mMaxElementAtteck = b_Component.GetNumerialFunc(E_GameProperty.MaxElementAtteck);
                        if (mResult > mMaxElementAtteck)
                        {
                            mResult = mMaxElementAtteck;
                        }
                    }
                    break;
                case E_GameProperty.Defense:
                    {
                        // 在身上有技能buffer 魔剑士技能玄月斩 1-50%防御力
                        if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.XuanYueZhan321, out var mTempBuffer))
                        {
                            if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                            {
                                int value = 100 - mTempBufferData.StrengthValue;
                                mResult = (int)(mResult * value * 0.01f);
                            }
                        }
                    }
                    break;
                case E_GameProperty.AttackSpeed:
                    {
                        int mResultLimit = GetAttackSpeedMax((E_GameOccupation)b_Component.Data.PlayerTypeId);
                        if (mResult > mResultLimit) mResult = mResultLimit;
                    }
                    break;
                case E_GameProperty.FangYuHuZhao:
                    {
                        if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.FangYuHuZhao, out var mTempBuffer))
                        {
                            if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                            {
                                var mTempBufferValue = mTempBufferData.CacheData[0];
                                mResult += mTempBufferValue;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            return mResult;
        }

        public static int GetNumerialSpecial(this GamePlayer b_Component, E_GameProperty b_GameProperty)
        {
            int mResult = 0;

            switch (b_GameProperty)
            {
                case E_GameProperty.PROP_HP:
                    {
                        mResult = b_Component.UnitData.Hp;
                        var mHPMax = b_Component.GetNumerial(E_GameProperty.PROP_HP_MAX);
                        if (mResult > mHPMax)
                        {
                            mResult = b_Component.UnitData.Hp = mHPMax;
                        }
                    }
                    break;
                case E_GameProperty.PROP_MP:
                    {
                        mResult = b_Component.UnitData.Mp;
                        var mMPMax = b_Component.GetNumerial(E_GameProperty.PROP_MP_MAX);
                        if (mResult > mMPMax)
                        {
                            mResult = b_Component.UnitData.Mp = mMPMax;
                        }
                    }
                    break;
                case E_GameProperty.PROP_AG:
                    {
                        mResult = b_Component.UnitData.AG;
                        var mAGMax = b_Component.GetNumerial(E_GameProperty.PROP_AG_MAX);
                        if (mResult > mAGMax)
                        {
                            mResult = b_Component.UnitData.AG = mAGMax;
                        }
                    }
                    break;
                case E_GameProperty.PROP_SD:
                    {
                        mResult = b_Component.UnitData.SD;
                        var mSDMax = b_Component.GetNumerial(E_GameProperty.PROP_SD_MAX);
                        if (mResult > mSDMax)
                        {
                            mResult = b_Component.UnitData.SD = mSDMax;
                        }
                    }
                    break;
                case E_GameProperty.AttackDistance:
                    {
                        mResult = b_Component.Config.AttackDistance;

                        switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
                        {
                            case E_GameOccupation.Archer:
                                {
                                    var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                    if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                    {
                                        switch (mWeaponEquipment.Type)
                                        {
                                            case EItemType.Bows:
                                            case EItemType.Crossbows:
                                                {
                                                    mResult += 6;
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case E_GameProperty.Level:
                    {
                        mResult = (int)b_Component.Data.Level;
                    }
                    break;
                case E_GameProperty.OccupationLevel:
                    {
                        mResult = (int)b_Component.Data.OccupationLevel;
                    }
                    break;
                case E_GameProperty.FreePoint:
                    {
                        mResult = b_Component.Data.FreePoint;
                    }
                    break;
                case E_GameProperty.Exprience:
                    {
                        mResult = (int)b_Component.Data.Exp;
                    }
                    break;
                //case E_GameProperty.GoldCoin:
                //    {
                //        mResult = b_Component.Data.GoldCoin;
                //    }
                //    break;
                case E_GameProperty.MiracleCoin:
                    {
                        mResult = b_Component.Data.MiracleCoin;
                    }
                    break;
                case E_GameProperty.YuanbaoCoin:
                    {
                        mResult = b_Component.Player.Data.YuanbaoCoin;
                    }
                    break;
                case E_GameProperty.PkNumber:
                    {
                        mResult = b_Component.UnitData.PkPoint;
                    }
                    break;
                default:
                    break;
            }

            return mResult;
        }

        public static int GetAttackSpeedMax(this E_GameOccupation b_GameOccupation)
        {
            int mResultLimit = 280;
            switch (b_GameOccupation)
            {
                case E_GameOccupation.Spell:
                    mResultLimit = 284;
                    break;
                case E_GameOccupation.Swordsman:
                    mResultLimit = 288;
                    break;
                case E_GameOccupation.Archer:
                    mResultLimit = 275;
                    break;
                case E_GameOccupation.Spellsword:
                    mResultLimit = 351;
                    break;
                case E_GameOccupation.Holyteacher:
                    mResultLimit = 450;
                    break;
                case E_GameOccupation.SummonWarlock:
                    mResultLimit = 188;
                    break;
                case E_GameOccupation.Combat:
                    mResultLimit = 441;
                    break;
                case E_GameOccupation.GrowLancer:
                    mResultLimit = 273;
                    break;
                default:
                    mResultLimit = 280;
                    break;
            }
            return mResultLimit;
        }

        public static void EmbedApplyPropTo(this GamePlayer self, int Id, int Value)
        {
            int AddValue = 0;
            switch (Id)
            {
                case (int)E_GameProperty.EmbedAttack20:
                    {
                        AddValue = (self.Data.Level / 20) * (Value / 10000);
                        self.AddEquipProperty(E_GameProperty.MinAtteck, AddValue);
                        self.AddEquipProperty(E_GameProperty.MaxAtteck, AddValue);
                        self.AddEquipProperty(E_GameProperty.MinMagicAtteck, AddValue);
                        self.AddEquipProperty(E_GameProperty.MaxMagicAtteck, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedAttackSpeed:
                    {
                        AddValue = Value / 10000;
                        self.AddEquipProperty(E_GameProperty.AttackSpeed, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedAttackMaxUp:
                    {
                        AddValue = Value / 10000;
                        self.AddEquipProperty(E_GameProperty.MaxAtteck, AddValue);
                        self.AddEquipProperty(E_GameProperty.MaxMagicAtteck, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedAttackMinUp:
                    {
                        AddValue = Value / 10000;
                        self.AddEquipProperty(E_GameProperty.MinAtteck, AddValue);
                        self.AddEquipProperty(E_GameProperty.MinMagicAtteck, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedAttack:
                    {
                        AddValue = Value / 10000;
                        self.AddEquipProperty(E_GameProperty.MinAtteck, AddValue);
                        self.AddEquipProperty(E_GameProperty.MaxAtteck, AddValue);
                        self.AddEquipProperty(E_GameProperty.MinMagicAtteck, AddValue);
                        self.AddEquipProperty(E_GameProperty.MaxMagicAtteck, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedAGDecrease:
                    {
                        AddValue = Value / 100;
                        self.AddEquipProperty(E_GameProperty.EmbedAGDecrease, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedDefRateUp:
                    {
                        AddValue = Value / 100;
                        self.AddEquipProperty(E_GameProperty.DefenseRate, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedDefUp:
                    {
                        AddValue = Value / 10000;
                        self.AddEquipProperty(E_GameProperty.Defense, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedShieldDefUp:
                    {
                        AddValue = Value / 10000;
                        var Item = self.Player.GetCustomComponent<EquipmentComponent>().GetEquipItemByPosition(EquipPosition.Shield);
                        if (Item != null)
                        {
                            int NewValue = Item.GetProp(EItemValue.Defense);
                            NewValue *= AddValue;
                            self.AddEquipProperty(E_GameProperty.Defense, NewValue);
                        }
                    }
                    break;
                case (int)E_GameProperty.EmbedHarmReduction:
                    {
                        AddValue = Value;
                        self.AddEquipProperty(E_GameProperty.InjuryValueRate_Reduce, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedNociceptiveReflex:
                    {
                        AddValue = Value / 100;
                        self.AddEquipProperty(E_GameProperty.BackInjuryRate, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedIncreaseLife:
                    {
                        AddValue = Value / 10000;
                        self.AddEquipProperty(E_GameProperty.EmbedIncreaseLife, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedIncreaseMagic:
                    {
                        AddValue = Value / 10000;
                        self.AddEquipProperty(E_GameProperty.EmbedIncreaseMagic, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedSkillAttack:
                    {
                        AddValue = Value / 10000;
                        self.AddEquipProperty(E_GameProperty.EmbedSkillAttack, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedAttackRateUp:
                    {
                        AddValue = Value / 10000;
                        self.AddEquipProperty(E_GameProperty.AtteckSuccessRate, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedDurabilityUp:
                    {
                        AddValue = Value / 100;
                        self.AddEquipProperty(E_GameProperty.MpsDownDur1, AddValue);
                        self.AddEquipProperty(E_GameProperty.MpsDownDur2, AddValue);
                        self.AddEquipProperty(E_GameProperty.MpsDownDur3, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedHpSelfRecovery:
                    {
                        AddValue = Value / 10000;
                        self.AddEquipProperty(E_GameProperty.ReplyHp, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedMaxHPUp:
                    {
                        AddValue = Value / 10000;
                        self.AddEquipProperty(E_GameProperty.PROP_HP_MAX, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedMaxHMUp:
                    {
                        AddValue = Value / 10000;
                        self.AddEquipProperty(E_GameProperty.PROP_MP_MAX, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedHmSelfRecovery:
                    {
                        AddValue = Value / 10000;
                        self.AddEquipProperty(E_GameProperty.ReplyMp, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedMaxAGUp:
                    {
                        AddValue = Value / 10000;
                        self.AddEquipProperty(E_GameProperty.PROP_AG_MAX, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedAGRecoveryUp:
                    {
                        AddValue = Value / 10000;
                        self.AddEquipProperty(E_GameProperty.EmbedAGRecoveryUp, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedGreatShotAttack:
                    {
                        AddValue = Value / 10000;
                        self.AddEquipProperty(E_GameProperty.EmbedGreatShotAttack, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedGreatShotRateUp:
                    {
                        AddValue = Value;// 100;
                        self.AddEquipProperty(E_GameProperty.ExcellentAttackRate, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedLuckyStrokeAttack:
                    {
                        AddValue = Value / 10000;
                        self.AddEquipProperty(E_GameProperty.EmbedLuckyStrokeAttack, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedLuckyStrokeRateUp:
                    {
                        {
                            AddValue = Value;// 100;
                            self.AddEquipProperty(E_GameProperty.LucklyAttackRate, AddValue);
                        }
                    }
                    break;
                case (int)E_GameProperty.EmbedStrengthUP:
                    {
                        AddValue = Value / 10000;
                        self.AddEquipProperty(E_GameProperty.Property_Strength, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedAgilityUP:
                    {
                        AddValue = Value / 10000;
                        self.AddEquipProperty(E_GameProperty.Property_Agility, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedStaminaUP:
                    {
                        AddValue = Value / 10000;
                        self.AddEquipProperty(E_GameProperty.Property_BoneGas, AddValue);
                    }
                    break;
                case (int)E_GameProperty.EmbedIntelligenceUP:
                    {
                        AddValue = Value / 10000;
                        self.AddEquipProperty(E_GameProperty.Property_Willpower, AddValue);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}