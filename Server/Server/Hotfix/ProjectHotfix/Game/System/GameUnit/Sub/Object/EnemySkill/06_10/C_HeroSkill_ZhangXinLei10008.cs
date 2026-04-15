
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
    /// 掌心雷
    /// </summary>
    [HeroSkillAttribute(BindId = (int)E_HeroSkillId.ZhangXinLei10008)]
    public partial class C_HeroSkill_ZhangXinLei10008 : C_HeroSkillSource
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
    /// 掌心雷
    /// </summary>
    public partial class C_HeroSkill_ZhangXinLei10008
    {
        public bool TryUseByUseStandard(CombatSource b_Attacker, IActorResponse b_Response)
        {
            if (b_Attacker.Identity == E_Identity.Hero)
            {
                return false;
            }
            return true;
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

            if (Vector2.Distance(b_BattleComponent.Parent.GetFindTheWay2D(b_Attacker).Vector2Pos, b_Cell.Vector2Pos) > mConfig.Distance)
            {
                return false;
            }

            if (b_Attacker.Identity != E_Identity.Enemy)
            {
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
            b_BattleComponent.Parent.SendNotice(mBeAttacker, mAttackStartNotice);

            b_Attacker.IsAttacking = true;
            b_Attacker.AttackTime = b_BattleComponent.CurrentTimeTick + mAttackTime + mAttackTime;

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
                Vector2 mSelfPos = b_BattleComponent.Parent.GetFindTheWay2D(b_Attacker).Vector2Pos;
                // 距离变化了 超过技能范围则取消技能效果
                if (Vector2.Distance(mSelfPos, b_Cell.Vector2Pos) > mConfig.Distance)
                {
                    G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                    mAttackResultNotice.HurtValueType = 6;
                    mAttackResultNotice.AttackTarget = mBeAttacker.InstanceId;
                    b_BattleComponent.Parent.SendNotice(mBeAttacker, mAttackResultNotice);

                    return;
                }

                // 是否命中
                bool IsHit = false;
                IsHit = b_Attacker.IsHitPvE(mBeAttacker, b_BattleComponent, true);

                if (IsHit == false)
                {
                    G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                    mAttackResultNotice.HurtValueType = 1;
                    mAttackResultNotice.AttackTarget = mBeAttacker.InstanceId;
                    b_BattleComponent.Parent.SendNotice(mBeAttacker, mAttackResultNotice);

                    return;
                }

                // 随机魔法伤害
                int mVirtualHurtValue;
                var mSpecialAttack = b_Attacker.AttackSpecial();
                if (mSpecialAttack == E_GameProperty.LucklyAttackRate)
                {
                    mVirtualHurtValue = mConfig.OtherDataDic[1] + b_Attacker.GetNumerialFunc(E_GameProperty.MaxAtteck) + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedLuckyStrokeAttack) + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedSkillAttack);
                }
                else
                {
                    int mMinMagicAtteck = b_Attacker.GetNumerialFunc(E_GameProperty.MinAtteck);
                    int mMaxMagicAtteck = b_Attacker.GetNumerialFunc(E_GameProperty.MaxAtteck);
                    mVirtualHurtValue = mConfig.OtherDataDic[1] + Help_RandomHelper.Range(mMinMagicAtteck, mMaxMagicAtteck) + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedSkillAttack);
                    if (mSpecialAttack == E_GameProperty.ExcellentAttackRate)
                        mVirtualHurtValue += b_Attacker.GetNumerialFunc(E_GameProperty.EmbedGreatShotAttack);
                }

                // 总伤害 = 魔法伤害 + 魔力伤害 + 魔杖魔力提升百分比
                int mHurtValue = mVirtualHurtValue;
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

                if (mBeAttacker.IsDeath == false && mBeAttacker.IsDisposeable == false)
                {
                    void JiTui()
                    {
                        // 击退长度
                        if (mConfig.OtherDataDic.TryGetValue(4, out int mRepelValue) && mRepelValue > 0)
                        {
                            C_FindTheWay2D mRandomResult;

                            bool xAxial = b_Cell.X == (int)mSelfPos.x;
                            bool yAxial = b_Cell.Y == (int)mSelfPos.y;
                            if (xAxial && yAxial)
                            {
                                return;
                            }
                            else
                            {
                                int mRepelValuex = xAxial ? 0 : (b_Cell.X > mSelfPos.x ? mRepelValue : mRepelValue * -1);
                                int mRepelValuey = yAxial ? 0 : (b_Cell.Y > mSelfPos.y ? mRepelValue : mRepelValue * -1);

                                mRandomResult = b_BattleComponent.Parent.GetFindTheWay2D(b_Cell.X + mRepelValuex, b_Cell.Y + mRepelValuey);
                            }
                            if (mRandomResult != null && mRandomResult.IsSafeArea == false && mRandomResult.IsStaticObstacle == false)
                            {

                            }
                            else
                            {
                                return;
                            }
                            switch (mBeAttacker.Identity)
                            {
                                case E_Identity.Hero:
                                    {
                                        var mBeAttackerGamePlayer = mBeAttacker as GamePlayer;
                                        // 公告移动信息
                                        b_BattleComponent.Parent.MoveSendNotice(b_Cell, mRandomResult, mBeAttackerGamePlayer, false);

                                        DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                                        var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)mBeAttackerGamePlayer.Player.GameAreaId);
                                        var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)mBeAttackerGamePlayer.Player.GameAreaId);
                                        mWriteDataComponent.Save(mBeAttacker.UnitData, dBProxy2).Coroutine();
                                    }
                                    break;
                                case E_Identity.Enemy:
                                    {
                                        // 公告移动信息
                                        b_BattleComponent.Parent.MoveSendNotice(b_Cell, mRandomResult, mBeAttacker as Enemy, false);
                                    }
                                    break;
                                case E_Identity.Npc:
                                    break;
                                case E_Identity.Pet:
                                    {
                                        // 公告移动信息
                                        b_BattleComponent.Parent.MoveSendNotice(b_Cell, mRandomResult, mBeAttacker as Pets, false);
                                    }
                                    break;
                                case E_Identity.Summoned:
                                    {
                                        // 公告移动信息
                                        b_BattleComponent.Parent.MoveSendNotice(b_Cell, mRandomResult, mBeAttacker as Summoned, false);
                                    }
                                    break;
                                default:
                                    break;
                            }
                            if (mBeAttacker.Pathlist != null) mBeAttacker.Pathlist = null;
                        }
                    }
                    // 雷抗击退
                    var mThunderResistance = mBeAttacker.GetNumerialFunc(E_GameProperty.ThunderResistance);
                    if (mThunderResistance == 0)
                    {
                        JiTui();
                    }
                    else
                    {
                        int mRandomValue = Help_RandomHelper.Range(0, 256);
                        if (mRandomValue % (mThunderResistance + 1) == 0)
                        {
                            JiTui();
                        }
                    }
                }
            };
            b_Attacker.CombatRoundId = 0;
            b_BattleComponent.WaitSync(mConfig.DamageWait, b_Attacker.InstanceId, mBeAttacker.InstanceId, mSyncAction);
            b_Attacker.CombatRoundId = b_BattleComponent.WaitSync(mAttackTime, b_Attacker.InstanceId, mBeAttacker.InstanceId, (b_CombatRoundId, b_AttackerId, b_BeAttackerId) => { if (b_Attacker.CombatRoundId == b_CombatRoundId) b_Attacker.IsAttacking = false; }, () => { b_Attacker.CombatRoundId = 0; });
            return true;
        }
    }
}
