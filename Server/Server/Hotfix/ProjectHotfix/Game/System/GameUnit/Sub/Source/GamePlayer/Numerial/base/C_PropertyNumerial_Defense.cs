
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
    [PropertyNumerialAttribute(BindId = (int)E_GameProperty.Defense)]
    public partial class C_PropertyNumerial_Defense : C_PropertyNumerial
    {
        public override int Run(GamePlayer b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;
            var mGameProperty = E_GameProperty.Defense;

            if (b_HasTemporary)
            {
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.ShouHuZhiGuang203, out var mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult += mTempBufferValue;
                    }
                }
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.PoYuZhen511, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult -= mTempBufferValue;
                    }
                }
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310019, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult += mTempBufferValue;
                    }
                }
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.KuangBaoShu509, out mTempBuffer))
                {
                    if (b_Component.BattleMasteryDic.ContainsKey(E_BattleMasteryState.KuangBao_Master554) == false)
                    {
                        if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                        {
                            if (mTempBufferData.CacheData.TryGetValue((int)mGameProperty, out var mTempValue))
                            {
                                mResult -= mTempValue;
                            }
                        }
                    }
                }
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.XinShouBuff, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        mResult += 50;
                    }
                }
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.DouShenYu609, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult += mTempBufferValue;
                    }
                }
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310117, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult += mTempBufferValue;
                    }
                }
            }
            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.BeCommon3008, out var mMasteryValue1))
            {
                mResult += mMasteryValue1;
            }
            if(!ConstServer.PlayerMaster)
            if (b_Component.Data.Level >= 400 && b_Component.Data.OccupationLevel >= 3)
            {
                {
                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Common2002, out var mMasteryValue))
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
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseDefense8, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseElementDefense9, out mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }

                            // 时空之翼防御强化
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Equipment_Defense_ShiKongZhiYi_19, out mMasteryValue))
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
                            // 整套防御力增加
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.SpecialDefenseRate113, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();

                            }
                        }
                        break;
                    case E_GameOccupation.Swordsman:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseDefense108, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }

                            // 暴风之翼防御强化
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Equipment_Defense_BaoFengZhiYi_119, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
                                {
                                    // 暴风之翼
                                    if (mWingEquipment.ConfigID == 220035)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }
                            // 整套防御力增加
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.SpecialDefenseRate113, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();

                            }

                            // 在身上有技能buffer
                            if (b_Component.HealthStatsDic.ContainsKey(E_BattleSkillStats.JiQiangDeXinNian114))
                            {
                                // 坚强的信念强化
                                if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Skill_JianQiangDeXinNian125, out mMasteryValue))
                                {
                                    mResult += mMasteryValue;
                                }
                            }
                        }
                        break;
                    case E_GameOccupation.Archer:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseDefense208, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }

                            // 时空之翼防御强化
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Equipment_Defense_ShiKongZhiYi_219, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
                                {
                                    // 时空之翼
                                    if (mWingEquipment.ConfigID == 220033)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }

                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Shield_Strengthening256, out mMasteryValue))
                            {
                                bool mNeedbreak = false;
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 盾牌强化
                                    if (mWeaponEquipment.Type == EItemType.Shields)
                                    {
                                        mResult += mMasteryValue;
                                        mNeedbreak = true;
                                    }
                                }
                                if (mNeedbreak == false && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
                                {
                                    // 盾牌强化
                                    if (mShieldWeaponEquipment.Type == EItemType.Shields)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }
                            // 整套防御力增加
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.SpecialDefenseRate113, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();

                            }
                        }
                        break;
                    case E_GameOccupation.Spellsword:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseDefense308, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }

                            // 毁灭之翼防御强化
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Equipment_Defense_HuiMieZhiYi_319, out mMasteryValue))
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

                            // 整套防御力增加
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.SpecialDefenseRate113, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();

                            }
                        }
                        break;
                    case E_GameOccupation.Holyteacher:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseDefense408, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }

                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Equipment_Defense_DiWangPiFeng420, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
                                {
                                    // 帝王披风
                                    if (mWingEquipment.ConfigID == 220031)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }

                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Equipment_HeiWangMaStrengthen428, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                var mWingEquipment = mEquipmentComponent.GetEquipItemByPosition(EquipPosition.Mounts);
                                if (mWingEquipment != null)
                                {
                                    if (mWingEquipment.ConfigID == 260012)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Shield_Strengthening446, out mMasteryValue))
                            {
                                bool mNeedbreak = false;
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 盾牌强化
                                    if (mWeaponEquipment.Type == EItemType.Shields)
                                    {
                                        mResult += mMasteryValue;
                                        mNeedbreak = true;
                                    }
                                }
                                if (mNeedbreak == false && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
                                {
                                    // 盾牌强化
                                    if (mShieldWeaponEquipment.Type == EItemType.Shields)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }


                        }
                        break;
                    case E_GameOccupation.SummonWarlock:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseDefense508, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }

                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Equipment_Defense_ChiYuanZhiYi_519, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
                                {
                                    // 帝王披风
                                    if (mWingEquipment.ConfigID == 220030)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }


                        }
                        break;
                    case E_GameOccupation.Combat:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute608, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute619, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
                                {
                                    if (mWingEquipment.ConfigID == 220029)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }
                        }
                        break;
                    case E_GameOccupation.GrowLancer:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute908, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute919, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Wing, out var mWingEquipment))
                                {
                                    if (mWingEquipment.ConfigID == 220027)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }

                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute946, out mMasteryValue))
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
        public override int Run(Summoned b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;
            var mGameProperty = E_GameProperty.Defense;

            if (b_HasTemporary)
            {
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.ShouHuZhiGuang203, out var mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult += mTempBufferValue;
                    }
                }
            }

            return mResult;
        }
    }
}
