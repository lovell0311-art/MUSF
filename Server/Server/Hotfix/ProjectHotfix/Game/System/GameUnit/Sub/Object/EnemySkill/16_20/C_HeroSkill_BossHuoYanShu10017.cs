
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
    /// 火焰术
    /// </summary>
    [HeroSkillAttribute(BindId = (int)E_HeroSkillId.BossHuoYanShu10017)]
    public partial class C_HeroSkill_BossHuoYanShu10017 : C_HeroSkillSource
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

            if (!(Config is Skill_monsterConfig mConfig))
            {
                return;
            }

            CoolTime = mConfig.CoolTime;

            //
            

            NextAttackTime = 0;
            IsDataHasError = false;
        }
    }

    /// <summary>
    /// 火焰术
    /// </summary>
    public partial class C_HeroSkill_BossHuoYanShu10017
    {
        public bool TryUseByUseStandard(CombatSource b_Attacker, IActorResponse b_Response)
        {
            if (b_Attacker.Identity == E_Identity.Hero)
            {
                return false;
            }
            return true;
        }
        public override CombatSource FindTarget(CombatSource b_Attacker, CombatSource b_BeAttacker, C_FindTheWay2D b_Cell, BattleComponent b_BattleComponent, IActorResponse b_Response)
        {
            return b_Attacker;
        }
        public override bool TryUse(CombatSource b_Attacker, CombatSource b_BeAttacker, C_FindTheWay2D b_Cell, BattleComponent b_BattleComponent, IActorResponse b_Response)
        {
            if (!(Config is Skill_monsterConfig mConfig))
            {
                return false;
            }
            if (b_Attacker.UnitData.Mp < MP || b_Attacker.UnitData.AG < AG)
            {
                return false;
            }

            return TryUseByUseStandard(b_Attacker, b_Response);
        }
        public override bool UseSkill(CombatSource b_Attacker, CombatSource b_BeAttacker, BattleComponent b_BattleComponent)
        {
            if (!(Config is Skill_monsterConfig mConfig))
            {
                return false;
            }
            //BattleComponent.Log($"使用技能 <<{mConfig.Name}>> 使用者为:{b_Attacker.InstanceId}", false);

            CombatSource mBeAttacker = b_BeAttacker;

            int mAttackTime = 0;
            switch (b_Attacker.Identity)
            {
                case E_Identity.Enemy:
                    {
                        mAttackTime = (int)((b_Attacker as Enemy).Config.AtSpeed * 0.5f);
                    }
                    break;
                case E_Identity.Summoned:
                    {
                        mAttackTime = (int)((b_Attacker as Summoned).Config.AtSpeed * 0.5f);
                    }
                    break;
                default:
                    mAttackTime = (int)(b_Attacker.GetAttackSpeed(true) * 0.5f);
                    break;
            }

            G2C_AttackStart_notice mAttackStartNotice = new G2C_AttackStart_notice();
            mAttackStartNotice.AttackSource = b_Attacker.InstanceId;
            mAttackStartNotice.AttackTarget = mBeAttacker.InstanceId;
            mAttackStartNotice.AttackType = Id;
            //mAttackStartNotice.Ticks = Help_TimeHelper.GetNow() + mAttackTime + mAttackTime;
            mAttackStartNotice.MpValue = b_Attacker.UnitData.Mp - (int)(this.MP * (100 - b_Attacker.GetNumerialFunc(E_GameProperty.MpConsumeRate_Reduce)) / 100f);
            mAttackStartNotice.AG = b_Attacker.UnitData.AG - this.AG;

            long mNowTime = b_BattleComponent.CurrentTimeTick;
            mAttackStartNotice.TimeTick = mNowTime;
            // 持续多少秒
            int mMaxPersistentTime = mConfig.PersistentTime / 1000;
            if (mMaxPersistentTime <= 0) mMaxPersistentTime = 1;
            // 每秒多少个
            int mMaxCountBySecond = 10;

            int mWarningTime = mConfig.OtherDataDic[5];
            int mTargetX = b_Attacker.UnitData.X;
            int mTargetY = b_Attacker.UnitData.Y;
            Vector2 mCenterPos = b_BattleComponent.Parent.GetFindTheWay2D(b_Attacker).Vector2Pos;
            Dictionary<int, Dictionary<int, C_FindTheWay2D>> mMapFieldDic = new Dictionary<int, Dictionary<int, C_FindTheWay2D>>();
            var mRadius = mConfig.OtherDataDic[2];
            var mRadiusMax = mRadius * 2;
            // 目标范围
            (int X, int Y) mStartPos = (mTargetX - mRadius, mTargetY - mRadius);

            var mRadiusLen = mRadius * 10 + 5;
            for (int i = 0; i < mMaxPersistentTime; i++)
            {
                int mStartTime = i * 1000;
                int mEndTime = mStartTime + 1000;

                if (mMapFieldDic.TryGetValue(i, out var mMapFieldDicTemp) == false)
                {
                    mMapFieldDicTemp = mMapFieldDic[i] = new Dictionary<int, C_FindTheWay2D>();
                }

                for (int j = 0; j < mMaxCountBySecond; j++)
                {
                    int mRandomTime = Help_RandomHelper.Range(mStartTime, mEndTime);
                    long mTargetTime = mRandomTime;

                    int mRandomX = mStartPos.X + Help_RandomHelper.Range(0, mRadiusMax);
                    int mRandomY = mStartPos.Y + Help_RandomHelper.Range(0, mRadiusMax);

                    var mCurrentTemp = b_BattleComponent.Parent.GetFindTheWay2D(mRandomX, mRandomY);
                    if (mCurrentTemp == null) continue;

                    if (10 * Vector2.Distance(mCenterPos, mCurrentTemp.Vector2Pos) >= mRadiusLen) continue;

                    long mPosData = ((long)mTargetTime << 32) | ((long)mRandomX << 16) | (long)mRandomY;
                    mAttackStartNotice.Pos.Add(mPosData);

                    mMapFieldDicTemp[mRandomTime + mWarningTime] = mCurrentTemp;
                }
            }
            b_BattleComponent.Parent.SendNotice(mBeAttacker, mAttackStartNotice);

            b_Attacker.IsAttacking = true;
            b_Attacker.AttackTime = b_BattleComponent.CurrentTimeTick + mAttackTime + mAttackTime;

            b_Attacker.InCasting = true;
            Action<long, long, long> mSyncAction = (b_CombatRoundId, b_AttackerId, b_BeAttackerId) =>
            {
                if (b_Attacker.InstanceId != b_AttackerId || b_Attacker.IsDeath || b_Attacker.IsDisposeable || b_Attacker.UnitData.Index != b_BattleComponent.Parent.MapId)
                {
                    b_BattleComponent.WaitSync(mAttackTime, b_AttackerId, b_BeAttackerId, (b_CombatRoundId, b_AttackerId, b_BeAttackerId) => { b_Attacker.InCasting = false; });
                    return;
                }

                int mMinAtteck = mConfig.OtherDataDic[1] + b_Attacker.GetNumerialFunc(E_GameProperty.MinAtteck);
                int mMaxAtteck = mConfig.OtherDataDic[1] + b_Attacker.GetNumerialFunc(E_GameProperty.MaxAtteck);
                var mAddScale = mConfig.OtherDataDic[11];
                int GetHurtValue(E_GameProperty b_SpecialAttack)
                {
                    int mVirtualHurtValue;

                    if (b_SpecialAttack == E_GameProperty.LucklyAttackRate)
                    {
                        mVirtualHurtValue = mMaxAtteck + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedLuckyStrokeAttack) + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedSkillAttack);
                    }
                    else if (b_SpecialAttack == E_GameProperty.ExcellentAttackRate)
                    {
                        mVirtualHurtValue = Help_RandomHelper.Range(mMinAtteck, mMaxAtteck) + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedGreatShotAttack) + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedSkillAttack);
                    }
                    else
                    {
                        mVirtualHurtValue = Help_RandomHelper.Range(mMinAtteck, mMaxAtteck) + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedSkillAttack);
                    }

                    // 总伤害 = 魔法伤害 + 魔力伤害 + 魔杖魔力提升百分比
                    int mHurtValue = mVirtualHurtValue;
                    if (mAddScale != 100)
                        mHurtValue = (int)(mHurtValue * mAddScale * 0.01f);

                    return mHurtValue;
                }

                var mCurrnetPKModel = b_Attacker.PKModel();
                bool mIsHasTeam = false;
                Dictionary<long, Player> mDic = null;
                if (mCurrnetPKModel == E_PKModel.Friend)
                {
                    if (b_Attacker.Identity == E_Identity.Summoned)
                    {
                        Summoned mSummoned = b_Attacker as Summoned;

                        var mTeamComponent = mSummoned.GamePlayer.Player.GetCustomComponent<TeamComponent>();
                        if (mTeamComponent != null)
                        {
                            TeamManageComponent mTeamManageComponent = Root.MainFactory.GetCustomComponent<TeamManageComponent>();
                            mDic = mTeamManageComponent.GetAllByTeamID(mTeamComponent.TeamID);
                            mIsHasTeam = mDic != null && mDic.ContainsKey(mSummoned.GamePlayer.InstanceId);
                        }
                    }
                }
                var mAttackerFanJiIdlist = b_Attacker.GetFanJiIdlist();
                long mPlayerInstance = b_Attacker.GetPlayerInstance();

                for (int p = 0; p < mMaxPersistentTime; p++)
                {
                    if (mMapFieldDic.TryGetValue(p, out var mMapFieldDicTemp) == false) continue;

                    var mMapFieldKeylist = mMapFieldDicTemp.Keys.ToArray();
                    for (int i = 0, len = mMapFieldKeylist.Length; i < len; i++)
                    {
                        var mMapFieldKey = mMapFieldKeylist[i];

                        if (mMapFieldDicTemp.TryGetValue(mMapFieldKey, out var mMapField) == false) continue;

                        Action<C_FindTheWay2D, long, long> mSyncActionTemp = (b_MapField, b_AttackerId, b_BeAttackerId) =>
                        {
                            if (b_Attacker.InstanceId != b_AttackerId || b_Attacker.IsDeath || b_Attacker.IsDisposeable || b_Attacker.UnitData.Index != b_BattleComponent.Parent.MapId)
                            {
                                b_BattleComponent.WaitSync(mAttackTime, b_AttackerId, b_BeAttackerId, (b_CombatRoundId, b_AttackerId, b_BeAttackerId) => { b_Attacker.InCasting = false; });
                                return;
                            }

                            var mMapComponent = b_MapField.Map;

                            int mTargetTempX = b_MapField.X;
                            int mTargetTempY = b_MapField.Y;
                            //Vector2 mCenterPosTemp = new Vector2(mTargetX, mTargetY);
                            Dictionary<long, C_FindTheWay2D> mMapFindTheWayDic = new Dictionary<long, C_FindTheWay2D>();
                            int mRadiusTemp = 2;
                            int mRadiusMaxTemp = mRadiusTemp * 2;
                            // 目标范围
                            (int X, int Y) mStartPosTemp = (mTargetTempX - mRadiusTemp, mTargetTempY - mRadiusTemp);

                            for (int i = 0; i <= mRadiusMaxTemp; i++)
                            {
                                for (int j = 0; j <= mRadiusMaxTemp; j++)
                                {
                                    var mCurrentTemp = mMapComponent.GetFindTheWay2D(mStartPosTemp.X + i, mStartPosTemp.Y + j);
                                    if (mCurrentTemp == null) continue;
                                    if (mCurrentTemp.HasUnit() == false) continue;

                                    //if (Vector2.Distance(mCenterPosTemp, new Vector2(mCurrentTemp.X, mCurrentTemp.Y)) < mRadiusTemp)
                                    {
                                        if (mMapFindTheWayDic.ContainsKey(mCurrentTemp.Id) == false)
                                        {
                                            mMapFindTheWayDic[mCurrentTemp.Id] = mCurrentTemp;
                                        }
                                    }
                                }
                            }

                            var mMapFieldTemplist = mMapFindTheWayDic.Values.ToArray();
                            for (int i = 0, len = mMapFieldTemplist.Length; i < len; i++)
                            {
                                var mMapFieldTemp = mMapFieldTemplist[i];

                                if (b_Attacker.Identity != E_Identity.Enemy && mMapFieldTemp.FieldEnemyDic.Count > 0)
                                {
                                    var mFieldEnemylist = mMapFieldTemp.FieldEnemyDic.Values.ToArray();
                                    for (int j = 0, jlen = mFieldEnemylist.Length; j < jlen; j++)
                                    {
                                        var mCurrentTemp = mFieldEnemylist[j];
                                        if (mCurrentTemp.IsDeath || mCurrentTemp.IsDisposeable) continue;

                                        //if (Vector2.Distance(mSelfPos, new Vector2(mCurrentTemp.UnitData.X, mCurrentTemp.UnitData.Y)) < mConfig.Distance)
                                        {
                                            // 是否命中
                                            bool IsHit = b_Attacker.IsHitPvE(mBeAttacker, b_BattleComponent, true);
                                            if (IsHit)
                                            {
                                                var mSpecialAttack = b_Attacker.AttackSpecial();
                                                var mHurtValue = GetHurtValue(mSpecialAttack);
                                                mCurrentTemp.InjureSkill(b_Attacker, E_BattleHurtType.MAGIC, mSpecialAttack, Id, mHurtValue, b_BattleComponent);
                                            }
                                            else
                                            {
                                                G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                                                mAttackResultNotice.AttackTarget = mCurrentTemp.InstanceId;
                                                mAttackResultNotice.HurtValueType = 1;
                                                b_BattleComponent.Parent.SendNotice(mCurrentTemp, mAttackResultNotice);
                                            }
                                        }
                                    }
                                }

                                if (mMapFieldTemp.FieldPlayerDic.Count > 0)
                                {
                                    var mFieldPlayerlist = mMapFieldTemp.FieldPlayerDic.Values.ToArray();
                                    for (int j = 0, jlen = mFieldPlayerlist.Length; j < jlen; j++)
                                    {
                                        var mCurrentTemp = mFieldPlayerlist[j];

                                        if (mCurrentTemp.IsDeath || mCurrentTemp.IsDisposeable) continue;
                                        if (mCurrentTemp.InstanceId == mPlayerInstance) continue;

                                        if (b_Attacker.Identity != E_Identity.Enemy)
                                        {
                                            var mTryUseResult = b_Attacker.TryUseByPlayerKilling(mCurrentTemp, mAttackerFanJiIdlist, mCurrnetPKModel, mIsHasTeam, mDic);
                                            if (mTryUseResult == false) continue;
                                        }

                                        //if (Vector2.Distance(mSelfPos, new Vector2(mCurrentTemp.UnitData.X, mCurrentTemp.UnitData.Y)) < mRadius)
                                        {
                                            // 是否命中
                                            bool IsHit = b_Attacker.IsHitPvE(mBeAttacker, b_BattleComponent, true);
                                            if (IsHit)
                                            {
                                                var mSpecialAttack = b_Attacker.AttackSpecial();
                                                var mHurtValue = GetHurtValue(mSpecialAttack);
                                                mCurrentTemp.InjureSkill(b_Attacker, E_BattleHurtType.MAGIC, mSpecialAttack, Id, mHurtValue, b_BattleComponent);
                                            }
                                            else
                                            {
                                                G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                                                mAttackResultNotice.AttackTarget = mCurrentTemp.Player.GameUserId;
                                                mAttackResultNotice.HurtValueType = 1;
                                                b_BattleComponent.Parent.SendNotice(mCurrentTemp, mAttackResultNotice);
                                            }
                                        }
                                    }
                                }
                                if (false && mMapFieldTemp.FieldPetsDic.Count > 0)
                                {
                                    var mFieldPetslist = mMapFieldTemp.FieldPetsDic.Values.ToArray();
                                    for (int j = 0, jlen = mFieldPetslist.Length; j < jlen; j++)
                                    {
                                        var mCurrentTemp = mFieldPetslist[j];
                                        if (mCurrentTemp.IsDeath || mCurrentTemp.IsDisposeable || mCurrentTemp.GamePlayer.IsDisposeable) continue;
                                        if (mCurrentTemp.GamePlayer.InstanceId == mPlayerInstance) continue;

                                        if (b_Attacker.Identity != E_Identity.Enemy)
                                        {
                                            var mTryUseResult = b_Attacker.TryUseByPlayerKilling(mCurrentTemp, mAttackerFanJiIdlist, mCurrnetPKModel, mIsHasTeam, mDic);
                                            if (mTryUseResult == false) continue;
                                        }
                                        //if (Vector2.Distance(mSelfPos, new Vector2(mCurrentTemp.UnitData.X, mCurrentTemp.UnitData.Y)) < mRadius)
                                        {
                                            // 是否命中
                                            bool IsHit = b_Attacker.IsHitPvE(mCurrentTemp, b_BattleComponent, true);
                                            if (IsHit)
                                            {
                                                var mSpecialAttack = b_Attacker.AttackSpecial();
                                                var mHurtValue = GetHurtValue(mSpecialAttack);
                                                mCurrentTemp.InjureSkill(b_Attacker, E_BattleHurtType.MAGIC, mSpecialAttack, Id, mHurtValue, b_BattleComponent);
                                            }
                                            else
                                            {
                                                G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                                                mAttackResultNotice.AttackTarget = mCurrentTemp.InstanceId;
                                                mAttackResultNotice.HurtValueType = 1;
                                                b_BattleComponent.Parent.SendNotice(mCurrentTemp, mAttackResultNotice);
                                            }
                                        }
                                    }
                                }
                                if (mMapFieldTemp.FieldSummonedDic.Count > 0)
                                {
                                    var mFieldSummonedlist = mMapFieldTemp.FieldSummonedDic.Values.ToArray();
                                    for (int j = 0, jlen = mFieldSummonedlist.Length; j < jlen; j++)
                                    {
                                        var mCurrentTemp = mFieldSummonedlist[j];
                                        if (mCurrentTemp.IsDeath || mCurrentTemp.IsDisposeable || mCurrentTemp.GamePlayer.IsDisposeable) continue;
                                        if (mCurrentTemp.GamePlayer.InstanceId == mPlayerInstance) continue;

                                        if (b_Attacker.Identity != E_Identity.Enemy)
                                        {
                                            var mTryUseResult = b_Attacker.TryUseByPlayerKilling(mCurrentTemp, mAttackerFanJiIdlist, mCurrnetPKModel, mIsHasTeam, mDic);
                                            if (mTryUseResult == false) continue;
                                        }
                                        //if (Vector2.Distance(mSelfPos, new Vector2(mCurrentTemp.UnitData.X, mCurrentTemp.UnitData.Y)) < mRadius)
                                        {
                                            // 是否命中
                                            bool IsHit = b_Attacker.IsHitPvE(mCurrentTemp, b_BattleComponent, true);
                                            if (IsHit)
                                            {
                                                var mSpecialAttack = b_Attacker.AttackSpecial();
                                                var mHurtValue = GetHurtValue(mSpecialAttack);
                                                mCurrentTemp.InjureSkill(b_Attacker, E_BattleHurtType.MAGIC, mSpecialAttack, Id, mHurtValue, b_BattleComponent);
                                            }
                                            else
                                            {
                                                G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                                                mAttackResultNotice.AttackTarget = mCurrentTemp.InstanceId;
                                                mAttackResultNotice.HurtValueType = 1;
                                                b_BattleComponent.Parent.SendNotice(mCurrentTemp, mAttackResultNotice);
                                            }
                                        }
                                    }
                                }
                            }
                        };
                        b_BattleComponent.WaitSync(mMapFieldKey, b_AttackerId, b_BeAttackerId, (b_CombatRoundId, b_AttackerId, b_BeAttackerId) => { mSyncActionTemp(mMapField, b_AttackerId, b_BeAttackerId); });
                    }
                }

                b_Attacker.InCastCombatRoundId = b_BattleComponent.WaitSync(mAttackTime + mConfig.PersistentTime, b_AttackerId, b_BeAttackerId, (b_CombatRoundId, b_AttackerId, b_BeAttackerId) => { if (b_Attacker.InCastCombatRoundId == b_CombatRoundId) b_Attacker.InCasting = false; }, () => { b_Attacker.InCastCombatRoundId = 0; });
            };
            b_Attacker.InCastCombatRoundId = 0;
            b_BattleComponent.WaitSync(mConfig.DamageWait, b_Attacker.InstanceId, mBeAttacker.InstanceId, mSyncAction);
            b_Attacker.CombatRoundId = b_BattleComponent.WaitSync(mAttackTime + mAttackTime, b_Attacker.InstanceId, mBeAttacker.InstanceId, (b_CombatRoundId, b_AttackerId, b_BeAttackerId) => { if (b_Attacker.CombatRoundId == b_CombatRoundId) b_Attacker.IsAttacking = false; }, () => { b_Attacker.CombatRoundId = 0; });
            return true;
        }

    }
}
