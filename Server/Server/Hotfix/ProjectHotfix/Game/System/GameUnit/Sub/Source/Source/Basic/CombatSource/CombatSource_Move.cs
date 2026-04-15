using System;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{



    public static partial class CombatSourceSystem
    {

        public static bool IsCanMove(this CombatSource b_CombatSource)
        {
            if (b_CombatSource.HealthStatsDic.ContainsKey(E_BattleSkillStats.BingFengJian214)) return false;
            if (b_CombatSource.HealthStatsDic.ContainsKey(E_BattleSkillStats.DiLaoShu326)) return false;
            if (b_CombatSource.HealthStatsDic.ContainsKey(E_BattleSkillStats.HunShuiShu502)) return false;

            if (b_CombatSource.GetCustomComponent<BuffWuDiForEnterMap>() != null)
            {
                return false;
            }
   

            return true;
        }

        public static float GetMoveProgress(this CombatSource b_CombatSource, long b_GameCurrentTime)
        {
            return 0;
            //return Math.Clamp((b_GameCurrentTime - b_CombatSource.MoveStartTime) * b_CombatSource.GetMoveSpeed() / 1000000f, 0f, 1f);
        }

        public static int GetMoveSpeed(this CombatSource b_CombatSource)
        {
            return 1000;
            //return b_CombatSource.GamePropertyDic[E_GameProperty.MoveSpeed];
        }
    }
}