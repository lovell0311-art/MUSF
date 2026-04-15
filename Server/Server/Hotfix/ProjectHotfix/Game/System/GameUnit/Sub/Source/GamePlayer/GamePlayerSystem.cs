using System;
using System.Threading.Tasks;
using ETModel;

using CustomFrameWork;
using CustomFrameWork.Component;
using System.Linq;
using UnityEngine;

namespace ETHotfix
{



    public static partial class GamePlayerSystem
    {
        //b_Component.UpdateTimeCompensate = mRunCodeTimeOnEnd - mRunCodeTimeOnBefore
        public static void Attack(this GamePlayer b_Attacker, CombatSource b_BeAttacker, long b_RunCodeTimeOnBefore, BattleComponent b_BattleComponent, bool b_CanBackInjure = true)
        {
            // 检查玩家状态是否为 Online
            if (b_Attacker.Player == null)
            {
                Log.Error("GamePlayer.Player == null");
                return;
            }
            if (b_Attacker.Player.OnlineStatus != EOnlineStatus.Online)
            {
                // 玩家正在登录 或 正在下线
                return;
            }

            if (b_Attacker.IsAttacking) return;
            b_Attacker.IsAttacking = true;

            long mTick = Help_TimeHelper.GetNow();
            int mAttackTimeSum = b_Attacker.GetAttackSpeed();
            b_Attacker.AttackTime = mTick + mAttackTimeSum;

            var mMapComponent = b_BattleComponent.Parent;

            G2C_AttackStart_notice mAttackStartNotice = new G2C_AttackStart_notice();
            mAttackStartNotice.AttackSource = b_Attacker.InstanceId;
            mAttackStartNotice.AttackTarget = b_BeAttacker.InstanceId;
            mAttackStartNotice.AttackType = 0;
            //mAttackStartNotice.Ticks = b_Attacker.AttackTime + 100;
            mAttackStartNotice.Ticks = b_RunCodeTimeOnBefore + mAttackTimeSum;
            mMapComponent.SendNotice(b_BeAttacker, mAttackStartNotice);

            long mRunCodeTimeInterval = mTick - b_RunCodeTimeOnBefore;

            int mAttackTime = (int)(mAttackTimeSum * 0.5f);
            long mOneTime = mAttackTime;
            if (mRunCodeTimeInterval > mAttackTime)
            {
                mOneTime = 0;
                mRunCodeTimeInterval -= mAttackTime;
            }
            else
            {
                mOneTime = mAttackTime - mRunCodeTimeInterval;
                mRunCodeTimeInterval = 0;
            }
            long mTwoTime = mAttackTime;
            if (mRunCodeTimeInterval > mAttackTime)
            {
                mTwoTime = 0;
            }
            else
            {
                mTwoTime = mAttackTime - mRunCodeTimeInterval;
            }

            //mOneTime = (long)(mOneTime * 0.5f);
            //mTwoTime = (long)(mTwoTime * 0.5f);

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
                    IsHit = b_Attacker.IsHitPvP(b_BeAttacker, b_BattleComponent, true);
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

                    int b_InjureValue;
                    var mSpecialAttack = b_Attacker.AttackSpecial();
                    if (mSpecialAttack == E_GameProperty.LucklyAttackRate)
                    {
                        b_InjureValue = b_Attacker.GetNumerialFunc(E_GameProperty.MaxAtteck);
                    }
                    else
                    {
                        b_InjureValue = Help_RandomHelper.Range(b_Attacker.GetNumerialFunc(E_GameProperty.MinAtteck), b_Attacker.GetNumerialFunc(E_GameProperty.MaxAtteck));
                    }

                    switch (b_BeAttacker.Identity)
                    {
                        case E_Identity.Enemy:
                            (b_BeAttacker as Enemy).Injure(b_Attacker, mAttackType, mBattleHurtType, mSpecialAttack, 0, b_InjureValue, b_BattleComponent, true);
                            break;
                        case E_Identity.Summoned:
                            (b_BeAttacker as Summoned).Injure(b_Attacker, mAttackType, mBattleHurtType, mSpecialAttack, 0, b_InjureValue, b_BattleComponent, true);
                            break;
                        case E_Identity.Pet:
                            (b_BeAttacker as Pets).Injure(b_Attacker, mAttackType, mBattleHurtType, mSpecialAttack, 0, b_InjureValue, b_BattleComponent, true);
                            break;
                        case E_Identity.Hero:
                            (b_BeAttacker as GamePlayer).Injure(b_Attacker, mAttackType, mBattleHurtType, mSpecialAttack, 0, b_InjureValue, b_BattleComponent, true);
                            break;
                        default:
                            break;
                    }
                }

