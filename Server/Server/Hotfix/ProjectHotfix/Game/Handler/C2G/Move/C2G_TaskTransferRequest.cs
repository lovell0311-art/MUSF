using System;
using ETModel;
using CustomFrameWork;
using System.Threading.Tasks;
using CustomFrameWork.Component;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_TaskTransferRequestHandler : AMActorRpcHandler<C2G_TaskTransferRequest, G2C_TaskTransferResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_TaskTransferRequest b_Request, G2C_TaskTransferResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_TaskTransferRequest b_Request, G2C_TaskTransferResponse b_Response, Action<IMessage> b_Reply)
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

            //var mPkNumber = mGamePlayer.GetNumerial(E_GameProperty.PkNumber);
            //if (mPkNumber > 43200)
            //{// 极恶
            //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(519);
            //    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("红名状态,不能传送!");
            //    b_Reply(b_Response);
            //    return false;
            //}
            //else if (mPkNumber > 21600)
            //{// 红 
            //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(519);
            //    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("红名状态,不能传送!");
            //    b_Reply(b_Response);
            //    return false;
            //}
            //else if (mPkNumber > 0)
            //{
            //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(519);
            //    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("红名状态,不能传送!");
            //    b_Reply(b_Response);
            //    return false;
            //}

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
                    if (mDataCache_Stall == null)
                    {
                        var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                        mDataCache_Stall = await HelpDb_DBStallItem.Init(mPlayer, mDataCacheManageComponent, dBProxy);
                    }
                    var mData_Stall = mDataCache_Stall.OnlyOne();
                    if (mData_Stall != null)
                    {
                        if (mData_Stall.IsDispose == 1)
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2118);
                            //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                            b_Reply(b_Response);
                            return false;
                        }
                    }
                }
            }
            if (BatteCopyManagerComponent.BattleCopyMapIDList.Contains(mGamePlayer.UnitData.Index))
            {
                Log.PLog("RunCopyLog", "TaskDelivery");
                bool result = mServerArea.GetCustomComponent<BatteCopyManagerComponent>().PlayerExitCopyHandler(mGamePlayer, mServerArea);
            }
            var mTransferId = b_Request.MapId;
            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            if (mReadConfigComponent.GetJson<Map_InfoConfigJson>().JsonDic.TryGetValue(mTransferId, out var _TransferConfig) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(513);
                b_Reply(b_Response);
                return false;
            }
            var City = mServerArea.GetCustomComponent<CitySiegeActivities>();
            if (City != null && City.GetSate())
            {
                if (4 == mTransferId)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2710);
                    b_Reply(b_Response);
                    return false;
                }
            }

            if (mGamePlayer.Data.Level < _TransferConfig.GotoMapByLevel)
            {
                // 进入等级不足
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(514);
                b_Reply(b_Response);
                return false;
            }

            /*if (mGamePlayer.Data.GoldCoin < _TransferPointConfig.MapCostGold)
            {
                // 金币不足
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(515);
                b_Reply(b_Response);
                return false;
            }*/
            var knapsack = mPlayer.GetCustomComponent<BackpackComponent>();
            Dictionary<long, Item> ItemList = null;
            if (knapsack != null)
            {
                ItemList = knapsack.GetAllItemByConfigID(320402);
                if (ItemList == null && ItemList.Count <= 0)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1609);
                    b_Reply(b_Response);
                    return false;
                }
            }

            MapComponent mTargetMapComponent = null;
            if (_TransferConfig.IsCopyMap == 0)
            {
                if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(mTransferId, out mTargetMapComponent) == false)
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
            if (mTransferId == 102 || mTransferId == 112)//古战场特殊处理
            {
                if (mPlayer.Data.YuanbaoCoin >= 1)
                {
                    mGamePlayer.UpdateCoin(E_GameProperty.YuanbaoCoin, -1, "古战场进入扣费");
                    DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                    DBProxyComponent dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                    dBProxy.Save(mGamePlayer.Player.Data).Coroutine();
                    G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
                    G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                    mBattleKVData.Key = (int)E_GameProperty.YuanbaoCoin;
                    mBattleKVData.Value = mGamePlayer.GetNumerial(E_GameProperty.YuanbaoCoin);
                    mChangeValue_notice.Info.Add(mBattleKVData);
                    mPlayer.Send(mChangeValue_notice);
                }
                else
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2205);
                    b_Reply(b_Response);
                    return false;
                }
            }
            /*if (mTargetMapComponent.TransferPointFindTheWayDic.TryGetValue(mTransferPointId, out var mTransferPointlist) == false)
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
            */
            var mTransferPoint = mTargetMapComponent.GetFindTheWay2D(b_Request.X, b_Request.Y);
            string str = $"传送前  x:{mData.X},y:{mData.Y}\n";
            Log.Debug($"{str}传送后  x:{mTransferPoint.X},y:{mTransferPoint.Y}");

            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);

            //mGamePlayer.UpdateCoin(E_GameProperty.GoldCoin, -_TransferPointConfig.MapCostGold, $"从{mapComponent.MapId} 传送到{_TransferPointConfig.Name}");
            //mWriteDataComponent.Save(mGamePlayer.Data, dBProxy2).Coroutine();

            // 金币扣除
            //G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
            //G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
            //mBattleKVData.Key = (int)E_GameProperty.GoldCoin;
            //mBattleKVData.Value = mGamePlayer.Data.GoldCoin;
            //mChangeValue_notice.Info.Add(mBattleKVData);
            //mPlayer.Send(mChangeValue_notice);

            //扣除道具
            foreach (var Item in ItemList)
            {
                if (Item.Value != null && Item.Value.GetProp(EItemValue.Quantity) >= 1)
                {
                    knapsack.UseItem(Item.Key, "任务传送");
                    break;
                }
            }
            // 公告移动信息
            mTargetMapComponent.MoveSendNotice(mFindTheWaySource, mTransferPoint, mGamePlayer, false);
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
            ETModel.EventType.PlayerUseMapDelivery.Instance.transferPointId = mTransferId;
            Root.EventSystem.OnRun("PlayerUseMapDelivery", ETModel.EventType.PlayerUseMapDelivery.Instance);

            b_Reply(b_Response);
            return true;
        }
    }
}