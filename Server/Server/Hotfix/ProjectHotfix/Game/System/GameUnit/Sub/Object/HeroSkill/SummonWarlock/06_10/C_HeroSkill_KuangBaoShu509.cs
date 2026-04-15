
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
    /// 狂暴术
    /// </summary>
    [HeroSkillAttribute(BindId = (int)E_HeroSkillId.KuangBaoShu509)]
    public partial class C_HeroSkill_KuangBaoShu509 : C_HeroSkillSource
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
    /// 狂暴术
    /// </summary>
    public partial class C_HeroSkill_KuangBaoShu509
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
            return b_Attacker;
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

            return TryUseByUseStandard(b_Attacker, b_Response);
        }
        public override bool UseSkill(CombatSource b_Attacker, CombatSource b_BeAttacker, BattleComponent b_BattleComponent)
        {
            if (!(Config is Skill_SummonWarlockConfig mConfig))
            {
                return false;
            }
            //BattleComponent.Log($"使用技能 <<{mConfig.Name}>> 使用者为:{b_Attacker.InstanceId}", false);

            CombatSource mBeAttacker = b_Attacker;

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

                int mWillpower = b_Attacker.GetNumerialFunc(E_GameProperty.Property_Willpower);
                int mStrength = b_Attacker.GetNumerialFunc(E_GameProperty.Property_Strength);
                int mAgility = b_Attacker.GetNumerialFunc(E_GameProperty.Property_Agility);
                // 单位:秒
                int mPersistentTime = (int)(mWillpower * 0.02f) + 50;
                mPersistentTime *= 1000;

                int mHpRateReduce = 40 - mWillpower / 63;
                if (mHpRateReduce < 10) mHpRateReduce = 10;
                int mHP_MAX = b_Attacker.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                mHpRateReduce = mHP_MAX * mHpRateReduce / 100;

                int mDefense = b_Attacker.GetNumerialFunc(E_GameProperty.Defense);
                int mDefenseRateReduce = 40 - mWillpower / 63;
                if (mDefenseRateReduce < 10) mDefenseRateReduce = 10;
                mDefenseRateReduce = mDefense * mDefenseRateReduce / 100;

                int mMpRateIncrease = mWillpower / 30;
                if (mMpRateIncrease <= 0) mMpRateIncrease = 1;
                int mMinIncrease = (140 + (mStrength + mAgility) / 50) * mMpRateIncrease / 100;
                int mMaxIncrease = (160 + (mStrength + mAgility) / 30) * mMpRateIncrease / 100;

                Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                {
                    var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                    mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.DeathRemoveTask;
                    mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                    mBattleSyncTimer.SyncWaitTime = mPersistentTime;
                    mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                    mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                    {
                        if (b_CombatSource.IsDisposeable) return;

                        b_CombatSource.RemoveHealthState(E_BattleSkillStats.KuangBaoShu509, b_BattleComponent);
                        b_CombatSource.UpdateHealthState();

                        void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                        {
                            G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                            mBattleKVData.Key = (int)b_GameProperty;
                            mBattleKVData.Value = b_CombatSource.GetNumerialFunc(b_GameProperty);
                            b_ChangeValue_notice.Info.Add(mBattleKVData);
                        }
                        G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                        mChangeValueMessage.GameUserId = b_CombatSource.InstanceId;

                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP_MAX);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_MP);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_MP_MAX);

                        b_BattleComponent.Parent.SendNotice(b_CombatSource, mChangeValueMessage);
                    };
                    mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                    {
                        if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                        if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.KuangBaoShu509, out var hp_Curse) == false)
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

                b_Attacker.AddHealthState(E_BattleSkillStats.KuangBaoShu509, 0, mPersistentTime, 0, mCreateFunc, b_BattleComponent);
                b_Attacker.UpdateHealthState();
                if (b_Attacker.HealthStatsDic.TryGetValue(E_BattleSkillStats.KuangBaoShu509, out var mTempBuffer))
                {
                    if (mTempBuffer.CacheDatas.TryGetValue(0, out var mTempBufferData) == false)
                    {
                        mTempBuffer.CacheDatas = new Dictionary<int, CombatSource.C_CombatUnitStatsSource>();
                    }
                    if (mTempBufferData.CacheData == null)
                    {
                        mTempBufferData.CacheData = new Dictionary<int, int>();
                    }

                    mTempBufferData.CacheData[(int)E_GameProperty.PROP_MP_MAXPct] = mMpRateIncrease;
                    mTempBufferData.CacheData[(int)E_GameProperty.MinAtteck] = mMinIncrease;
                    mTempBufferData.CacheData[(int)E_GameProperty.MaxAtteck] = mMaxIncrease;
                    mTempBufferData.CacheData[(int)E_GameProperty.MinMagicAtteck] = mMinIncrease;
                    mTempBufferData.CacheData[(int)E_GameProperty.MaxMagicAtteck] = mMaxIncrease;
                    if (b_Attacker.BattleMasteryDic.TryGetValue(E_BattleMasteryState.Skill_KuangBaoShu_Strengthen550, out var mMasteryValue))
                    {
                        mTempBufferData.CacheData[(int)E_GameProperty.MinMagicAtteck] = mMinIncrease + mMasteryValue;
                        mTempBufferData.CacheData[(int)E_GameProperty.MaxMagicAtteck] = mMaxIncrease + mMasteryValue;
                    }
                    if (b_Attacker.BattleMasteryDic.TryGetValue(E_BattleMasteryState.KuangBao_Master554, out mMasteryValue))
                    {
                        mTempBufferData.CacheData[(int)E_GameProperty.AttackSpeed] = mMasteryValue;
                    }
                    else
                    {
                        mTempBufferData.CacheData[(int)E_GameProperty.PROP_HP_MAX] = mHpRateReduce;
                        mTempBufferData.CacheData[(int)E_GameProperty.Defense] = mDefenseRateReduce;
                    }

                    mTempBufferData.CacheData[(int)E_GameProperty.MinDamnationAtteck] = mMinIncrease;
                    mTempBufferData.CacheData[(int)E_GameProperty.MaxDamnationAtteck] = mMaxIncrease;
                }
                b_Attacker.ChangeNumerialType(E_GameProperty.PROP_HP_MAX);
                b_Attacker.ChangeNumerialType(E_GameProperty.Defense);
                b_Attacker.ChangeNumerialType(E_GameProperty.PROP_MP_MAXPct);
                b_Attacker.ChangeNumerialType(E_GameProperty.MinAtteck);
                b_Attacker.ChangeNumerialType(E_GameProperty.MaxAtteck);
                b_Attacker.ChangeNumerialType(E_GameProperty.MinMagicAtteck);
                b_Attacker.ChangeNumerialType(E_GameProperty.MaxMagicAtteck);
                b_Attacker.ChangeNumerialType(E_GameProperty.MinDamnationAtteck);
                b_Attacker.ChangeNumerialType(E_GameProperty.MaxDamnationAtteck);

                G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                mChangeValueMessage.GameUserId = b_Attacker.InstanceId;
                // 当前值
                G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                mPropertyData.Key = (int)E_GameProperty.PROP_HP;
                mPropertyData.Value = b_Attacker.GetNumerialFunc(E_GameProperty.PROP_HP);
                mChangeValueMessage.Info.Add(mPropertyData);
                // 最大值
                mPropertyData = new G2C_BattleKVData();
                mPropertyData.Key = (int)E_GameProperty.PROP_HP_MAX;
                mPropertyData.Value = b_Attacker.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                mChangeValueMessage.Info.Add(mPropertyData);

                mPropertyData = new G2C_BattleKVData();
                mPropertyData.Key = (int)E_GameProperty.PROP_MP;
                mPropertyData.Value = b_Attacker.GetNumerialFunc(E_GameProperty.PROP_MP);
                mChangeValueMessage.Info.Add(mPropertyData);
                // 最大值
                mPropertyData = new G2C_BattleKVData();
                mPropertyData.Key = (int)E_GameProperty.PROP_MP_MAX;
                mPropertyData.Value = b_Attacker.GetNumerialFunc(E_GameProperty.PROP_MP_MAX);
                mChangeValueMessage.Info.Add(mPropertyData);

                b_BattleComponent.Parent.SendNotice(b_Attacker, mChangeValueMessage);

                //b_Attacker.CombatRoundId = b_BattleComponent.WaitSync(mAttackTime, (b_CombatRoundId, b_AttackerId, b_BeAttackerId) => { if (b_Attacker.CombatRoundId == b_CombatRoundId) b_Attacker.IsAttacking = false; }, () => { b_Attacker.CombatRoundId = 0; });
            };
            b_Attacker.CombatRoundId = 0;
            b_BattleComponent.WaitSync(mDamageWait, b_Attacker.InstanceId, mBeAttacker.InstanceId, mSyncAction);
            b_Attacker.CombatRoundId = b_BattleComponent.WaitSync(mAttackTime, b_Attacker.InstanceId, mBeAttacker.InstanceId, (b_CombatRoundId, b_AttackerId, b_BeAttackerId) => { if (b_Attacker.CombatRoundId == b_CombatRoundId) b_Attacker.IsAttacking = false; }, () => { b_Attacker.CombatRoundId = 0; });
            return true;
        }
    }
}
