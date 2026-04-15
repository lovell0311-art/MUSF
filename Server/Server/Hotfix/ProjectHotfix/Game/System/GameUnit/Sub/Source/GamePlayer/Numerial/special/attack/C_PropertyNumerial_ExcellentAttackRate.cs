
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
    [PropertyNumerialAttribute(BindId = (int)E_GameProperty.ExcellentAttackRate)]
    public partial class C_PropertyNumerial_ExcellentAttackRate : C_PropertyNumerial
    {
        public override int Run(GamePlayer b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;

            if (b_HasTemporary)
            {
            }

            if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.ZhiMingShengYin410, out var mTempBuffer))
            {
                if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.ZhiMingYiJi_master440, out var mMasteryValue1))
                {
                    mResult += mMasteryValue1 * 110;
                }
            }
            
            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.BeCommon3036, out var mMasteryValue2))
            {
                mResult += mMasteryValue2*100;  
            }
            if (ConstServer.PlayerMaster || (b_Component.Data.Level >= 400 && b_Component.Data.OccupationLevel >= 3))
            {
                switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
                {
                    case E_GameOccupation.None:
                        break;
                    case E_GameOccupation.Spell:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseZuoYueYiJiRate66, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Swordsman:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseZuoYueYiJiRate165, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Archer:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseZuoYueYiJiRate271, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Spellsword:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseZuoYueYiJiRate373, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Holyteacher:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseZuoYueYiJiRate467, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.SummonWarlock:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.IncreaseZuoYueYiJiRate561, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Combat:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute657, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.GrowLancer:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute963, out var mMasteryValue))
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
