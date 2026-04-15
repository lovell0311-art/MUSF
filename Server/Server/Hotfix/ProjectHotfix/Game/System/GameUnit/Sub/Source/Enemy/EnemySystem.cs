using System;
using System.Threading.Tasks;
using ETModel;
using CustomFrameWork.Component;
using CustomFrameWork;
using System.Linq;
using TencentCloud.Bri.V20190328.Models;
using System.Collections.Generic;
using UnityEngine;
using TencentCloud.Dc.V20180410.Models;
using System.Drawing;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ETHotfix
{



    public static partial class EnemySystem
    {
        public static void Attack(this Enemy b_Attacker, CombatSource b_BeAttacker, BattleComponent b_BattleComponent, bool b_CanBackInjure = true)
        {
            if (b_Attacker.IsAttacking) return;
            b_Attacker.IsAttacking = true;

            b_Attacker.RunAction(b_Attacker.ActionAttack, b_Attacker, b_BeAttacker, b_BattleComponent);

            var mMapComponent = b_BattleComponent.Parent;

            G2C_AttackStart_notice mAttackStartNotice = new G2C_AttackStart_notice();
            mAttackStartNotice.AttackSource = b_Attacker.InstanceId;
            mAttackStartNotice.AttackTarget = b_BeAttacker.InstanceId;
            mAttackStartNotice.AttackType = 0;
            mMapComponent.SendNotice(b_BeAttacker, mAttackStartNotice);

            int mAttackTime = (int)(b_Attacker.Config.AtSpeed * 0.5f);

            Action<long, long, long> mSyncAction = (b_CombatRoundId, b_AttackerId, b_BeAttackerId) =>
            {
                //if (b_Attacker.CombatRoundId != b_CombatRoundId) return;

                if (b_Attacker.InstanceId != b_AttackerId || b_Attacker.IsDeath || b_Attacker.IsDisposeable || b_Attacker.UnitData.Index != b_BattleComponent.Parent.MapId)
                {
                    b_Attacker.IsAttacking = false;
                    return;
                }
                if (b_BeAttacker.InstanceId != b_BeAttackerId || b_BeAttacker.IsDeath || b_BeAttacker.IsDisposeable || b_BeAttacker.UnitData.Index != b_BattleComponent.Parent.MapId)
                {
                    b_Attacker.IsAttacking = false;
                    return;
                }

                // 是否命中
                bool IsHit = false;
                if (b_BeAttacker.Identity == E_Identity.Hero)
                {
                    IsHit = b_Attacker.IsHitPvE(b_BeAttacker, b_BattleComponent, true);
                }
                else
                {
                    IsHit = b_Attacker.IsHitPvE(b_BeAttacker, b_BattleComponent, true);
                }

                if (IsHit == false)
                {
                    G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                    mAttackResultNotice.HurtValueType = 1;
                    mAttackResultNotice.AttackTarget = b_BeAttacker.InstanceId;
                    mMapComponent.SendNotice(b_BeAttacker, mAttackResultNotice);
                }
                else
                {
                    E_BattleHurtAttackType mAttackType = E_BattleHurtAttackType.BASIC;
                    E_BattleHurtType mBattleHurtType = E_BattleHurtType.PHYSIC;

                    int mVirtualInjureValue;
                    var mSpecialAttack = b_Attacker.AttackSpecial();
                    if (mSpecialAttack == E_GameProperty.LucklyAttackRate)
                    {
                        mVirtualInjureValue = b_Attacker.GetNumerialFunc(E_GameProperty.MaxAtteck);
                        mVirtualInjureValue += b_Attacker.GetNumerialFunc(E_GameProperty.EmbedLuckyStrokeAttack);
                    }
                    else
                    {
                        mVirtualInjureValue = Help_RandomHelper.Range(b_Attacker.GetNumerialFunc(E_GameProperty.MinAtteck), b_Attacker.GetNumerialFunc(E_GameProperty.MaxAtteck));
                        if (mSpecialAttack == E_GameProperty.ExcellentAttackRate)
                            mVirtualInjureValue += b_Attacker.GetNumerialFunc(E_GameProperty.EmbedGreatShotAttack);
                    }

                    switch (b_BeAttacker.Identity)
                    {
                        case E_Identity.Enemy:
                            (b_BeAttacker as Enemy).Injure(b_Attacker, mAttackType, mBattleHurtType, mSpecialAttack, 0, mVirtualInjureValue, b_BattleComponent, true);
                            break;
                        case E_Identity.Summoned:
                            (b_BeAttacker as Summoned).Injure(b_Attacker, mAttackType, mBattleHurtType, mSpecialAttack, 0, mVirtualInjureValue, b_BattleComponent, true);
                            break;
                        case E_Identity.Pet:
                            (b_BeAttacker as Pets).Injure(b_Attacker, mAttackType, mBattleHurtType, mSpecialAttack, 0, mVirtualInjureValue, b_BattleComponent, true);
                            break;
                        case E_Identity.Hero:
                            (b_BeAttacker as GamePlayer).Injure(b_Attacker, mAttackType, mBattleHurtType, mSpecialAttack, 0, mVirtualInjureValue, b_BattleComponent, true);
                            break;
                        default:
                            break;
                    }
                }

                b_Attacker.CombatRoundId = b_BattleComponent.WaitSync(mAttackTime, b_AttackerId, b_BeAttackerId, (b_CombatRoundId, b_AttackerIdTemp, b_BeAttackerIdTemp) => { if (b_Attacker.CombatRoundId == b_CombatRoundId) b_Attacker.IsAttacking = false; }, () => { b_Attacker.CombatRoundId = 0; });
            };
            ///攻击间隔毫秒 = 60000 / (50 + (240 - 50) * [攻击速度] / 280)
            b_Attacker.CombatRoundId = 0;
            b_Attacker.CombatRoundId = b_BattleComponent.WaitSync(mAttackTime, b_Attacker.InstanceId, b_BeAttacker.InstanceId, mSyncAction);
        }

        public static int Injure(this Enemy b_BeAttacker, CombatSource b_Attacker,
            E_BattleHurtAttackType b_AttackType,
            E_BattleHurtType b_BattleHurtType,
            E_GameProperty b_SpecialAttack,
            int b_HurtTypeId,
            int b_InjureValue,
            BattleComponent b_BattleComponent,
            bool b_CanDefense = true,
            bool b_CanBackInjure = true)
        {
            if (b_Attacker.Identity == E_Identity.Hero)
            {
                // 检查玩家状态是否为 Online
                if ((b_Attacker as GamePlayer).Player.OnlineStatus != EOnlineStatus.Online)
                {
                    // 玩家正在登录 或 正在下线
                    return 0;
                }
            }
            if (b_BeAttacker.CreatePlayerId != 0 && b_BeAttacker.CreatePlayerId != b_Attacker.InstanceId) return 0;
            if(b_BeAttacker.IsDeath) return 0;
            //if (b_BeAttacker.Config.Monster_Type == 6)
            //{
            //    switch (b_Attacker.Identity)
            //    {
            //        case E_Identity.Hero:
            //            if (!(b_Attacker as GamePlayer).Data.SpecialTitle) return 0;
            //            break;
            //        case E_Identity.Summoned:
            //            if (!(b_Attacker as Summoned).GamePlayer.Data.SpecialTitle) return 0;
            //            break;
            //    }

            //}
            (int, int) mRealInjure;
            bool mIgnoreDefense = false;
            if (b_CanDefense || b_BattleHurtType != E_BattleHurtType.REAL)
            {
                if (b_Attacker.Identity == E_Identity.Hero)
                {
                    int mAttackIgnoreDefenseRate = b_Attacker.GetNumerialFunc(E_GameProperty.AttackIgnoreDefenseRate);
                    if (mAttackIgnoreDefenseRate > 0)
                    {
                        //mAttackIgnoreDefenseRate *= 100;
                        if (mAttackIgnoreDefenseRate > 8000) mAttackIgnoreDefenseRate = 8000;

                        int mRandomResult = CustomFrameWork.Help_RandomHelper.Range(0, 10000);
                        if (mRandomResult <= mAttackIgnoreDefenseRate)
                        {
                            mIgnoreDefense = true;
                        }
                    }
                }

                // 防御后的数值 被抗性衰减 减伤后的数值 真实掉血量
                mRealInjure = b_BeAttacker.Defense(b_Attacker, b_AttackType, b_BattleHurtType, mIgnoreDefense, b_HurtTypeId, b_SpecialAttack, b_InjureValue, b_BattleComponent, b_CanDefense);
            }
            else
            {
                mRealInjure = (b_InjureValue, 0);
            }
           
            //活动boos
            if (b_BeAttacker.Config.Monster_Type == 6)
            {
                //if ((b_Attacker as GamePlayer).Data.SpecialTitle)
                {
                    b_BeAttacker.DamageRanking ??= new Dictionary<long, (string, int)>();
                    if (b_BeAttacker.DamageRanking.TryGetValue(b_Attacker.InstanceId, out var Info))
                    {
                        (string, int) NewInfo = ((b_Attacker as GamePlayer).Data.NickName, Info.Item2 + mRealInjure.Item1);
                        b_BeAttacker.DamageRanking[b_Attacker.InstanceId] = NewInfo;
                    }
                    else
                    {
                        (string, int) NewInfo = ((b_Attacker as GamePlayer).Data.NickName, mRealInjure.Item1);
                        b_BeAttacker.DamageRanking.Add(b_Attacker.InstanceId, NewInfo);
                    }
                    async Task SendRanKing()
                    {
                        List<(string, int)> values = b_BeAttacker.DamageRanking.Values.ToList();
                        G2C_SendDamageRanking g2C_SendDamageRanking = new G2C_SendDamageRanking();
                        foreach (var NC in values)
                        {
                            g2C_SendDamageRanking.InfoList.Add(new RanKingInfo
                            {
                                Name = NC.Item1,
                                Cnt = NC.Item2
                            });
                        }
                        var mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().GetAllPlayers();

                        foreach (var Player in b_BeAttacker.DamageRanking)
                        {

                            if (mPlayer[1].TryGetValue(Player.Key, out var Info))
                            {
                                Info.Send(g2C_SendDamageRanking);
                            }
                        }
                    }
                    SendRanKing().Coroutine();
                }
            }
            var mRealInjureValue = mRealInjure.Item1;
            var mMapComponent = b_BattleComponent.Parent;
            bool AreYouOK = false;
            if (b_BeAttacker.IsCheckState(4))
            {
                {//副本计次
                    mRealInjureValue = 0;
                    int Value = b_BeAttacker.GetNumerialFunc(E_GameProperty.PROP_HP_MAX) - (b_BeAttacker.UnitData.Hp - 1);
                    if (CheckCopy(mMapComponent, b_Attacker, Value, out bool State))
                    {
                        mRealInjureValue = 1;//伤害计次怪
                        if (State) AreYouOK = true;
                    }
                }
                /*{正常被击次数怪
                    mRealInjureValue = 1;
                    int Value = b_BeAttacker.UnitData.Hp;
                    if (b_BeAttacker.GetNumerialFunc(E_GameProperty.PROP_HP_MAX) - Value >= b_BeAttacker.Config.BeHitCnt) AreYouOK = true;
                }*/
            }

            if (b_BeAttacker.UnitData.Hp <= mRealInjureValue || AreYouOK)
            {
                b_BeAttacker.UnitData.Hp = 0;
                b_BeAttacker.IsDeath = true;
                if (b_BeAttacker.CreatePlayerId != 0) b_BeAttacker.IsReallyDeath = true;
                b_BeAttacker.RemoveAllHealthState(b_BattleComponent);

                if (b_BeAttacker.Pathlist != null) b_BeAttacker.Pathlist = null;
                b_BeAttacker.Enemy = null;
                b_BeAttacker.TargetEnemy = null;

                b_BeAttacker.MoveStartTime = 0;
                b_BeAttacker.MoveNeedTime = 0;
                b_BeAttacker.MoveSleepTime = 0;
                b_BeAttacker.MoveRestTime = 0;

                if (b_BeAttacker.Config.Monster_Type != 5)
                {
                    b_BeAttacker.DeathSleepTime = b_BattleComponent.CurrentTimeTick + b_BeAttacker.Config.Regen;
                }
                else
                {
                    var mTick = DateTime.UtcNow.Date.AddHours(20 - 8).Ticks;
                    mTick = Help_TimeHelper.GetTargetTick(mTick);
                    if (mTick <= b_BattleComponent.CurrentTimeTick)
                    {
                        // 加一天
                        mTick += 1000 * 60 * 60 * 24;
                    }
                    b_BeAttacker.DeathSleepTime = mTick;
                }
                //清除血色城堡大门周围设置的静态障碍
                if (mMapComponent.MapId == 100 && b_BeAttacker.Config.Id == 197)
                {
                    List<int> Y = new List<int>() { 109, 110, 111, 112, 113 };
                    foreach (var y in Y)
                    {
                        mMapComponent.GetFindTheWay2D(86, y).IsStaticObstacle = false;
                    }
                }
                var mFindTheWay = mMapComponent.GetFindTheWay2D(b_BeAttacker);

                if (b_BeAttacker.Config.Monster_Type == 1)
                {
                    switch (b_Attacker.Identity)
                    {
                        case E_Identity.Hero:
                            {
                                G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                                mMoveMessageNotice.X = mFindTheWay.X;
                                mMoveMessageNotice.Y = mFindTheWay.Y;
                                mMoveMessageNotice.ViewId = 11;
                                mMoveMessageNotice.MapId = mFindTheWay.Map.MapId;
                                mMoveMessageNotice.ModelId = b_BeAttacker.Config.Id;
                                mMoveMessageNotice.NickName = (b_Attacker as GamePlayer).Data.NickName;

                                mMapComponent.SendNoticeByServer(mMoveMessageNotice).Coroutine();
                            }
                            break;
                        case E_Identity.Enemy:
                            break;
                        case E_Identity.Npc:
                            break;
                        case E_Identity.Pet:
                            {
                                G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                                mMoveMessageNotice.X = mFindTheWay.X;
                                mMoveMessageNotice.Y = mFindTheWay.Y;
                                mMoveMessageNotice.ViewId = 11;
                                mMoveMessageNotice.MapId = mFindTheWay.Map.MapId;
                                mMoveMessageNotice.ModelId = b_BeAttacker.Config.Id;
                                mMoveMessageNotice.NickName = (b_Attacker as Pets).GamePlayer.Data.NickName;

                                mMapComponent.SendNoticeByServer(mMoveMessageNotice).Coroutine();
                            }
                            break;
                        case E_Identity.Summoned:
                            {
                                G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                                mMoveMessageNotice.X = mFindTheWay.X;
                                mMoveMessageNotice.Y = mFindTheWay.Y;
                                mMoveMessageNotice.ViewId = 11;
                                mMoveMessageNotice.MapId = mFindTheWay.Map.MapId;
                                mMoveMessageNotice.ModelId = b_BeAttacker.Config.Id;
                                mMoveMessageNotice.NickName = (b_Attacker as Summoned).GamePlayer.Data.NickName;

                                mMapComponent.SendNoticeByServer(mMoveMessageNotice).Coroutine();
                            }
                            break;
                        default:
                            break;
                    }
                }
                
                 if (b_BeAttacker.Config.Monster_Type == 6)
                {
                    async Task SendRanKing()
                    {
                        int mAreaId = b_BattleComponent.Parent.Parent.Parent.GameAreaId;
                        if (b_BeAttacker.DamageRanking != null)
                        {
                            var Lsit = b_BeAttacker.DamageRanking.OrderByDescending(p => p.Value.Item2).ToDictionary(p => p.Key, o => o.Value.Item2);

                            int i = 0;
                            int Cnt = 1;
                            foreach (var Id in Lsit)
                            {
                                Cnt = 1;
                                switch (i)
                                {
                                    case 0: Cnt = 5; break;
                                    case 1: Cnt = 4; break;
                                    case 2: Cnt = 3; break;
                                }
                                MailInfo mailinfo = new MailInfo();
                                mailinfo.MailId = IdGeneraterNew.Instance.GenerateId();
                                mailinfo.MailName = "系统邮件";
                                mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                                mailinfo.MailContent = "活动BOSS排行奖励";
                                mailinfo.MailState = 0;
                                mailinfo.ReceiveOrNot = 0;
                                mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                                MailItem mailItem = new MailItem();
                                ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                                itemCreateAttr.Quantity = Cnt;
                                mailItem.ItemConfigID = 310155;
                                mailItem.ItemID = 0;
                                mailItem.CreateAttr = itemCreateAttr;
                                mailinfo.MailEnclosure.Add(mailItem);
                                MailSystem.SendMail(Id.Key, mailinfo, mAreaId).Coroutine();
                                if (i < 10)
                                    i++;
                                else
                                    return;
                            }
                        }
                    }

                    SendRanKing().Coroutine();
                   
                }
                // 发布 EnemyDeath 事件
                ETModel.EventType.EnemyDeath.Instance.enemy = b_BeAttacker;
                ETModel.EventType.EnemyDeath.Instance.attacker = b_Attacker;
                ETModel.EventType.EnemyDeath.Instance.map = mMapComponent;
                Root.EventSystem.OnRun("EnemyDeath", ETModel.EventType.EnemyDeath.Instance);

                void AddExprience(GamePlayer b_GamePlayer, int b_Exprience)
                {
                    if (b_GamePlayer != null)
                    {
                        b_GamePlayer.AddExprience(b_Exprience);
                    }

                    var mTianyingExprience = (int)(b_Exprience * 0.2f);

                    {   // 坐骑添加经验
                        EquipmentComponent equipCmpt = b_GamePlayer.Player.GetCustomComponent<EquipmentComponent>();
                        Item mounts = equipCmpt.GetEquipItemByPosition(EquipPosition.Mounts);
                        Item TianYings = equipCmpt.GetEquipItemByPosition(EquipPosition.TianYing);
                        if (mounts != null)
                        {
                            //黑王马经验分给天鹰一半
                            if (mounts.ConfigID == 260012 && TianYings != null)
                            {
                                mTianyingExprience = mTianyingExprience / 2;
                            }
                            if (mounts.AddMountsExp(mTianyingExprience, b_GamePlayer))
                            {
                                equipCmpt.ApplyEquipProp();
                            }
                        }
                        if (TianYings != null)
                        {
                            if (TianYings.AddMountsExp(mTianyingExprience, b_GamePlayer))
                            {
                                equipCmpt.ApplyEquipProp();
                            }
                        }
                    }
                }
                var mExperienceBonus = b_Attacker.GetNumerialFunc(E_GameProperty.ExperienceBonus);

                (int exp, int sourceExp) CalculationExperience(int death_level, int attack_level)
                {
                    //var death_level = b_BeAttacker.Config.Lvl;
                    //var attack_level = mAttacker.Config.Lvl;

                    int mLevel = (death_level + 25) * death_level / 3;
                    if ((death_level + 10) < attack_level)
                    {
                        mLevel = mLevel * (death_level + 10) / attack_level;
                    }
                    //控制升级速度，
                    //if (death_level >= 65)
                    //{
                    //    mLevel = mLevel + (death_level - 15) * ((int)(death_level * 0.25f));
                    //}
                    mLevel *= 3;//升级提升三倍近15天到400
                    int sourceExp = mLevel;
                    if (mExperienceBonus != 0)
                    {
                        mLevel = (int)(mLevel * mExperienceBonus / 100f);
                    }

                    return (mLevel, sourceExp);
                }
                void AddExprienceByTeam(GamePlayer b_GamePlayer, int b_Exprience, int b_AddExprienceDistance)
                {
                    if (b_GamePlayer.CurrentMap == null) return;

                    //b_Exprience = (int)(b_Exprience * (1 - 0.7f));//经验降低百分之70

                    var mTeamComponent = b_GamePlayer.Player.GetCustomComponent<TeamComponent>();
                    if (mTeamComponent != null)
                    {
                        TeamManageComponent mTeamManageComponent = Root.MainFactory.GetCustomComponent<TeamManageComponent>();
                        var mDic = mTeamManageComponent.GetAllByTeamID(mTeamComponent.TeamID);
                        if (mDic != null)
                        {
                            int sqrMagnitude = b_AddExprienceDistance * b_AddExprienceDistance;
                            int totalLevel = 0;
                            int totalExp = b_Exprience;
                            long mapId = b_GamePlayer.CurrentMap.Id;  // 地图唯一id

                            // TODO 找出队伍中符合条件的角色
                            using ListComponent<GamePlayer> teamGamePlayerList = ListComponent<GamePlayer>.Create();
                            foreach (Player player in mDic.Values)
                            {
                                GamePlayer gamePlayer = player.GetCustomComponent<GamePlayer>();
                                if (gamePlayer.CurrentMap == null) continue;
                                if (mapId != gamePlayer.CurrentMap.Id) continue;

                                if ((gamePlayer.Position - b_GamePlayer.Position).sqrMagnitude < sqrMagnitude)
                                {
                                    totalLevel += gamePlayer.Data.Level;
                                    teamGamePlayerList.Add(gamePlayer);
                                }
                            }
                            if (totalLevel == 0)
                            {
                                string str = "";
                                foreach (GamePlayer gamePlayer in teamGamePlayerList) str += $",{gamePlayer.InstanceId}";
                                Log.Error($"totalLevel == 0! ({str})");
                                return;
                            }
                            int teamCount = teamGamePlayerList.Count;
                            // 经验加成
                            switch (teamCount)
                            {
                                case 2: totalExp += (int)(totalExp * 0.40f); break;
                                case 3: totalExp += (int)(totalExp * 0.50f); break;
                                case 4: totalExp += (int)(totalExp * 0.60f); break;
                                case 5: totalExp += (int)(totalExp * 0.70f); break;
                            }
                            // 分享经验
                            int totalExpTemp = totalExp;
                            for (int i = 0; i < teamCount; ++i)
                            {
                                GamePlayer teamGamePlayer = teamGamePlayerList[i];
                                if (teamGamePlayer.Id == b_GamePlayer.Id) continue;
                                int exp = (int)(totalExp * ((float)teamGamePlayer.Data.Level / totalLevel));
                                totalExpTemp -= exp;
                                AddExprience(teamGamePlayer,exp);
                            }
                            // 把剩余的经验全给经验获得者
                            AddExprience(b_GamePlayer,totalExpTemp);

                            return;
                        }
                    }

                    AddExprience(b_GamePlayer, b_Exprience);
                }
                // 死亡TODO
                int mLevel = 0;
                int sourceExp = 0;  // 原经验，没有加成百分比
                // 经验获取
                switch (b_Attacker.Identity)
                {
                    case E_Identity.Enemy:
                        break;
                    case E_Identity.Summoned:
                        {
                            var mAttacker = b_Attacker as Summoned;
                            var death_level = b_BeAttacker.Config.Lvl;
                            var attack_level = mAttacker.GamePlayer.Data.Level;
                            (mLevel, sourceExp) = CalculationExperience(death_level, attack_level);
                            /*mLevel = (death_level + 25) * death_level / 3;
                            if ((death_level + 10) < attack_level)
                            {
                                mLevel = mLevel * (death_level + 10) / attack_level;
                            }
                            if (death_level >= 65)
                            {
                                mLevel = mLevel + (death_level - 15) * ((int)(death_level * 0.25f));
                            }*/

                            //暂定 目前是 以ID 判断是否副本的怪物
                            if (mMapComponent.MapId == 101)
                            {
                                C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(mAttacker.GamePlayer.Player.SourceGameAreaId);
                                var battleCopyCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>().Get((int)CopyType.DemonSquae);
                                mLevel = battleCopyCpt.RecordScore(mAttacker.UnitData.GameUserId, mLevel, b_BeAttacker.Config.Lvl);
                            }
                            else if (mMapComponent.MapId == 100)
                            {
                                C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(mAttacker.GamePlayer.Player.SourceGameAreaId);
                                var battleCopyCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>().Get((int)CopyType.RedCastle);
                                mLevel = battleCopyCpt.RecordKilledMonsters(mAttacker.UnitData.GameUserId, b_BeAttacker.Config.Id, mLevel);
                            }

                            AddExprienceByTeam(mAttacker.GamePlayer, mLevel, 24);
                            if (mAttacker.GamePlayer.Pets != null && !mAttacker.GamePlayer.Pets.IsDeath)
                            {
                                var level = mAttacker.GamePlayer.Pets.dBPetsData.PetsLevel;
                                var mLevel2 = CalculationExperience(death_level, level);
                                mAttacker.GamePlayer.Pets.AddExprience(mLevel2.exp).Coroutine();
                            }
                            if (mAttacker.GamePlayer.UnitData.PkPoint > 0)
                            {
                                //mAttacker.GamePlayer.UnitData.PkPoint -= (b_BeAttacker.Config.Lvl / 2);
                                mAttacker.GamePlayer.UnitData.PkPoint--;
                                if (mAttacker.GamePlayer.UnitData.PkPoint < 0) mAttacker.GamePlayer.UnitData.PkPoint = 0;

                                G2C_ChangeValue_notice mChangeValue = new G2C_ChangeValue_notice();
                                G2C_BattleKVData mChildChangeValue = new G2C_BattleKVData();
                                mChildChangeValue.Key = (int)E_GameProperty.PkNumber;
                                mChildChangeValue.Value = mAttacker.GamePlayer.GetNumerialFunc(E_GameProperty.PkNumber);
                                mChangeValue.Info.Add(mChildChangeValue);

                                mChangeValue.GameUserId = mAttacker.GamePlayer.InstanceId;
                                (mAttacker.GamePlayer).Player.Send(mChangeValue);
                            }


                        }
                        break;
                    case E_Identity.Pet:
                        {
                            var mAttacker = b_Attacker as Pets;
                            var death_level = b_BeAttacker.Config.Lvl;
                            var attack_level = mAttacker.GamePlayer.Data.Level;
                            (mLevel, sourceExp) = CalculationExperience(death_level, attack_level);
                            /*mLevel = (death_level + 25) * death_level / 3;
                            if ((death_level + 10) < attack_level)
                            {
                                mLevel = mLevel * (death_level + 10) / attack_level;
                            }
                            if (death_level >= 65)
                            {
                                mLevel = mLevel + (death_level - 15) * ((int)(death_level * 0.25f));
                            }*/

                            //暂定 目前是 以地图ID 判断怪物
                            if (mMapComponent.MapId == 101)
                            {
                                C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(mAttacker.GamePlayer.Player.SourceGameAreaId);
                                var battleCopyCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>().Get((int)CopyType.DemonSquae);
                                mLevel = battleCopyCpt.RecordScore(mAttacker.UnitData.GameUserId, mLevel, b_BeAttacker.Config.Lvl);
                            }
                            else if (mMapComponent.MapId == 100)
                            {
                                C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(mAttacker.GamePlayer.Player.SourceGameAreaId);
                                var battleCopyCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>().Get((int)CopyType.RedCastle);
                                mLevel = battleCopyCpt.RecordKilledMonsters(mAttacker.UnitData.GameUserId, b_BeAttacker.Config.Id, mLevel);
                            }

                            mAttacker.KillReply(b_BattleComponent);

                            AddExprienceByTeam(mAttacker.GamePlayer, mLevel, 24);
                            if (mAttacker.GamePlayer.Pets != null && !mAttacker.GamePlayer.Pets.IsDeath)
                            {
                                var level = mAttacker.GamePlayer.Pets.dBPetsData.PetsLevel;
                                var mLevel2 = CalculationExperience(death_level, level);
                                mAttacker.GamePlayer.Pets.AddExprience(mLevel2.exp).Coroutine();
                            }
                            if (mAttacker.GamePlayer.UnitData.PkPoint > 0)
                            {
                                mAttacker.GamePlayer.UnitData.PkPoint -= (b_BeAttacker.Config.Lvl / 2);
                                if (mAttacker.GamePlayer.UnitData.PkPoint < 0) mAttacker.GamePlayer.UnitData.PkPoint = 0;

                                G2C_ChangeValue_notice mChangeValue = new G2C_ChangeValue_notice();
                                G2C_BattleKVData mChildChangeValue = new G2C_BattleKVData();
                                mChildChangeValue.Key = (int)E_GameProperty.PkNumber;
                                mChildChangeValue.Value = mAttacker.GamePlayer.GetNumerialFunc(E_GameProperty.PkNumber);
                                mChangeValue.Info.Add(mChildChangeValue);

                                mChangeValue.GameUserId = mAttacker.GamePlayer.InstanceId;
                                (mAttacker.GamePlayer).Player.Send(mChangeValue);
                            }
                        }
                        break;
                    case E_Identity.Hero:
                        {
                            var mAttacker = b_Attacker as GamePlayer;
                            var death_level = b_BeAttacker.Config.Lvl;
                            var attack_level = mAttacker.Data.Level;
                            (mLevel, sourceExp) = CalculationExperience(death_level, attack_level);
                            /*mLevel = (death_level + 25) * death_level / 3;
                            if ((death_level + 10) < attack_level)
                            {
                                mLevel = mLevel * (death_level + 10) / attack_level;
                            }
                            if (death_level >= 65)
                            {
                                mLevel = mLevel + (death_level - 15) * ((int)(death_level * 0.25f));
                            }*/

                            //暂定 目前是 以地图ID 判断怪物
                            if (mMapComponent.MapId == 101)
                            {
                                C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(mAttacker.Player.SourceGameAreaId);
                                var battleCopyCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>().Get((int)CopyType.DemonSquae);
                                mLevel = battleCopyCpt.RecordScore(mAttacker.UnitData.GameUserId, mLevel, b_BeAttacker.Config.Lvl);
                            }
                            else if (mMapComponent.MapId == 100)
                            {
                                C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(mAttacker.Player.SourceGameAreaId);
                                var battleCopyCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>().Get((int)CopyType.RedCastle);
                                mLevel = battleCopyCpt.RecordKilledMonsters(mAttacker.UnitData.GameUserId, b_BeAttacker.Config.Id, mLevel);
                            }

                            if (mAttacker.UnitData.PkPoint > 0)
                            {
                                mAttacker.UnitData.PkPoint -= (b_BeAttacker.Config.Lvl / 2);
                                if (mAttacker.UnitData.PkPoint < 0) mAttacker.UnitData.PkPoint = 0;

                                G2C_ChangeValue_notice mChangeValue = new G2C_ChangeValue_notice();
                                G2C_BattleKVData mChildChangeValue = new G2C_BattleKVData();
                                mChildChangeValue.Key = (int)E_GameProperty.PkNumber;
                                mChildChangeValue.Value = mAttacker.GetNumerialFunc(E_GameProperty.PkNumber);
                                mChangeValue.Info.Add(mChildChangeValue);

                                mChangeValue.GameUserId = mAttacker.InstanceId;
                                (mAttacker).Player.Send(mChangeValue);
                            }

                            AddExprienceByTeam(mAttacker, mLevel, 24);
                            if (mAttacker.Pets != null && !mAttacker.Pets.IsDeath)
                            {
                                var level = mAttacker.Pets.dBPetsData.PetsLevel;
                                var mLevel2 = CalculationExperience(death_level, level);
                                mAttacker.Pets.AddExprience(mLevel2.exp).Coroutine();
                            }
                        }
                        break;
                    case E_Identity.HolyteacherSummoned:
                        {
                            var mAttacker = b_Attacker as HolyteacherSummoned;
                            var death_level = b_BeAttacker.Config.Lvl;
                            var attack_level = mAttacker.GamePlayer.Data.Level;
                            (mLevel, sourceExp) = CalculationExperience(death_level, attack_level);

                            //暂定 目前是 以ID 判断是否副本的怪物
                            if (mMapComponent.MapId == 101)
                            {
                                C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(mAttacker.GamePlayer.Player.SourceGameAreaId);
                                var battleCopyCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>().Get((int)CopyType.DemonSquae);
                                mLevel = battleCopyCpt.RecordScore(mAttacker.UnitData.GameUserId, mLevel, b_BeAttacker.Config.Lvl);
                            }
                            else if (mMapComponent.MapId == 100)
                            {
                                C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(mAttacker.GamePlayer.Player.SourceGameAreaId);
                                var battleCopyCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>().Get((int)CopyType.RedCastle);
                                mLevel = battleCopyCpt.RecordKilledMonsters(mAttacker.UnitData.GameUserId, b_BeAttacker.Config.Id, mLevel);
                            }

                            AddExprienceByTeam(mAttacker.GamePlayer, mLevel, 24);
                            if (mAttacker.GamePlayer.Pets != null && !mAttacker.GamePlayer.Pets.IsDeath)
                            {
                                var level = mAttacker.GamePlayer.Pets.dBPetsData.PetsLevel;
                                var mLevel2 = CalculationExperience(death_level, level);
                                mAttacker.GamePlayer.Pets.AddExprience(mLevel2.exp).Coroutine();
                            }
                            if (mAttacker.GamePlayer.UnitData.PkPoint > 0)
                            {
                                mAttacker.GamePlayer.UnitData.PkPoint -= (b_BeAttacker.Config.Lvl / 2);
                                if (mAttacker.GamePlayer.UnitData.PkPoint < 0) mAttacker.GamePlayer.UnitData.PkPoint = 0;

                                G2C_ChangeValue_notice mChangeValue = new G2C_ChangeValue_notice();
                                G2C_BattleKVData mChildChangeValue = new G2C_BattleKVData();
                                mChildChangeValue.Key = (int)E_GameProperty.PkNumber;
                                mChildChangeValue.Value = mAttacker.GamePlayer.GetNumerialFunc(E_GameProperty.PkNumber);
                                mChangeValue.Info.Add(mChildChangeValue);

                                mChangeValue.GameUserId = mAttacker.GamePlayer.InstanceId;
                                (mAttacker.GamePlayer).Player.Send(mChangeValue);
                            }
                        }
                        break;
                    default:
                        break;

                }
                if (b_BeAttacker.IsCheckState(3))
                {
                    int Id = b_BeAttacker.DropId != 0 ? b_BeAttacker.DropId : b_BeAttacker.Config.SpecialDrop;
                    if (b_BeAttacker.Config.Id == 197) { }  // 血色城门 特殊处理
                    else if (Id == 0) Log.Warning($"怪物{b_BeAttacker.Config.Id}需要特殊掉落却没有掉落组DropId{Id}");
                    else if (b_Attacker is GamePlayer gamePlayer)
                    {
                        SpecialDrop(Id, gamePlayer, b_BeAttacker, mFindTheWay);
                    }
                    else if (b_Attacker is Pets pet)
                    {
                        SpecialDrop(Id, pet.GamePlayer, b_BeAttacker, mFindTheWay);
                    }
                    if(b_BeAttacker.CreatePlayerId != 0 &&
                        b_BeAttacker.CreateItemUID != 0)
                    {
                        // 宝藏怪物
                        GamePlayer gamePlayer = b_Attacker.GetMaster();
                        Player player = gamePlayer.Player;
                        BackpackComponent backpack = player.GetCustomComponent<BackpackComponent>();
                        Item targetItem = backpack.GetItemByUID(b_BeAttacker.CreateItemUID);
                        if(targetItem != null)
                        {
                            backpack.DeleteItem(targetItem, "宝藏怪物死亡，销毁创建道具");
                        }
                        // 通知客户端，删除小地图icon
                        G2C_CangBaoTuPosClose_notice g2c_CangBaoTuPosClose_notice = new G2C_CangBaoTuPosClose_notice();
                        g2c_CangBaoTuPosClose_notice.Id = b_BeAttacker.CreateItemUID;
                        player.Send(g2c_CangBaoTuPosClose_notice);
                    }
                }
                else
                {
                    if (b_Attacker is GamePlayer game)
                        KaliMaShenMiao(b_BeAttacker.Config.Id, game.Player.GameUserId, mFindTheWay);
                    else if (b_Attacker is Pets pet2)
                        KaliMaShenMiao(b_BeAttacker.Config.Id, pet2.GamePlayer.Player.GameUserId, mFindTheWay);
                    // 物品掉落
                    MapItem.MonsterDeathCount += 1;
                    bool stopDrop = false;    // 停止还没执行的随机掉落物品逻辑
                    MapItem mDropItem = null;
                    if (b_Attacker is GamePlayer gamePlayer)
                    {
                        // 玩家击杀怪物 任务掉落
                        mDropItem = GameTasksHelper.DropItem(b_BeAttacker, gamePlayer, mFindTheWay);
                        stopDrop = (mDropItem != null);
                    }
                    else if (b_Attacker is Pets pet)
                    {
                        // 宠物击杀怪物 任务掉落
                        mDropItem = GameTasksHelper.DropItem(b_BeAttacker, pet.GamePlayer, mFindTheWay);
                        stopDrop = (mDropItem != null);
                    }
                    if (stopDrop == false)
                    {
                        int PlayerLevel = 1;
                        switch (b_Attacker.Identity)
                        {
                            case E_Identity.Enemy:
                                break;
                            case E_Identity.Summoned:
                                {
                                    PlayerLevel = (b_Attacker as Summoned).GamePlayer.Data.Level;
                                }
                                break;
                            case E_Identity.Pet:
                                {
                                    PlayerLevel = (b_Attacker as Pets).GamePlayer.Data.Level;
                                }
                                break;
                            case E_Identity.Hero:
                                {
                                    PlayerLevel = (b_Attacker as GamePlayer).Data.Level;
                                }
                                break;
                            case E_Identity.HolyteacherSummoned:
                                {
                                    PlayerLevel = (b_Attacker as HolyteacherSummoned).GamePlayer.Data.Level;
                                }
                                break;
                            default:
                                break;
                        }
                        (stopDrop, mDropItem) = Drop(b_BeAttacker.Config.Lvl, mFindTheWay, b_Attacker.GetNumerialFunc(E_GameProperty.ExplosionRate), PlayerLevel);
                        //if (b_Attacker is GamePlayer gamePlayer2 && stopDrop)
                        //{
                        //    if (!gamePlayer2.CheckDrop(mDropItem, b_BeAttacker.Config.Lvl))
                        //    {
                        //        stopDrop = false;
                        //        mDropItem = null;
                        //    }
                        //}
                    }
                    if (stopDrop == false)
                    {
                        // 自定义掉落
                        if (b_Attacker is GamePlayer gamePlayer2)
                        {
                            // 玩家击杀怪物
                            mDropItem = ItemCustomDropComponentHelper.DropItem(b_BeAttacker, gamePlayer2, mFindTheWay);

                            //if (!gamePlayer2.CheckDrop(mDropItem, b_BeAttacker.Config.Lvl))
                            //{
                            //    mDropItem = null;
                            //}

                        }
                        else if (b_Attacker is Pets pet2)
                        {
                            // 宠物击杀怪物
                            mDropItem = ItemCustomDropComponentHelper.DropItem(b_BeAttacker, pet2.GamePlayer, mFindTheWay);
                            //if (!pet2.GamePlayer.CheckDrop(mDropItem, b_BeAttacker.Config.Lvl))
                            //{
                            //    mDropItem = null;
                            //}
                        }
                        stopDrop = (mDropItem != null);
                    }
                    if (mDropItem != null)
                    {
                        if (mDropItem.ConfigId == 320316)
                        {
                            Random random = new Random();
                            int randomIndex = 1;
                            if (b_Attacker.GetMaster().Data.SpecialTitle)
                                randomIndex  = random.Next(10,41); 
                            else
                                randomIndex = random.Next(5, 21);

                            mDropItem.Count = randomIndex;
                        }
                        if (!b_Attacker.GetMaster().CheckDrop(mDropItem, b_BeAttacker.Config.Lvl))
                        {
                            mDropItem = null;
                        }
                    }
                    if (mDropItem != null)
                    {
                        if (mDropItem.ConfigId == 320294)
                        {
                            // 掉落的是金币
                            // 掉落金币数量 = 怪物击杀经验值(没加成百分比的经验) / 6
                            mDropItem.Count = (int)(sourceExp * 0.1667f);
                            //switch (b_Attacker.Identity)
                            //{
                            //    case E_Identity.Summoned:
                            //        {
                            mDropItem.Count += (int)(mDropItem.Count * b_Attacker.GetNumerialFunc(E_GameProperty.AddGoldCoinRate_Increase) * 0.01f);
                            //        }
                            //        break;
                            //    case E_Identity.Pet:
                            //        {
                            //            mDropItem.Count += (int)(mDropItem.Count * b_Attacker.GetNumerialFunc(E_GameProperty.AddGoldCoinRate_Increase) * 0.01f);
                            //        }
                            //        break;
                            //}

                            /*if (mMapComponent.MapId == 101)
                            {
                                var mAttacker = b_Attacker as GamePlayer;
                                C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(mAttacker.Player.SourceGameAreaId);
                                var battleCopyCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>().Get((int)CopyType.DemonSquae);
                                battleCopyCpt.RecordCoin(b_Attacker.UnitData.GameUserId, mDropItem.Count);
                            }*/

                        }
                        
                        // 添加拾取保护
                        switch (b_Attacker.Identity)
                        {
                            case E_Identity.Enemy:
                                break;
                            case E_Identity.Summoned:
                                {
                                    mDropItem.KillerId.Add((b_Attacker as Summoned).GamePlayer.InstanceId);
                                }
                                break;
                            case E_Identity.Pet:
                                {
                                    mDropItem.KillerId.Add((b_Attacker as Pets).GamePlayer.InstanceId);
                                }
                                break;
                            case E_Identity.Hero:
                                {
                                    mDropItem.KillerId.Add(b_Attacker.InstanceId);
                                }
                                break;
                            case E_Identity.HolyteacherSummoned:
                                {
                                    mDropItem.KillerId.Add((b_Attacker as HolyteacherSummoned).GamePlayer.InstanceId);
                                }
                                break;
                            default:
                                break;
                        }

                        if (mDropItem.Count > 0)
                        {
                            if (mDropItem.ConfigId != 320294)
                            {
                                // 金币掉落不记录
                                GamePlayer gamePlayer2 = b_Attacker.GetMaster();
                                Log.Info($"#物品掉落# r:{(gamePlayer2 != null ? gamePlayer2.InstanceId : 0)} MonsterId:{b_BeAttacker.Config.Id} 总击杀怪物 {MapItem.MonsterDeathCount} ({mDropItem.ToLogString()})");
                            }
                            mDropItem.MonsterConfigId = b_BeAttacker.Config.Id;
                            mMapComponent.MapEntityEnter(mDropItem, mFindTheWay.X, mFindTheWay.Y);
                        }
                        else
                        {
                            mDropItem.Dispose();
                            mDropItem = null;
                        }
                    }
                }

                #region 击杀回复
                G2C_ChangeValue_notice AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                {
                    if (b_ChangeValue_notice == null) b_ChangeValue_notice = new G2C_ChangeValue_notice();

                    G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                    mBattleKVData.Key = (int)b_GameProperty;
                    mBattleKVData.Value = b_Attacker.GetNumerialFunc(b_GameProperty);
                    b_ChangeValue_notice.Info.Add(mBattleKVData);

                    return b_ChangeValue_notice;
                }
                G2C_ChangeValue_notice mChangeValue_notice = null;
                var mKillEnemyReply = b_Attacker.GetNumerialFunc(E_GameProperty.KillEnemyReplyHpRate);
                if (mKillEnemyReply > 0)
                {
                    var mMax = b_Attacker.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                    b_Attacker.UnitData.Hp += mKillEnemyReply * mMax / 10000;
                    if (b_Attacker.UnitData.Hp > mMax) b_Attacker.UnitData.Hp = mMax;
                    mChangeValue_notice = AddPropertyNotice(mChangeValue_notice, E_GameProperty.PROP_HP);
                }
                mKillEnemyReply = b_Attacker.GetNumerialFunc(E_GameProperty.EmbedIncreaseLife);
                if (mKillEnemyReply > 0)
                {
                    var mMax = b_Attacker.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                    b_Attacker.UnitData.Hp += mKillEnemyReply;
                    if (b_Attacker.UnitData.Hp > mMax) b_Attacker.UnitData.Hp = mMax;
                    mChangeValue_notice = AddPropertyNotice(mChangeValue_notice, E_GameProperty.PROP_HP);
                }
                mKillEnemyReply = b_Attacker.GetNumerialFunc(E_GameProperty.KillMonsterReplyHp_8);
                if (mKillEnemyReply > 0)
                {
                    var mMax = b_Attacker.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                    b_Attacker.UnitData.Hp += mMax / 8 * mKillEnemyReply;
                    if (b_Attacker.UnitData.Hp > mMax) b_Attacker.UnitData.Hp = mMax;
                    mChangeValue_notice = AddPropertyNotice(mChangeValue_notice, E_GameProperty.PROP_HP);
                }
                mKillEnemyReply = b_Attacker.GetNumerialFunc(E_GameProperty.KillEnemyReplyMpRate);
                if (mKillEnemyReply > 0)
                {
                    var mMax = b_Attacker.GetNumerialFunc(E_GameProperty.PROP_MP_MAX);
                    b_Attacker.UnitData.Mp += mKillEnemyReply * mMax / 10000;
                    if (b_Attacker.UnitData.Mp > mMax) b_Attacker.UnitData.Mp = mMax;
                    mChangeValue_notice = AddPropertyNotice(mChangeValue_notice, E_GameProperty.PROP_MP);
                }
                mKillEnemyReply = b_Attacker.GetNumerialFunc(E_GameProperty.EmbedIncreaseMagic);
                if (mKillEnemyReply > 0)
                {
                    var mMax = b_Attacker.GetNumerialFunc(E_GameProperty.PROP_MP_MAX);
                    b_Attacker.UnitData.Mp += mKillEnemyReply;
                    if (b_Attacker.UnitData.Mp > mMax) b_Attacker.UnitData.Mp = mMax;
                    mChangeValue_notice = AddPropertyNotice(mChangeValue_notice, E_GameProperty.PROP_MP);
                }
                mKillEnemyReply = b_Attacker.GetNumerialFunc(E_GameProperty.KillMonsterReplyMp_8);
                if (mKillEnemyReply > 0)
                {
                    var mMax = b_Attacker.GetNumerialFunc(E_GameProperty.PROP_MP_MAX);
                    b_Attacker.UnitData.Mp += mMax / 8 * mKillEnemyReply;
                    if (b_Attacker.UnitData.Mp > mMax) b_Attacker.UnitData.Mp = mMax;
                    mChangeValue_notice = AddPropertyNotice(mChangeValue_notice, E_GameProperty.PROP_MP);
                }
                mKillEnemyReply = b_Attacker.GetNumerialFunc(E_GameProperty.KillEnemyReplySDRate);
                if (mKillEnemyReply > 0)
                {
                    var mMax = b_Attacker.GetNumerialFunc(E_GameProperty.PROP_SD_MAX);
                    b_Attacker.UnitData.SD += mKillEnemyReply * mMax / 10000;
                    if (b_Attacker.UnitData.SD > mMax) b_Attacker.UnitData.SD = mMax;
                    mChangeValue_notice = AddPropertyNotice(mChangeValue_notice, E_GameProperty.PROP_SD);
                }
                mKillEnemyReply = b_Attacker.GetNumerialFunc(E_GameProperty.KillEnemyReplyAGRate);
                if (mKillEnemyReply > 0)
                {
                    var mMax = b_Attacker.GetNumerialFunc(E_GameProperty.PROP_AG_MAX);
                    b_Attacker.UnitData.AG += mKillEnemyReply * mMax / 10000;
                    if (b_Attacker.UnitData.AG > mMax) b_Attacker.UnitData.AG = mMax;
                    mChangeValue_notice = AddPropertyNotice(mChangeValue_notice, E_GameProperty.PROP_AG);
                }
                if (mChangeValue_notice != null)
                {
                    mChangeValue_notice.GameUserId = b_Attacker.InstanceId;
                    mMapComponent.SendNotice(b_Attacker, mChangeValue_notice);
                }
                #endregion
            }
            else
            {
                b_BeAttacker.UnitData.Hp -= mRealInjureValue;
                b_BeAttacker.CanBackInjure(b_Attacker, b_AttackType, b_BattleHurtType, mRealInjure.Item1 + mRealInjure.Item2, b_BattleComponent, b_CanBackInjure);

                if (b_BeAttacker.IsCheckState(3))
                {

                }
                else
                {
                    if (b_BeAttacker.TargetEnemy == null)
                    {
                        var mFindTheWay = b_BattleComponent.Parent.GetFindTheWay2D(b_Attacker);
                        if (mFindTheWay != null && mFindTheWay.IsSafeArea == false)
                        {
                            if (Vector2.Distance(b_BattleComponent.Parent.GetFindTheWay2D(b_BeAttacker).Vector2Pos, mFindTheWay.Vector2Pos) < b_BeAttacker.Config.Ran)
                            {// 是否脱战了
                                b_BeAttacker.TargetEnemy = b_Attacker;
                            }
                        }
                    }
                }
            }
            G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
            switch (b_SpecialAttack)
            {
                case E_GameProperty.InjuryValueRate_2:
                    mAttackResultNotice.HurtValueType = 4;
                    break;
                case E_GameProperty.InjuryValueRate_3:
                    mAttackResultNotice.HurtValueType = 5;
                    break;
                case E_GameProperty.LucklyAttackRate:
                    mAttackResultNotice.HurtValueType = 2;
                    break;
                case E_GameProperty.ExcellentAttackRate:
                    mAttackResultNotice.HurtValueType = 3;
                    break;
                default:
                    break;
            }

            if (mIgnoreDefense) mAttackResultNotice.HurtValueType += 100;
            if (b_AttackType == E_BattleHurtAttackType.BACKINJURY) mAttackResultNotice.HurtValueType = 9;

            mAttackResultNotice.AttackSource = b_Attacker.InstanceId;
            mAttackResultNotice.AttackTarget = b_BeAttacker.InstanceId;
            mAttackResultNotice.HurtValue = mRealInjureValue;
            mAttackResultNotice.HpValue = b_BeAttacker.UnitData.Hp;
            mAttackResultNotice.HpMaxValue = b_BeAttacker.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
            if (b_HurtTypeId == 122)
                mAttackResultNotice.HurtValueSource = b_HurtTypeId;

            mMapComponent.SendNotice(b_BeAttacker, mAttackResultNotice);
            return mRealInjureValue;
        }

        public static int InjureSkill(this Enemy b_BeAttacker, CombatSource b_Attacker,
            E_BattleHurtType b_BattleHurtType,
            E_GameProperty b_SpecialAttack,
            int b_HurtTypeId,
            int b_InjureValue,
            BattleComponent b_BattleComponent,
            bool b_CanDefense = true,
            bool b_CanBackInjure = true)
        {
            E_BattleHurtAttackType mAttackType = E_BattleHurtAttackType.SKILL;
            int mRealInjureValue = b_BeAttacker.Injure(b_Attacker, mAttackType, b_BattleHurtType, b_SpecialAttack, b_HurtTypeId, b_InjureValue, b_BattleComponent, b_CanDefense, b_CanBackInjure);

            return mRealInjureValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_EnemyLevel">死亡怪物的等级</param>
        /// <param name="b_FindTheWay">死亡位置</param>
        /// <returns>bool 
        /// false.随到了 无掉落，后面逻辑可以正常处理掉落。 
        /// true.没随到 无掉落，停止后面的掉落逻辑</returns>
        public static (bool, MapItem) Drop(int b_EnemyLevel, C_FindTheWay2D b_FindTheWay, int DropR, int PlayerLevel)
        {
            if (b_EnemyLevel < 0)
            {
                Log.Error($"死亡怪物的等级 < 0,b_EnemyLevel={b_EnemyLevel}");
                return (false, null);
            }
            else if (b_EnemyLevel > ItemDefaultDropComponent.MAX_MONSTER_LEVEL)
            {
                Log.Error($"死亡怪物的等级 > {ItemDefaultDropComponent.MAX_MONSTER_LEVEL},b_EnemyLevel={b_EnemyLevel}");
                return (false, null);
            }

            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var mJsonDic = mReadConfigComponent.GetJson<Enemy_DropConfigJson>().JsonDic;
            #region 用ItemDropInfo，创建MapItem掉落
            void RandStrengthenLevel(MapItem mapItem, RandomSelector<int> randomSelector)
            {
                if (b_EnemyLevel > 100) return; // 只有100级以下的怪物才会掉落强化物品

                if (randomSelector.TryGetValue(out int level))
                {
                    mapItem.Level = 0;
                }
            }

            MapItem CreateDropItemWithItemDropInfo(ItemDropInfo dropInfo)
            {
                ItemConfigManagerComponent itemConfigManager = Root.MainFactory.GetCustomComponent<ItemConfigManagerComponent>();
                ItemConfig itemConfig = itemConfigManager.Get(dropInfo.ItemConfigId);
                if (itemConfig == null)
                {
                    Log.Error($"默认掉落物品,配置物品配置不存在。dropInfo.ItemConfigId={dropInfo.ItemConfigId}");
                    return null;
                }
                Log.Debug().Log("#默认掉落# 掉落物品 {0}:{1} DropType:{2}", itemConfig.Id, itemConfig.Name, dropInfo.DropType);
                MapItem mapItem = null;
                int quality = 0;
                switch (dropInfo.DropType)
                {
                    case ItemDropType.NoDrop:
                        break;
                    case ItemDropType.Normal:
                        // 普通装备
                        quality |= 1 << 1;
                        mapItem = MapItemFactory.Create(itemConfig.Id, EMapItemCreateType.MonsterDrop);
                        mapItem.Quality = quality;
                        mapItem.Count = 1;
                        {
                            ItemDefaultDropComponent itemDefaultDrop = Root.MainFactory.GetCustomComponent<ItemDefaultDropComponent>();
                            RandStrengthenLevel(mapItem, itemDefaultDrop.DefaultDropStrengthenLevel);
                        }
                        break;
                    case ItemDropType.Append:
                        // 普通装备(追加)
                        if (itemConfig.AppendAttrId.Count == 0)
                        {
                            Log.Warning($"默认掉落物品，物品没有追加。ItemConfigId={itemConfig.Id}");
                            return null;
                        }
                        quality |= 1 << 1;
                        mapItem = MapItemFactory.Create(itemConfig.Id, EMapItemCreateType.MonsterDrop);
                        mapItem.Quality = quality;
                        mapItem.Count = 1;
                        mapItem.OptListId = RandomHelper.RandomNumber(0, itemConfig.AppendAttrId.Count);
                        mapItem.OptLevel = 1;
                        {
                            ItemDefaultDropComponent itemDefaultDrop = Root.MainFactory.GetCustomComponent<ItemDefaultDropComponent>();
                            RandStrengthenLevel(mapItem, itemDefaultDrop.DefaultDropStrengthenLevel);
                        }
                        break;
                    case ItemDropType.Skill:
                        // 普通 + 技能 装备
                        if (itemConfig.Skill == 0)
                        {
                            Log.Warning($"默认掉落物品，物品没有技能。ItemConfigId={itemConfig.Id}");
                            return null;
                        }
                        quality |= 1;
                        quality |= 1 << 1;
                        mapItem = MapItemFactory.Create(itemConfig.Id, EMapItemCreateType.MonsterDrop);
                        mapItem.Quality = quality;
                        mapItem.Count = 1;
                        {
                            ItemDefaultDropComponent itemDefaultDrop = Root.MainFactory.GetCustomComponent<ItemDefaultDropComponent>();
                            RandStrengthenLevel(mapItem, itemDefaultDrop.DefaultDropStrengthenLevel);
                        }
                        break;
                    case ItemDropType.Lucky:
                        // 普通 + 幸运 装备
                        if (itemConfig.Skill > 0)
                        {
                            // 普通 + 幸运 + 技能 装备
                            quality |= 1;
                        }
                        quality |= 1 << 1;
                        quality |= 1 << 2;
                        mapItem = MapItemFactory.Create(itemConfig.Id, EMapItemCreateType.MonsterDrop);
                        mapItem.Quality = quality;
                        mapItem.Count = 1;
                        break;
                    case ItemDropType.Excellent:
                        // 普通 + 卓越 装备
                        if (itemConfig.Skill > 0)
                        {
                            // 普通 + 卓越 + 技能 装备
                            quality |= 1;
                        }
                        quality |= 1 << 1;
                        quality |= 1 << 3;
                        mapItem = MapItemFactory.Create(itemConfig.Id, EMapItemCreateType.MonsterDrop);
                        mapItem.Quality = quality;
                        mapItem.Count = 1;
                        break;
                    case ItemDropType.Set:
                        // 普通 + 套装 装备
                        {
                            int setId = 0;
                            ItemSetManager itemSetManager = Root.MainFactory.GetCustomComponent<ItemSetManager>();
                            if (itemSetManager.ItemSetSelector.TryGetValue(itemConfig.Id, out var selector))
                            {
                                selector.TryGetValue(out setId);
                            }
                            if (setId == 0)
                            {
                                Log.Warning($"默认掉落物品，物品没套装。ItemConfigId={itemConfig.Id}");
                                return null;
                            }
                            if (itemConfig.Skill > 0)
                            {
                                // 普通 + 套装 + 技能 装备
                                quality |= 1;
                            }
                            quality |= 1 << 1;
                            quality |= 1 << 4;
                            mapItem = MapItemFactory.Create(itemConfig.Id, EMapItemCreateType.MonsterDrop);
                            mapItem.Quality = quality;
                            mapItem.SetId = setId;
                            mapItem.Count = 1;
                        }
                        break;
                    case ItemDropType.Socket:
                        // 普通 + 镶嵌 装备
                        if (itemConfig.Skill > 0)
                        {
                            // 普通 + 镶嵌 + 技能 装备
                            quality |= 1;
                        }
                        quality |= 1 << 1;
                        quality |= 1 << 5;
                        mapItem = MapItemFactory.Create(itemConfig.Id, EMapItemCreateType.MonsterDrop);
                        mapItem.Quality = quality;
                        mapItem.Count = 1;
                        break;
                    default:
                        Log.Error($"默认掉落物品，未知的DropType。dropInfo.DropType={dropInfo.DropType}");
                        break;
                }
                return mapItem;
            }
            #endregion

            if (mJsonDic.TryGetValue(b_EnemyLevel, out var mDropRateConfig))
            {
                if(b_FindTheWay.Map.MapId == ConstMapId.QiuJinZhiDao)
                {
                    //掉率 + 20%
                    DropR += 20;
                }
                //var ServerInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>();
                //var info = ServerInfo?.GetStartUpInfo(OptionComponent.Options.AppType, OptionComponent.Options.AppId);
                //if (info.IsVIP == 1)
                //{
                //    DropR += 10;
                //}
                //var sum = mDropRateConfig.sum - (int)(mDropRateConfig.NoDrop * DropR * 0.01f);
                float dropWeight = mDropRateConfig.sum - mDropRateConfig.NoDrop;
                float sumWeight = mDropRateConfig.sum;
                int sum = (int)(sumWeight / (((dropWeight / sumWeight + DropR * 0.01f) * sumWeight) / dropWeight));
                //Log.Console($"掉率:{(dropWeight / sum)} DropR:{DropR}");
                //sum = DynamicControlValue(sum, b_EnemyLevel, PlayerLevel);
                if (sum == 0) return (true, null);
                if (sum < dropWeight)
                {
                    sum = (int)dropWeight;
                }
                //int sum = (int)(((float)mDropRateConfig.NoDrop / mDropRateConfig.sum - DropR * 0.01f) * mDropRateConfig.sum);
                int mRandomWeight = Help_RandomHelper.Range(0, sum);
                int mDropWeight = 0;
                if (mDropRateConfig.Equip != 0)
                {
                    mDropWeight += mDropRateConfig.Equip;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 装备掉落
                        Log.Debug("#默认掉落# 装备掉落");
                        ItemDefaultDropComponent itemDefaultDrop = Root.MainFactory.GetCustomComponent<ItemDefaultDropComponent>();
                        if (itemDefaultDrop.ItemDrop[b_EnemyLevel].Equip.TryGetValue(out ItemDropInfo dropInfo))
                        {

                            return (true, CreateDropItemWithItemDropInfo(dropInfo));
                        }

                        // 当前等级没有可以掉落的物品
                        return (true, null);
                    }
                }
                if (mDropRateConfig.Necklace != 0)
                {
                    mDropWeight += mDropRateConfig.Necklace;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 项链
                        Log.Debug("#默认掉落# 项链掉落");
                        ItemDefaultDropComponent itemDefaultDrop = Root.MainFactory.GetCustomComponent<ItemDefaultDropComponent>();
                        if (itemDefaultDrop.ItemDrop[b_EnemyLevel].Necklace.TryGetValue(out ItemDropInfo dropInfo))
                        {

                            return (true, CreateDropItemWithItemDropInfo(dropInfo));
                        }

                        // 当前等级没有可以掉落的物品
                        return (true, null);
                    }
                }
                if (mDropRateConfig.Ring != 0)
                {
                    mDropWeight += mDropRateConfig.Ring;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 戒指
                        Log.Debug("#默认掉落# 戒指掉落");
                        ItemDefaultDropComponent itemDefaultDrop = Root.MainFactory.GetCustomComponent<ItemDefaultDropComponent>();
                        if (itemDefaultDrop.ItemDrop[b_EnemyLevel].Ring.TryGetValue(out ItemDropInfo dropInfo))
                        {

                            return (true, CreateDropItemWithItemDropInfo(dropInfo));
                        }

                        // 当前等级没有可以掉落的物品
                        return (true, null);
                    }
                }
                if (mDropRateConfig.SkillBook != 0)
                {
                    mDropWeight += mDropRateConfig.SkillBook;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 技能书
                        Log.Debug("#默认掉落# 技能书掉落");
                        ItemDefaultDropComponent itemDefaultDrop = Root.MainFactory.GetCustomComponent<ItemDefaultDropComponent>();
                        if (itemDefaultDrop.ItemDrop[b_EnemyLevel].SkillBook.TryGetValue(out ItemDropInfo dropInfo))
                        {

                            return (true, CreateDropItemWithItemDropInfo(dropInfo));
                        }

                        // 当前等级没有可以掉落的物品
                        return (true, null);
                    }
                }
                if (mDropRateConfig.Consumables != 0)
                {
                    mDropWeight += mDropRateConfig.Consumables;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 消耗品
                        Log.Debug("#默认掉落# 消耗品掉落");
                        ItemDefaultDropComponent itemDefaultDrop = Root.MainFactory.GetCustomComponent<ItemDefaultDropComponent>();
                        if (itemDefaultDrop.ItemDrop[b_EnemyLevel].Consumables.TryGetValue(out ItemDropInfo dropInfo))
                        {

                            return (true, CreateDropItemWithItemDropInfo(dropInfo));
                        }

                        // 当前等级没有可以掉落的物品
                        return (true, null);
                    }
                }
                if (mDropRateConfig.LightStone != 0)
                {
                    mDropWeight += mDropRateConfig.LightStone;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 物品表 - 荧光宝石
                        Log.Debug("#默认掉落# 荧光宝石掉落");
                        MapItem mDropItem = MapItemFactory.Create(270001, EMapItemCreateType.MonsterDrop);
                        return (true, mDropItem);
                    }
                }
                if (mDropRateConfig.MayaStone != 0)
                {
                    mDropWeight += mDropRateConfig.MayaStone;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 物品表 - 宝石
                        Log.Debug("#默认掉落# 玛雅之石掉落");
                        MapItem mDropItem = MapItemFactory.Create(280001, EMapItemCreateType.MonsterDrop);
                        return (true, mDropItem);
                    }
                }
                if (mDropRateConfig.BlessingGem != 0)
                {
                    mDropWeight += mDropRateConfig.BlessingGem;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 物品表 - 宝石
                        Log.Debug("#默认掉落# 祝福宝石掉落");
                        MapItem mDropItem = MapItemFactory.Create(280003, EMapItemCreateType.MonsterDrop);
                        return (true, mDropItem);
                    }
                }
                if (mDropRateConfig.SoulGem != 0)
                {
                    mDropWeight += mDropRateConfig.SoulGem;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 物品表 - 宝石
                        Log.Debug("#默认掉落# 灵魂宝石掉落");
                        MapItem mDropItem = MapItemFactory.Create(280004, EMapItemCreateType.MonsterDrop);
                        return (true, mDropItem);
                    }
                }
                if (mDropRateConfig.LifeGem != 0)
                {
                    mDropWeight += mDropRateConfig.LifeGem;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 物品表 - 宝石
                        Log.Debug("#默认掉落# 生命宝石掉落");
                        MapItem mDropItem = MapItemFactory.Create(280005, EMapItemCreateType.MonsterDrop);
                        return (true, mDropItem);
                    }
                }
                if (mDropRateConfig.CreateGem != 0)
                {
                    mDropWeight += mDropRateConfig.CreateGem;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 物品表 - 宝石
                        Log.Debug("#默认掉落# 创造宝石掉落");
                        MapItem mDropItem = MapItemFactory.Create(280006, EMapItemCreateType.MonsterDrop);
                        return (true, mDropItem);
                    }
                }
                if (mDropRateConfig.GuardGem != 0)
                {
                    mDropWeight += mDropRateConfig.GuardGem;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 物品表 - 宝石
                        Log.Debug("#默认掉落# 守护宝石掉落");
                        MapItem mDropItem = MapItemFactory.Create(280007, EMapItemCreateType.MonsterDrop);
                        return (true, mDropItem);
                    }
                }
                if (mDropRateConfig.LuckyGem != 0)
                {
                    mDropWeight += mDropRateConfig.LuckyGem;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 物品表 - 宝石
//                         Log.Debug("#默认掉落# 幸运宝石掉落");
//                         MapItem mDropItem = MapItemFactory.Create(280021, EMapItemCreateType.MonsterDrop);
//                         return (true, mDropItem);
                    }
                }
                if (mDropRateConfig.ExcellentGem != 0)
                {
                    mDropWeight += mDropRateConfig.ExcellentGem;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 物品表 - 宝石
//                         Log.Debug("#默认掉落# 卓越宝石掉落");
//                         MapItem mDropItem = MapItemFactory.Create(280022, EMapItemCreateType.MonsterDrop);
//                         return (true, mDropItem);
                    }
                }
                if (mDropRateConfig.ReviveOriginalStone != 0)
                {
                    mDropWeight += mDropRateConfig.ReviveOriginalStone;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 物品表 - 宝石
                        Log.Debug("#默认掉落# 再生原石掉落");
                        MapItem mDropItem = MapItemFactory.Create(280002, EMapItemCreateType.MonsterDrop);
                        return (true, mDropItem);
                    }
                }
                if (mDropRateConfig.GoldCoin != 0)
                {
                    mDropWeight += mDropRateConfig.GoldCoin;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 物品表 - 其他物品
                        MapItem mDropItem = MapItemFactory.Create(320294, EMapItemCreateType.MonsterDrop);
                        return (true, mDropItem);
                    }
                }
                if (mDropRateConfig.MiracleCoin != 0)
                {
                    mDropWeight += mDropRateConfig.MiracleCoin;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 物品表 - 其他物品
                        MapItem mDropItem = MapItemFactory.Create(320316, EMapItemCreateType.MonsterDrop);
                        return (true, mDropItem);
                    }
                }
                if (mDropRateConfig.FluorescentDropsFire != 0)
                {
                    mDropWeight += mDropRateConfig.FluorescentDropsFire;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 物品表 - 其他物品
                        MapItem mDropItem = MapItemFactory.Create(270008, EMapItemCreateType.MonsterDrop);
                        return (true, mDropItem);
                    }
                }
                if (mDropRateConfig.FluorescentDropsSoil != 0)
                {
                    mDropWeight += mDropRateConfig.FluorescentDropsSoil;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 物品表 - 其他物品
                        MapItem mDropItem = MapItemFactory.Create(270013, EMapItemCreateType.MonsterDrop);
                        return (true, mDropItem);
                    }
                }
                if (mDropRateConfig.FluorescentDropsMine != 0)
                {
                    mDropWeight += mDropRateConfig.FluorescentDropsMine;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 物品表 - 其他物品
                        MapItem mDropItem = MapItemFactory.Create(270012, EMapItemCreateType.MonsterDrop);
                        return (true, mDropItem);
                    }
                }
                if (mDropRateConfig.FluorescentDropsWind != 0)
                {
                    mDropWeight += mDropRateConfig.FluorescentDropsWind;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 物品表 - 其他物品
                        MapItem mDropItem = MapItemFactory.Create(270011, EMapItemCreateType.MonsterDrop);
                        return (true, mDropItem);
                    }
                }
                if (mDropRateConfig.FluorescentDropsIce != 0)
                {
                    mDropWeight += mDropRateConfig.FluorescentDropsIce;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 物品表 - 其他物品
                        MapItem mDropItem = MapItemFactory.Create(270010, EMapItemCreateType.MonsterDrop);
                        return (true, mDropItem);
                    }
                }
                if (mDropRateConfig.FluorescentDropsWater != 0)
                {
                    mDropWeight += mDropRateConfig.FluorescentDropsWater;
                    if (mRandomWeight <= mDropWeight)
                    {
                        // 物品表 - 其他物品
                        MapItem mDropItem = MapItemFactory.Create(270009, EMapItemCreateType.MonsterDrop);
                        return (true, mDropItem);
                    }
                }
            }
            return (false, null);
        }

        /// <summary>            
        /// 对应配置表 Monster_Type字段 
        /// Type == 0 普通怪 :没有特殊处理逻辑
        /// Type == 1 boos怪 :道具掉落和技能有特殊处理
        /// Type == 2 游走怪 :只游走不攻击玩家用于活动
        /// Type == 3 建筑怪 :不移动攻击伤害计次死亡
        /// Type == 4 活动怪 :特殊掉落
        /// Type == 5 黄金怪 :特殊掉落特殊刷新
        /// Skiplogic == 1：不处理攻击逻辑
        /// Skiplogic == 2：不处理位移逻辑
        /// Skiplogic == 3：处理特殊掉落
        /// Skiplogic == 4: 伤害计次
        /// </summary>
        /// <param name="b_Attacker"></param>
        /// <returns></returns>
        public static bool IsCheckState(this Enemy b_Attacker, int Skiplogic)
        {
            int Type = b_Attacker.Config.Monster_Type;
            if (Type == 0) return false;

            switch (Skiplogic)
            {
                case 1:
                    {
                        if (Type == 2 || Type == 3) return true;
                    }
                    break;
                case 2:
                    {
                        if (Type == 3) return true;
                    }
                    break;
                case 3:
                    {
                        if (Type == 2 || Type == 1 || Type == 3 || Type == 4 || Type == 5) return true;
                    }
                    break;
                case 4:
                    {
                        if (Type == 3) return true;
                    }
                    break;
                default:
                    return false;
            }
            return false;
        }

        public static bool CheckCopy(MapComponent map, CombatSource b_Attacker, int Num, out bool EndNum)
        {
            var mServerArea = map.Parent.Parent;
            long userId = 0;
            if (b_Attacker.Identity == E_Identity.Hero)
            {
                userId = (b_Attacker as GamePlayer).Player.GameUserId;
            }
            else if (b_Attacker.Identity == E_Identity.Pet)
            {
                userId = (b_Attacker as Pets).GamePlayer.Player.GameUserId;
            }
            else if (b_Attacker.Identity == E_Identity.Summoned)
            {
                userId = (b_Attacker as Summoned).GamePlayer.Player.GameUserId;
            }

            if (mServerArea != null)
            {
                BatteCopyManagerComponent batteCopyManagerCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>();
                if (batteCopyManagerCpt != null)
                {
                    BattleCopyComponent battleCopyCpt = batteCopyManagerCpt.Get((int)CopyType.RedCastle);
                    if (battleCopyCpt != null)
                    {
                        if (battleCopyCpt.copyRankDataDic.ContainsKey(userId))
                        {
                            CopyRankData copyRankData = battleCopyCpt.copyRankDataDic[userId];
                            long level = copyRankData.Level;
                            int index = copyRankData.Index;
                            BattleCopyRoom battleCopyRoom = battleCopyCpt.battleCopyRoomDic[level][index];
                            if (battleCopyRoom != null)
                            {
                                bool State = battleCopyRoom.AttackCopy1bstacle(Num, out EndNum);
                                if (EndNum) battleCopyRoom.AttackCopyObstacle(userId);
                                return State;
                            }
                        }
                    }
                }
            }
            EndNum = false;
            return false;
        }
        public static bool GetMobRandomDropPos(MapComponent map, int x, int y, int count, ref List<C_FindTheWay2D> allPos)
        {
            if (count <= 0) return false;
            if (count == 1)
            {
                // 返回丢弃的位置
                C_FindTheWay2D pos = map.GetFindTheWay2D(x, y);
                if (pos == null)
                {
                    return false;
                }
                allPos.Add(pos);
                return true;
            }

            int width = 3;
            int minPosX = x - width;
            int minPosY = y - width;
            int maxPosX = x + width;
            int maxPosY = y + width;


            using ListComponent<C_FindTheWay2D> tempPosList = ListComponent<C_FindTheWay2D>.Create();

            for (int posX = minPosX; posX < maxPosX; ++posX)
            {
                if (map.TryGetPosX(posX))
                {
                    for (int posY = minPosY; posY < maxPosY; ++posY)
                    {
                        var pos = map.GetFindTheWay2D(posX, posY);
                        if (pos != null)
                        {
                            if (pos.IsObstacle == false)
                            {
                                tempPosList.Add(pos);
                            }
                        }
                    }
                }
            }
            if (tempPosList.Count == 0) return false;
            var rand = new Random();

            // 不能出现重复的位置
            bool notRepeat = true;
            if (tempPosList.Count < count)
            {
                // 可以出现重复的位置
                notRepeat = false;
            }

            for (int i = 0; i < count; ++i)
            {
                int randId = rand.Next(0, tempPosList.Count);
                allPos.Add(tempPosList[randId]);
                if (notRepeat)
                {
                    tempPosList.RemoveAt(randId);
                }
            }
            return true;
        }

        public static MapItem SpecialDrop(int DropID, GamePlayer gamePlayer, Enemy enemy, C_FindTheWay2D b_FindTheWay)
        {
            int GameAreaId = gamePlayer.Player.GameAreaId;
            long GameUserId = gamePlayer.Player.GameUserId;
            var treasureChest = Root.MainFactory.GetCustomComponent<TreasureChestManager>();
            var readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            if (!readConfig.GetJson<BoosEnemy_DropConfigJson>().JsonDic.TryGetValue(DropID, out var dropConfig))
            {
                // 没有掉落配置
                Log.Error($"配置掉落的物品 DropID={DropID}");
                return null;
            }
            if (!treasureChest.SpecialDrop.TryGetValue(DropID, out var selector))
            {
                Log.Error($"怪物没有配置掉落的物品 DropID={DropID}");
                return null;
            }
            var itemInfoJson = readConfig.GetJson<DropItem_SpecialConfigJson>().JsonDic;


            // TODO 随机能掉落的物品
            if (dropConfig.NotRepeat == 1)
            {
                selector = new RandomSelector<int>(selector);
            }

            //var rand = new Random();
            //int count = rand.Next(dropConfig.MinCount, dropConfig.MaxCount);
            int count = dropConfig.GetCountDrop();
            using ListComponent<Item> itemList = ListComponent<Item>.Create();
            if (dropConfig.MustFall.Count >= 1)
            {
                foreach (var Info in dropConfig.MustFall)
                {
                    itemInfoJson.TryGetValue(Info.Key, out var itemInfoConfig);
                    for (int j = 0; j < Info.Value; j++)
                    {
                        var item = ItemFactory.TryCreate(itemInfoConfig.ItemId, GameAreaId, itemInfoConfig.ToItemCreateAttr());
                        if (item == null)
                        {
                            Log.Error($"配置的物品id不存在DropID={DropID},itemInfoConfig.ItemId={itemInfoConfig.ItemId}");
                            return null;
                        }
                        itemList.Add(item);
                    }
                }
            }
            for (int i = 0; i < count; i++)
            {
                int itemInfoId = 0;
                if (dropConfig.NotRepeat == 1)
                {
                    if (!selector.TryGetValueAndRemove(out itemInfoId))
                    {
                        Log.Error($"配置的物品数量不够DropID={DropID}");
                        return null;
                    }
                }
                else
                {
                    if (!selector.TryGetValue(out itemInfoId))
                    {
                        Log.Error($"配置的物品数量不够DropID={DropID}");
                        return null;
                    }
                }

                if (enemy.MGItem.Item1 == itemInfoId)
                    gamePlayer.DeleteDrop(enemy.MGItem);

                int MGItem = gamePlayer.CheckDrop(enemy.MGItem);
                if (MGItem != 0)
                {
                    itemInfoId = MGItem;
                }
                itemInfoJson.TryGetValue(itemInfoId, out var itemInfoConfig);
                try
                {
                    var item = ItemFactory.Create(itemInfoConfig.ItemId, GameAreaId, itemInfoConfig.ToItemCreateAttr());
                    itemList.Add(item);
                }
                catch (ItemNotSupportAttrException e)
                {
                    Log.Warning($"配置的物品无法添加指定属性，DropID={DropID},itemInfoConfig.ItemId={itemInfoConfig.ItemId}({e.ToString()})");
                }
                catch (ItemConfigNotExistException e)
                {
                    Log.Error($"配置的物品id不存在DropID={DropID},itemInfoConfig.ItemId={itemInfoConfig.ItemId}({e.ToString()})");
                    return null;
                }

            }

            // TODO 随机掉落的位置
            if (itemList.Count == 0)
            {
                Log.Error($"没有配置可以掉落的物品DropID={DropID}");
                return null;
            }
            using ListComponent<C_FindTheWay2D> allPos = ListComponent<C_FindTheWay2D>.Create();
            List<C_FindTheWay2D> allPosList = allPos;

            if (!GetMobRandomDropPos(b_FindTheWay.Map, b_FindTheWay.X, b_FindTheWay.Y, itemList.Count, ref allPosList))
            {
                Log.Error($"位置信息错误DropID={DropID}");
                return null;
            }

            for (int i = 0; i < itemList.Count; ++i)
            {
                var item = itemList[i];
                var pos = allPos[i];

                Log.Info($"#物品掉落##特殊掉落# r:{(gamePlayer != null ? gamePlayer.InstanceId : 0)} 击杀怪物 MonsterId:{enemy.Config.Id} DropId:{DropID} ({item.ToLogString()})");

                MapItem mDropItem = MapItemFactory.Create(item, EMapItemCreateType.MonsterDrop);
                mDropItem.MonsterConfigId = enemy.Config.Id;
                // 添加拾取保护
                mDropItem.KillerId.Add(GameUserId);
                // 设置保护时间 1小时。在物品被清除前，禁止其他玩家拾取
                //mDropItem.ProtectTick = Help_TimeHelper.GetNowSecond(10000000 * 60 * 60L);
                b_FindTheWay.Map.MapEntityEnter(mDropItem, pos.X, pos.Y);
            }

            /*// TODO 清除宝箱的拥有者，防止在一帧时间内，被自己拾取
            args.mapItem.KillerId.Clear();

            var mapItem = args.mapItem;
            var player = args.player;

            async void DeleteItemAsync()
            {
                try
                {
                    await ETModel.ET.TimerComponent.Instance.WaitFrameAsync();
                    if (map.IsDisposeable) return;
                    if (mapItem.IsDisposeable) return;
                    // TODO 删除宝箱
                    G2C_BattlePickUpDropItem_notice mMessageNotice = new G2C_BattlePickUpDropItem_notice();
                    mMessageNotice.InstanceId.Add(mapItem.Id);
                    var pos = map.GetFindTheWay2D(mapItem.X, mapItem.Y);
                    map.SendNotice(pos, mMessageNotice);

                    MapCellAreaComponent mTempMapCellField = map.GetMapAreaByAreaPos(pos.AreaPosX, pos.AreaPosY);

                    mTempMapCellField.MapItemRes.Remove(mapItem.Id);

                    if (mapItem.Item != null)
                    {
                        mapItem.Item.DisposeDB(player.GameAreaId, "开启宝箱");
                    }
                    mapItem.Dispose();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }*/

            return null;
        }

        public static void KaliMaShenMiao(int MobId, long GameUserId, C_FindTheWay2D b_FindTheWay)
        {
            if (MobId <= 0) return;
            if (b_FindTheWay == null || b_FindTheWay.Map == null) return;
            int MapID = b_FindTheWay.Map.MapId;
            if (MapID == 103 || MapID == 104 || MapID == 105 || MapID == 106 || MapID == 107 || MapID == 108 || MapID == 109)
            {
                MapItem mDropItem = MapItemFactory.Create(320106, EMapItemCreateType.MonsterDrop);
                mDropItem.KillerId.Add(GameUserId);
                switch (MobId)
                {
                    case 75:
                        {
                            mDropItem.Level = 2;
                        }
                        break;
                    case 83:
                        {
                            mDropItem.Level = 3;
                        }
                        break;
                    case 91:
                        {
                            mDropItem.Level = 4;
                        }
                        break;
                    case 99:
                        {
                            mDropItem.Level = 5;
                        }
                        break;
                    case 107:
                        {
                            mDropItem.Level = 6;
                        }
                        break;
                    case 115:
                        {
                            mDropItem.Level = 7;
                        }
                        break;
                    default:
                        return;
                }
                b_FindTheWay.Map.MapEntityEnter(mDropItem, 70, 38);
            }
        }

        public static int CheckDrop(GamePlayer b_GamePlayer, (int, int) Id)
        {
            if (b_GamePlayer.dBCharacterDroplimit == null) return 0;
            if (Id.Item1 <= 0) return 0;

            if (b_GamePlayer.dBCharacterDroplimit.MGLsit.TryGetValue(Id, out int Cnt))
            {
                if (Id.Item2 < Cnt) return Id.Item1;
            }
            return 0;
        }

        public static int DynamicControlValue(int Sum, int MobLv, int PlayerLv)
        {
            double ThresholdValue = 0;
            if (50 <= MobLv && MobLv < 70)
            {
                if (1 <= PlayerLv && PlayerLv < 200) ThresholdValue = -0.5;
                else if (200 <= PlayerLv && PlayerLv < 250) ThresholdValue = 0;
                else if (250 <= PlayerLv && PlayerLv < 300) ThresholdValue = 0.5;
                else if (300 <= PlayerLv && PlayerLv < 400) ThresholdValue = 1.5;
                else if (400 <= PlayerLv) ThresholdValue = -1;
            }
            else if (70 <= MobLv && MobLv < 90)
            {
                if (1 <= PlayerLv && PlayerLv < 200) ThresholdValue = -0.8;
                else if (200 <= PlayerLv && PlayerLv < 250) ThresholdValue = -0.5;
                else if (250 <= PlayerLv && PlayerLv < 300) ThresholdValue = 0;
                else if (300 <= PlayerLv && PlayerLv < 350) ThresholdValue = 0.5;
                else if (350 <= PlayerLv && PlayerLv < 450) ThresholdValue = 1.5;
                else if (450 <= PlayerLv) ThresholdValue = -1;
            }
            else if (90 <= MobLv && MobLv < 100)
            {
                if (1 <= PlayerLv && PlayerLv < 200) ThresholdValue = -0.9;
                else if (200 <= PlayerLv && PlayerLv < 250) ThresholdValue = -0.8;
                else if (250 <= PlayerLv && PlayerLv < 300) ThresholdValue = -0.7;
                else if (300 <= PlayerLv && PlayerLv < 350) ThresholdValue = -0.5;
                else if (350 <= PlayerLv && PlayerLv < 400) ThresholdValue = 0;
                else if (400 <= PlayerLv && PlayerLv < 450) ThresholdValue = 0.5;
                else if (450 <= PlayerLv && PlayerLv < 600) ThresholdValue = 1.5;
                else if (600 <= PlayerLv) ThresholdValue = -1;
            }
            else if (100 <= MobLv && MobLv < 110)
            {
                if (PlayerLv < 250) ThresholdValue = -0.8;
                else if (250 <= PlayerLv && PlayerLv < 300) ThresholdValue = -0.5;
                else if (300 <= PlayerLv && PlayerLv < 350) ThresholdValue = 0;
                else if (350 <= PlayerLv && PlayerLv < 400) ThresholdValue = 0.5;
                else if (400 <= PlayerLv && PlayerLv < 500) ThresholdValue = 1.5;
                else if (500 <= PlayerLv ) ThresholdValue = -1;
               
            }
            else if (110 <= MobLv && MobLv < 120)
            {
                if (PlayerLv < 250) ThresholdValue = -0.9;
                else if (250 <= PlayerLv && PlayerLv < 300) ThresholdValue = -0.8;
                else if (300 <= PlayerLv && PlayerLv < 350) ThresholdValue = -0.5;
                else if (350 <= PlayerLv && PlayerLv < 400) ThresholdValue = 0;
                else if (400 <= PlayerLv && PlayerLv < 450) ThresholdValue = 0.5;
                else if (450 <= PlayerLv && PlayerLv < 550) ThresholdValue = 1.5;
                else if (550 <= PlayerLv) ThresholdValue = -1;
            }
            else if (120 <= MobLv && MobLv < 130)
            {
                if (PlayerLv < 300) ThresholdValue = -0.95;
                else if (300 <= PlayerLv && PlayerLv < 350) ThresholdValue = -0.9;
                else if (350 <= PlayerLv && PlayerLv < 400) ThresholdValue = -0.8;
                else if (400 <= PlayerLv && PlayerLv < 450) ThresholdValue = -0.5;
                else if (450 <= PlayerLv && PlayerLv < 500) ThresholdValue = 0;
                else if (500 <= PlayerLv && PlayerLv < 550) ThresholdValue = 0.5;
                else if (550 <= PlayerLv && PlayerLv < 600) ThresholdValue = 1.5;
                else if (600 <= PlayerLv) ThresholdValue = -1;
            }
            else if (130 <= MobLv && MobLv < 140)
            {
                if (PlayerLv < 400) ThresholdValue = -0.9;
                else if (400 <= PlayerLv && PlayerLv < 450) ThresholdValue = -0.8;
                else if (450 <= PlayerLv && PlayerLv < 500) ThresholdValue = -0.5;
                else if (500 <= PlayerLv && PlayerLv < 550) ThresholdValue = 0;
                else if (550 <= PlayerLv && PlayerLv < 600) ThresholdValue = 0.5;
                else if (600 <= PlayerLv && PlayerLv < 700) ThresholdValue = 1.5;
                else if (700 <= PlayerLv) ThresholdValue = -1;
            }
            else if (140 <= MobLv && MobLv < 145)
            {
                if (PlayerLv < 400) ThresholdValue = -0.95;
                else if (400 <= PlayerLv && PlayerLv < 450) ThresholdValue = -0.9;
                else if (450 <= PlayerLv && PlayerLv < 500) ThresholdValue = -0.8;
                else if (500 <= PlayerLv && PlayerLv < 550) ThresholdValue = -0.5;
                else if (550 <= PlayerLv && PlayerLv < 600) ThresholdValue = 0;
                else if (600 <= PlayerLv && PlayerLv < 650) ThresholdValue = 0.5;
                else if (650 <= PlayerLv && PlayerLv < 750) ThresholdValue = 1.5;
                else if(750 <= PlayerLv) ThresholdValue = -1;
            }
            else if (145 <= MobLv && MobLv < 150)
            {
                if (PlayerLv < 400) ThresholdValue = -0.98;
                else if (400 <= PlayerLv && PlayerLv < 450) ThresholdValue = -0.95;
                else if (450 <= PlayerLv && PlayerLv < 500) ThresholdValue = -0.9;
                else if (500 <= PlayerLv && PlayerLv < 550) ThresholdValue = -0.8;
                else if (550 <= PlayerLv && PlayerLv < 600) ThresholdValue = -0.5;
                else if (600 <= PlayerLv && PlayerLv < 650) ThresholdValue = 0;
                else if (650 <= PlayerLv && PlayerLv < 750) ThresholdValue = 0.5;
                else if (750 <= PlayerLv) ThresholdValue = -1;
            }
            Sum = (int)(Sum + Sum * ThresholdValue);
            return Sum;
        }
    }
}