using System;
using ETModel;
using CustomFrameWork;
using System.Threading.Tasks;
using CustomFrameWork.Component;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_MapDeliveryRequestHandler : AMActorRpcHandler<C2G_MapDeliveryRequest, G2C_MapDeliveryResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_MapDeliveryRequest b_Request, G2C_MapDeliveryResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_MapDeliveryRequest b_Request, G2C_MapDeliveryResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家没有找到!");
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.OnlineStatus != EOnlineStatus.Online)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(504);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
                b_Reply(b_Response);
                return false;
            }

            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            if (mGamePlayer.IsDeath)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(501);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("请等待复活后尝试移动!");
                b_Reply(b_Response);
                return false;
            }
            if (mGamePlayer.IsCanOperation() == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(504);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("异常状态,不能移动!");
                b_Reply(b_Response);
                return false;
            }
            var mData = mGamePlayer.UnitData;

            var mPkNumber = mGamePlayer.GetNumerial(E_GameProperty.PkNumber);
            if (mPkNumber > 43200)
            {// 极恶
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(519);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("红名状态,不能传送!");
                b_Reply(b_Response);
                return false;
            }
            else if (mPkNumber > 21600)
            {// 红 
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(519);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("红名状态,不能传送!");
                b_Reply(b_Response);
                return false;
            }
            else if (mPkNumber > 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(519);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("红名状态,不能传送!");
                b_Reply(b_Response);
                return false;
            }

            /*if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(mData.Index, out var mapComponent) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(507);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                b_Reply(b_Response);
                return false;
            }*/
            MapComponent mapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mData.Index, mPlayer.GameUserId);
            if (mapComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(507);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                b_Reply(b_Response);
                return false;
            }

            if (mapComponent.TryGetPosX(mData.X) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(508);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置数据异常x,不可行走!");
                b_Reply(b_Response);
                return false;
            }
            C_FindTheWay2D mFindTheWaySource = mapComponent.GetFindTheWay2D(mData.X, mData.Y);
            if (mFindTheWaySource == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(509);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置数据异常y,不可行走!");
                b_Reply(b_Response);
                return false;
            }
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DataCacheManageComponent mDataCacheManageComponent = mPlayer.AddCustomComponent<DataCacheManageComponent>();
            if (mData.Index == 1)
            {
                if (mFindTheWaySource.IsSafeArea)
                {
                    var mDataCache_Stall = mDataCacheManageComponent.Get<DBStallItem>();
                    if (mDataCache_Stall != null)
                    {
                        var mDatalist_Stall = mDataCache_Stall.OnlyOne();
                        if (mDatalist_Stall != null)
                        {
                            if (mDatalist_Stall.IsDispose == 1)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2118);
                                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                                b_Reply(b_Response);
                                return false;
                            }
                        }
                    }
                }
            }
            if (BatteCopyManagerComponent.BattleCopyMapIDList.Contains(mGamePlayer.UnitData.Index))
            {
                Log.PLog("RunCopyLog", "MapDelivery");
                bool result = mServerArea.GetCustomComponent<BatteCopyManagerComponent>().PlayerExitCopyHandler(mGamePlayer, mServerArea);
            }
            var mTransferPointId = b_Request.MapId;
            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            if (mReadConfigComponent.GetJson<Map_TransferPointConfigJson>().JsonDic.TryGetValue(mTransferPointId, out var _TransferPointConfig) == false)
            {
                // 传送点不存在
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(513);
                b_Reply(b_Response);
                return false;
            }
            var City = mServerArea.GetCustomComponent<CitySiegeActivities>();
            if (City != null && City.GetSate())
            {
                if (406 == mTransferPointId)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2710);
                    b_Reply(b_Response);
                    return false;
                }
            }

            if (mGamePlayer.Data.Level < _TransferPointConfig.MapMinLevel)
            {
                // 进入等级不足
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(514);
                b_Reply(b_Response);
                return false;
            }

            if (mGamePlayer.Data.GoldCoin < _TransferPointConfig.MapCostGold)
            {
                // 金币不足
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(515);
                b_Reply(b_Response);
                return false;
            }

            if (mReadConfigComponent.GetJson<Map_InfoConfigJson>().JsonDic.TryGetValue(_TransferPointConfig.MapId, out var mMapConfig) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(516);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                b_Reply(b_Response);
                return false;
            }

            MapComponent mTargetMapComponent = null;
            if (mMapConfig.IsCopyMap == 0)
            {
                if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(_TransferPointConfig.MapId, out mTargetMapComponent) == false)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(516);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                    b_Reply(b_Response);
                    return false;
                }
            }
            else
            {
                mTargetMapComponent = null;// Help_MapHelper.GetMapByMapId(mServerArea, mData.Index, mPlayer.GameUserId);
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(518);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                b_Reply(b_Response);
                return false;
            }
            if (_TransferPointConfig.MapId == 102 || _TransferPointConfig.MapId == 112)//古战场特殊处理
            {
                if (mPlayer.Data.YuanbaoCoin >= 1)
                {
                    mGamePlayer.UpdateCoin(E_GameProperty.YuanbaoCoin, -1, "古战场进入扣费");
                    DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                    DBProxyComponent dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                    dBProxy.Save(mGamePlayer.Player.Data).Coroutine();
                    G2C_ChangeValue_notice mChangeValue_notice1 = new G2C_ChangeValue_notice();
                    G2C_BattleKVData mBattleKVData1 = new G2C_BattleKVData();
                    mBattleKVData1.Key = (int)E_GameProperty.YuanbaoCoin;
                    mBattleKVData1.Value = mGamePlayer.GetNumerial(E_GameProperty.YuanbaoCoin);
                    mChangeValue_notice1.Info.Add(mBattleKVData1);
                    mPlayer.Send(mChangeValue_notice1);
                }
                else
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2205);
                    b_Reply(b_Response);
                    return false;
                }
            }
            if (_TransferPointConfig.MapId == 10)
            {
                if (mPlayer.GetCustomComponent<EquipmentComponent>().GetEquipItemByPosition(EquipPosition.Wing) == null)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2210);
                    b_Reply(b_Response);
                    return false;
                }
            }
            if (_TransferPointConfig.MapId == 17)
            {
                var Title = mPlayer.GetCustomComponent<PlayerTitle>();
                if (Title.CheckTitle(60002) && Title.CheckTitle(60001))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2213);
                    b_Reply(b_Response);
                    return false;
                }
            }
            if (mTargetMapComponent.TransferPointFindTheWayDic.TryGetValue(mTransferPointId, out var mTransferPointlist) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(517);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("传送点初始化异常!");
                b_Reply(b_Response);
                return false;
            }
            if (mTransferPointlist == null || mTransferPointlist.Count == 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(517);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("传送点初始化异常!");
                b_Reply(b_Response);
                return false;
            }

            var mRandomIndex = Help_RandomHelper.Range(0, mTransferPointlist.Count);
            var mTransferPoint = mTransferPointlist[mRandomIndex];
           
            string str = $"传送前  x:{mData.X},y:{mData.Y}\n";
            Log.Debug($"{str}传送后  x:{mTransferPoint.X},y:{mTransferPoint.Y}");

            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);

            mGamePlayer.UpdateCoin(E_GameProperty.GoldCoin, -_TransferPointConfig.MapCostGold, $"从{mapComponent.MapId} 传送到{_TransferPointConfig.Name}");
            mWriteDataComponent.Save(mGamePlayer.Data, dBProxy2).Coroutine();

            // 金币扣除
            G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
            G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
            mBattleKVData.Key = (int)E_GameProperty.GoldCoin;
            mBattleKVData.Value = mGamePlayer.Data.GoldCoin;
            mChangeValue_notice.Info.Add(mBattleKVData);
            mPlayer.Send(mChangeValue_notice);

            // 公告移动信息
            mTargetMapComponent.MoveSendNotice(mFindTheWaySource, mTransferPoint, mGamePlayer,false);
