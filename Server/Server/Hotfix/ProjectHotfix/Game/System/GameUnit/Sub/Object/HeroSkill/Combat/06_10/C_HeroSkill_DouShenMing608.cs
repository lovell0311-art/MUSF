
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
    /// 斗神-命
    /// </summary>
    [HeroSkillAttribute(BindId = (int)E_HeroSkillId.DouShenMing608)]
    public partial class C_HeroSkill_DouShenMing608 : C_HeroSkillSource
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

            if (!(Config is Skill_CombatConfig mConfig))
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
    /// 斗神-命
    /// </summary>
    public partial class C_HeroSkill_DouShenMing608
    {
        public bool TryUseByUseStandard(CombatSource b_Attacker, IActorResponse b_Response)
        {
            if (!(Config is Skill_CombatConfig mConfig))
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
            if (!(Config is Skill_CombatConfig mConfig))
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
            if (!(Config is Skill_CombatConfig mConfig))
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
                int mMasteryValue = 0;
                if (b_Attacker.BattleMasteryDic.ContainsKey(E_BattleMasteryState.MasterAttribute646))
                    mMasteryValue = b_Attacker.BattleMasteryDic[E_BattleMasteryState.MasterAttribute646];

                void AddBoneGas(CombatSource b_AddAttacker)
                {
                    if (!b_AddAttacker.HealthStatsDic.ContainsKey(E_BattleSkillStats.DouShenMing608))
                    {
                        (b_AddAttacker as GamePlayer).Data.BoneGas += mMasteryValue;
                        (b_AddAttacker as GamePlayer).DataUpdateProperty();

                        void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                        {
                            G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                            mBattleKVData.Key = (int)b_GameProperty;
                            mBattleKVData.Value = (b_AddAttacker as GamePlayer).GetNumerial(b_GameProperty);
                            b_ChangeValue_notice.Info.Add(mBattleKVData);
                        }
                        G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                        mChangeValueMessage.GameUserId = (b_AddAttacker as GamePlayer).InstanceId;
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG_MAX);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD_MAX);

                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP_MAX);

                        switch ((E_GameOccupation)(b_AddAttacker as GamePlayer).Data.PlayerTypeId)
                        {
                            case E_GameOccupation.Combat:
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.AdvanceAttackPower);
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.RangeAttack);
                                AddPropertyNotice(mChangeValueMessage, E_GameProperty.SacredBeast);
                                break;
                            default:
                                break;
                        }
                        b_BattleComponent.Parent.SendNotice(mGamePlayer, mChangeValueMessage);
                    }
                }
                var mHpmax = (b_Attacker as GamePlayer).GetNumerial(E_GameProperty.PROP_HP_MAX, false);
                var mWillpower = b_Attacker.GetNumerialFunc(E_GameProperty.Property_Willpower);
                var mBoneGas = b_Attacker.GetNumerialFunc(E_GameProperty.Property_BoneGas);

                var mStrengthValueProportion = mWillpower * 0.05f + mBoneGas * 0.01f + 10;

                int mStrengthValue = (int)(mHpmax * mStrengthValueProportion * 0.01f);

                //if (mStrengthValue > 1000) mStrengthValue = 1000;
                int mPersistentTime = 60 + (int)(mWillpower * 0.1f);
                mPersistentTime *= 1000;

                void action(CombatSource b_BeAttacker)
                {
                    int b_AddValue = mStrengthValue;
                    //Console.WriteLine($"斗神命属性值{b_AddValue}");
                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                    {
                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.DeathRemoveTask;
                        mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                        mBattleSyncTimer.SyncWaitTime = mConfig.PersistentTime;
                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                        mBattleSyncTimer.DisposeAction = (b_BeAttacker, b_BattleComponent, b_TimerTask) =>
                        {
                            if (b_BeAttacker.IsDisposeable) return;
                            //Console.WriteLine($"<<{(b_Attacker as GamePlayer).Data.NickName}>>释放技能斗神命前--血量:{b_BeAttacker.GetNumerialFunc(E_GameProperty.PROP_HP_MAX)}");
                            b_BeAttacker.RemoveHealthState(E_BattleSkillStats.DouShenMing608, b_BattleComponent);
                            b_BeAttacker.UpdateHealthState();
                            (b_BeAttacker as GamePlayer).Data.BoneGas -= mMasteryValue;
                            (b_BeAttacker as GamePlayer).DataUpdateProperty();

                            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                            {
                                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                                mBattleKVData.Key = (int)b_GameProperty;
                                mBattleKVData.Value = (b_BeAttacker as GamePlayer).GetNumerial(b_GameProperty);
                                b_ChangeValue_notice.Info.Add(mBattleKVData);
                            }
                            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                            mChangeValueMessage.GameUserId = (b_BeAttacker as GamePlayer).InstanceId;
                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG);
                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_AG_MAX);
                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD);
                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_SD_MAX);

                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP);
                            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP_MAX);

                            switch ((E_GameOccupation)(b_BeAttacker as GamePlayer).Data.PlayerTypeId)
                            {
                                case E_GameOccupation.Combat:
                                    AddPropertyNotice(mChangeValueMessage, E_GameProperty.AdvanceAttackPower);
                                    AddPropertyNotice(mChangeValueMessage, E_GameProperty.RangeAttack);
                                    AddPropertyNotice(mChangeValueMessage, E_GameProperty.SacredBeast);
                                    break;
                                default:
                                    break;
                            }
                            b_BattleComponent.Parent.SendNotice(mGamePlayer, mChangeValueMessage);
                            ////Console.WriteLine($"<<{(b_Attacker as GamePlayer).Data.NickName}>>释放技能斗神命后--血量:{b_BeAttacker.GetNumerialFunc(E_GameProperty.PROP_HP_MAX)}");
                            //G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                            //mChangeValueMessage.GameUserId = b_BeAttacker.InstanceId;
                            //// 最大值
                            //G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                            //mPropertyData.Key = (int)E_GameProperty.PROP_HP_MAX;
                            //mPropertyData.Value = b_BeAttacker.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                            //mChangeValueMessage.Info.Add(mPropertyData);

                            //b_BattleComponent.Parent.SendNotice(b_BeAttacker, mChangeValueMessage);
                        };
                        mBattleSyncTimer.SyncAction = (b_BeAttacker, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                        {
                            if (b_BeAttacker.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                            if (b_BeAttacker.HealthStatsDic.TryGetValue(E_BattleSkillStats.DouShenMing608, out var hp_Curse) == false)
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

                                b_BeAttacker.AddTask(b_TimerTask);
                            }
                            return CombatSource.E_SyncTimerTaskResult.NextRound;
                        };

                        return mBattleSyncTimer;
                    };
                    //Console.WriteLine($"斗神命添加状态前血量上限:{b_BeAttacker.GetNumerialFunc(E_GameProperty.PROP_HP_MAX)}");
                    // 提高攻击力
                    b_BeAttacker.AddHealthState(E_BattleSkillStats.DouShenMing608, b_AddValue, mPersistentTime, 0, mCreateFunc, b_BattleComponent);
                    b_BeAttacker.UpdateHealthState();
                    b_BeAttacker.ChangeNumerialType(E_GameProperty.Property_BoneGas);
                    //Console.WriteLine($"斗神命通知状态时血量上限:{b_BeAttacker.GetNumerialFunc(E_GameProperty.PROP_HP_MAX)}");
                    G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                    mChangeValueMessage.GameUserId = b_BeAttacker.InstanceId;
                    // 最大值
                    G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                    mPropertyData.Key = (int)E_GameProperty.PROP_HP_MAX;
                    mPropertyData.Value = b_BeAttacker.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                    mChangeValueMessage.Info.Add(mPropertyData);

                    b_BattleComponent.Parent.SendNotice(b_BeAttacker, mChangeValueMessage);
                }
                //Console.WriteLine($"开始给<<{(b_Attacker as GamePlayer).Data.NickName}>>附加技能斗神命");
                action(b_Attacker);
                //Console.WriteLine($"附加技能斗神命结束");
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
                                AddBoneGas(mCurrentTemp);
                                action(mCurrentTemp);
                            }

                            //if (mCurrentTemp.Data.PlayerTypeId == (int)E_GameOccupation.Archer)
                            //{
                            //    if (mCurrentTemp.Summoned != null && mCurrentTemp.Summoned.IsDeath == false && mCurrentTemp.Summoned.IsDisposeable == false)
                            //    {
                            //        mTargetFindTheWay = b_BattleComponent.Parent.GetFindTheWay2D(mCurrentTemp.Summoned);
                            //        if (mTargetFindTheWay != null)
                            //        {
                            //            if (10 * Vector2.Distance(mCenterPos, mTargetFindTheWay.Vector2Pos) <= mRadiusLen)
                            //            {
                            //                action(mCurrentTemp.Summoned);
                            //            }
                            //        }
                            //    }
                            //}
                        }
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
