
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using CustomFrameWork;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 龙卷风
    /// </summary>
    [HeroSkillAttribute(BindId = (int)E_HeroSkillId.LongJuanFeng)]
    public partial class C_HeroSkill_LongJuanFeng10 : C_HeroSkillSource
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

            if (!(Config is Skill_SpellConfig mConfig))
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
    /// 龙卷风
    /// </summary>
    public partial class C_HeroSkill_LongJuanFeng10
    {
        public bool TryUseByUseStandard(CombatSource b_Attacker, IActorResponse b_Response)
        {
            if (!(Config is Skill_SpellConfig mConfig))
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
            if (!(Config is Skill_SpellConfig mConfig))
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
            if (!(Config is Skill_SpellConfig mConfig))
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
                // 对敌方目标及其直线的矩形范围内所有敌人造成伤害
                // 距离变化了 超过技能范围则取消技能效果
                Vector2 mSelfPos = b_BattleComponent.Parent.GetFindTheWay2D(b_Attacker).Vector2Pos;
                Vector2 mTargetPos = b_Cell.Vector2Pos;
                if (Vector2.Distance(mSelfPos, mTargetPos) > mConfig.Distance)
                {
                    G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                    mAttackResultNotice.HurtValueType = 6;
                    mAttackResultNotice.AttackTarget = mBeAttacker.InstanceId;
                    b_BattleComponent.Parent.SendNotice(mBeAttacker, mAttackResultNotice);

                    return;
                }

                // 随机魔法伤害
                int mMagicHurtValue;
                var mSpecialAttack = b_Attacker.AttackSpecial();
                if (mSpecialAttack == E_GameProperty.LucklyAttackRate)
                {
                    mMagicHurtValue = (int)(mConfig.OtherDataDic[1] * 1.5f) + b_Attacker.GetNumerialFunc(E_GameProperty.MaxMagicAtteck);

                    // 魔杖魔力提升百分比
                    int mMaxMagicAtteckIncrease = (int)(mMagicHurtValue * b_Attacker.GetNumerialFunc(E_GameProperty.MagicRate_Increase) * 0.01f);
                    mMagicHurtValue += mMaxMagicAtteckIncrease + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedLuckyStrokeAttack) + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedSkillAttack);
                }
                else
                {
                    // 随机魔力伤害
                    int mMinMagicAtteck = mConfig.OtherDataDic[1] + b_Attacker.GetNumerialFunc(E_GameProperty.MinMagicAtteck);
                    int mMaxMagicAtteck = (int)(mConfig.OtherDataDic[1] * 1.5f) + b_Attacker.GetNumerialFunc(E_GameProperty.MaxMagicAtteck);
                    mMagicHurtValue = Help_RandomHelper.Range(mMinMagicAtteck, mMaxMagicAtteck);

                    // 魔杖魔力提升百分比
                    int mMaxMagicAtteckIncrease = (int)(mMaxMagicAtteck * b_Attacker.GetNumerialFunc(E_GameProperty.MagicRate_Increase) * 0.01f);
                    mMagicHurtValue += mMaxMagicAtteckIncrease + b_Attacker.GetNumerialFunc(E_GameProperty.EmbedSkillAttack);
                    if (mSpecialAttack == E_GameProperty.ExcellentAttackRate)
                        mMagicHurtValue += b_Attacker.GetNumerialFunc(E_GameProperty.EmbedGreatShotAttack);
                }
                var mWillpower = b_Attacker.GetNumerialFunc(E_GameProperty.Property_Willpower) / mConfig.OtherDataDic[10];
                // 总伤害 = 魔法伤害 + 魔力伤害 + 魔杖魔力提升百分比
                int mHurtValue = mMagicHurtValue + mWillpower;
                var mAddScale = mConfig.OtherDataDic[11];
                if (mAddScale != 100)
                    mHurtValue = (int)(mHurtValue * mAddScale * 0.01f);

                float mAreaWidth = 4;

                Dictionary<long, C_FindTheWay2D> mMapFieldDic = new Dictionary<long, C_FindTheWay2D>();

                if (b_Attacker.UnitData.X == b_Cell.X || b_Attacker.UnitData.Y == b_Cell.Y)
                {
                    (int X, int Y) mSelfPosInt = (b_Attacker.UnitData.X, b_Attacker.UnitData.Y);
                    (int X, int Y) mTargetPosInt = (b_Cell.X, b_Cell.Y);

                    bool mDirection = mSelfPosInt.X == mTargetPosInt.X;

                    // 起始点x轴偏移量  起始点y轴偏移量
                    int mStartPointOffsetX = mDirection ? (int)(mAreaWidth * 0.5f) : 0;
                    int mStartPointOffsetY = mDirection ? 0 : (int)(mAreaWidth * 0.5f);

                    int mIntervalX = 0, mIntervalY = 0;
                    if (mDirection)
                    {
                        mIntervalY = mConfig.Distance;
                        if (mSelfPosInt.Y > mTargetPosInt.Y) mIntervalY *= -1;
                    }
                    else
                    {
                        mIntervalX = mConfig.Distance;
                        if (mSelfPosInt.X > mTargetPosInt.X) mIntervalX *= -1;
                    }

                    (int X, int Y) mTargetCenter = (mSelfPosInt.X + mIntervalX, mSelfPosInt.Y + mIntervalY);

                    (int X, int Y) mStartUp = (mSelfPosInt.X + mStartPointOffsetX, mSelfPosInt.Y + mStartPointOffsetY);
                    (int X, int Y) mStartDown = (mSelfPosInt.X - mStartPointOffsetX, mSelfPosInt.Y - mStartPointOffsetY);

                    (int X, int Y) mTargetUp = (mTargetCenter.X + mStartPointOffsetX, mTargetCenter.Y + mStartPointOffsetY);
                    (int X, int Y) mTargetDown = (mTargetCenter.X - mStartPointOffsetX, mTargetCenter.Y - mStartPointOffsetY);

                    (int X, int Y) mCenter = ((mSelfPosInt.X + mTargetCenter.X) / 2, (mSelfPosInt.Y + mTargetCenter.Y) / 2);

                    int mRadius = (int)Math.Ceiling(mConfig.Distance * 0.5f);
                    (int X, int Y) mStartPos = (mCenter.X - mRadius, mCenter.Y - mRadius);

                    int mDiameter = mConfig.Distance * 2;
                    for (int i = 0; i < mDiameter; i++)
                    {
                        for (int j = 0; j < mDiameter; j++)
                        {
                            (int X, int Y) mCurrentTempPos = (mStartPos.X + i, mStartPos.Y + j);

                            if (mCurrentTempPos.X < MathF.Min(mStartDown.X, mTargetDown.X)) continue;
                            if (mCurrentTempPos.X > MathF.Max(mStartUp.X, mTargetUp.X)) continue;

                            if (mCurrentTempPos.Y < MathF.Min(mStartDown.Y, mTargetDown.Y)) continue;
                            if (mCurrentTempPos.Y > MathF.Max(mStartUp.Y, mTargetUp.Y)) continue;

                            var mCurrentTemp = b_BattleComponent.Parent.GetFindTheWay2D(mCurrentTempPos.X, mCurrentTempPos.Y);
                            if (mCurrentTemp == null) continue;
                            if (mCurrentTemp.HasUnit() == false) continue;

                            if (mMapFieldDic.ContainsKey(mCurrentTemp.Id) == false)
                            {
                                mMapFieldDic[mCurrentTemp.Id] = mCurrentTemp;
                            }
                        }
                    }
                }
                else
                {
                    //float hight = MathF.Abs(mTargetPos.y - mSelfPos.y);
                    //float width = MathF.Abs(mTargetPos.x - mSelfPos.x);
                    //float c = Mathf.Sqrt(hight * hight + width * width);
                    //float sin = hight / c;
                    //float cos = width / c;
                    // 方向向量
                    //float newWidth = (width + mConfig.Distance);
                    //float newHight = newWidth * tan;
                    Vector2 mTargetCenterNormalize = Vector2.Normalize(mTargetPos - mSelfPos) * (mConfig.Distance + 1);
                    Vector2 mTargetCenter = mSelfPos + mTargetCenterNormalize;

                    //CustomFrameWork.Component.LogToolComponent.SimpleError($"长度为:{mTargetCenterNormalize.magnitude}", false);
                    //CustomFrameWork.Component.LogToolComponent.SimpleError($"mTargetCenter:{mTargetCenter.ToString()}", false);
                    // 斜率k = (y1 - y2) / (x1 - x2)
                    float k = (mSelfPos.y - mTargetPos.y) / (mSelfPos.x - mTargetPos.x);
                    // 垂线的斜率
                    float k2 = -1 / k;

                    //var  b2 = (x1 + x2) / (2 * k1) + (y1 + y2) / 2; //垂直平分线的纵截距b值
                    //double cenX = (mSelfPos.x + mTargetPos.x) / 2;//交点x
                    //double cenY = (mSelfPos.y + mTargetPos.y) / 2;//交点y

                    var x1 = mSelfPos.x;
                    var x2 = mTargetPos.x;
                    var y1 = mSelfPos.y;
                    var y2 = mTargetPos.y;
                    double dist = Math.Sqrt(Math.Abs(x1 - x2) * Math.Abs(x1 - x2) + Math.Abs(y1 - y2) * Math.Abs(y1 - y2)) / 2 * mAreaWidth;//每个点距离交点(中心点)距离相同
                    //double dist = Vector2.Normalize(mTargetPos - mSelfPos).magnitude * mAreaWidth;
                    double douAngle = Math.Atan(k2);//垂直平分线和X轴的夹角

                    //计算起始点和终止点
                    x1 = (float)(dist * Math.Cos(douAngle));
                    y1 = (float)(dist * Math.Sin(douAngle));

                    x2 = (float)(dist * Math.Cos(douAngle));
                    y2 = (float)(dist * Math.Sin(douAngle));

                    Vector2 mStartUp = new Vector2(mSelfPos.x + x1, mSelfPos.y + y1);
                    Vector2 mStartDown = new Vector2(mSelfPos.x - x2, mSelfPos.y - y2);

                    Vector2 mTargetUp = new Vector2(mTargetCenter.x + x1, mTargetCenter.y + y1);
                    Vector2 mTargetDown = new Vector2(mTargetCenter.x - x2, mTargetCenter.y - y2);

                    //添加构建多边形的点
                    List<PointF> mPointlist = new List<PointF>();
                    mPointlist.Add(new PointF(mStartUp.x, mStartUp.y));
                    mPointlist.Add(new PointF(mStartDown.x, mStartDown.y));
                    mPointlist.Add(new PointF(mTargetUp.x, mTargetUp.y));
                    mPointlist.Add(new PointF(mTargetDown.x, mTargetDown.y));

                    System.Drawing.Drawing2D.GraphicsPath myGraphicsPath = new System.Drawing.Drawing2D.GraphicsPath();
                    myGraphicsPath.Reset();
                    myGraphicsPath.AddPolygon(mPointlist.ToArray());

                    Region myRegion = new Region();
                    myRegion.MakeEmpty();
                    myRegion.Union(myGraphicsPath);

                    (int X, int Y) mCenter = ((int)((b_Attacker.UnitData.X + mTargetCenter.x) / 2), (int)((b_Attacker.UnitData.Y + mTargetCenter.y) / 2));

                    int mRadius = (int)Math.Ceiling(mConfig.Distance * 0.5f);
                    (int X, int Y) mStartPos = (mCenter.X - mRadius, mCenter.Y - mRadius);

                    int mDiameter = mConfig.Distance * 2;
                    for (int i = 0; i < mDiameter; i++)
                    {
                        for (int j = 0; j < mDiameter; j++)
                        {
                            var mCurrentTemp = b_BattleComponent.Parent.GetFindTheWay2D(mStartPos.X + i, mStartPos.Y + j);
                            if (mCurrentTemp == null) continue;
                            if (mCurrentTemp.HasUnit() == false) continue;

                            //返回判断点是否在多边形里
                            bool myPoint = myRegion.IsVisible(new Point(mCurrentTemp.X, mCurrentTemp.Y));
                            if (myPoint)
                            {
                                if (mMapFieldDic.ContainsKey(mCurrentTemp.Id) == false)
                                {
                                    mMapFieldDic[mCurrentTemp.Id] = mCurrentTemp;
                                }
                            }
                            else
                            {
                                //CustomFrameWork.Component.LogToolComponent.Log($"{mCurrentTemp.X} {mCurrentTemp.Y} 不再矩形范围内", false);
                            }
                        }
                    }
                    myGraphicsPath.Dispose();
                    myRegion.Dispose();
                }

                var mSelfFindTheWay = b_BattleComponent.Parent.GetFindTheWay2D(b_Attacker);
                if (mMapFieldDic.ContainsKey(mSelfFindTheWay.Id) == false)
                {
                    mMapFieldDic[mSelfFindTheWay.Id] = mSelfFindTheWay;
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

                            //if (Vector2.Distance(mSelfPos, new Vector2(mCurrentTemp.UnitData.X, mCurrentTemp.UnitData.Y)) < mConfig.Distance)
                            {
                                // 是否命中
                                bool IsHit = b_Attacker.IsHitPvE(mCurrentTemp, b_BattleComponent, true);
                                if (IsHit)
                                {
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

                            //if (Vector2.Distance(mSelfPos, new Vector2(mCurrentTemp.UnitData.X, mCurrentTemp.UnitData.Y)) < mConfig.Distance)
                            {
                                // 是否命中
                                bool IsHit = b_Attacker.IsHitPvP(mCurrentTemp, b_BattleComponent, true);
                                if (IsHit)
                                {
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

                            //if (Vector2.Distance(mSelfPos, new Vector2(mCurrentTemp.UnitData.X, mCurrentTemp.UnitData.Y)) < mConfig.Distance)
                            {
                                // 是否命中
                                bool IsHit = b_Attacker.IsHitPvE(mCurrentTemp, b_BattleComponent, true);
                                if (IsHit)
                                {
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

                            //if (Vector2.Distance(mSelfPos, new Vector2(mCurrentTemp.UnitData.X, mCurrentTemp.UnitData.Y)) < mConfig.Distance)
                            {
                                // 是否命中
                                bool IsHit = b_Attacker.IsHitPvE(mCurrentTemp, b_BattleComponent, true);
                                if (IsHit)
                                {
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
