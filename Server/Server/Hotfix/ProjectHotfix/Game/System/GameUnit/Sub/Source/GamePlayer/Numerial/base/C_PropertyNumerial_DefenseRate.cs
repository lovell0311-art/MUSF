
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
    [PropertyNumerialAttribute(BindId = (int)E_GameProperty.DefenseRate)]
    public partial class C_PropertyNumerial_DefenseRate : C_PropertyNumerial
    {
        public override int Run(GamePlayer b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;

            if (b_HasTemporary)
            {
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.ShenShengQiXuan611, out var mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult -= mTempBufferValue;
                    }
                }
            }

            if (b_Component.Data.Level >= 400 && b_Component.Data.OccupationLevel >= 3)
            {
                switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
                {
                    case E_GameOccupation.None:
                        break;
                    case E_GameOccupation.Spell:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Shield_Master52, out var mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
                                {
                                    // 盾牌
                                    if (mShieldWeaponEquipment.Type == EItemType.Shields)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }

                        }
                        break;
                    case E_GameOccupation.Swordsman:
                        break;
                    case E_GameOccupation.Archer:
                        {

                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Shield_Master259, out var mMasteryValue))
                            {
                                bool mNeedbreak = false;
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 盾牌
                                    if (mWeaponEquipment.Type == EItemType.Shields)
                                    {
                                        mResult += mMasteryValue;
                                        mNeedbreak = true;
                                    }
                                }
                                if (mNeedbreak == false && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
                                {
                                    // 盾牌
                                    if (mShieldWeaponEquipment.Type == EItemType.Shields)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }

                        }
                        break;
                    case E_GameOccupation.Spellsword:
                        break;
                    case E_GameOccupation.Holyteacher:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Shield_Master450, out var mMasteryValue))
                            {
                                bool mNeedbreak = false;
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 盾牌
                                    if (mWeaponEquipment.Type == EItemType.Shields)
                                    {
                                        mResult += mMasteryValue;
                                        mNeedbreak = true;
                                    }
                                }
                                if (mNeedbreak == false && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
                                {
                                    // 盾牌
                                    if (mShieldWeaponEquipment.Type == EItemType.Shields)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }
                        }
                        break;
                    case E_GameOccupation.SummonWarlock:
                        break;
                    case E_GameOccupation.Combat:
                        break;
                    case E_GameOccupation.GrowLancer:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute946, out var mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
                                {
                                    // 盾牌
                                    if (mShieldWeaponEquipment.Type == EItemType.Shields)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            return mResult;
        }

        public override int Run(HolyteacherSummoned b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;
            var mGameProperty = E_GameProperty.DefenseRate;

            if (b_HasTemporary)
            {
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.ShenShengQiXuan611, out var mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult -= mTempBufferValue;
                    }
                }
            }

            if (b_Component.GamePlayer != null)
            {
             
            }

            return mResult;
        }

        public override int Run(Summoned b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;
            var mGameProperty = E_GameProperty.DefenseRate;

            if (b_HasTemporary)
            {
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.ShenShengQiXuan611, out var mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult -= mTempBufferValue;
                    }
                }
            }

            return mResult;
        }
        public override int Run(Enemy b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;
            var mGameProperty = E_GameProperty.DefenseRate;

            if (b_HasTemporary)
            {
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.ShenShengQiXuan611, out var mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        mResult -= mTempBufferData.StrengthValue;
                    }
                }
            }

            return mResult;
        }
    }
}
