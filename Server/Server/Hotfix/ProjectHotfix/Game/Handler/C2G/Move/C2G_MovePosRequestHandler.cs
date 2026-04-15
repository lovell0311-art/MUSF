using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_MovePosRequestHandler : AMActorRpcHandler<C2G_MovePosRequest, G2C_MovePosResponse>
    {
        private static bool IsWalkableCell(C_FindTheWay2D cell)
        {
            return cell != null && cell.IsStaticObstacle == false;
        }

        private static C_FindTheWay2D FindNearestWalkableCell(MapComponent mapComponent, int posX, int posY, int maxRadius = 16)
        {
            if (mapComponent == null)
            {
                return null;
            }

            int centerX = Math.Max(0, Math.Min(mapComponent.MapWidth - 1, posX));
            int centerY = Math.Max(0, Math.Min(mapComponent.MapHight - 1, posY));

            C_FindTheWay2D centerCell = mapComponent.GetFindTheWay2D(centerX, centerY);
            if (IsWalkableCell(centerCell))
            {
                return centerCell;
            }

            for (int radius = 1; radius <= maxRadius; ++radius)
            {
                C_FindTheWay2D nearestCell = null;
                int nearestDistance = int.MaxValue;

                for (int dx = -radius; dx <= radius; ++dx)
                {
                    for (int dy = -radius; dy <= radius; ++dy)
                    {
                        if (Math.Abs(dx) != radius && Math.Abs(dy) != radius)
                        {
                            continue;
                        }

                        C_FindTheWay2D candidate = mapComponent.GetFindTheWay2D(centerX + dx, centerY + dy);
                        if (IsWalkableCell(candidate) == false)
                        {
                            continue;
                        }

                        int distance = dx * dx + dy * dy;
                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestCell = candidate;
                        }
                    }
                }

                if (nearestCell != null)
                {
                    return nearestCell;
                }
            }

            return null;
        }

        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_MovePosRequest b_Request, G2C_MovePosResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_MovePosRequest b_Request, G2C_MovePosResponse b_Response, Action<IMessage> b_Reply)
        {
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get((int)(b_Request.AppendData >> 16), b_Request.ActorId);
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

            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            if (mGamePlayer.IsDeath)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(501);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("请等待复活后尝试移动!");
                b_Reply(b_Response);
                return false;
            }
            if (b_Request.X == mGamePlayer.UnitData.X && b_Request.Y == mGamePlayer.UnitData.Y)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(508);
                b_Reply(b_Response);
                return false;
            }
            /*
            long mMoveNeedTime = Help_TimeHelper.GetNow();
            if (mGamePlayer.MoveNeedTime > mMoveNeedTime)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(508);
                b_Reply(b_Response);
                return false;
            }
            mGamePlayer.MoveNeedTime = mMoveNeedTime + 50;
            */
            //if (mGamePlayer.IsAttacking)
            //{
            //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(506);
            //    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("正在攻击!");
            //    b_Reply(b_Response);
            //    return false;
            //}
            //else 
            if (mGamePlayer.IsCanMove() == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(504);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("异常状态,不能移动!");
                b_Reply(b_Response);
                return false;
            }

            Log.Debug().Log("目标 x:{0},y:{1} 现在:x:{2},y:{3} ",
                b_Request.X,
                b_Request.Y,
                mGamePlayer.UnitData.X,
                     mGamePlayer.UnitData.Y);
            if (MathF.Abs(mGamePlayer.UnitData.X - b_Request.X) > 2 || MathF.Abs(mGamePlayer.UnitData.Y - b_Request.Y) > 2)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(500);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage($"行走异常!意图移动距离过大 x:{mData.X} =>{b_Request.X} y:{mData.Y} =>{b_Request.Y}");

                b_Response.X = mGamePlayer.UnitData.X;
                b_Response.Y = mGamePlayer.UnitData.Y;
                b_Response.Angle = mGamePlayer.UnitData.Angle;

                mPlayer.Send(new G2C_MovePos_notice()
                {
                    UnitType = (int)E_Identity.Hero,
                    GameUserId = mGamePlayer.InstanceId,
                    MapId = mGamePlayer.UnitData.Index,
                    X = mGamePlayer.UnitData.X,
                    Y = mGamePlayer.UnitData.Y,
                    Angle = mGamePlayer.UnitData.Angle,
                    IsNeedMove = 0,
                    Title = mGamePlayer.Data.Title,
                    WallTitle = mGamePlayer.Data.WallTile,
                    ReincarnateCnt = mGamePlayer.Data.ReincarnateCnt
                });

                b_Reply(b_Response);
                return false;
            }

            /*var mTransferPointId = mData.Index;
            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            if (mReadConfigComponent.GetJson<Map_InfoConfigJson>().JsonDic.TryGetValue(mTransferPointId, out var mMapConfig) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(516);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                b_Reply(b_Response);
                return false;
            }

            MapComponent mapComponent = null;
            if (mMapConfig.IsCopyMap == 0)
            {
                if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(mData.Index, out mapComponent) == false)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(516);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                    b_Reply(b_Response);
                    return false;
                }
            }
            else
            {
                mapComponent = mServerArea.GetCustomComponent<BatteCopyManagerComponent>().GetRoomMapComponent(mData.Index, mPlayer.GameUserId);
                if (mapComponent == null)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(518);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                    b_Reply(b_Response);
                    return false;
                }
            }*/
            //MapComponent mapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mGamePlayer.UnitData.Index, mPlayer.GameUserId);
            MapComponent mapComponent = mGamePlayer.CurrentMap;
            if (mapComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(507);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                b_Reply(b_Response);
                return false;
            }

            void ReplyWithCurrentPosition(int errorCode)
            {
                b_Response.Error = errorCode;
                b_Response.X = mGamePlayer.UnitData.X;
                b_Response.Y = mGamePlayer.UnitData.Y;
                b_Response.Angle = mGamePlayer.UnitData.Angle;

                mPlayer.Send(new G2C_MovePos_notice()
                {
                    UnitType = (int)E_Identity.Hero,
                    GameUserId = mGamePlayer.InstanceId,
                    MapId = mGamePlayer.UnitData.Index,
                    X = mGamePlayer.UnitData.X,
                    Y = mGamePlayer.UnitData.Y,
                    Angle = mGamePlayer.UnitData.Angle,
                    IsNeedMove = 0,
                    Title = mGamePlayer.Data.Title,
                    WallTitle = mGamePlayer.Data.WallTile,
                    ReincarnateCnt = mGamePlayer.Data.ReincarnateCnt
                });
            }

            if (mapComponent.TryGetPosX(mGamePlayer.UnitData.X) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(508);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置数据异常x,不可行走!");
                b_Reply(b_Response);
                return false;
            }
            var mFindTheWaySource = mapComponent.GetFindTheWay2D(mGamePlayer);
            if (mFindTheWaySource == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(509);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置数据异常y,不可行走!");
                b_Reply(b_Response);
                return false;
            }
            if (mFindTheWaySource.IsStaticObstacle)
            {
                C_FindTheWay2D repairedCell = FindNearestWalkableCell(mapComponent, mGamePlayer.UnitData.X, mGamePlayer.UnitData.Y);
                if (repairedCell == null)
                {
                    ReplyWithCurrentPosition(Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(509));
                    b_Reply(b_Response);
                    return false;
                }

                mapComponent.MoveSendNotice(mFindTheWaySource, repairedCell, mGamePlayer, false);
                mFindTheWaySource = repairedCell;

                {
                    DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                    var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                    var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
                    mWriteDataComponent.Save(mGamePlayer.UnitData, dBProxy).Coroutine();
                }

                ReplyWithCurrentPosition(Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(509));
                b_Reply(b_Response);
                return false;
            }

            if (mapComponent.TryGetPosX(b_Request.X) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(510);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置数据异常x,不可行走!");
                b_Reply(b_Response);
                return false;
            }
            var mMapCellTarget = mapComponent.GetFindTheWay2D(b_Request.X, b_Request.Y);
            if (mMapCellTarget == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(511);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置数据异常y,不可行走!");
                b_Reply(b_Response);
                return false;
            }
            if (mMapCellTarget.IsStaticObstacle)
            {
                ReplyWithCurrentPosition(Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(511));
                b_Reply(b_Response);
                return false;
            }

            if (mGamePlayer.UnitData.Index == 1)
            {
                if (mMapCellTarget.IsSafeArea)
                {
                    DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();

                    DataCacheManageComponent mDataCacheManageComponent = mPlayer.AddCustomComponent<DataCacheManageComponent>();
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
                            var mMapCellField = mapComponent.GetMapCellField(mMapCellTarget);
                            if (mMapCellField == null)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2117);
                                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置区域数据异常x,不能攻击!");
                                b_Reply(b_Response);
                                return false;
                            }
                            if (mMapCellField.MapStallDic.TryGetValue(mData_Stall.GameUserId, out var mStall) && mStall.IsStalling == true)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2117);
                                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                                b_Reply(b_Response);
                                return false;
                            }
                        }
                    }
                }
            }

            if (mFindTheWaySource == null) mFindTheWaySource = mMapCellTarget;

            bool needTransfer = false;
            if (mMapCellTarget.TransferPoint == 0)
            {
                if (mGamePlayer.MoveIgnoreTransferId != 0)
                {
                    mGamePlayer.MoveIgnoreTransferId = 0;
                }
            }
            else
            {
                if (mGamePlayer.MoveIgnoreTransferId == mMapCellTarget.TransferPoint)
                {
                    // 忽略此次传送
                }
                else
                {
                    if (mapComponent.MapId == 10)
                    {
                        if (mPlayer.GetCustomComponent<EquipmentComponent>().GetEquipItemByPosition(EquipPosition.Wing) == null)
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2210);
                            b_Reply(b_Response);
                            return false;
                        }
                    }

                    // 执行此次传送
                    mGamePlayer.MoveIgnoreTransferId = mMapCellTarget.TransferPoint;

                    var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Map_TransferPointConfigJson>().JsonDic;
                    if (mJsonDic.TryGetValue(mMapCellTarget.TransferPoint, out var _TransferPointConfig) == false)
                    {
                        // 传送点不存在
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(512);
                        b_Reply(b_Response);
                        return false;
                    }

                    // 发布 GamePlayerEnterTransferPoint 事件
                    ETModel.EventType.GamePlayerEnterTransferPoint.Instance.triggerGamePlayer = mGamePlayer;
                    ETModel.EventType.GamePlayerEnterTransferPoint.Instance.transferPoint = mMapCellTarget.TransferPoint;
                    Root.EventSystem.OnRun("GamePlayerEnterTransferPoint", ETModel.EventType.GamePlayerEnterTransferPoint.Instance);


                    if (_TransferPointConfig.TargetIndex != 0)
                    {
                        if (mJsonDic.TryGetValue(_TransferPointConfig.TargetIndex, out var _TargetTransferPointConfig) == false)
                        {
                            // 传送点目标不存在
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(513);
                            b_Reply(b_Response);
                            return false;
                        }
                        if (mGamePlayer.Data.Level < _TargetTransferPointConfig.MapMinLevel)
                        {
                            // 传送地图进入等级不足
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(514);
                            b_Reply(b_Response);
                            return false;
                        }

                        //if (mGamePlayer.Data.GoldCoin < _TargetTransferPointConfig.MapCostGold)
                        //{
                        //    // 传送地图进入金币不足
                        //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(515);
                        //    b_Reply(b_Response);
                        //    return false;
                        //}

                        /*if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(_TargetTransferPointConfig.MapId, out var targetMapComponent) == false)
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(516);
                            //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                            b_Reply(b_Response);
                            return false;
                        }*/
                        MapComponent targetMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, _TargetTransferPointConfig.MapId, mPlayer.GameUserId);
                        if (targetMapComponent == null)
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(507);
                            //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                            b_Reply(b_Response);
                            return false;
                        }
                        if (targetMapComponent.TransferPointFindTheWayDic.TryGetValue(_TargetTransferPointConfig.Id, out var mTransferPointlist) == false)
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

                        mGamePlayer.MoveIgnoreTransferId = mTransferPoint.TransferPoint;

                        mMapCellTarget = mTransferPoint;

                        needTransfer = true;
                        Log.Debug().Log("传送点  目标 x:{0},y:{1} 现在:x:{2},y:{3} ",
                            b_Request.X,
                            b_Request.Y,
                            mMapCellTarget.X,
                            mMapCellTarget.Y);
                    }
                }
            }

            // 公告移动信息
            if(needTransfer)
            {
                mGamePlayer.SwitchMap(mMapCellTarget.Map, mMapCellTarget.X, mMapCellTarget.Y);
            }
            else
            {
                mGamePlayer.Move(mMapCellTarget.X, mMapCellTarget.Y);
            }
            mGamePlayer.UnitData.Angle = b_Request.Angle;

            {
                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();

                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
                mWriteDataComponent.Save(mGamePlayer.UnitData, dBProxy2).Coroutine();
            }

            b_Reply(b_Response);

            // 发布 GamePlayerEnterTransferPoint 事件
            ETModel.EventType.GamePlayerEnterTransferPoint.Instance.triggerGamePlayer = mGamePlayer;
            ETModel.EventType.GamePlayerEnterTransferPoint.Instance.transferPoint = mMapCellTarget.TransferPoint;
            Root.EventSystem.OnRun("GamePlayerEnterTransferPoint", ETModel.EventType.GamePlayerEnterTransferPoint.Instance);
            return true;
        }
    }
}
