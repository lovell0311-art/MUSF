using System;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{



    public static partial class CombatSourceSystem
    {
        public static bool IsCanOperation(this CombatSource b_CombatSource)
        {
            if (b_CombatSource.HealthStatsDic.ContainsKey(E_BattleSkillStats.HunShuiShu502))
            {
                return false;
            }


            return true;
        }

        public static int GetAttackSpeed(this CombatSource b_CombatSource, bool b_IsSkill = false, E_GameOccupation b_GameOccupation = E_GameOccupation.None, int b_MinActionTime = 0, int b_MaxActionTime = 0)
        {
            if (b_IsSkill == false)
            {
                // 60000 / (50 + (240 - 50) * [攻击速度] / 280)
                return 60000 / (50 + (240 - 50) * b_CombatSource.GetNumerialFunc(E_GameProperty.AttackSpeed) / 280);
            }
            else
            {
                if(b_GameOccupation == E_GameOccupation.None)
                {
                    return 60000 / (50 + (240 - 50) * b_CombatSource.GetNumerialFunc(E_GameProperty.AttackSpeed) / 280);
                }
                // 将攻击间隔转为每分钟攻击次数
                // 线性增加每分钟攻击次数
                // 计算完成后，再转为攻击间隔 毫秒
                float attackSpeed = b_CombatSource.GetNumerialFunc(E_GameProperty.AttackSpeed);
                float attackSpeedMax = 500f;
                int speed = (int)((long)(attackSpeedMax * b_MinActionTime * b_MaxActionTime) / (attackSpeed * b_MaxActionTime + attackSpeedMax * b_MinActionTime - attackSpeed * b_MinActionTime));
                return speed;
            }
        }

        public static int GetSkillDamageWait(this CombatSource b_CombatSource,int b_DamageWait,int b_DamageWait2 = 0)
        {
            if(b_CombatSource is GamePlayer mGamePlayer)
            {
                float attackSpeed = b_CombatSource.GetNumerialFunc(E_GameProperty.AttackSpeed);
                float attackSpeedMax = 500f;
                int damageWaitMax = b_DamageWait + b_DamageWait2;
                int damageWait = (int)((long)(attackSpeedMax * b_DamageWait * damageWaitMax) / (attackSpeed * damageWaitMax + attackSpeedMax * b_DamageWait - attackSpeed * b_DamageWait));
                return damageWait;
            }
            return b_DamageWait;

        }
    }
}