
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
    [PropertyNumerialAttribute(BindId = (int)E_GameProperty.MaxDamnationAtteck)]
    public partial class C_PropertyNumerial_MaxDamnationAtteck : C_PropertyNumerial
    {
        public override int Run(GamePlayer b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;

            if (b_HasTemporary)
            {
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310021, out var mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult += mTempBufferValue;
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
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseYingHuaHuaBan310063, out mTempBuffer))
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
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.DouShenPo607, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        var mTempBufferValue = mTempBufferData.StrengthValue;
                        mResult += mTempBufferValue;
                    }
                }
            }

            if (b_Component.Data.Level >= 400 && b_Component.Data.OccupationLevel >= 3)
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