                b_Attacker.CombatRoundId = b_BattleComponent.WaitSync((int)mTwoTime, b_AttackerId, b_BeAttackerId, (b_CombatRoundId, b_AttackerIdTemp, b_BeAttackerIdTemp) => { if (b_Attacker.CombatRoundId == b_CombatRoundId) b_Attacker.IsAttacking = false; }, () => { b_Attacker.CombatRoundId = 0; });
            };
            ///攻击间隔毫秒 = 60000 / (50 + (240 - 50) * [攻击速度] / 280)
            b_Attacker.CombatRoundId = 0;
            b_Attacker.CombatRoundId = b_BattleComponent.WaitSync(mAttackTime, b_Attacker.InstanceId, b_BeAttacker.InstanceId, mSyncAction);
        }
        public static int Injure(this GamePlayer b_BeAttacker, CombatSource b_Attacker,
            E_BattleHurtAttackType b_AttackType,
            E_BattleHurtType b_BattleHurtType,
            E_GameProperty b_SpecialAttack,
            int b_HurtTypeId,
            int b_InjureValue,
            BattleComponent b_BattleComponent,
            bool b_CanDefense = true,
            bool b_CanBackInjure = true)
        {
            // 检查玩家状态是否为 Online
            if (b_BeAttacker.Player.OnlineStatus != EOnlineStatus.Online)
            {
                // 玩家正在登录 或 正在下线
                return 0;
            }
            if (b_BeAttacker.IsDeath) return 0;
            var mPos = b_BattleComponent.Parent.GetFindTheWay2D(b_BeAttacker);
            if (mPos == null || mPos.IsSafeArea) return 0;

            (int, int) mRealInjureValue;
            bool mIgnoreDefense = false;
            if (b_CanDefense || b_BattleHurtType != E_BattleHurtType.REAL)
            {
                #region 守护盾
                var mGuardShieldRate = b_BeAttacker.GetNumerial(E_GameProperty.GuardShieldRate);
                if (mGuardShieldRate > 0)
                {
                    int mRandomResult = CustomFrameWork.Help_RandomHelper.Range(0, 10000);
                    if (mRandomResult <= mGuardShieldRate)
                    {
                        var mEquipmentComponent = b_BeAttacker.Player.GetCustomComponent<EquipmentComponent>();
                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
                        {
                            // 盾牌
                            if (mShieldWeaponEquipment.Type == EItemType.Shields)
                            {
                                var mValue = mShieldWeaponEquipment.GetProp(EItemValue.DefenseRate) * 10;

                                b_InjureValue -= mValue;

                                if (b_InjureValue <= 0)
                                {
                                    G2C_AttackResult_notice mAttackResultNoticeTemp = new G2C_AttackResult_notice();
                                    mAttackResultNoticeTemp.HurtValueType = 7;
                                    mAttackResultNoticeTemp.AttackTarget = b_BeAttacker.InstanceId;
                                    b_BattleComponent.Parent.SendNotice(b_BeAttacker, mAttackResultNoticeTemp);
                                    return 0;
                                }
                            }
                        }
                    }
                }
                #endregion
                #region 防盾
                var mGridBlockRate = b_BeAttacker.GetNumerial(E_GameProperty.DefenseShieldRate);
                if (mGridBlockRate > 0)
                {
                    int mRandomResult = CustomFrameWork.Help_RandomHelper.Range(0, 10000);
                    if (mRandomResult <= mGridBlockRate)
                    {
                        var mEquipmentComponent = b_BeAttacker.Player.GetCustomComponent<EquipmentComponent>();
                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldWeaponEquipment))
                        {
                            // 盾牌
                            if (mShieldWeaponEquipment.Type == EItemType.Shields)
                            {
                                G2C_AttackResult_notice mAttackResultNoticeTemp = new G2C_AttackResult_notice();
                                mAttackResultNoticeTemp.HurtValueType = 8;
                                mAttackResultNoticeTemp.AttackTarget = b_BeAttacker.InstanceId;
                                b_BattleComponent.Parent.SendNotice(b_BeAttacker, mAttackResultNoticeTemp);
                                return 0;
                            }
                        }
                    }
                }
                #endregion

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
                mRealInjureValue = b_BeAttacker.Defense(b_Attacker, b_AttackType, b_BattleHurtType, mIgnoreDefense, b_HurtTypeId, b_SpecialAttack, b_InjureValue, b_BattleComponent, b_CanDefense);
            }
            else
            {
                mRealInjureValue = (b_InjureValue, 0);
            }
            var mMapComponent = b_BattleComponent.Parent;

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_BeAttacker.Player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_BeAttacker.Player.GameAreaId);

            if (mRealInjureValue.Item2 > 0)
            {
                b_BeAttacker.UnitData.SD -= mRealInjureValue.Item2;

                if (b_BeAttacker.UnitData.SD < 0) b_BeAttacker.UnitData.SD = 0;
                mWriteDataComponent.Save(b_BeAttacker.UnitData, dBProxy2).Coroutine();
            }

            if (b_BeAttacker.UnitData.Hp <= mRealInjureValue.Item1)
            {
                b_BeAttacker.UnitData.Hp = 0;

                mWriteDataComponent.Save(b_BeAttacker.UnitData, dBProxy2).Coroutine();

                b_BeAttacker.IsDeath = true;
                b_BeAttacker.RemoveAllHealthState(b_BattleComponent);

                if (b_BeAttacker.Pathlist != null) b_BeAttacker.Pathlist = null;
                b_BeAttacker.Enemy = null;
                b_BeAttacker.TargetEnemy = null;

                b_BeAttacker.MoveStartTime = 0;
                b_BeAttacker.MoveNeedTime = 0;
                b_BeAttacker.MoveSleepTime = 0;
                b_BeAttacker.MoveRestTime = 0;
                b_BeAttacker.DeathSleepTime = b_BattleComponent.CurrentTimeTick + b_BeAttacker.Config.DeathSleepTime;

                bool isAddPkPoint = true;

                {
                    var mPkNumber = b_BeAttacker.GetNumerial(E_GameProperty.PkNumber);
                    isAddPkPoint = mPkNumber <= 0;
                    if (isAddPkPoint)
                    {
                        switch (mMapComponent.MapId)
                        {
                            case 4:
                                {
                                    var City = mMapComponent.Parent.Parent.GetCustomComponent<CitySiegeActivities>();
                                    if (City != null && City.GetSate())
                                    {
                                        isAddPkPoint = false;
                                    }
                                }
                                break;
                            case 112:
                            case 102:
                                {
                                    // 古战场不加红名
                                    isAddPkPoint = false;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                var ServerInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>();
                var info = ServerInfo?.GetStartUpInfo(OptionComponent.Options.AppType, OptionComponent.Options.AppId);
                if (info.IsPVP == 1)
                {
                    isAddPkPoint = false;
                }
                

                #region 红名数据
                if (isAddPkPoint)
                {
                    Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
                    {
                        var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                        mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                        //mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateUnitId(b_ZoneId);
                        mBattleSyncTimer.SyncWaitTime = 60 * 1000;
                        mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;
                        mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                        {
                            if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                            if (b_CombatSource.IsDeath) return CombatSource.E_SyncTimerTaskResult.AutoNextRound;

                            if (b_CombatSource.UnitData.PkPoint <= 0) return CombatSource.E_SyncTimerTaskResult.Dispose;

                            b_CombatSource.UnitData.PkPoint -= (int)120;
                            if (b_CombatSource.UnitData.PkPoint < 0) b_CombatSource.UnitData.PkPoint = 0;

                            G2C_ChangeValue_notice mChangeValue = new G2C_ChangeValue_notice();
                            G2C_BattleKVData mChildChangeValue = new G2C_BattleKVData();
                            mChildChangeValue.Key = (int)E_GameProperty.PkNumber;
                            mChildChangeValue.Value = b_CombatSource.GetNumerialFunc(E_GameProperty.PkNumber);
                            mChangeValue.Info.Add(mChildChangeValue);

                            mChangeValue.GameUserId = b_CombatSource.InstanceId;
                            (b_CombatSource as GamePlayer).Player.Send(mChangeValue);

                            if (b_CombatSource.UnitData.PkPoint <= 0) return CombatSource.E_SyncTimerTaskResult.Dispose;

                            return CombatSource.E_SyncTimerTaskResult.AutoNextRound;
                        };
                        return mBattleSyncTimer;
                    };

                    switch (b_Attacker.Identity)
                    {
                        case E_Identity.Hero:
                            {
                                var mGamePlayer = (b_Attacker as GamePlayer);

                                var mFanJiIdlist = mGamePlayer.GetFanJiIdlist();
                                if (mFanJiIdlist != null)
                                {
                                    // 在反击列表 不加pk
                                    if (mFanJiIdlist.TryGetValue(b_BeAttacker.InstanceId, out var mAttackerAttackTime) && mAttackerAttackTime + 60 > b_BattleComponent.CurrentTimeTick / 1000)
                                    {
                                        isAddPkPoint = false;
                                    }
                                }

                                if (isAddPkPoint)
                                {
                                    if (mGamePlayer.UnitData.PkPoint == 0) mGamePlayer.AddTask(mCreateFunc.Invoke());

                                    mGamePlayer.UnitData.PkPoint += 21600;

                                    mWriteDataComponent.Save(b_BeAttacker.UnitData, dBProxy2).Coroutine();

                                    G2C_ChangeValue_notice mChangeValue = new G2C_ChangeValue_notice();
                                    mChangeValue.GameUserId = mGamePlayer.InstanceId;

                                    G2C_BattleKVData mChildChangeValue = new G2C_BattleKVData();
                                    mChildChangeValue.Key = (int)E_GameProperty.PkNumber;
                                    mChildChangeValue.Value = mGamePlayer.UnitData.PkPoint;
                                    mChangeValue.Info.Add(mChildChangeValue);

                                    mMapComponent.SendNotice(b_BeAttacker, mChangeValue);
                                }
                                var AttackerWarAlliance = mGamePlayer.Player.GetCustomComponent<PlayerWarAllianceComponent>();
                                if (AttackerWarAlliance != null && AttackerWarAlliance.WarAllianceID != 0)
                                {
                                    var BeAttackerWarAlliance = b_BeAttacker.Player.GetCustomComponent<PlayerWarAllianceComponent>();
                                    if (BeAttackerWarAlliance != null && BeAttackerWarAlliance.WarAllianceID != 0)
                                    {
                                        AttackerWarAlliance.AllianceScore += 1;
                                        AttackerWarAlliance.UpDateWarAlliancePlayerInfo();
                                        G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                        G2C_BattleKVData mGoldCoinData = new G2C_BattleKVData();
                                        mGoldCoinData.Key = (int)E_GameProperty.AllianceScoreChange;
                                        mGoldCoinData.Value = AttackerWarAlliance.AllianceScore;
                                        mChangeValueMessage.Info.Add(mGoldCoinData);
                                        mGamePlayer.Player.Send(mChangeValueMessage);
                                    }
                                }
                            }
                            break;
                        case E_Identity.Pet:
                            {
                                var mGamePlayer = (b_Attacker as Pets).GamePlayer;

                                var mFanJiIdlist = mGamePlayer.GetFanJiIdlist();
                                if (mFanJiIdlist != null)
                                {
                                    // 在反击列表 不加pk
                                    if (mFanJiIdlist.TryGetValue(b_BeAttacker.InstanceId, out var mAttackerAttackTime) && mAttackerAttackTime + 60 > b_BattleComponent.CurrentTimeTick / 1000)
                                    {
                                        isAddPkPoint = false;
                                    }
                                }
                                if (isAddPkPoint)
                                {
                                    if (mGamePlayer.UnitData.PkPoint == 0) mGamePlayer.AddTask(mCreateFunc.Invoke());
                                    mGamePlayer.UnitData.PkPoint += 21600;

                                    mWriteDataComponent.Save(b_BeAttacker.UnitData, dBProxy2).Coroutine();

                                    G2C_ChangeValue_notice mChangeValue = new G2C_ChangeValue_notice();
                                    mChangeValue.GameUserId = mGamePlayer.InstanceId;

                                    G2C_BattleKVData mChildChangeValue = new G2C_BattleKVData();
                                    mChildChangeValue.Key = (int)E_GameProperty.PkNumber;
                                    mChildChangeValue.Value = mGamePlayer.UnitData.PkPoint;
                                    mChangeValue.Info.Add(mChildChangeValue);

                                    mMapComponent.SendNotice(b_BeAttacker, mChangeValue);
                                }
                            }
                            break;
                        case E_Identity.Summoned:
                            {
                                var mGamePlayer = (b_Attacker as Summoned).GamePlayer;

                                var mFanJiIdlist = mGamePlayer.GetFanJiIdlist();
                                if (mFanJiIdlist != null)
                                {
                                    // 在反击列表 不加pk
                                    if (mFanJiIdlist.TryGetValue(b_BeAttacker.InstanceId, out var mAttackerAttackTime) && mAttackerAttackTime + 60 > b_BattleComponent.CurrentTimeTick / 1000)
                                    {
                                        isAddPkPoint = false;
                                    }
                                }
                                if (isAddPkPoint)
                                {
                                    if (mGamePlayer.UnitData.PkPoint == 0) mGamePlayer.AddTask(mCreateFunc.Invoke());
                                    mGamePlayer.UnitData.PkPoint += 21600;

                                    mWriteDataComponent.Save(b_BeAttacker.UnitData, dBProxy2).Coroutine();

                                    G2C_ChangeValue_notice mChangeValue = new G2C_ChangeValue_notice();
                                    mChangeValue.GameUserId = mGamePlayer.InstanceId;

                                    G2C_BattleKVData mChildChangeValue = new G2C_BattleKVData();
                                    mChildChangeValue.Key = (int)E_GameProperty.PkNumber;
                                    mChildChangeValue.Value = mGamePlayer.UnitData.PkPoint;
                                    mChangeValue.Info.Add(mChildChangeValue);

                                    mMapComponent.SendNotice(b_BeAttacker, mChangeValue);
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    var mLevel = b_BeAttacker.GetNumerial(E_GameProperty.Level);
                    if (mLevel > 10)
                    {
                        var mPkNumber = b_BeAttacker.GetNumerial(E_GameProperty.PkNumber);
                        if (mPkNumber > 43200)
                        {// 极恶 20
                            b_BeAttacker.AddExprienceMaxRate(-20);
                        }
                        else if (mPkNumber > 21600)
                        {// 红 10
                            b_BeAttacker.AddExprienceMaxRate(-10);
                        }
                        else if (mPkNumber > 0)
                        {// 5
                            b_BeAttacker.AddExprienceMaxRate(-5);
                        }
                        else
                        {
                            if (mLevel > 220)
                            {// 1
                                b_BeAttacker.AddExprienceMaxRate(-1);
                            }
                            else if (mLevel > 150)
                            {// 2
                                b_BeAttacker.AddExprienceMaxRate(-2);
                            }
                            else if (mLevel > 10)
                            {// 3
                                b_BeAttacker.AddExprienceMaxRate(-3);
                            }
                        }
                    }
                }
                #endregion

                if (b_BeAttacker.Summoned != null && b_BeAttacker.Summoned.IsDisposeable == false)
                {
                    b_BeAttacker.Summoned.IsDeath = true;
                    b_BeAttacker.Summoned.IsReallyDeath = true;
                }

                // 宠物回收
                //                 if (b_BeAttacker.Pets != null)
                //                 {
                //                     b_BeAttacker.Pets.IsDeath = true;
                //                 }

                if (b_BeAttacker.Identity == E_Identity.Hero)
                {
                    Log.PLog($"b_BeAttacker.InstanceId:{b_BeAttacker.InstanceId} 死亡");
                }
                else
                {
                    Log.Debug().Log("b_BeAttacker.InstanceId:{instanceId} 死亡", b_BeAttacker.InstanceId);
                }
                // 死亡TODO
                if (b_BeAttacker.Identity == E_Identity.Hero && b_Attacker.Identity == E_Identity.Hero)
                {
                    GamePlayer b_AttackerPlayer = b_Attacker as GamePlayer;
                    Player b_BeAttackerplayer = b_BeAttacker.Player;
                    FriendComponent friendComponent = b_BeAttackerplayer.GetCustomComponent<FriendComponent>();
                    if (friendComponent == null)
                    {
                        friendComponent.AddCustomComponent<FriendComponent>();
                    }
                    friendComponent.AddFOEList(b_BeAttacker, b_AttackerPlayer).Coroutine();
                }

                switch (b_Attacker.Identity)
                {
                    case E_Identity.Hero:
                        {
                            G2C_KillResult_notice mKillResultnotice = new G2C_KillResult_notice();
                            mKillResultnotice.AttackId = (b_Attacker as GamePlayer).Data.NickName;
                            mKillResultnotice.BeAttackId = b_BeAttacker.Data.NickName;

                            mMapComponent.SendNoticeByServer(mKillResultnotice).Coroutine();
                        }
                        break;
                    case E_Identity.Summoned:
                        {
                            G2C_KillResult_notice mKillResultnotice = new G2C_KillResult_notice();
                            mKillResultnotice.AttackId = (b_Attacker as Summoned).GamePlayer.Data.NickName;
                            mKillResultnotice.BeAttackId = b_BeAttacker.Data.NickName;

                            mMapComponent.SendNoticeByServer(mKillResultnotice).Coroutine();
                        }
                        break;
                    case E_Identity.HolyteacherSummoned:
                        {
                            G2C_KillResult_notice mKillResultnotice = new G2C_KillResult_notice();
                            mKillResultnotice.AttackId = (b_Attacker as HolyteacherSummoned).GamePlayer.Data.NickName;
                            mKillResultnotice.BeAttackId = b_BeAttacker.Data.NickName;

                            mMapComponent.SendNoticeByServer(mKillResultnotice).Coroutine();
                        }
                        break;
                    //case E_Identity.FenShen:
                    //    break;
                    default:
                        break;
                }

                // TODO 发布 GamePlayerDeath 事件，放到死亡逻辑最后
                ETModel.EventType.GamePlayerDeath.Instance.gamePlayer = b_BeAttacker;
                ETModel.EventType.GamePlayerDeath.Instance.attacker = b_Attacker;
                ETModel.EventType.GamePlayerDeath.Instance.map = mMapComponent;
                Root.EventSystem.OnRun("GamePlayerDeath", ETModel.EventType.GamePlayerDeath.Instance);
            }
            else
            {
                b_BeAttacker.UnitData.Hp -= mRealInjureValue.Item1;
                mWriteDataComponent.Save(b_BeAttacker.UnitData, dBProxy2).Coroutine();
                b_BeAttacker.CanBackInjure(b_Attacker, b_AttackType, b_BattleHurtType, mRealInjureValue.Item1 + mRealInjureValue.Item2, b_BattleComponent, b_CanBackInjure);

                #region 被击回复
                G2C_ChangeValue_notice AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                {
                    if (b_ChangeValue_notice == null) b_ChangeValue_notice = new G2C_ChangeValue_notice();

                    G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                    mBattleKVData.Key = (int)b_GameProperty;
                    mBattleKVData.Value = b_BeAttacker.GetNumerialFunc(b_GameProperty);
                    b_ChangeValue_notice.Info.Add(mBattleKVData);

                    return b_ChangeValue_notice;
                }
                G2C_ChangeValue_notice mChangeValue_notice = null;
                var mReplyValueRate = b_BeAttacker.GetNumerial(E_GameProperty.Injury_ReplyAllHpRate);
                if (mReplyValueRate > 0)
                {
                    var mRate = Help_RandomHelper.Range(0, 100);
                    if (mRate < mReplyValueRate)
                    {
                        var mMax = b_BeAttacker.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                        if (b_BeAttacker.UnitData.Hp != mMax)
                        {
                            b_BeAttacker.UnitData.Hp = mMax;
                            mChangeValue_notice = AddPropertyNotice(mChangeValue_notice, E_GameProperty.PROP_HP);
                        }
                    }
                }
                mReplyValueRate = b_BeAttacker.GetNumerial(E_GameProperty.Injury_ReplyAllMpRate);
                if (mReplyValueRate > 0)
                {
                    var mRate = Help_RandomHelper.Range(0, 100);
                    if (mRate < mReplyValueRate)
                    {
                        var mMax = b_BeAttacker.GetNumerialFunc(E_GameProperty.PROP_MP_MAX);
                        if (b_BeAttacker.UnitData.Mp != mMax)
                        {
                            b_BeAttacker.UnitData.Mp = mMax;
                            mChangeValue_notice = AddPropertyNotice(mChangeValue_notice, E_GameProperty.PROP_MP);
                        }
                    }
                }
                if (mChangeValue_notice != null)
                {
                    mChangeValue_notice.GameUserId = b_BeAttacker.InstanceId;
                    mMapComponent.SendNotice(b_BeAttacker, mChangeValue_notice);
                }
                #endregion
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
            mAttackResultNotice.HurtValue = mRealInjureValue.Item1;
            mAttackResultNotice.SD = b_BeAttacker.GetNumerial(E_GameProperty.PROP_SD);
            mAttackResultNotice.SDMaxValue = b_BeAttacker.GetNumerial(E_GameProperty.PROP_SD_MAX);
            mAttackResultNotice.HpValue = b_BeAttacker.GetNumerial(E_GameProperty.PROP_HP);
            mAttackResultNotice.HpMaxValue = b_BeAttacker.GetNumerial(E_GameProperty.PROP_HP_MAX);
            mMapComponent.SendNotice(b_BeAttacker, mAttackResultNotice);

            return mRealInjureValue.Item1 + mRealInjureValue.Item2;
        }

        public static int InjureSkill(this GamePlayer b_BeAttacker, CombatSource b_Attacker,
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

        public static E_GameProperty AttackSpecial(this CombatSource b_Attacker)
        {
            int mRandomValue = Help_RandomHelper.Range(0, 10000);
            if (mRandomValue < b_Attacker.GetNumerialFunc(E_GameProperty.InjuryValueRate_3))
            {
                return E_GameProperty.InjuryValueRate_3;
            }
            mRandomValue = Help_RandomHelper.Range(0, 10000);
            if (mRandomValue < b_Attacker.GetNumerialFunc(E_GameProperty.InjuryValueRate_2))
            {
                return E_GameProperty.InjuryValueRate_2;
            }
            mRandomValue = Help_RandomHelper.Range(0, 10000);
            if (mRandomValue < b_Attacker.GetNumerialFunc(E_GameProperty.ExcellentAttackRate))
            {
                return E_GameProperty.ExcellentAttackRate;
            }
            mRandomValue = Help_RandomHelper.Range(0, 10000);
            if (mRandomValue < b_Attacker.GetNumerialFunc(E_GameProperty.LucklyAttackRate))
            {
                return E_GameProperty.LucklyAttackRate;
            }
            return E_GameProperty.NullAttack;
        }
    }
}