using System;
using ETModel;
using CustomFrameWork;
using System.Threading.Tasks;
using CustomFrameWork.Component;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_EnterBattleCopyRequestHandler : AMActorRpcHandler<C2G_EnterBattleCopyRequest, G2C_EnterBattleCopyResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_EnterBattleCopyRequest b_Request, G2C_EnterBattleCopyResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }


        protected override async Task<bool> Run(C2G_EnterBattleCopyRequest b_Request, G2C_EnterBattleCopyResponse b_Response, Action<IMessage> b_Reply)
        {
            int type = b_Request.Type;
            if (type < 1 || type > 4)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2605);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("副本类型参数错误");
                b_Reply(b_Response);
                return false;
            }
            
            int level = b_Request.Level;
            if (type <= 3 && (level < 1 || level > 7))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2606);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("副本等级错误");
                b_Reply(b_Response);
                return false;
            }

            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家没有找到!");
                b_Reply(b_Response);
                return false;
            }

            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }

            BatteCopyManagerComponent batteCopyManagerCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>();
            if (batteCopyManagerCpt == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2600);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("没有找到副本管理组件");
                b_Reply(b_Response);
                return false;
            }
            BattleCopyComponent battleCopyCpt = batteCopyManagerCpt.Get(type);
            if (battleCopyCpt == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2601);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("没有找到副本组件");
                b_Reply(b_Response);
                return false;
            }
            if (battleCopyCpt.copyState != CopyState.Prepare && type != 4)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2607);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("只有在副本开启前五分钟可以进入广场");
                b_Reply(b_Response);
                return false;
            }
            
            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DBProxyComponent dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
            DataCacheManageComponent mDataCacheManageComponent = mPlayer.GetCustomComponent<DataCacheManageComponent>();

            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            //藏宝图逻辑
            if (type == 4)
            {
                if (b_Request.NPCID == 0)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2800);
                    b_Reply(b_Response);
                    return false;
                }
                //进入藏宝图
                if (battleCopyCpt.battleCopyRoomDic.TryGetValue(b_Request.NPCID, out var copyRoomList) == false || copyRoomList.Count == 0)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2801);
                    b_Reply(b_Response);
                    return false;
                }
                if(copyRoomList[0].IsJionState) 
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2607);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("只有在副本开启前五分钟可以进入广场");
                    b_Reply(b_Response);
                    return false;
                }
                GamePlayer gamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
                if (copyRoomList[0].BelongGameUserId != mPlayer.GameUserId)
                {
                    var Teaminfo = mPlayer.GetCustomComponent<TeamComponent>();
                    TeamManageComponent teamManager = Root.MainFactory.GetCustomComponent<TeamManageComponent>();
                    if (teamManager != null && Teaminfo != null)
                    {
                        var TeamPlayerList = teamManager.GetAllByTeamID(Teaminfo.TeamID);
                        if (TeamPlayerList.ContainsKey(b_Request.PlayerID))
                        {

                        }
                        else
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2802);
                            b_Reply(b_Response);
                            return false;
                        }
                    }
                    else
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2802);
                        b_Reply(b_Response);
                        return false;
                    }
                }
                else
                {
                    G2C_CangBaoTuOpen_notice g2C_CangBaoTuOpen_Notice = new G2C_CangBaoTuOpen_notice();
                    g2C_CangBaoTuOpen_Notice.PlayerName = gamePlayer.Data.NickName;
                    g2C_CangBaoTuOpen_Notice.PlayerID = mPlayer.GameUserId;
                    g2C_CangBaoTuOpen_Notice.NpcID = b_Request.NPCID;

                    var Teaminfo = mPlayer.GetCustomComponent<TeamComponent>();
                    TeamManageComponent teamManager = Root.MainFactory.GetCustomComponent<TeamManageComponent>();
                    if (teamManager != null && Teaminfo != null)
                    {
                        var TeamPlayerList = teamManager.GetAllByTeamID(Teaminfo.TeamID);
                        foreach ( var TeamPlayer in TeamPlayerList )
                        {
                            if (TeamPlayer.Value.GameUserId == mPlayer.GameUserId) continue;
                            TeamPlayer.Value.Send(g2C_CangBaoTuOpen_Notice);
                        }
                    }
                    copyRoomList[0].MobTime = new MobTime(mPlayer.SourceGameAreaId, b_Request.NPCID, copyRoomList[0].MobBossId);
                    copyRoomList[0].copyTime = new CopyTime(mPlayer.SourceGameAreaId, mPlayer.GameUserId, b_Request.NPCID);
                }
                var playerUnitData = gamePlayer.UnitData;
                MapComponent mapComponent_C = Help_MapHelper.GetMapByMapId(mServerArea, playerUnitData.Index, mPlayer.GameUserId);
                C_FindTheWay2D mFindTheWaySource_C = mapComponent_C.GetFindTheWay2D(playerUnitData.X, playerUnitData.Y);
                MapComponent cangBaoTuRoom = copyRoomList[0].mapComponent;

                if (cangBaoTuRoom.TransferPointFindTheWayDic.TryGetValue(10400, out var mTransferPointlist_C) == false)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(517);
                    b_Reply(b_Response);
                    return false;
                }
                var mRandomIndex_C = Help_RandomHelper.Range(0, mTransferPointlist_C.Count);
                var mTransferPoint_C = mTransferPointlist_C[mRandomIndex_C];
                cangBaoTuRoom.MoveSendNotice(mFindTheWaySource_C, mTransferPoint_C, gamePlayer);

                var mWriteDataComponent_C = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
                mWriteDataComponent_C.Save(playerUnitData, dBProxy).Coroutine();

                var knapsack = mPlayer.GetCustomComponent<BackpackComponent>();
                if (knapsack != null)
                {
                    Item item = knapsack.GetItemByUID(copyRoomList[0].BelongItemID);
                    if (item != null)
                        knapsack.UseItem(item,"成功进入藏宝图");
                    // 通知客户端，删除小地图icon
                    G2C_CangBaoTuPosClose_notice g2c_CangBaoTuPosClose_notice = new G2C_CangBaoTuPosClose_notice();
                    g2c_CangBaoTuPosClose_notice.Id = copyRoomList[0].BelongItemID;
                    mPlayer.Send(g2c_CangBaoTuPosClose_notice);
                }
                copyRoomList[0].JoinRoom(mPlayer.GameUserId, mGamePlayer.Data.NickName);
                CopyRankData copyRankData = new CopyRankData()
                {
                    Level = b_Request.NPCID,
                    Index = 0,
                };
                Log.PLog("RunCopyLog", $"PlayerId:{mPlayer.GameUserId}CopyId:{copyRoomList[0].Id}NpcId:{b_Request.NPCID}    藏宝图副本进入");
                if (battleCopyCpt.copyRankDataDic.ContainsKey(mPlayer.GameUserId))
                {
                    int Index1 = battleCopyCpt.copyRankDataDic[mPlayer.GameUserId].Index;
                    long Index2 = battleCopyCpt.copyRankDataDic[mPlayer.GameUserId].Level;
                    battleCopyCpt.battleCopyRoomDic[Index2]?[Index1]?.QuitRoom(mPlayer.GameUserId);
                    battleCopyCpt.copyRankDataDic.Remove(mPlayer.GameUserId);
                }
                if (battleCopyCpt.players.ContainsKey(mPlayer.GameUserId))
                    battleCopyCpt.players.Remove(mPlayer.GameUserId);


                battleCopyCpt.copyRankDataDic.Add(mPlayer.GameUserId, copyRankData);
                battleCopyCpt.players.Add(mPlayer.GameUserId, mPlayer);
                b_Reply(b_Response);
                return true;
            }

            var mBattleCopyData = mDataCacheManageComponent.Get<DBBattleCopyData>();
            if (mBattleCopyData == null)
            {
                mBattleCopyData = await mDataCacheManageComponent.Add<DBBattleCopyData>(dBProxy, p => p.GameUserId == mPlayer.GameUserId && p.GameAreaId == mPlayer.GameAreaId);
            }
            var mDatalist2 = mBattleCopyData.DataQuery(p => p.GameUserId == mPlayer.GameUserId && p.GameAreaId == mPlayer.GameAreaId);
            if (mDatalist2.Count == 0)
            {
                DBBattleCopyData bBattleCopyData = new DBBattleCopyData()
                {
                    Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId),
                    GameUserId = mPlayer.GameUserId,
                    GameAreaId = mPlayer.GameAreaId,
                    demonSquaeNum = batteCopyManagerCpt.demonSquaeNum,
                    redCastleNum = batteCopyManagerCpt.redCastleNum,
                    updateTime = Help_TimeHelper.GetCurrenTimeStamp(),
                };
                bool mSaveResult = await dBProxy.Save(bBattleCopyData);
                if (mSaveResult == false)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1513);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("保存数据失败!");
                    b_Reply(b_Response);
                    return false;
                }
                mBattleCopyData.DataAdd(bBattleCopyData);
            }

            mDatalist2 = mBattleCopyData.DataQuery(p => p.GameUserId == mPlayer.GameUserId && p.GameAreaId == mPlayer.GameAreaId);
            if (mDatalist2.Count <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1500);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏异常!");
                b_Reply(b_Response);
                return false;
            }
            DBBattleCopyData battleCopyData = mDatalist2[0];
            DateTime UpdateDate = Help_TimeHelper.ConvertStringToDateTime(battleCopyData.updateTime);
            DateTime CurrentDate = DateTime.Now;
            if (UpdateDate.Day != CurrentDate.Day)
            {
                battleCopyData.demonSquaeNum = batteCopyManagerCpt.demonSquaeNum;
                battleCopyData.redCastleNum = batteCopyManagerCpt.redCastleNum;
                battleCopyData.TrialTowerNum = 0;
                battleCopyData.updateTime = Help_TimeHelper.GetCurrenTimeStamp();
            }

            int residueNum = type == 1 ? battleCopyData.demonSquaeNum : battleCopyData.redCastleNum;
            if (residueNum == 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2608);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("今天的挑战次数已经用完");
                b_Reply(b_Response);
                return false;
            }

            if (!battleCopyCpt.DecisionPlayerLevel(type, level, mGamePlayer.Data))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2609);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage($"不能进入该等级的副本");
                b_Reply(b_Response);
                return false;
            }

            BackpackComponent backpackCpt = mPlayer.GetCustomComponent<BackpackComponent>();
            if (backpackCpt == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(704);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包组件异常!");
                b_Reply(b_Response);
                return false;
            }
            Item ticket = null;
            int goodId = battleCopyCpt.DecisionTicket(type);
            ticket = backpackCpt.GetLevelItemFromConfigID(goodId, level);//优先消耗合成卷
            if (ticket == null)
            {
                ticket = battleCopyCpt.GetDecisionTicket(backpackCpt, type);
                if (ticket == null)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2611);
                    b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("获取不到对应等级的恶魔广场通行证");
                    b_Reply(b_Response);
                    return false;
                }

            }

            DBPlayerUnitData mPlayerUnitData = mGamePlayer.UnitData;

            /*if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(mPlayerUnitData.Index, out var mapComponent) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(507);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                b_Reply(b_Response);
                return false;
            }*/
            MapComponent mapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mPlayerUnitData.Index, mPlayer.GameUserId);
            if (mapComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(416);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                b_Reply(b_Response);
                return false;
            }

            if (mapComponent.TryGetPosX(mPlayerUnitData.X) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(508);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置数据异常x,不可行走!");
                b_Reply(b_Response);
                return false;
            }
            var mFindTheWaySource = mapComponent.GetFindTheWay2D(mPlayerUnitData.X, mPlayerUnitData.Y);
            if (mFindTheWaySource == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(509);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置数据异常y,不可行走!");
                b_Reply(b_Response);
                return false;
            }

            int mapId = battleCopyCpt.DecisionTransferPointId(type);
            MapComponent mTargetMapComponent = battleCopyCpt.JoinRoom(level, mPlayer, mGamePlayer.Data.NickName, mapId, mServerArea);
            var PlayerShopInfo = mPlayer.GetCustomComponent<PlayerShopMallComponent>();
            if (PlayerShopInfo != null)
            {
                if (PlayerShopInfo.GetPlayerShopState(DeviationType.MaxMonthlyCard))
                {
                    mGamePlayer.CopyLiveCnt = 3;
                }
            }
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

            int mTransferPointId = mapId * 100;
            var mJsonMapDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Map_TransferPointConfigJson>().JsonDic;
            if (mJsonMapDic.TryGetValue(mTransferPointId, out var _TransferPointConfig) == false)
            {
                // 传送点不存在
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(513);
                b_Reply(b_Response);
                return false;
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

            //string str = $"传送前  x:{mPlayerUnitData.X},y:{mPlayerUnitData.Y}\n";

            //mPlayerUnitData.Index = _TransferPointConfig.MapId;
            //mPlayerUnitData.X = mTransferPoint.X;
            //mPlayerUnitData.Y = mTransferPoint.Y;
            //Log.Debug($"{str}传送后  x:{mPlayerUnitData.X},y:{mPlayerUnitData.Y}");

            // 公告移动信息
            mTargetMapComponent.MoveSendNotice(mFindTheWaySource, mTransferPoint, mGamePlayer);

            battleCopyCpt.DeductionChallengeNum(type, battleCopyData);

            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
            mWriteDataComponent.Save(mPlayerUnitData, dBProxy).Coroutine();
            mWriteDataComponent.Save(battleCopyData, dBProxy).Coroutine();
            backpackCpt.UseItem(ticket.ItemUID,"进入副本消耗");
            if (CopyType.DemonSquae == (CopyType)type)
            {
                // 发布 PlayerUseMapDelivery 事件
                ETModel.EventType.PlayerCompleteDemonCopy.Instance.player = mPlayer;
                ETModel.EventType.PlayerCompleteDemonCopy.Instance.transferPointId = mTransferPointId;
                Root.EventSystem.OnRun("GamePlayerCompleteDemonCopy", ETModel.EventType.PlayerCompleteDemonCopy.Instance);
            }
            else if (CopyType.RedCastle == (CopyType)type)
            {
                // 发布 PlayerUseMapDelivery 事件
                ETModel.EventType.PlayerCompleteRedCopy.Instance.player = mPlayer;
                ETModel.EventType.PlayerCompleteRedCopy.Instance.transferPointId = mTransferPointId;
                Root.EventSystem.OnRun("GamePlayerCompleteRedCopy", ETModel.EventType.PlayerCompleteRedCopy.Instance);
            }
            b_Reply(b_Response);
            return true;
        }

    }
}
