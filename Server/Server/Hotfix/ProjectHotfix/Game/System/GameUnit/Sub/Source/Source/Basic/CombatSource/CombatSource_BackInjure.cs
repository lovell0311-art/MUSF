using System;
using System.Threading.Tasks;
using CustomFrameWork;
using ETModel;

namespace ETHotfix
{



    public static partial class CombatSourceSystem
    {
        /// <summary>
        /// 反伤
        /// </summary>
        /// <param name="b_CombatSource"></param>
        /// <param name="b_BeAttacker"></param>
        /// <param name="b_AttackType"></param>
        /// <param name="b_HurtType"></param>
        /// <param name="b_InjureValue"></param>
        /// <param name="b_BattleComponent"></param>
        /// <param name="b_CanBackInjure"></param>
        /// <returns></returns>
        public static bool CanBackInjure(this CombatSource b_CombatSource, CombatSource b_BeAttacker,
          E_BattleHurtAttackType b_AttackType,
          E_BattleHurtType b_HurtType,
          int b_InjureValue,
          BattleComponent b_BattleComponent,
          bool b_CanBackInjure)
        {
            if (b_CombatSource.CurrentMap == null || b_BeAttacker.CurrentMap == null) return false;
            if (b_CombatSource.CurrentMap.Id != b_BeAttacker.CurrentMap.Id) return false;

            if (b_CanBackInjure)
            {
                var mReboundRate = b_CombatSource.GetNumerialFunc(E_GameProperty.ReboundRate);
                if (mReboundRate > 0)
                {
                    int mRandomResult = CustomFrameWork.Help_RandomHelper.Range(0, 10000);
                    if (mRandomResult <= mReboundRate)
                    {
                        switch (b_BeAttacker.Identity)
                        {
                            case E_Identity.Enemy:
                                (b_BeAttacker as Enemy).Injure(b_CombatSource, E_BattleHurtAttackType.BACKINJURY, E_BattleHurtType.REAL, E_GameProperty.NullAttack, 0, b_InjureValue, b_BattleComponent, b_CanDefense: false, b_CanBackInjure: false);
                                break;
                            case E_Identity.Summoned:
                                (b_BeAttacker as Summoned).Injure(b_CombatSource, E_BattleHurtAttackType.BACKINJURY, E_BattleHurtType.REAL, E_GameProperty.NullAttack, 0, b_InjureValue, b_BattleComponent, b_CanDefense: false, b_CanBackInjure: false);
                                break;
                            case E_Identity.Pet:
                                (b_BeAttacker as Pets).Injure(b_CombatSource, E_BattleHurtAttackType.BACKINJURY, E_BattleHurtType.REAL, E_GameProperty.NullAttack, 0, b_InjureValue, b_BattleComponent, b_CanDefense: false, b_CanBackInjure: false);
                                break;
                            case E_Identity.Hero:
                                (b_BeAttacker as GamePlayer).Injure(b_CombatSource, E_BattleHurtAttackType.BACKINJURY, E_BattleHurtType.REAL, E_GameProperty.NullAttack, 0, b_InjureValue, b_BattleComponent, b_CanDefense: false, b_CanBackInjure: false);
                                break;
                            default:
                                break;
                        }
                    }
                }
                var mBackInjuryRate = b_CombatSource.GetNumerialFunc(E_GameProperty.BackInjuryRate);
                if (mBackInjuryRate > 0)
                {
                    if (mBackInjuryRate > 80) mBackInjuryRate = 80;

                    var mHurtValue = b_InjureValue * mBackInjuryRate / 100;
                    if (mHurtValue > 0)
                    {
                        switch (b_BeAttacker.Identity)
                        {
                            case E_Identity.Enemy:
                                (b_BeAttacker as Enemy).Injure(b_CombatSource, E_BattleHurtAttackType.BACKINJURY, E_BattleHurtType.REAL, E_GameProperty.NullAttack, 0, mHurtValue, b_BattleComponent, b_CanDefense: false, b_CanBackInjure: false);
                                break;
                            case E_Identity.Summoned:
                                (b_BeAttacker as Summoned).Injure(b_CombatSource, E_BattleHurtAttackType.BACKINJURY, E_BattleHurtType.REAL, E_GameProperty.NullAttack, 0, mHurtValue, b_BattleComponent, b_CanDefense: false, b_CanBackInjure: false);
                                break;
                            case E_Identity.Pet:
                                (b_BeAttacker as Pets).Injure(b_CombatSource, E_BattleHurtAttackType.BACKINJURY, E_BattleHurtType.REAL, E_GameProperty.NullAttack, 0, mHurtValue, b_BattleComponent, b_CanDefense: false, b_CanBackInjure: false);
                                break;
                            case E_Identity.Hero:
                                (b_BeAttacker as GamePlayer).Injure(b_CombatSource, E_BattleHurtAttackType.BACKINJURY, E_BattleHurtType.REAL, E_GameProperty.NullAttack, 0, mHurtValue, b_BattleComponent, b_CanDefense: false, b_CanBackInjure: false);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return false;
        }
    }
}