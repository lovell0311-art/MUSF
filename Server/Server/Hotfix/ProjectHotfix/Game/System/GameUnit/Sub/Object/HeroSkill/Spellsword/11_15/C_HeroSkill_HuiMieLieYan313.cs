
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
    /// 毁灭烈焰
    /// </summary>
    [HeroSkillAttribute(BindId = (int)E_HeroSkillId.HuiMieLieYan313)]
    public partial class C_HeroSkill_HuiMieLieYan313 : C_HeroSkillSource
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
    /// 毁灭烈焰
    /// </summary>
    public partial class C_HeroSkill_HuiMieLieYan313
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
            return b_Attacker;
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

            return TryUseByUseStandard(b_Attacker, b_Response);
        }
        public override bool UseSkill(CombatSource b_Attacker, CombatSource b_BeAttacker, BattleComponent b_BattleComponent)
        {
            if (!(Config is Skill_SpellswordConfig mConfig))
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


                int mMinAtteck = mConfig.OtherDataDic[1] + b_Attacker.GetNumerialFunc(E_GameProperty.MinMagicAtteck);
                int mMaxAtteck = (int)(mConfig.OtherDataDic[1] * 1.5f) + b_Attacker.GetNumerialFunc(E_GameProperty.MaxMagicAtteck);
                var mAppendValue = b_Attacker.GetNumerialFunc(E_GameProperty.Property_Willpower) / mConfig.OtherDataDic[10];
                var mAddScale = mConfig.OtherDataDic[11];
                var mMagicRate_Increase = b_Attacker.GetNumerialFunc(E_GameProperty.MagicRate_Increase);
                int GetHurtValue(E_GameProperty b_SpecialAttack)
                {
                    int mVirtualHurtValue;

                    if (b_SpecialAttack == E_GameProperty.LucklyAttackRate)
                    {
                        mVirtualHurtValue = mMaxAtteck + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedLuckyStrokeAttack) + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedSkillAttack);
                    }
                    else
                    {
                        mVirtualHurtValue = Help_RandomHelper.Range(mMinAtteck, mMaxAtteck) + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedSkillAttack);
                        if (b_SpecialAttack == E_GameProperty.ExcellentAttackRate)
                            mVirtualHurtValue += b_Attacker.GetNumerialFunc(E_GameProperty.EmbedGreatShotAttack);
                    }

                    // 魔杖魔力提升百分比
                    int mMaxMagicAtteckIncrease = (int)(mMaxAtteck * mMagicRate_Increase * 0.01f);
                    mVirtualHurtValue += mMaxMagicAtteckIncrease;

                    // 总伤害 = 魔法伤害 + 魔力伤害 + 魔杖魔力提升百分比
                    int mHurtValue = mVirtualHurtValue + mAppendValue;
                    if (mAddScale != 100)
                        mHurtValue = (int)(mHurtValue * mAddScale * 0.01f);

                    return mHurtValue;
                }

                int mTargetX = b_Attacker.UnitData.X;
                int mTargetY = b_Attacker.UnitData.Y;
                Vector2 mCenterPos = b_BattleComponent.Parent.GetFindTheWay2D(b_Attacker).Vector2Pos;
                Dictionary<long, C_FindTheWay2D> mMapFieldDic = new Dictionary<long, C_FindTheWay2D>();
                var mRadius = mConfig.OtherDataDic[2];
                var mRadiusMax = mRadius * 2;
                // 目标范围
                (int X, int Y) mStartPos = (mTargetX - mRadius, mTargetY - mRadius);

                var mRadiusLen = mRadius * 10 + 5;
                for (int i = 0; i <= mRadiusMax; i++)
                {
                    for (int j = 0; j <= mRadiusMax; j++)
                    {
                        var mCurrentTemp = b_BattleComponent.Parent.GetFindTheWay2D(mStartPos.X + i, mStartPos.Y + j);
                        if (mCurrentTemp == null) continue;
                        if (mCurrentTemp.HasUnit() == false) continue;

                        if (10 * Vector2.Distance(mCenterPos, mCurrentTemp.Vector2Pos) <= mRadiusLen)
                        {
                            if (mMapFieldDic.ContainsKey(mCurrentTemp.Id) == false)
                            {
                                mMapFieldDic[mCurrentTemp.Id] = mCurrentTemp;
                            }
                        }
                    }
                }

                var mCurrnetPKModel = b_Attacker.PKModel();
                bool mIsHasTeam = false;
                Dictionary<long, Player> mDic = null;
                if (mCurrnetPKModel == E_PKModel.Friend)
                {
                    var mTeamComponent = (b_Attacker as GamePlayer).Player.GetCustomComponent<TeamComponent>();
                    if (mTeamComponent != null)
                    {
                        TeamManageComponent mTeamManageComponent = Root.MainFactory.GetCustomComponent<TeamManageComponent>();
                        mDic = mTeamManageComponent.GetAllByTeamID(mTeamComponent.TeamID);
                        mIsHasTeam = mDic != null && mDic.ContainsKey(b_Attacker.InstanceId);
                    }
                }
                var mAttackerFanJiIdlist = b_Attacker.GetFanJiIdlist();

                var mMapFieldlist = mMapFieldDic.Values.ToArray();
                for (int i = 0, len = mMapFieldlist.Length; i < len; i++)
                {
                    var mMapField = mMapFieldlist[i];

                    if (mMapField.FieldEnemyDic.Count > 0)
                    {
                        var mFieldEnemylist = mMapField.FieldEnemyDic.Values.ToArray();
                        for (int j = 0, jlen = mFieldEnemylist.Length; j < jlen; j++)
                        {
                            var mCurrentTemp = mFieldEnemylist[j];
                            if (mCurrentTemp.IsDeath || mCurrentTemp.IsDisposeable) continue;

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

                    if (mMapField.FieldPlayerDic.Count > 0)
                    {
                        var mFieldPlayerlist = mMapField.FieldPlayerDic.Values.ToArray();
                        for (int j = 0, jlen = mFieldPlayerlist.Length; j < jlen; j++)
                        {
                            var mCurrentTemp = mFieldPlayerlist[j];

                            if (mCurrentTemp.IsDeath || mCurrentTemp.IsDisposeable) continue;
                            if (mCurrentTemp.InstanceId == b_Attacker.InstanceId) continue;

                            var mTryUseResult = b_Attacker.TryUseByPlayerKilling(mCurrentTemp, mAttackerFanJiIdlist, mCurrnetPKModel, mIsHasTeam, mDic);
                            if (mTryUseResult == false) continue;

                            //if (Vector2.Distance(mSelfPos, new Vector2(mCurrentTemp.UnitData.X, mCurrentTemp.UnitData.Y)) < mRadius)
                            {
                                // 是否命中
                                bool IsHit = b_Attacker.IsHitPvP(mCurrentTemp, b_BattleComponent, true);
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
                    if (false && mMapField.FieldPetsDic.Count > 0)
                    {

                        var mFieldPetslist = mMapField.FieldPetsDic.Values.ToArray();
                        for (int j = 0, jlen = mFieldPetslist.Length; j < jlen; j++)
                        {
                            var mCurrentTemp = mFieldPetslist[j];
                            if (mCurrentTemp.IsDeath || mCurrentTemp.IsDisposeable || mCurrentTemp.GamePlayer.IsDisposeable) continue;
                            if (mCurrentTemp.GamePlayer.InstanceId == b_Attacker.InstanceId) continue;

                            var mTryUseResult = b_Attacker.TryUseByPlayerKilling(mCurrentTemp, mAttackerFanJiIdlist, mCurrnetPKModel, mIsHasTeam, mDic);
                            if (mTryUseResult == false) continue;

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
                    if (mMapField.FieldSummonedDic.Count > 0)
                    {
                        var mFieldSummonedlist = mMapField.FieldSummonedDic.Values.ToArray();
                        for (int j = 0, jlen = mFieldSummonedlist.Length; j < jlen; j++)
                        {
                            var mCurrentTemp = mFieldSummonedlist[j];
                            if (mCurrentTemp.IsDeath || mCurrentTemp.IsDisposeable || mCurrentTemp.GamePlayer.IsDisposeable) continue;
                            if (mCurrentTemp.GamePlayer.InstanceId == b_Attacker.InstanceId) continue;

                            var mTryUseResult = b_Attacker.TryUseByPlayerKilling(mCurrentTemp, mAttackerFanJiIdlist, mCurrnetPKModel, mIsHasTeam, mDic);
                            if (mTryUseResult == false) continue;

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

                             //b_Attacker.CombatRoundId = b_BattleComponent.WaitSync(mAttackTime, b_Attacker.InstanceId, mBeAttacker.InstanceId, (b_CombatRoundId, b_AttackerId, b_BeAttackerId) => { if (b_Attacker.CombatRoundId == b_CombatRoundId) b_Attacker.IsAttacking = false; }, () => { b_Attacker.CombatRoundId = 0; });
            };
            b_Attacker.CombatRoundId = 0;
            b_BattleComponent.WaitSync(mDamageWait, b_Attacker.InstanceId, mBeAttacker.InstanceId, mSyncAction);
            b_Attacker.CombatRoundId = b_BattleComponent.WaitSync(mAttackTime, b_Attacker.InstanceId, mBeAttacker.InstanceId, (b_CombatRoundId, b_AttackerId, b_BeAttackerId) => { if (b_Attacker.CombatRoundId == b_CombatRoundId) b_Attacker.IsAttacking = false; }, () => { b_Attacker.CombatRoundId = 0; });
            return true;
        }

    }
}
