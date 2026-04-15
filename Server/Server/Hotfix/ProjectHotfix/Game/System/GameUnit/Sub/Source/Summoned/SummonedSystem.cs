using System;
using System.Threading.Tasks;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using UnityEngine;

namespace ETHotfix
{
    [EventMethod("CombatSourceEnterOrSwitchMap")]
    public class CombatSourceEnterOrSwitchMap_SummonedSwitchMap : ITEventMethodOnRun<ETModel.EventType.CombatSourceEnterOrSwitchMap>
    {
        public void OnRun(ETModel.EventType.CombatSourceEnterOrSwitchMap args)
        {
            if (args.combatSource.Identity != E_Identity.Hero) return;

            // 玩家进入或切换地图
            GamePlayer gamePlayer = (GamePlayer)args.combatSource;
            if (gamePlayer.Summoned == null || gamePlayer.Summoned.IsDisposeable == true) return;
            if (gamePlayer.Summoned.IsDeath == true) return;
            if (gamePlayer.Summoned.CurrentMap == null) return; // 不在地图中

            // 清除单位的移动路径
            gamePlayer.Summoned.Pathlist = null;
            // 将召唤兽传送到玩家身边
            gamePlayer.Summoned.SwitchMap(
                gamePlayer.CurrentMap,
                gamePlayer.Position);
        }
    }

    public static partial class SummonedSystem
    {
        public static void Attack(this Summoned b_Attacker, CombatSource b_BeAttacker, BattleComponent b_BattleComponent, bool b_CanBackInjure = true)
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

            int mAttackTime = (int)(b_Attacker.Config.AtSpeed * 0.5f);

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

        public static int Injure(this Summoned b_BeAttacker, CombatSource b_Attacker,
            E_BattleHurtAttackType b_AttackType,
            E_BattleHurtType b_BattleHurtType,
            E_GameProperty b_SpecialAttack,
            int b_HurtTypeId,
            int b_InjureValue,
            BattleComponent b_BattleComponent,
            bool b_CanDefense = true,
            bool b_CanBackInjure = true)
        {
            if (b_Attacker.Identity == E_Identity.Hero)
            {
                // 检查玩家状态是否为 Online
                if ((b_Attacker as GamePlayer).Player.OnlineStatus != EOnlineStatus.Online)
                {
                    // 玩家正在登录 或 正在下线
                    return 0;
                }
            }
            if(b_BeAttacker.IsDeath)  return 0; 
            (int, int) mRealInjure;
            bool mIgnoreDefense = false;
            if (b_CanDefense || b_BattleHurtType != E_BattleHurtType.REAL)
            {
                if (b_Attacker.Identity == E_Identity.Hero)
                {
                    int mAttackIgnoreDefenseRate = b_Attacker.GetNumerialFunc(E_GameProperty.AttackIgnoreDefenseRate);
                    if (mAttackIgnoreDefenseRate > 0)
                    {
                        //mAttackIgnoreDefenseRate *= 100;
                        if (mAttackIgnoreDefenseRate > 8000) mAttackIgnoreDefenseRate = 8000;

                        int mRandomResult = CustomFrameWork.Help_RandomHelper.Range(0, 10000);
                        if (mRandomResult <= mAttackIgnoreDefenseRate)
                        {
                            mIgnoreDefense = true;
                        }
                    }
                }

                // 防御后的数值 被抗性衰减 减伤后的数值 真实掉血量
                mRealInjure = b_BeAttacker.Defense(b_Attacker, b_AttackType, b_BattleHurtType, mIgnoreDefense, b_HurtTypeId, b_SpecialAttack, b_InjureValue, b_BattleComponent, b_CanDefense);
            }
            else
            {
                mRealInjure = (b_InjureValue, 0);
            }
            
            var mRealInjureValue = mRealInjure.Item1;

            var mMapComponent = b_BattleComponent.Parent;

            if (b_BeAttacker.UnitData.Hp <= mRealInjureValue)
            {
                b_BeAttacker.UnitData.Hp = 0;
                b_BeAttacker.IsDeath = true;
                b_BeAttacker.RemoveAllHealthState(b_BattleComponent);

                if (b_BeAttacker.Pathlist != null) b_BeAttacker.Pathlist = null;
                b_BeAttacker.Enemy = null;
                b_BeAttacker.TargetEnemy = null;

                b_BeAttacker.MoveStartTime = 0;
                b_BeAttacker.MoveNeedTime = 0;
                b_BeAttacker.MoveSleepTime = 0;
                b_BeAttacker.MoveRestTime = 0;
                b_BeAttacker.DeathSleepTime = b_BattleComponent.CurrentTimeTick + b_BeAttacker.Config.Regen;

                var mFindTheWay = mMapComponent.GetFindTheWay2D(b_BeAttacker);
            }
            else
            {
                b_BeAttacker.UnitData.Hp -= mRealInjureValue;
                b_BeAttacker.CanBackInjure(b_Attacker, b_AttackType, b_BattleHurtType, mRealInjure.Item1 + mRealInjure.Item2, b_BattleComponent, b_CanBackInjure);

                if (b_BeAttacker.TargetEnemy == null)
                {
                    var mFindTheWay = b_BattleComponent.Parent.GetFindTheWay2D(b_Attacker);
                    if (mFindTheWay != null && mFindTheWay.IsSafeArea == false)
                    {
                        if (Vector2.Distance(b_BattleComponent.Parent.GetFindTheWay2D(b_BeAttacker).Vector2Pos, mFindTheWay.Vector2Pos) < b_BeAttacker.Config.Ran)
                        {// 是否脱战了
                            b_BeAttacker.TargetEnemy = b_Attacker;
                        }
                    }
                }
            }

            G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
            switch (b_SpecialAttack)
            {
                case E_GameProperty.InjuryValueRate_2:
                    mAttackResultNotice.HurtValueType = 4;
                    break;
                case E_GameProperty.InjuryValueRate_3:
                    mAttackResultNotice.HurtValueType = 5;
                    break;
                case E_GameProperty.LucklyAttackRate:
                    mAttackResultNotice.HurtValueType = 2;
                    break;
                case E_GameProperty.ExcellentAttackRate:
                    mAttackResultNotice.HurtValueType = 3;
                    break;
                default:
                    break;
            }

            if (mIgnoreDefense) mAttackResultNotice.HurtValueType += 100;
            if (b_AttackType == E_BattleHurtAttackType.BACKINJURY) mAttackResultNotice.HurtValueType = 9;

            mAttackResultNotice.AttackSource = b_Attacker.InstanceId;
            mAttackResultNotice.AttackTarget = b_BeAttacker.InstanceId;
            mAttackResultNotice.HurtValue = mRealInjureValue;
            mAttackResultNotice.HpValue = b_BeAttacker.UnitData.Hp;
            mAttackResultNotice.HpMaxValue = b_BeAttacker.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
            mMapComponent.SendNotice(b_BeAttacker, mAttackResultNotice);

            return mRealInjureValue;
        }

        public static int InjureSkill(this Summoned b_BeAttacker, CombatSource b_Attacker,
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