
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
    [PropertyNumerialAttribute(BindId = (int)E_GameProperty.MaxAtteckPct)]
    public partial class C_PropertyNumerial_MaxAtteckPct : C_PropertyNumerial
    {
        public override int Run(GamePlayer b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;

            if (b_HasTemporary)
            {
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.XinShouBuff, out var mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        mResult += 20;
                    }
                }
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.YouMingQingLangQuan601, out mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        mResult -= mTempBufferData.StrengthValue;
                    }
                }
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310128, out var mTempBuffer1))
                {
                    if (mTempBuffer1.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        mResult += mTempBufferData.StrengthValue;
                    }
                }
            }

            if (b_Component.Data.Level >= 400 && b_Component.Data.OccupationLevel >= 3)
            {
                switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
                {
                    case E_GameOccupation.None:
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
        public override int Run(HolyteacherSummoned b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;
            var mGameProperty = E_GameProperty.MaxAtteckPct;

            if (b_HasTemporary)
            {
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.YouMingQingLangQuan601, out var mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        mResult -= mTempBufferData.StrengthValue;
                    }
                }
            }

            if (b_Component.GamePlayer != null)
            {

            }

            return mResult;
        }

        public override int Run(Summoned b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;
            var mGameProperty = E_GameProperty.MaxAtteckPct;

            if (b_HasTemporary)
            {
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.YouMingQingLangQuan601, out var mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        mResult -= mTempBufferData.StrengthValue;
                    }
                }
            }

            return mResult;
        }
        public override int Run(Enemy b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;
            var mGameProperty = E_GameProperty.MaxAtteckPct;

            if (b_HasTemporary)
            {
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.YouMingQingLangQuan601, out var mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData))
                    {
                        mResult -= mTempBufferData.StrengthValue;
                    }
                }
            }

            return mResult;
        }
    }
}
