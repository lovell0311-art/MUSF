
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
    [PropertyNumerialAttribute(BindId = (int)E_GameProperty.MaxMagicAtteck)]
    public partial class C_PropertyNumerial_MaxMagicAtteck : C_PropertyNumerial
    {
        public override int Run(GamePlayer b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;
            var mGameProperty = E_GameProperty.MaxMagicAtteck;

            if (b_HasTemporary)
            {
                // 在身上有技能buffer
                // 在身上有技能buffer
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.JiGuDeBiHu115, out var mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult += mTempBufferValue;
                    }

                    // 坚固的庇护精通
                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Skill_JianQiangDeBiHu_Master133, out var mMasteryValue))
                    {
                        mResult += mMasteryValue;
                    }
                }
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310116, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult += mTempBufferValue;
                    }
                }
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.ZhanShenZhiLi204, out mTempBuffer))
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
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310021, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult += mTempBufferValue;
                    }
                }
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseYingHuaHuaBan310063, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult += mTempBufferValue;
                    }
                }
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.XinShouBuff, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        mResult += 50;
                    }
                }
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.XuRuoZhen510, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult -= mTempBufferValue;
                    }
                }
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.DouShenPo607, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult += mTempBufferValue;
                    }
                }
            }
            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.BeCommon3029, out var mMasteryValue1))
            {
                mResult += mMasteryValue1;
            }
            if (ConstServer.PlayerMaster || (b_Component.Data.Level >= 400 && b_Component.Data.OccupationLevel >= 3))
            {
                {
                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Common2001, out var mMasteryValue))
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
                            // 魔法精通
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MagicMaster32, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseMaxMagic61, out mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }

                            // 时空之翼攻击强化
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Equipment_Attack_ShiKongZhiYi_21, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
                                {
                                    // 时空之翼
                                    if (mWingEquipment.ConfigID == 220034)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }

                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.DanShouJian_Strengthening47, out mMasteryValue))
                            {
                                bool mNeedbreak = false;
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 单手杖
                                    if (mWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 0)
                                    {
                                        mResult += mMasteryValue;
                                        mNeedbreak = true;
                                    }
                                }
                                if (mNeedbreak == false && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
                                {
                                    // 单手杖
                                    if (mShieldWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 0)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.ShuangShouJian_Strengthening48, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 双手杖
                                    if (mWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 1)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }

                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Shield_Strengthening49, out mMasteryValue))
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
                        break;
                    case E_GameOccupation.Spellsword:
                        {
                            // 魔法精通
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MagicMaster335, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseMaxMagic367, out mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }

                            // 毁灭之翼攻击强化
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Equipment_Attack_HuiMieZhiYi_322, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
                                {
                                    // 破灭之翼
                                    if (mWingEquipment.ConfigID == 220032)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }

                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.DanShouJian_Strengthening355, out mMasteryValue))
                            {
                                bool mNeedbreak = false;
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 单手杖
                                    if (mWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 0)
                                    {
                                        mResult += mMasteryValue;
                                        mNeedbreak = true;
                                    }
                                }
                                if (mNeedbreak == false && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
                                {
                                    // 单手杖
                                    if (mShieldWeaponEquipment.Type == EItemType.Staffs && mShieldWeaponEquipment.ConfigData.TwoHand == 0)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.ShuangShouJian_Strengthening356, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 双手杖
                                    if (mWeaponEquipment.Type == EItemType.Staffs && mWeaponEquipment.ConfigData.TwoHand == 1)
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
                        {
                            // 魔法精通
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MagicMaster532, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseMaxMagicAndDamnation556, out mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }

                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Equipment_Attack_ChiYuanZhiYi_520, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
                                {
                                    // 破灭之翼
                                    if (mWingEquipment.ConfigID == 220030)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.QuanZhang_Strengthen543, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 法杖
                                    if (mWeaponEquipment.Type == EItemType.Staffs)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }
                            
                        }
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
