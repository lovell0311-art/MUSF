
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
    [PropertyNumerialAttribute(BindId = (int)E_GameProperty.BackInjuryRate)]
    public partial class C_PropertyNumerial_BackInjuryRate : C_PropertyNumerial
    {
        public override int Run(GamePlayer b_Component, bool b_HasTemporary = true)
        {
            int mResult = 0;

            if (b_HasTemporary)
            {
                if (b_Component.HealthStatsDic.TryGetValue(E_BattleSkillStats.ShangHaiFanShe506, out var mTempBuffer))
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
    }
}
