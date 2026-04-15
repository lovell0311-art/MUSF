using System;
using System.Threading.Tasks;
using ETModel;
using CustomFrameWork;
using UnityEngine;
using CustomFrameWork.Baseic;

namespace ETHotfix
{
    [EventMethod("CombatSourceEnterOrSwitchMap")]
    public class CombatSourceEnterOrSwitchMap_HolyteacherSummonedSwitchMap : ITEventMethodOnRun<ETModel.EventType.CombatSourceEnterOrSwitchMap>
    {
        public void OnRun(ETModel.EventType.CombatSourceEnterOrSwitchMap args)
        {
            if (args.combatSource.Identity != E_Identity.Hero) return;

            // 玩家进入或切换地图
            GamePlayer gamePlayer = (GamePlayer)args.combatSource;
            if (gamePlayer.HolyteacherSummoned == null || gamePlayer.HolyteacherSummoned.IsDisposeable == true) return;
            if (gamePlayer.HolyteacherSummoned.IsDeath == true) return;
            if (gamePlayer.HolyteacherSummoned.CurrentMap == null) return; // 不在地图中

            // 清除单位的移动路径
            gamePlayer.HolyteacherSummoned.Pathlist = null;
            // 将召唤兽传送到玩家身边
            gamePlayer.HolyteacherSummoned.SwitchMap(
                gamePlayer.CurrentMap,
                gamePlayer.Position);
        }
    }
    public static partial class HolyteacherSummonedSystem
    {
        public static void Attack(this HolyteacherSummoned b_Attacker, CombatSource b_BeAttacker, BattleComponent b_BattleComponent, bool b_CanBackInjure = true)
        {
            if (b_Attacker.IsAttacking) return;
            b_Attacker.IsAttacking = true;

            b_Attacker.RunAction(b_Attacker.ActionAttack, b_Attacker, b_BeAttacker, b_BattleComponent);

            var mMapComponent = b_BattleComponent.Parent;

            G2C_AttackStart_notice mAttackStartNotice = new G2C_AttackStart_notice();
            mAttackStartNotice.AttackSource = b_Attacker.InstanceId;
            mAttackStartNotice.AttackTarget = b_BeAttacker.InstanceId;
            mAttackStartNotice.AttackType = 0;
            mMapComponent.SendNotice(b_BeAttacker, mAttackStartNotice);

            int mAttackTime = (int)(b_Attacker.GetAttackSpeed() * 0.5f);

            Action<long, long, long> mSyncAction = (b_CombatRoundId, b_AttackerId, b_BeAttackerId) =>
            {
                //if (b_Attacker.CombatRoundId != b_CombatRoundId) return;

                if (b_Attacker.InstanceId != b_AttackerId || b_Attacker.IsDeath || b_Attacker.IsDisposeable || b_Attacker.UnitData.Index != b_BattleComponent.Parent.MapId)
                {
                    b_Attacker.IsAttacking = false;
                    return;
                }
                if (b_BeAttacker.InstanceId != b_BeAttackerId || b_BeAttacker.IsDeath || b_BeAttacker.IsDisposeable || b_BeAttacker.UnitData.Index != b_BattleComponent.Parent.MapId)
                {
                    b_Attacker.IsAttacking = false;
                    return;
                }

                // 是否命中
                bool IsHit = false;
                if (b_BeAttacker.Identity == E_Identity.Hero)
                {
                    IsHit = b_Attacker.IsHitPvE(b_BeAttacker, b_BattleComponent, true);
                }
                else
                {
                    IsHit = b_Attacker.IsHitPvE(b_BeAttacker, b_BattleComponent, true);
                }
                if (IsHit == false)
                {
                    G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                    mAttackResultNotice.HurtValueType = 1;
                    mAttackResultNotice.AttackTarget = b_BeAttacker.InstanceId;
                    mMapComponent.SendNotice(b_BeAttacker, mAttackResultNotice);
                }
                else
                {
                    E_BattleHurtAttackType mAttackType = E_BattleHurtAttackType.BASIC;
                    E_BattleHurtType mBattleHurtType = E_BattleHurtType.PHYSIC;

                    int mVirtualInjureValue;
                    var mSpecialAttack = b_Attacker.AttackSpecial();
                    if (mSpecialAttack == E_GameProperty.LucklyAttackRate)
                    {
                        mVirtualInjureValue = b_Attacker.GetNumerialFunc(E_GameProperty.MaxAtteck);
                    }
                    else
                    {
                        mVirtualInjureValue = Help_RandomHelper.Range(b_Attacker.GetNumerialFunc(E_GameProperty.MinAtteck), b_Attacker.GetNumerialFunc(E_GameProperty.MaxAtteck));
                    }

                    switch (b_BeAttacker.Identity)
                    {
                        case E_Identity.Enemy:
                            (b_BeAttacker as Enemy).Injure(b_Attacker, mAttackType, mBattleHurtType, mSpecialAttack, 0, mVirtualInjureValue, b_BattleComponent, true);
                            break;
                        case E_Identity.Summoned:
                            (b_BeAttacker as Summoned).Injure(b_Attacker, mAttackType, mBattleHurtType, mSpecialAttack, 0, mVirtualInjureValue, b_BattleComponent, true);
                            break;
                        case E_Identity.Pet:
                            (b_BeAttacker as Pets).Injure(b_Attacker, mAttackType, mBattleHurtType, mSpecialAttack, 0, mVirtualInjureValue, b_BattleComponent, true);
                            break;
                        case E_Identity.Hero:
                            (b_BeAttacker as GamePlayer).Injure(b_Attacker, mAttackType, mBattleHurtType, mSpecialAttack, 0, mVirtualInjureValue, b_BattleComponent, true);
                            break;
                        default:
                            break;
                    }
                }

                b_Attacker.CombatRoundId = b_BattleComponent.WaitSync(mAttackTime, b_AttackerId, b_BeAttackerId, (b_CombatRoundId, b_AttackerIdTemp, b_BeAttackerIdTemp) => { if (b_Attacker.CombatRoundId == b_CombatRoundId) b_Attacker.IsAttacking = false; }, () => { b_Attacker.CombatRoundId = 0; });
            };
            ///攻击间隔毫秒 = 60000 / (50 + (240 - 50) * [攻击速度] / 280)
            b_Attacker.CombatRoundId = 0;
            b_Attacker.CombatRoundId = b_BattleComponent.WaitSync(mAttackTime, b_Attacker.InstanceId, b_BeAttacker.InstanceId, mSyncAction);
        }

