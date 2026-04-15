
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
    [PropertyNumerialAttribute(BindId = (int)E_GameProperty.MaxAtteck)]
    public partial class C_PropertyNumerial_MaxAtteck : C_PropertyNumerial
    {
        public override int Run(GamePlayer b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;
            var mGameProperty = E_GameProperty.MaxAtteck;

            //if (b_Component.Data.PlayerTypeId == (int)E_GameOccupation.Holyteacher)
            //{
            //    if (b_Component.HolyteacherSummoned != null && b_Component.HolyteacherSummoned.IsDeath == false)
            //    {
            //        //var mLevel = b_Component.HolyteacherSummoned.Item.GetProp(EItemValue.MountsLevel);
            //        //mResult += 50 + mLevel * 2;

            //        if (b_Component.HolyteacherSummoned.GamePropertyDic.TryGetValue(mGameProperty, out var mPropertyValue))
            //        {
            //            mResult += mPropertyValue;
            //        }
            //    }
            //}

            if (b_HasTemporary)
            {
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
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.XuRuoZhen510, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult -= mTempBufferValue;
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
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310020, out mTempBuffer))
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
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.DouShenPo607, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult += mTempBufferValue;
                    }
                }
            }
            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.BeCommon3030, out var mMasteryValue1))
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
                        break;
                    case E_GameOccupation.Swordsman:
                        {
                            // 物理攻击力提高
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Skill_WeaponMaster127, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            // 最大伤害值 大师增益
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseMaxAttack160, out mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }

                            // 暴风之翼攻击强化
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Equipment_Attack_BaoFengZhiYi_120, out mMasteryValue))
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

                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.ShuangShouJian_Strengthening144, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 双手剑
                                    if (mWeaponEquipment.Type == EItemType.Swords && mWeaponEquipment.ConfigData.TwoHand == 1)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.DanShouJian_Strengthening145, out mMasteryValue))
                            {
                                bool mNeedbreak = false;
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 单手剑
                                    if (mWeaponEquipment.Type == EItemType.Swords && mWeaponEquipment.ConfigData.TwoHand == 0)
                                    {
                                        mResult += mMasteryValue;
                                        mNeedbreak = true;
                                    }
                                }
                                if (mNeedbreak == false && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
                                {
                                    // 锤
                                    if (mShieldWeaponEquipment.Type == EItemType.Swords)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Hammers_Strengthening146, out mMasteryValue))
                            {
                                bool mNeedbreak = false;
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 锤
                                    if (mWeaponEquipment.Type == EItemType.Maces)
                                    {
                                        mResult += mMasteryValue;
                                        mNeedbreak = true;
                                    }
                                }
                                if (mNeedbreak == false && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
                                {
                                    // 锤
                                    if (mShieldWeaponEquipment.Type == EItemType.Maces)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Spear_Strengthening147, out mMasteryValue))
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
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.PVPShuangShouJian_Strengthening148, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 双手剑
                                    if (mWeaponEquipment.Type == EItemType.Swords && mWeaponEquipment.ConfigData.TwoHand == 1)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }

                        }
                        break;
                    case E_GameOccupation.Archer:
                        {
                            // 物理攻击力提高
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Skill_WeaponMaster233, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            // 最大伤害值 大师增益
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseMaxAttack266, out mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }

                            // 时空之翼攻击强化
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Equipment_Attack_ShiKongZhiYi_221, out mMasteryValue))
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
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Arch_Strengthening254, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 矛
                                    if (mWeaponEquipment.Type == EItemType.Bows)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Crossbow_Master258, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 怒类装备时对目标的攻击力追加增加{0:F}。
                                    if (mWeaponEquipment.Type == EItemType.Crossbows)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }


                        }
                        break;
                    case E_GameOccupation.Spellsword:
                        {
                            // 物理攻击力提高
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.WeaponMaster332, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            // 最大伤害值 大师增益
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseMaxAttack368, out mMasteryValue))
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
                            // 双手剑强化
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.ShuangShouJian_Strengthening353, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 双手剑
                                    if (mWeaponEquipment.Type == EItemType.Swords && mWeaponEquipment.ConfigData.TwoHand == 1)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                    else if (mWeaponEquipment.Type == EItemType.MagicSword && mWeaponEquipment.ConfigData.TwoHand == 1)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }
                            // 单手剑强化
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.DanShouJian_Strengthening354, out mMasteryValue))
                            {
                                bool mNeedbreak = false;
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 单手剑
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
                                    // 单手剑
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

                        }
                        break;
                    case E_GameOccupation.Holyteacher:
                        {
                            // 物理攻击力提高
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Skill_WeaponMaster431, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            // 最大伤害值 大师增益
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseMaxAttack462, out mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }

                            // 帝王披风攻击强化
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Equipment_Attack_DiWangPiFeng422, out mMasteryValue))
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
                            // 权杖强化
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.QuanZhang_Strengthen445, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    // 双手剑
                                    if (mWeaponEquipment.Type == EItemType.Scepter)
                                    {
                                        mResult += mMasteryValue;
                                    }
                                }
                            }
                            // 统率攻击力 增加
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseAttackByCommand451, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                {
                                    if (mWeaponEquipment.Type == EItemType.Scepter)
                                    {
                                        int mProperty_CommandValue = b_Component.GetNumerial(E_GameProperty.Property_Command);
                                        // 装备权杖时统率属性每 38.56 攻击力增加1
                                        mResult += mProperty_CommandValue / mMasteryValue;
                                    }
                                }
                            }
                        }
                        break;
                    case E_GameOccupation.SummonWarlock:
                        break;
                    case E_GameOccupation.Combat:
                        {
                            // 物理攻击力提高
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute630, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute652, out mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute620, out mMasteryValue))
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
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute642, out mMasteryValue))
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
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute958, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute928, out mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute921, out mMasteryValue))
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
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute944, out mMasteryValue))
                            {
                                var mEquipmentComponent = b_Component.Player.GetCustomComponent<EquipmentComponent>();
                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWingEquipment))
                                {
                                    if (mWingEquipment.Type == EItemType.Spears)
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
            var mGameProperty = E_GameProperty.MaxAtteck;

            if (b_HasTemporary)
            {

            }

            if (b_Component.GamePlayer != null)
            {
                var mGamePlayer = b_Component.GamePlayer;
                if (mGamePlayer.Data.Level >= 400 && mGamePlayer.Data.OccupationLevel >= 3)
                {
                    if (mGamePlayer.BattleMasteryDic.TryGetValue(E_BattleMasteryState.QuanZhang_Master447, out var mMasteryValue))
                    {
                        bool mNeedbreak = false;
                        var mEquipmentComponent = mGamePlayer.Player.GetCustomComponent<EquipmentComponent>();
                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                        {
                            // 权杖
                            if (mWeaponEquipment.Type == EItemType.Scepter)
                            {
                                mResult += mMasteryValue;
                                mNeedbreak = true;
                            }
                        }
                        if (mNeedbreak == false && mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
                        {
                            // 权杖
                            if (mShieldWeaponEquipment.Type == EItemType.Scepter)
                            {
                                mResult += mMasteryValue;
                            }
                        }
                    }

                }
            }

            return mResult;
        }

        public override int Run(Summoned b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;
            var mGameProperty = E_GameProperty.MaxAtteck;

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
