
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
    [PropertyNumerialAttribute(BindId = (int)E_GameProperty.InjuryValueRate_2)]
    public partial class C_PropertyNumerial_InjuryValueRate_2 : C_PropertyNumerial
    {
        public override int Run(GamePlayer b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;

            if (b_HasTemporary)
            {

            }
            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.BeCommon3020, out var mMasteryValue1))
            {
                mResult += mMasteryValue1 * 100;
            }
            if (ConstServer.PlayerMaster || (b_Component.Data.Level >= 400 && b_Component.Data.OccupationLevel >= 3))
            {
                switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
                {
                    case E_GameOccupation.None:
                        break;
                    case E_GameOccupation.Spell:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseShuangBeiYiJiRate69, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Swordsman:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseShuangBeiYiJiRate168, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }

                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Spear_master151, out mMasteryValue))
                            {
                                bool mNeedbreak = false;
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 矛
                                    if (mWeaponEquipment.Type == EItemType.Spears)
                                    {
                                        mResult += mMasteryValue;
                                        mNeedbreak = true;
                                    }
                                }
                                if (mNeedbreak == false && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
                                {
                                    // 矛
                                    if (mShieldWeaponEquipment.Type == EItemType.Spears)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }


                        }
                        break;
                    case E_GameOccupation.Archer:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseShuangBeiYiJiRate274, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Spellsword:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.ShuangShouJian_Strengthening357, out var mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 双手剑精通
                                    if (mWeaponEquipment.Type == EItemType.Swords && mWeaponEquipment.ConfigData.TwoHand == 1)
                                    {
                                        mResult += mMasteryValue *1000;
                                    }
                                    else if (mWeaponEquipment.Type == EItemType.MagicSword && mWeaponEquipment.ConfigData.TwoHand == 1)
                                    {
                                        mResult += mMasteryValue * 1000;
                                    }
                                }
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.ShuangShouJian_Master360, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 双手杖精通
                                    if (mWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 1)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseShuangBeiYiJiRate375, out mMasteryValue))
                            {
                                mResult += mMasteryValue * 1000;
                            }


                        }
                        break;
                    case E_GameOccupation.Holyteacher:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseShuangBeiYiJiRate469, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.SummonWarlock:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseShuangBeiYiJiRate564, out var mMasteryValue))
                            {
                                mResult += mMasteryValue * 1000;
                            }
                        }
                        break;
                    case E_GameOccupation.Combat:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute659, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute644, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWingEquipment) != false)
                                {
                                    mResult += mMasteryValue;
                                }
                            }
                        }
                        break;
                    case E_GameOccupation.GrowLancer:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute965, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute947, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWingEquipment))
                                {
                                    if (mWingEquipment.Type == EItemType.Spears)
                                        mResult += mMasteryValue;
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
    }
}
