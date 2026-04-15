
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
    [PropertyNumerialAttribute(BindId = (int)E_GameProperty.ReallyDefense)]
    public partial class C_PropertyNumerial_ReallyDefense : C_PropertyNumerial
    {
        public override int Run(GamePlayer b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;

            if (b_HasTemporary)
            {

            }

            if (b_Component.Data.Level >= 400 && b_Component.Data.OccupationLevel >= 3)
            {
                switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
                {
                    case E_GameOccupation.None:
                        break;
                    case E_GameOccupation.Spell:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.SteelCarapace_22, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Swordsman:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.SteelCarapace_121, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Archer:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.SteelCarapace_222, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Spellsword:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.SteelCarapace_323, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Holyteacher:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.SteelCarapace_423, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.SummonWarlock:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.SteelCarapace_521, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.Combat:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute621, out var mMasteryValue))
                            {
                                mResult += mMasteryValue;
                            }
                        }
                        break;
                    case E_GameOccupation.GrowLancer:
                        {
                            if (b_Component.BattleMasteryDic.TryGetValue(E_BattleMasteryState.MasterAttribute922, out var mMasteryValue))
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
