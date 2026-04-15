
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
    [PropertyNumerialAttribute(BindId = (int)E_GameProperty.ReplyHpRate)]
    public partial class C_PropertyNumerial_ReplyHpRate : C_PropertyNumerial
    {
        public override int Run(GamePlayer b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;
            var mGameProperty = E_GameProperty.ReplyHpRate;

            if (b_HasTemporary)
            {
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310034, out var mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        if (mTempBufferData.CacheData.TryGetValue((int)mGameProperty, out var mTempValue))
                        {
                            mResult += mTempValue;
                        }
                    }
                }
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310103, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        mResult += mTempBufferData.StrengthValue;
                    }
                }
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310133, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        mResult += mTempBufferData.StrengthValue;
                    }
                }
            }
            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.BeCommon3007, out var mMasteryValue1))
            {
                mResult += mMasteryValue1;
            }
            if(!ConstServer.PlayerMaster)
            if (b_Component.Data.Level >= 400 && b_Component.Data.OccupationLevel >= 3)
            {
                switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
                {
                    case E_GameOccupation.None:
                        break;
                    case E_GameOccupation.Spell:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryHp7, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Swordsman:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryHp107, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Archer:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryHp207, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Spellsword:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryHp307, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Holyteacher:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryHp407, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.SummonWarlock:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.RecoveryHp507, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Combat:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute607, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.GrowLancer:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute907, out var mMasteryValue))
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
