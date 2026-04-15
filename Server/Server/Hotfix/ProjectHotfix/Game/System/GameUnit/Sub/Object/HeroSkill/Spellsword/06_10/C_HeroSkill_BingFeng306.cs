
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomFrameWork;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 冰封
    /// </summary>
    [HeroSkillAttribute(BindId = (int)E_HeroSkillId.BingFeng306)]
    public partial class C_HeroSkill_BingFeng306 : C_HeroSkillSource
    {
        public override void AfterAwake()
        { // 只调用一次  
            IsDataHasError = false;
            DataUpdate();
        }
        public override void DataUpdate()
        {  //数据变化 更新变更数据 
            if (IsDataHasError) return;
            IsDataHasError = true;

            if (!(Config is Skill_SpellswordConfig mConfig))
            {
                return;
            }

            MP = mConfig.ConsumeDic[1];
            if (mConfig.ConsumeDic.TryGetValue(2, out var mAG))
            {
                AG = mAG;
            }

            CoolTime = mConfig.CoolTime;

            //
            

            NextAttackTime = 0;
            IsDataHasError = false;
        }
    }

    /// <summary>
    /// 冰封
    /// </summary>
    public partial class C_HeroSkill_BingFeng306
    {
        public bool TryUseByUseStandard(CombatSource b_Attacker, IActorResponse b_Response)
        {
            if (!(Config is Skill_SpellswordConfig mConfig))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(425);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("技能配置数据异常!");
                return false;
            }

            var mGamePlayer = b_Attacker as GamePlayer;
            if (mGamePlayer != null)
            {
                var mKeys = mConfig.UseStandardDic.Keys.ToArray();
                for (int i = 0, len = mKeys.Length; i < len; i++)
                {
                    int key = mKeys[i];
                    int value = mConfig.UseStandardDic[key];

                    switch (key)
                    {
                        case 1:
                            {  // 等级
                                if (mGamePlayer.Data.Level < value)
                                {
                                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(408);
                                    return false;
                                }
                            }
                            break;
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                            {
                                var mPropertyValue = mGamePlayer.GetNumerial((E_GameProperty)(key - 1));
                                if (mPropertyValue < value)
                                {
                                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(409);
                                    return false;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return true;
        }
        public override CombatSource FindTarget(CombatSource b_Attacker, long b_BeAttackerId, C_FindTheWay2D b_Cell, BattleComponent b_BattleComponent, IActorResponse b_Response)
        {
            return this.FindTargetByBeAttackerId(b_Attacker, b_BeAttackerId, b_Cell, b_BattleComponent, b_Response);
        }
        public override bool TryUse(CombatSource b_Attacker, CombatSource b_BeAttacker, C_FindTheWay2D b_Cell, BattleComponent b_BattleComponent, IActorResponse b_Response)
        {
            if (!(Config is Skill_SpellswordConfig mConfig))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(425);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("技能配置数据异常!");
                return false;
            }
            if (b_Attacker.UnitData.Mp < MP || b_Attacker.UnitData.AG < AG)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(410);
                return false;
            }

            if (Vector2.Distance(b_BattleComponent.Parent.GetFindTheWay2D(b_Attacker).Vector2Pos, b_Cell.Vector2Pos) > mConfig.Distance)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(411);
                return false;
            }

            switch (b_BeAttacker.Identity)
            {
                case E_Identity.Hero:
                    {
                        var mTryUseResult = b_Attacker.TryUseByPlayerKilling(b_BeAttacker as GamePlayer, b_Response);
                        if (mTryUseResult == false)
                        {
                            return false;
                        }
                    }
                    break;
                case E_Identity.Enemy:
                    break;
                case E_Identity.Npc:
                    break;
                case E_Identity.Pet:
                    {
                        var mTryUseResult = b_Attacker.TryUseByPlayerKilling(b_BeAttacker as Pets, b_Response);
                        if (mTryUseResult == false)
                        {
                            return false;
                        }
                    }
                    break;
                case E_Identity.Summoned:
                    {
                        var mTryUseResult = b_Attacker.TryUseByPlayerKilling(b_BeAttacker as Summoned, b_Response);
                        if (mTryUseResult == false)
                        {
                            return false;
                        }
                    }
                    break;
                case E_Identity.HolyteacherSummoned:
                    break;
                default:
                    break;
            }

            return TryUseByUseStandard(b_Attacker, b_Response);
        }
        public override bool UseSkill(CombatSource b_Attacker, CombatSource b_BeAttacker, BattleComponent b_BattleComponent)
        {
            if (!(Config is Skill_SpellswordConfig mConfig))
            {
                return false;
            }
            //BattleComponent.Log($"使用技能 <<{mConfig.Name}>> 使用者为:{b_Attacker.InstanceId}", false);

            CombatSource mBeAttacker = b_BeAttacker;

			GamePlayer mGamePlayer = b_Attacker as GamePlayer;
            int mDamageWait = b_Attacker.GetSkillDamageWait(mConfig.DamageWait, mConfig.DamageWait2);
            int mAttackTime = (int)(b_Attacker.GetAttackSpeed(true, (E_GameOccupation)mGamePlayer.Data.PlayerTypeId, mConfig.MinActionTime, mConfig.MaxActionTime));

            G2C_AttackStart_notice mAttackStartNotice = new G2C_AttackStart_notice();
            mAttackStartNotice.AttackSource = b_Attacker.InstanceId;
            mAttackStartNotice.AttackTarget = mBeAttacker.InstanceId;
            mAttackStartNotice.AttackType = Id;
            mAttackStartNotice.Ticks = mGamePlayer.Player.ClientTime.ClientTime + mAttackTime;
            mAttackStartNotice.MpValue = b_Attacker.UnitData.Mp - (int)(this.MP * (100 - b_Attacker.GetNumerialFunc(E_GameProperty.MpConsumeRate_Reduce)) / 100f);
            mAttackStartNotice.AG = b_Attacker.UnitData.AG - this.AG;
            b_BattleComponent.Parent.SendNotice(mBeAttacker, mAttackStartNotice);

            b_Attacker.IsAttacking = true;
			b_Attacker.NextAttackTime = mAttackStartNotice.Ticks;

            Action<long, long, long> mSyncAction = (b_CombatRoundId, b_AttackerId, b_BeAttackerId) =>
            {
                //if (b_Attacker.CombatRoundId != b_CombatRoundId) return;

                if (b_Attacker.InstanceId != b_AttackerId || b_Attacker.IsDeath || b_Attacker.IsDisposeable || b_Attacker.UnitData.Index != b_BattleComponent.Parent.MapId)
                {
                    G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                    mAttackResultNotice.HurtValueType = 6;
                    mAttackResultNotice.AttackTarget = mBeAttacker.InstanceId;
                    b_BattleComponent.Parent.SendNotice(mBeAttacker, mAttackResultNotice);
                    return;
                }
                if (mBeAttacker.InstanceId != b_BeAttackerId || mBeAttacker.IsDeath || mBeAttacker.IsDisposeable || mBeAttacker.UnitData.Index != b_BattleComponent.Parent.MapId)
                {
                    G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                    mAttackResultNotice.HurtValueType = 6;
                    mAttackResultNotice.AttackTarget = mBeAttacker.InstanceId;
                    b_BattleComponent.Parent.SendNotice(mBeAttacker, mAttackResultNotice);
                    return;
                }
                C_FindTheWay2D b_Cell = b_BattleComponent.Parent.GetFindTheWay2D(mBeAttacker);
                // 距离变化了 超过技能范围则取消技能效果
                if (Vector2.Distance(b_BattleComponent.Parent.GetFindTheWay2D(b_Attacker).Vector2Pos, b_Cell.Vector2Pos) > mConfig.Distance)
                {
                    G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                    mAttackResultNotice.HurtValueType = 6;
                    mAttackResultNotice.AttackTarget = mBeAttacker.InstanceId;
                    b_BattleComponent.Parent.SendNotice(mBeAttacker, mAttackResultNotice);

                                 //b_Attacker.CombatRoundId = b_BattleComponent.WaitSync(mAttackTime, b_Attacker.InstanceId, mBeAttacker.InstanceId, (b_CombatRoundId, b_AttackerId, b_BeAttackerId) => { if (b_Attacker.CombatRoundId == b_CombatRoundId) b_Attacker.IsAttacking = false; }, () => { b_Attacker.CombatRoundId = 0; });
                    return;
                }

                // 是否命中
                bool IsHit = false;
                if (mBeAttacker.Identity == E_Identity.Hero)
                {
                    IsHit = b_Attacker.IsHitPvP(mBeAttacker, b_BattleComponent, true);
                }
                else
                {
                    IsHit = b_Attacker.IsHitPvE(mBeAttacker, b_BattleComponent, true);
                }

                if (IsHit == false)
                {
                    G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                    mAttackResultNotice.HurtValueType = 1;
                    mAttackResultNotice.AttackTarget = mBeAttacker.InstanceId;
                    b_BattleComponent.Parent.SendNotice(mBeAttacker, mAttackResultNotice);

                                 //b_Attacker.CombatRoundId = b_BattleComponent.WaitSync(mAttackTime, b_Attacker.InstanceId, mBeAttacker.InstanceId, (b_CombatRoundId, b_AttackerId, b_BeAttackerId) => { if (b_Attacker.CombatRoundId == b_CombatRoundId) b_Attacker.IsAttacking = false; }, () => { b_Attacker.CombatRoundId = 0; });
                    return;
                }

                // 随机魔法伤害
                int mMagicHurtValue;
                var mSpecialAttack = b_Attacker.AttackSpecial();
                if (mSpecialAttack == E_GameProperty.LucklyAttackRate)
                {
                    mMagicHurtValue = (int)(mConfig.OtherDataDic[1] * 1.5f) + b_Attacker.GetNumerialFunc(E_GameProperty.MaxMagicAtteck) + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedLuckyStrokeAttack) + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedSkillAttack);

                    // 魔杖魔力提升百分比
                    int mMaxMagicAtteckIncrease = (int)(mMagicHurtValue * b_Attacker.GetNumerialFunc(E_GameProperty.MagicRate_Increase) * 0.01f);
                    mMagicHurtValue += mMaxMagicAtteckIncrease;
                }
                else if (mSpecialAttack == E_GameProperty.ExcellentAttackRate)
                {
                    // 随机魔力伤害
                    int mMinMagicAtteck = mConfig.OtherDataDic[1] + b_Attacker.GetNumerialFunc(E_GameProperty.MinMagicAtteck);
                    int mMaxMagicAtteck = (int)(mConfig.OtherDataDic[1] * 1.5f) + b_Attacker.GetNumerialFunc(E_GameProperty.MaxMagicAtteck);
                    mMagicHurtValue = Help_RandomHelper.Range(mMinMagicAtteck, mMaxMagicAtteck);

                    // 魔杖魔力提升百分比
                    int mMaxMagicAtteckIncrease = (int)(mMaxMagicAtteck * b_Attacker.GetNumerialFunc(E_GameProperty.MagicRate_Increase) * 0.01f);
                    mMagicHurtValue += mMaxMagicAtteckIncrease +b_Attacker.GetNumerialFunc(E_GameProperty.EmbedGreatShotAttack) + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedSkillAttack);
                }
                else
                {
                    // 随机魔力伤害
                    int mMinMagicAtteck = mConfig.OtherDataDic[1] + b_Attacker.GetNumerialFunc(E_GameProperty.MinMagicAtteck);
                    int mMaxMagicAtteck = (int)(mConfig.OtherDataDic[1] * 1.5f) + b_Attacker.GetNumerialFunc(E_GameProperty.MaxMagicAtteck);
                    mMagicHurtValue = Help_RandomHelper.Range(mMinMagicAtteck, mMaxMagicAtteck) + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedSkillAttack);

                    // 魔杖魔力提升百分比
                    int mMaxMagicAtteckIncrease = (int)(mMaxMagicAtteck * b_Attacker.GetNumerialFunc(E_GameProperty.MagicRate_Increase) * 0.01f);
                    mMagicHurtValue += mMaxMagicAtteckIncrease;
                }
                var mWillpower = b_Attacker.GetNumerialFunc(E_GameProperty.Property_Willpower) / mConfig.OtherDataDic[10];
                // 总伤害 = 魔法伤害 + 魔力伤害 + 魔杖魔力提升百分比
                int mHurtValue = mMagicHurtValue + mWillpower;
                var mAddScale = mConfig.OtherDataDic[11];
                if (mAddScale != 100)
                    mHurtValue = (int)(mHurtValue * mAddScale * 0.01f);

                switch (mBeAttacker.Identity)
                {
                    case E_Identity.Enemy:
                        (mBeAttacker as Enemy).InjureSkill(b_Attacker, E_BattleHurtType.MAGIC, mSpecialAttack, Id, mHurtValue, b_BattleComponent);
                        break;
                    case E_Identity.Summoned:
                        (mBeAttacker as Summoned).InjureSkill(b_Attacker, E_BattleHurtType.MAGIC, mSpecialAttack, Id, mHurtValue, b_BattleComponent);
                        break;
                    case E_Identity.Pet:
                        (mBeAttacker as Pets).InjureSkill(b_Attacker, E_BattleHurtType.MAGIC, mSpecialAttack, Id, mHurtValue, b_BattleComponent);
                        break;
                    case E_Identity.Hero:
                        (mBeAttacker as GamePlayer).InjureSkill(b_Attacker, E_BattleHurtType.MAGIC, mSpecialAttack, Id, mHurtValue, b_BattleComponent);
                        break;
                    default:
                        break;
                }

                Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                {
                    var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                    mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.DeathRemoveTask;
                    mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                    mBattleSyncTimer.SyncWaitTime = mConfig.PersistentTime;
                    mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                    mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                    {
                        if (b_CombatSource.IsDisposeable) return;

                        b_CombatSource.RemoveHealthState(E_BattleSkillStats.BingFeng, b_BattleComponent);
                        b_CombatSource.UpdateHealthState();
                    };
                    mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                    {
                        if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                        if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.BingFeng, out var hp_Curse) == false)
                        {
                            return CombatSource.E_SyncTimerTaskResult.Dispose;
                        }

                        if (b_TimerTask.NextWaitTime == hp_Curse.ContinueTimeMax)
                        {
                            return CombatSource.E_SyncTimerTaskResult.Dispose;
                        }
                        if (b_TimeTick > hp_Curse.ContinueTimeMax)
                        {
                            return CombatSource.E_SyncTimerTaskResult.Dispose;
                        }
                        else
                        {
                            b_TimerTask.NextWaitTime = hp_Curse.ContinueTimeMax;

                            b_CombatSource.AddTask(b_TimerTask);
                        }
                        return CombatSource.E_SyncTimerTaskResult.NextRound;
                    };

                    return mBattleSyncTimer;
                };

                var mIceResistance = mBeAttacker.GetNumerialFunc(E_GameProperty.IceResistance);
                if (mIceResistance == 0)
                {
                    mBeAttacker.AddHealthState(E_BattleSkillStats.BingFeng, 0, mConfig.PersistentTime, 0, mCreateFunc, b_BattleComponent);
                    mBeAttacker.UpdateHealthState();
                }
                else
                {
                    int mRandomValue = Help_RandomHelper.Range(0, 256);
                    if (mRandomValue % (mIceResistance + 1) == 0)
                    {
                        mBeAttacker.AddHealthState(E_BattleSkillStats.BingFeng, 0, mConfig.PersistentTime, 0, mCreateFunc, b_BattleComponent);
                        mBeAttacker.UpdateHealthState();
                    }
                }

                             //b_Attacker.CombatRoundId = b_BattleComponent.WaitSync(mAttackTime, b_Attacker.InstanceId, mBeAttacker.InstanceId, (b_CombatRoundId, b_AttackerId, b_BeAttackerId) => { if (b_Attacker.CombatRoundId == b_CombatRoundId) b_Attacker.IsAttacking = false; }, () => { b_Attacker.CombatRoundId = 0; });
            };
            b_Attacker.CombatRoundId = 0;
            b_BattleComponent.WaitSync(mDamageWait, b_Attacker.InstanceId, mBeAttacker.InstanceId, mSyncAction);
            b_Attacker.CombatRoundId = b_BattleComponent.WaitSync(mAttackTime, b_Attacker.InstanceId, mBeAttacker.InstanceId, (b_CombatRoundId, b_AttackerId, b_BeAttackerId) => { if (b_Attacker.CombatRoundId == b_CombatRoundId) b_Attacker.IsAttacking = false; }, () => { b_Attacker.CombatRoundId = 0; });
            return true;
        }
    }
}
