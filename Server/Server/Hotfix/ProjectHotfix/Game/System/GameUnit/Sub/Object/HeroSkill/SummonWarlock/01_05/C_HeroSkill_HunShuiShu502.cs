
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
    /// 昏睡术
    /// </summary>
    [HeroSkillAttribute(BindId = (int)E_HeroSkillId.HunShuiShu502)]
    public partial class C_HeroSkill_HunShuiShu502 : C_HeroSkillSource
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

            if (!(Config is Skill_SummonWarlockConfig mConfig))
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
    /// 昏睡术
    /// </summary>
    public partial class C_HeroSkill_HunShuiShu502
    {
        public bool TryUseByUseStandard(CombatSource b_Attacker, IActorResponse b_Response)
        {
            if (!(Config is Skill_SummonWarlockConfig mConfig))
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
            if (!(Config is Skill_SummonWarlockConfig mConfig))
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
            if (!(Config is Skill_SummonWarlockConfig mConfig))
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

                    return;
                }

                // 随机诅咒伤害
                int mMagicHurtValue;
                var mSpecialAttack = b_Attacker.AttackSpecial();
                if (mSpecialAttack == E_GameProperty.LucklyAttackRate)
                {
                    mMagicHurtValue = (int)(mConfig.OtherDataDic[1] * 1.5f) + b_Attacker.GetNumerialFunc(E_GameProperty.MaxDamnationAtteck);

                    // 书诅咒提升百分比
                    int mMaxMagicAtteckIncrease = (int)(mMagicHurtValue * b_Attacker.GetNumerialFunc(E_GameProperty.DamnationRate_Increase) * 0.01f);
                    mMagicHurtValue += mMaxMagicAtteckIncrease + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedLuckyStrokeAttack) + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedSkillAttack);
                }
                else
                {
                    // 随机诅咒伤害
                    int mMinDamnationAtteck = mConfig.OtherDataDic[1] + b_Attacker.GetNumerialFunc(E_GameProperty.MinDamnationAtteck);
                    int mMaxDamnationAtteck = (int)(mConfig.OtherDataDic[1] * 1.5f) + b_Attacker.GetNumerialFunc(E_GameProperty.MaxDamnationAtteck);
                    mMagicHurtValue = Help_RandomHelper.Range(mMinDamnationAtteck, mMaxDamnationAtteck) + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedSkillAttack);

                    // 书诅咒提升百分比
                    int mMaxMagicAtteckIncrease = (int)(mMaxDamnationAtteck * b_Attacker.GetNumerialFunc(E_GameProperty.DamnationRate_Increase) * 0.01f);
                    mMagicHurtValue += mMaxMagicAtteckIncrease;
                    if (mSpecialAttack == E_GameProperty.ExcellentAttackRate)
                        mMagicHurtValue += b_Attacker.GetNumerialFunc(E_GameProperty.EmbedGreatShotAttack);
                }
                var mWillpowerHurtValue = b_Attacker.GetNumerialFunc(E_GameProperty.Property_Willpower) / mConfig.OtherDataDic[10];
                // 总伤害 = 诅咒 + 魔力伤害 + 书诅咒提升百分比
                int mHurtValue = mMagicHurtValue + mWillpowerHurtValue;
                var mAddScale = mConfig.OtherDataDic[11];
                if (mAddScale != 100)
                    mHurtValue = (int)(mHurtValue * mAddScale * 0.01f);

                // 是否命中
                bool IsHit = false;
                int mHitRate = 0;
                int mPersistentTime = 0;
                switch (mBeAttacker.Identity)
                {
                    case E_Identity.Hero:
                        {
                            int mWillpower = b_Attacker.GetNumerialFunc(E_GameProperty.Property_Willpower);
                            mHitRate = 15 + mWillpower / 37 + mHurtValue / 6;
                            int mLevel = (b_Attacker as GamePlayer).Data.Level - (mBeAttacker as GamePlayer).Data.Level;
                            mPersistentTime = 4 + mWillpower / 250 - mLevel / 100;
                        }
                        break;
                    case E_Identity.Enemy:
                        {
                            int mWillpower = b_Attacker.GetNumerialFunc(E_GameProperty.Property_Willpower);
                            mHitRate = 20 + 30 + mHurtValue / 6;
                            int mLevel = (mBeAttacker as Enemy).Config.Lvl;
                            mPersistentTime = 5 + mWillpower / 100 - mLevel / 20;
                        }
                        break;
                    case E_Identity.Npc:
                        break;
                    case E_Identity.Pet:
                        {
                            int mWillpower = b_Attacker.GetNumerialFunc(E_GameProperty.Property_Willpower);
                            mHitRate = 15 + mWillpower / 37 + mHurtValue / 6;
                            int mLevel = (b_Attacker as GamePlayer).Data.Level - (mBeAttacker as Pets).dBPetsData.PetsLevel;
                            mPersistentTime = 4 + mWillpower / 250 - mLevel / 100;
                        }
                        break;
                    case E_Identity.Summoned:
                        {
                            int mWillpower = b_Attacker.GetNumerialFunc(E_GameProperty.Property_Willpower);
                            mHitRate = 20 + 30 + mHurtValue / 6;
                            int mLevel = (mBeAttacker as Summoned).Config.Lvl;
                            mPersistentTime = 5 + mWillpower / 100 - mLevel / 20;
                        }
                        break;
                    default:
                        break;
                }

                mPersistentTime *= 1000;
                mHitRate *= 10;
                if (mHitRate < 50) mHitRate = 50;
                if (mHitRate >= 990) mHitRate = 990;

                int mRandomValue = Help_RandomHelper.Range(0, 1000);
                if (mRandomValue > mHitRate || mPersistentTime < 1000)
                {
                    IsHit = false;
                }
                else
                {
                    IsHit = true;
                }

                if (IsHit)
                {
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

                            b_CombatSource.RemoveHealthState(E_BattleSkillStats.HunShuiShu502, b_BattleComponent);
                            b_CombatSource.UpdateHealthState();
                        };
                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                        {
                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                            if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.HunShuiShu502, out var hp_Curse) == false)
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

                    mBeAttacker.AddHealthState(E_BattleSkillStats.HunShuiShu502, 0, mConfig.PersistentTime, 0, mCreateFunc, b_BattleComponent);
                    mBeAttacker.UpdateHealthState();
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
