
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
    [PropertyNumerialAttribute(BindId = (int)E_GameProperty.PROP_HP_MAX)]
    public partial class C_PropertyNumerial_HP_MAX : C_PropertyNumerial
    {
        public override int Run(GamePlayer b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;
            var mGameProperty = E_GameProperty.PROP_HP_MAX;

            if (b_HasTemporary)
            {
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseYingHuaBing310062, out var mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult += mTempBufferValue;
                    }
                }
                
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.ShengMingZhiGuang110, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult += mTempBufferValue;
                    }
                }
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310022, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult += mTempBufferValue;
                    }
                }
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310034, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        if (mTempBufferData.CacheData.TryGetValue((int)mGameProperty, out var mTempValue))
                        {
                            mResult += mTempValue;
                        }
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
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.DouShenMing608, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult += mTempBufferValue;
                    }
                }
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310118, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult += mTempBufferValue;
                    }
                }
            }
            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.BeCommon3015, out var mMasteryValue1))
            {
                mResult += mMasteryValue1;
            }
            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.BeCommon3018, out mMasteryValue1))
            {
                mResult += mMasteryValue1;
            }
            if (ConstServer.PlayerMaster || (b_Component.Data.Level >= 400 && b_Component.Data.OccupationLevel >= 3))
            {
                {
                    if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Common2003, out var mMasteryValue))
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
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax33, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax42, out mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Swordsman:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax131, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax140, out mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Archer:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax234, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax243, out mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.AddHpMax239, out mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Spellsword:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax336, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax347, out mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Holyteacher:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax433, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax442, out mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.SummonWarlock:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax531, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseHpMax540, out mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Combat:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute629, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute637, out var mMasteryValue2))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.GrowLancer:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute931, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute940, out var mMasteryValue2))
                            {
                                mResult += mMasteryValue;
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
