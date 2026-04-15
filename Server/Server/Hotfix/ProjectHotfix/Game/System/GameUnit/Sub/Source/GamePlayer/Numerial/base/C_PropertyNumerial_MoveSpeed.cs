
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
    [PropertyNumerialAttribute(BindId = (int)E_GameProperty.MoveSpeed)]
    public partial class C_PropertyNumerial_MoveSpeed : C_PropertyNumerial
    {
        public override int Run(GamePlayer b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;
            var mGameProperty = E_GameProperty.MoveSpeed;

            if (b_HasTemporary)
            {
             
            }

            if (b_Component.Data.Level >= 400 && b_Component.Data.OccupationLevel >= 3)
            {

            }

            return mResult;
        }
        public override int Run(HolyteacherSummoned b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;
            var mGameProperty = E_GameProperty.MoveSpeed;

            if (b_HasTemporary)
            {
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.BingFeng, out var mTempBuffer))
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
            var mGameProperty = E_GameProperty.MoveSpeed;

            if (b_HasTemporary)
            {
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.BingFeng, out var mTempBuffer))
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
            var mGameProperty = E_GameProperty.MoveSpeed;

            if (b_HasTemporary)
            {
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.BingFeng, out var mTempBuffer))
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