//             mPlayer.Send(new G2C_MovePos_notice()
//             {
//                 UnitType = (int)E_Identity.Hero,
//                 GameUserId = mGamePlayer.InstanceId,
//                 MapId = mGamePlayer.UnitData.Index,
//                 X = mData.X,
//                 Y = mData.Y,
//                 Angle = mData.Angle,
//                 Title = mGamePlayer.Data.Title,
//                 WallTitle = mGamePlayer.Data.WallTile,
//                 IsNeedMove = 0
//             });
            mGamePlayer.MoveIgnoreTransferId = mTransferPoint.TransferPoint;
            mWriteDataComponent.Save(mData, dBProxy2).Coroutine();

            // 发布 PlayerUseMapDelivery 事件
            ETModel.EventType.PlayerUseMapDelivery.Instance.player = mPlayer;
            ETModel.EventType.PlayerUseMapDelivery.Instance.transferPointId = mTransferPointId;
            Root.EventSystem.OnRun("PlayerUseMapDelivery", ETModel.EventType.PlayerUseMapDelivery.Instance);

            //无敌buf

            var b_BattleComponent = mapComponent.GetCustomComponent<BattleComponent>();
            Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
            {
                var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.DeathRemoveTask;
                mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                mBattleSyncTimer.SyncWaitTime = 5000;
                mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                {
                    if (b_CombatSource.IsDisposeable) return;

                    b_CombatSource.RemoveHealthState(E_BattleSkillStats.WuDi, b_BattleComponent);
                    b_CombatSource.UpdateHealthState();
                };
                mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                {
                    if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                    if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.WuDi, out var hp_Curse) == false)
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

            // 移除进入场景用的无敌buff
            mGamePlayer.RemoveCustomComponent<BuffWuDiForEnterMap>();

            mGamePlayer.AddHealthState(E_BattleSkillStats.WuDi, 0, 5000, 0, mCreateFunc, b_BattleComponent);
            mGamePlayer.UpdateHealthState();


            b_Reply(b_Response);
            return true;
        }
    }
}