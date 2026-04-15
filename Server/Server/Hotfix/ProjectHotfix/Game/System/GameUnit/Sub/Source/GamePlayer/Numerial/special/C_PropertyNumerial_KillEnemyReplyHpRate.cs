
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
    [PropertyNumerialAttribute(BindId = (int)E_GameProperty.KillEnemyReplyHpRate)]
    public partial class C_PropertyNumerial_KillEnemyReplyHpRate : C_PropertyNumerial
    {
        public override int Run(GamePlayer b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;

            if (b_HasTemporary)
            {

            }
            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.BeCommon3025, out var mBattleMasteryValue1))
            {
                mResult += mBattleMasteryValue1;
            }
            if (ConstServer.PlayerMaster || (b_Component.Data.Level >= 400 && b_Component.Data.OccupationLevel >= 3))
            {
                switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
                {
                    case E_GameOccupation.None:
                        break;
                    case E_GameOccupation.Spell:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoveryHp56, out var mBattleMasteryValue))
                            {
                                mResult += mBattleMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Swordsman:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoveryHp155, out var mBattleMasteryValue))
                            {
                                mResult += mBattleMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Archer:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoveryHp262, out var mBattleMasteryValue))
                            {
                                mResult += mBattleMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Spellsword:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoveryHp363, out var mBattleMasteryValue))
                            {
                                mResult += mBattleMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Holyteacher:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoveryHp455, out var mBattleMasteryValue))
                            {
                                mResult += mBattleMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.SummonWarlock:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KillRecoveryHp549, out var mBattleMasteryValue))
                            {
                                mResult += mBattleMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Combat:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute649, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.GrowLancer:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute953, out var mMasteryValue))
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
