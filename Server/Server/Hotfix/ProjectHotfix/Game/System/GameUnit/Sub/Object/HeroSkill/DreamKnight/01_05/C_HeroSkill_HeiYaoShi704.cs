
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
    /// 黑曜石
    /// </summary>
    [HeroSkillAttribute(BindId = (int)E_HeroSkillId.HeiYaoShi704)]
    public partial class C_HeroSkill_HeiYaoShi704 : C_HeroSkillSource
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

            if (!(Config is Skill_DreamKnightConfig mConfig))
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
    /// 黑曜石
    /// </summary>
    public partial class C_HeroSkill_HeiYaoShi704
    {
        public bool TryUseByUseStandard(CombatSource b_Attacker, IActorResponse b_Response)
        {
            if (!(Config is Skill_DreamKnightConfig mConfig))
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
            return b_Attacker;
        }
        public override bool TryUse(CombatSource b_Attacker, CombatSource b_BeAttacker, C_FindTheWay2D b_Cell, BattleComponent b_BattleComponent, IActorResponse b_Response)
        {
            if (!(Config is Skill_DreamKnightConfig mConfig))
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

            return TryUseByUseStandard(b_Attacker, b_Response);
        }

        public override bool UseSkill(CombatSource b_Attacker, CombatSource b_BeAttacker, BattleComponent b_BattleComponent)
        {
            if (!(Config is Skill_DreamKnightConfig mConfig))
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

                var mWillpower = b_Attacker.GetNumerialFunc(E_GameProperty.Property_Willpower);
                int mStrengthValue = 3 + (int)(mWillpower / 9);
                void action(CombatSource b_BeAttacker1)
                {

                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                    {
                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.DeathRemoveTask;
                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                        mBattleSyncTimer.SyncWaitTime = mConfig.PersistentTime;
                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                        mBattleSyncTimer.DisposeAction = (b_BeAttacker1, b_BattleComponent, b_TimerTask) =>
                        {
                            if (b_BeAttacker1.IsDisposeable) return;

                            b_BeAttacker1.HealthStatsDic.TryGetValue(E_BattleSkillStats.HeiYaoShi704, out var varInfo);
                            if (varInfo == null) return;
                            var MinAtteck = b_BeAttacker1.GamePropertyDic[E_GameProperty.MinAtteck];
                            var MaxAtteck = b_BeAttacker1.GamePropertyDic[E_GameProperty.MaxAtteck];
                            var ValueI = MinAtteck - varInfo.CacheDatas[0].StrengthValue;
                            var ValueA = MaxAtteck - varInfo.CacheDatas[0].StrengthValue;

                            b_BeAttacker1.GamePropertyDic[E_GameProperty.MinAtteck] = ValueI;
                            b_BeAttacker1.GamePropertyDic[E_GameProperty.MaxAtteck] = ValueA;

                            b_BeAttacker1.RemoveHealthState(E_BattleSkillStats.HeiYaoShi704, b_BattleComponent);
                            b_BeAttacker1.UpdateHealthState();
                            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                            {
                                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                mBattleKVData.Key = (int)b_GameProperty;
                                mBattleKVData.Value = b_BeAttacker1.GetNumerialFunc(b_GameProperty);
                                b_ChangeValue_notice.Info.Add(mBattleKVData);
                            }
                            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                            mChangeValueMessage.GameUserId = b_BeAttacker1.InstanceId;

                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.MinAtteck);
                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.MaxAtteck);

                            b_BattleComponent.Parent.SendNotice(b_BeAttacker1, mChangeValueMessage);
                        };
                        mBattleSyncTimer.SyncAction = (b_BeAttacker1, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                        {

                            if (b_BeAttacker1.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                            if (b_BeAttacker1.HealthStatsDic.TryGetValue(E_BattleSkillStats.HeiYaoShi704, out var hp_Curse) == false)
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

                                b_BeAttacker1.AddTask(b_TimerTask);
                            }
                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                        };

                        return mBattleSyncTimer;
                    };


                    int b_AddValue = mStrengthValue;
                    if (b_Attacker.BattleMasteryDic.ContainsKey(E_BattleMasteryState.MasterAttribute932))
                    {
                        int mMasteryValue = b_Attacker.BattleMasteryDic[E_BattleMasteryState.MasterAttribute932];
                        b_AddValue += b_AddValue * mMasteryValue/100;
                    }
                    if (!b_BeAttacker1.HealthStatsDic.ContainsKey(E_BattleSkillStats.HeiYaoShi704))
                    {
                        // 提高攻击力
                        b_BeAttacker1.AddHealthState(E_BattleSkillStats.HeiYaoShi704, b_AddValue, mConfig.PersistentTime, 0, mCreateFunc, b_BattleComponent);
                        b_BeAttacker1.UpdateHealthState();
                        b_BeAttacker1.GamePropertyDic[E_GameProperty.MinAtteck] += b_AddValue;
                        b_BeAttacker1.GamePropertyDic[E_GameProperty.MaxAtteck] += b_AddValue;
                        G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                        mChangeValueMessage.GameUserId = b_BeAttacker1.InstanceId;
                        // 当前值
                        G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                        mPropertyData.Key = (int)E_GameProperty.MinAtteck;
                        mPropertyData.Value = b_BeAttacker1.GetNumerialFunc(E_GameProperty.MinAtteck);
                        mChangeValueMessage.Info.Add(mPropertyData);
                        // 最大值
                        mPropertyData = new G2C_BattleKVData();
                        mPropertyData.Key = (int)E_GameProperty.MaxAtteck;
                        mPropertyData.Value = b_BeAttacker1.GetNumerialFunc(E_GameProperty.MaxAtteck);
                        mChangeValueMessage.Info.Add(mPropertyData);

                        b_BattleComponent.Parent.SendNotice(b_BeAttacker1, mChangeValueMessage);
                    }

                }

                //action(b_Attacker);

                bool mIsHasTeam = false;
                Dictionary<long, Player> mDic = null;
                var mTeamComponent = (b_Attacker as GamePlayer).Player.GetCustomComponent<TeamComponent>();
                if (mTeamComponent != null)
                {
                    TeamManageComponent mTeamManageComponent = Root.MainFactory.GetCustomComponent<TeamManageComponent>();
                    mDic = mTeamManageComponent.GetAllByTeamID(mTeamComponent.TeamID);
                    mIsHasTeam = mDic != null && mDic.ContainsKey(b_Attacker.InstanceId);
                }

                if (mDic != null && mDic.Count > 0)
                {
                    var mRadius = mConfig.OtherDataDic[2];

                    var mRadiusLen = mRadius * 10 + 5;
                    Vector2 mCenterPos = b_BattleComponent.Parent.GetFindTheWay2D(b_Attacker).Vector2Pos;
                    var mPlayerlist = mDic.Values.ToArray();
                    for (int i = 0; i < mPlayerlist.Length; i++)
                    {
                        var mPlayer = mPlayerlist[i];

                        var mCurrentTemp = mPlayer.GetCustomComponent<GamePlayer>();
                        if (mCurrentTemp == null) continue;
                        if (mCurrentTemp.IsDeath || mCurrentTemp.IsDisposeable) continue;
                        if (mCurrentTemp.InstanceId == b_Attacker.InstanceId) continue;

                        var mTargetFindTheWay = b_BattleComponent.Parent.GetFindTheWay2D(mCurrentTemp);
                        if (mTargetFindTheWay != null)
                        {
                            if (10 * Vector2.Distance(mCenterPos, mTargetFindTheWay.Vector2Pos) <= mRadiusLen)
                            {
                                action(mCurrentTemp);
                            }
                        }
                    }
                }

            };
            b_Attacker.CombatRoundId = 0;
            b_BattleComponent.WaitSync(mDamageWait, b_Attacker.InstanceId, mBeAttacker.InstanceId, mSyncAction);
            b_Attacker.CombatRoundId = b_BattleComponent.WaitSync(mAttackTime, b_Attacker.InstanceId, mBeAttacker.InstanceId, (b_CombatRoundId, b_AttackerId, b_BeAttackerId) => { if (b_Attacker.CombatRoundId == b_CombatRoundId) b_Attacker.IsAttacking = false; }, () => { b_Attacker.CombatRoundId = 0; });
            return true;
        }

    }
}
