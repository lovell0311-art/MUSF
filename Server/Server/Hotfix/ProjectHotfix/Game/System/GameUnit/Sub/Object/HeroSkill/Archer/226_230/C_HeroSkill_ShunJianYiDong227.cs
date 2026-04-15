
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
    /// 幻影移行
    /// </summary>
    [HeroSkillAttribute(BindId = (int)E_HeroSkillId.ShunJianYiDong227)]
    public partial class C_HeroSkill_ShunJianYiDong227 : C_HeroSkillSource
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

            if (!(Config is Skill_ArcherConfig mConfig))
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
    /// 幻影移行
    /// </summary>
    public partial class C_HeroSkill_ShunJianYiDong227
    {
        public bool TryUseByUseStandard(CombatSource b_Attacker, IActorResponse b_Response)
        {
            if (!(Config is Skill_ArcherConfig mConfig))
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
            if (!(Config is Skill_ArcherConfig mConfig))
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

            if (b_Cell.IsStaticObstacle)
            {
                //目标数据异常
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(412);
                return false;
            }

            return TryUseByUseStandard(b_Attacker, b_Response);
        }

        public override bool UseSkill(CombatSource b_Attacker, CombatSource b_BeAttacker, C_FindTheWay2D b_Cell, BattleComponent b_BattleComponent)
        {
            if (!(Config is Skill_ArcherConfig mConfig))
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
            mAttackStartNotice.AttackTarget = 0;
            mAttackStartNotice.AttackType = Id;
            mAttackStartNotice.Ticks = mGamePlayer.Player.ClientTime.ClientTime + mAttackTime;
            mAttackStartNotice.MpValue = b_Attacker.UnitData.Mp - (int)(this.MP * (100 - b_Attacker.GetNumerialFunc(E_GameProperty.MpConsumeRate_Reduce)) / 100f);
            mAttackStartNotice.AG = b_Attacker.UnitData.AG - this.AG;
            b_BattleComponent.Parent.SendNotice(b_Attacker, mAttackStartNotice);

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
                // 瞬间移动是否是指定位置传送
                var mFindTheWaySource = b_BattleComponent.Parent.GetFindTheWay2D(b_Attacker);
                if (Vector2.Distance(mFindTheWaySource.Vector2Pos, b_Cell.Vector2Pos) > mConfig.Distance)
                {
                    G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                    mAttackResultNotice.HurtValueType = 6;
                    mAttackResultNotice.AttackTarget = mBeAttacker.InstanceId;
                    b_BattleComponent.Parent.SendNotice(mBeAttacker, mAttackResultNotice);

                    return;
                }

                bool mTrigger = false;
                C_FindTheWay2D mTargetPoint = mFindTheWaySource;

                float k = (float)(b_Cell.Y - mFindTheWaySource.Y) / (b_Cell.X - mFindTheWaySource.X);

                if (b_Cell.X == mFindTheWaySource.X)
                {
                    int mIntervalx = Math.Abs(b_Cell.Y - mFindTheWaySource.Y);
                    int mSymbolx = mFindTheWaySource.Y <= b_Cell.Y ? 1 : -1;
                    for (int i = 1; i < mIntervalx; i++)
                    {
                        int mAddValue = i * mSymbolx;

                        int mIndexValuex = mFindTheWaySource.X;
                        int mIndexValuey = mFindTheWaySource.Y + mAddValue;
                        var mCurrentTemp = b_BattleComponent.Parent.GetFindTheWay2D(mIndexValuex, mIndexValuey);

                        if (mCurrentTemp != null)
                        {
                            if (mCurrentTemp.IsStaticObstacle)
                            {
                                // 中间截断
                                mTrigger = true;
                                break;
                            }
                            mTargetPoint = mCurrentTemp;
                        }
                    }
                }
                else
                {
                    int mIntervalx = Math.Abs(b_Cell.X - mFindTheWaySource.X);
                    int mSymbolx = mFindTheWaySource.X <= b_Cell.X ? 1 : -1;
                    for (int i = 1; i < mIntervalx; i++)
                    {
                        int mAddValue = i * mSymbolx;
                        int y = (int)(k * mAddValue);

                        int mIndexValuex = mFindTheWaySource.X + mAddValue;
                        int mIndexValuey = mFindTheWaySource.Y + y;
                        var mCurrentTemp = b_BattleComponent.Parent.GetFindTheWay2D(mIndexValuex, mIndexValuey);

                        if (mCurrentTemp != null)
                        {
                            if (mCurrentTemp.IsStaticObstacle)
                            {
                                // 中间截断
                                mTrigger = true;
                                break;
                            }
                            mTargetPoint = mCurrentTemp;
                        }
                    }
                }
                if (mTrigger == false)
                {
                    mTargetPoint = b_Cell;
                }

                var mGamePlayer = b_Attacker as GamePlayer;
                // 公告移动信息
                b_BattleComponent.Parent.MoveSendNotice(mFindTheWaySource, mTargetPoint, mGamePlayer, false);

                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)mGamePlayer.Player.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)mGamePlayer.Player.GameAreaId);
                mWriteDataComponent.Save(b_Attacker.UnitData, dBProxy2).Coroutine();

            };
            b_Attacker.CombatRoundId = 0;
            b_BattleComponent.WaitSync(mDamageWait, b_Attacker.InstanceId, mBeAttacker.InstanceId, mSyncAction);
            b_Attacker.CombatRoundId = b_BattleComponent.WaitSync(mAttackTime, b_Attacker.InstanceId, mBeAttacker.InstanceId, (b_CombatRoundId, b_AttackerId, b_BeAttackerId) => { if (b_Attacker.CombatRoundId == b_CombatRoundId) b_Attacker.IsAttacking = false; }, () => { b_Attacker.CombatRoundId = 0; });
            return true;
        }
    }
}
