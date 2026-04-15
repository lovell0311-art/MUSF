
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 
    /// </summary>
    [PropertyNumerialAttribute(BindId = (int)E_GameProperty.AttackSpeed)]
    public partial class C_PropertyNumerial_AttackSpeed : C_PropertyNumerial
    {
        public override int Run(GamePlayer b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;
            var mGameProperty = E_GameProperty.AttackSpeed;

            //if (b_Component.Data.PlayerTypeId == (int)E_GameOccupation.Holyteacher)
            //{
            //    if (b_Component.HolyteacherSummoned != null && b_Component.HolyteacherSummoned.IsDeath == false)
            //    {
            //        //mResult += 10;

            //        if (b_Component.HolyteacherSummoned.GamePropertyDic.TryGetValue(mGameProperty, out var mPropertyValue))
            //        {
            //            mResult += mPropertyValue;
            //        }
            //    }
            //}

            if (b_HasTemporary)
            {
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310018, out var mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult += mTempBufferValue;
                    }
                }

                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.KuangBaoShu509, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        if (mTempBufferData.CacheData.TryGetValue((int)mGameProperty, out var mTempValue))
                        {
                            mResult += mTempValue;
                        }
                    }
                }
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310127, out var mTempBuffer1))
                {
                    if (mTempBuffer1.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult += mTempBufferValue;
                    }
                }
            }

            if (b_Component.Data.Level >= 400 && b_Component.Data.OccupationLevel >= 3)
            {
                {
                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Common2005, out var mMasteryValue))
                    {
                        mResult += mMasteryValue;
                    }
                }
                switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
                {
                    case E_GameOccupation.None:
                        break;
                    case E_GameOccupation.Spell:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.DanShouJian_Master50, out var mMasteryValue))
                            {
                                bool mNeedbreak = false;
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 单手杖精通
                                    if (mWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 0)
                                    {
                                        mResult += mMasteryValue;
                                        mNeedbreak = true;
                                    }
                                }
                                if (mNeedbreak == false && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
                                {
                                    // 单手杖精通
                                    if (mShieldWeaponEquipment.Type == EItemType.Staffs && mShieldWeaponEquipment.ConfigData.TwoHand == 0)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }
                        }
                        break;
                    case E_GameOccupation.Swordsman:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.DanShouJian_master149, out var mMasteryValue))
                            {
                                bool mNeedbreak = false;
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 单手剑精通
                                    if (mWeaponEquipment.Type == EItemType.Swords && mWeaponEquipment.ConfigData.TwoHand == 0)
                                    {
                                        mResult += mMasteryValue;
                                        mNeedbreak = true;
                                    }
                                }
                                if (mNeedbreak == false && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
                                {
                                    // 单手剑精通
                                    if (mShieldWeaponEquipment.Type == EItemType.Swords && mShieldWeaponEquipment.ConfigData.TwoHand == 0)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }
                        }
                        break;
                    case E_GameOccupation.Archer:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Arch_Master257, out var mMasteryValue))
                            {
                                bool mNeedbreak = false;
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 弓类精通
                                    if (mWeaponEquipment.Type == EItemType.Bows)
                                    {
                                        mResult += mMasteryValue;
                                        mNeedbreak = true;
                                    }
                                }
                                if (mNeedbreak == false && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
                                {
                                    // 弓类精通
                                    if (mShieldWeaponEquipment.Type == EItemType.Bows)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }
                        }
                        break;
                    case E_GameOccupation.Spellsword:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.DanShouJian_master358, out var mMasteryValue))
                            {
                                bool mNeedbreak = false;
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 单手剑精通
                                    if (mWeaponEquipment.Type == EItemType.Swords && mWeaponEquipment.ConfigData.TwoHand == 0)
                                    {
                                        mResult += mMasteryValue;
                                        mNeedbreak = true;
                                    }
                                    else if (mWeaponEquipment.Type == EItemType.MagicSword && mWeaponEquipment.ConfigData.TwoHand == 0)
                                    {
                                        mResult += mMasteryValue;
                                        mNeedbreak = true;
                                    }
                                }
                                if (mNeedbreak == false && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
                                {
                                    // 单手剑精通
                                    if (mShieldWeaponEquipment.Type == EItemType.Swords && mShieldWeaponEquipment.ConfigData.TwoHand == 0)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                    else if (mShieldWeaponEquipment.Type == EItemType.MagicSword && mShieldWeaponEquipment.ConfigData.TwoHand == 0)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.DanShouJian_Master359, out mMasteryValue))
                            {
                                bool mNeedbreak = false;
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 单手杖精通
                                    if (mWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 0)
                                    {
                                        mResult += mMasteryValue;
                                        mNeedbreak = true;
                                    }
                                }
                                if (mNeedbreak == false && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
                                {
                                    // 单手杖精通
                                    if (mShieldWeaponEquipment.Type == EItemType.Staffs && mShieldWeaponEquipment.ConfigData.TwoHand == 0)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }
                        }
                        break;
                    case E_GameOccupation.Holyteacher:
                        break;
                    case E_GameOccupation.SummonWarlock:
                        break;
                    case E_GameOccupation.Combat:
                        break;
                    case E_GameOccupation.GrowLancer:
                        break;
                    default:
                        break;
                }
            }

            return mResult;
        }
    }
}