        public static int Injure(this HolyteacherSummoned b_BeAttacker, CombatSource b_Attacker,
            E_BattleHurtAttackType b_AttackType,
            E_BattleHurtType b_BattleHurtType,
            E_GameProperty b_SpecialAttack,
            int b_HurtTypeId,
            int b_InjureValue,
            BattleComponent b_BattleComponent,
            bool b_CanDefense = true,
            bool b_CanBackInjure = true)
        {
            // 防御后的数值 被抗性衰减 减伤后的数值 真实掉血量

            return 0;
        }

        public static int InjureSkill(this HolyteacherSummoned b_BeAttacker, CombatSource b_Attacker,
            E_BattleHurtType b_BattleHurtType,
            E_GameProperty b_SpecialAttack,
            int b_HurtTypeId,
            int b_InjureValue,
            BattleComponent b_BattleComponent,
            bool b_CanDefense = true,
            bool b_CanBackInjure = true)
        {
            E_BattleHurtAttackType mAttackType = E_BattleHurtAttackType.SKILL;
            int mRealInjureValue = b_BeAttacker.Injure(b_Attacker, mAttackType, b_BattleHurtType, b_SpecialAttack, b_HurtTypeId, b_InjureValue, b_BattleComponent, b_CanDefense, b_CanBackInjure);

            return mRealInjureValue;
        }
    }
}