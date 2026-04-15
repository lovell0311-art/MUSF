using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using System.Drawing;
using MongoDB.Bson;

namespace ETHotfix
{
    [FriendOf(typeof(DBPlayerUnitData))]
    [MessageHandler(AppType.Game)]
    public class C2G_ReadyRequestHandler : AMActorRpcHandler<C2G_ReadyRequest, G2C_ReadyResponse>
    {
        private static bool IsValidEnterCell(C_FindTheWay2D cell)
        {
            return cell != null && cell.IsStaticObstacle == false;
        }

        private static C_FindTheWay2D FindNearestWalkableCell(MapComponent mapComponent, int posX, int posY, int maxRadius = 24)
        {
            if (mapComponent == null)
            {
                return null;
            }

            int centerX = Math.Max(0, Math.Min(mapComponent.MapWidth - 1, posX));
            int centerY = Math.Max(0, Math.Min(mapComponent.MapHight - 1, posY));

            C_FindTheWay2D centerCell = mapComponent.GetFindTheWay2D(centerX, centerY);
            if (IsValidEnterCell(centerCell))
            {
                return centerCell;
            }

            for (int radius = 1; radius <= maxRadius; ++radius)
            {
                C_FindTheWay2D bestCell = null;
                int bestDistance = int.MaxValue;

                for (int dx = -radius; dx <= radius; ++dx)
                {
                    for (int dy = -radius; dy <= radius; ++dy)
                    {
                        if (Math.Abs(dx) != radius && Math.Abs(dy) != radius)
                        {
                            continue;
                        }

                        int x = centerX + dx;
                        int y = centerY + dy;
                        C_FindTheWay2D candidate = mapComponent.GetFindTheWay2D(x, y);
                        if (IsValidEnterCell(candidate) == false)
                        {
                            continue;
                        }

                        int distance = dx * dx + dy * dy;
                        if (distance < bestDistance)
                        {
                            bestDistance = distance;
                            bestCell = candidate;
                        }
                    }
                }

                if (bestCell != null)
                {
                    return bestCell;
                }
            }

            return null;
        }

        private static C_FindTheWay2D GetRandomSpawnSafeCell(MapComponent mapComponent)
        {
            if (mapComponent == null || mapComponent.SpawnSafeFindTheWayDic.Count == 0)
            {
                return null;
            }

            var safeAreaValues = mapComponent.SpawnSafeFindTheWayDic.Values.ToArray();
            if (safeAreaValues.Length == 0)
            {
                return null;
            }

            int randomIndex = Help_RandomHelper.Range(0, safeAreaValues.Length);
            var safeAreaDic = safeAreaValues[randomIndex];
            if (safeAreaDic == null || safeAreaDic.Count == 0)
            {
                return null;
            }

            var safeAreaKeys = safeAreaDic.Keys.ToArray();
            int randomKeyIndex = Help_RandomHelper.Range(0, safeAreaKeys.Length);
            int randomKey = safeAreaKeys[randomKeyIndex];
            return safeAreaDic[randomKey];
        }

        private static bool TryResolveEnterCell(
            MapComponent mapComponent,
            int sourcePosX,
            int sourcePosY,
            out C_FindTheWay2D resultCell,
            out string repairReason)
        {
            resultCell = null;
            repairReason = string.Empty;

            if (mapComponent != null &&
                mapComponent.TryGetPosX(sourcePosX) &&
                mapComponent.TryGetPosY(sourcePosY))
            {
                C_FindTheWay2D sourceCell = mapComponent.GetFindTheWay2D(sourcePosX, sourcePosY);
                if (IsValidEnterCell(sourceCell))
                {
                    resultCell = sourceCell;
                    return true;
                }
            }

            C_FindTheWay2D repairedCell = FindNearestWalkableCell(mapComponent, sourcePosX, sourcePosY);
            if (repairedCell != null)
            {
                resultCell = repairedCell;
                repairReason = $"nearby({sourcePosX},{sourcePosY})->({repairedCell.X},{repairedCell.Y})";
                return true;
            }

            repairedCell = GetRandomSpawnSafeCell(mapComponent);
            if (repairedCell != null)
            {
                resultCell = repairedCell;
                repairReason = $"spawnSafe({sourcePosX},{sourcePosY})->({repairedCell.X},{repairedCell.Y})";
                return true;
            }

            return false;
        }

        protected override async Task<bool> BeforeCodeAsync(Session session, C2G_ReadyRequest b_Request, G2C_ReadyResponse b_Response, Action<IMessage> b_Reply)
        {
            using CoroutineLock actorLock = await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId);

            int mAreaId = (int)(b_Request.AppendData >> 16);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().UnsafeGet(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
                b_Reply(b_Response);
                return false;
            }

            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGame, mPlayer.UserId))
            {
                return await base.BeforeCodeAsync(session, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_ReadyRequest b_Request, G2C_ReadyResponse b_Response, Action<IMessage> b_Reply)
        {
            long startTime = Help_TimeHelper.GetNow();

            int mAreaId = (int)(b_Request.AppendData >> 16);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().UnsafeGet(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
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
            if (mPlayer.OnlineStatus != EOnlineStatus.StartGame)
            {
                // 重复请求 C2G_ReadyRequest
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(105);
                b_Reply(b_Response);
                return false;
            }
            mPlayer.OnlineStatus = EOnlineStatus.Ready;
            Log.PLog("Login", $"a:{mPlayer.UserId} r:{mPlayer.GameUserId} 玩家角色开始读取数据");
            {
                // 添加Player必备组件
                if (mPlayer.GetCustomComponent<ClientTimeComponent>() == null)
                {
                    mPlayer.ClientTime = mPlayer.AddCustomComponent<ClientTimeComponent>();
                }
                if (mPlayer.GetCustomComponent<HackerComponent>() == null)
                {
                    mPlayer.AddCustomComponent<HackerComponent>();
                }
            }

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DataCacheManageComponent mDataCacheComponent = mPlayer.AddCustomComponent<DataCacheManageComponent>();

            // 初始化玩家移动信息
            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            DBPlayerUnitData mPlayerUnitData = mGamePlayer.UnitData;
            int enterMapId = mPlayerUnitData.Index;
            int enterPosX = mPlayerUnitData.X;
            int enterPosY = mPlayerUnitData.Y;

            { // 角色技能数据
                await mGamePlayer.DataUpdateSkill(false);
            }

            // 初始化大师技能需要
            if (!ConstServer.PlayerMaster)
            {
                if (mGamePlayer.Data.Level >= 400 && mGamePlayer.Data.OccupationLevel >= 3)
                {
                    await mGamePlayer.DataUpdateMaster();
                }
            }
            else
            {
                await mGamePlayer.DataUpdateMaster();
            }

            /*if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(mPlayerUnitData.Index, out var mapComponent) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1503);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常");
                b_Reply(b_Response);
                return false;
            }*/
            if (enterMapId == ConstMapId.GuZhanChang || enterMapId == 112)
            {
                enterMapId = 4;//冰风谷初始位置
                enterPosX = 322;
                enterPosY = 324;
            }
            var mTransferPointId = enterMapId;
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
                if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(mTransferPointId, out mapComponent) == false)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1503);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                    b_Reply(b_Response);
                    return false;
                }
                List<int> MapId = new List<int>() { 103, 104, 105, 106, 107, 108, 109 };
                if (MapId.Contains(mMapConfig.Id))
                {
                    mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(4, out mapComponent);
                    enterMapId = 4;//冰风谷初始位置
                    enterPosX = 322;
                    enterPosY = 324;
                }
            }
            else
            {
                if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(4, out mapComponent))
                {
                    enterMapId = 4;//冰风谷初始位置
                    enterPosX = 322;
                    enterPosY = 324;
                }
                else
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1503);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                    b_Reply(b_Response);
                    return false;
                }
            }
            int originalEnterPosX = enterPosX;
            int originalEnterPosY = enterPosY;
            if (TryResolveEnterCell(mapComponent, enterPosX, enterPosY, out var mMapCellTarget, out string enterRepairReason) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1505);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不可行走!Y");
                b_Reply(b_Response);
                return false;
            }
            enterPosX = mMapCellTarget.X;
            enterPosY = mMapCellTarget.Y;

            if (originalEnterPosX != enterPosX || originalEnterPosY != enterPosY)
            {
                mPlayerUnitData.index = enterMapId;
                mPlayerUnitData.x = enterPosX;
                mPlayerUnitData.y = enterPosY;

                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
                mWriteDataComponent.Save(mPlayerUnitData, dBProxy).Coroutine();

                Log.PLog("Login", $"a:{mPlayer.UserId} r:{mPlayer.GameUserId} 登录落点纠偏 map:{enterMapId} {enterRepairReason}");
            }

            {// 勇者大陆安全区
                if (enterMapId == 1 && mMapCellTarget.IsSafeArea)
                {
                    var mDataCache_Stall = mDataCacheComponent.Get<DBStallItem>();
                    if (mDataCache_Stall == null)
                    {
                        var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                        mDataCache_Stall = await HelpDb_DBStallItem.Init(mPlayer, mDataCacheComponent, dBProxy2);
                    }
                    var mDBStallItem = mDataCache_Stall.OnlyOne();
                    if (mDBStallItem != null)
                    {
                        if (mDBStallItem.IsDispose == 1)
                        {
                            mDBStallItem.IsDispose = 0;
                            if (mDBStallItem.StallItemlist == null) mDBStallItem.DeSerialize();

                            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
                            mWriteDataComponent.Save(mDBStallItem, dBProxy).Coroutine();

                            var mMapCellField = mapComponent.GetMapCellField(mMapCellTarget);
                            if (mMapCellField == null)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(424);
                                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置区域数据异常y,不能攻击!");
                                b_Reply(b_Response);
                                return false;
                            }
                            if (mMapCellField.MapStallDic.TryGetValue(mDBStallItem.GameUserId, out var mStall) == false)
                            {
                                mStall = mMapCellField.MapStallDic[mDBStallItem.GameUserId] = Root.CreateBuilder.GetInstance<C_Stall, long>(mDBStallItem.GameUserId);
                            }
                            mStall.IsStalling = false;
                        }
                    }
                }
            }

            #region 账户区信息加载
            C_DataCache<DBAccountZoneData> dataCacheAccountData = mDataCacheComponent.Get<DBAccountZoneData>();
            if (dataCacheAccountData == null)
            {
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                dataCacheAccountData = await mDataCacheComponent.Add<DBAccountZoneData>(dBProxy2, p => p.Id == mPlayer.UserId);
            }
            DBAccountZoneData dbAccountZoneData = dataCacheAccountData.DataQueryById(mPlayer.UserId);
            if (dbAccountZoneData == null)
            {
                // 创建新存档
                dbAccountZoneData = new DBAccountZoneData()
                {
                    Id = mPlayer.UserId,
                    NewAccount = 1,
                };
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                bool mSaveResult = await dBProxy.Save(dbAccountZoneData);
                if (mSaveResult == false)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1501);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("保存失败!");
                    b_Reply(b_Response);
                    return false;
                }
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                await dataCacheAccountData.DataQueryInit(dBProxy2, p => p.Id == mPlayer.UserId);
                dbAccountZoneData = dataCacheAccountData.DataQueryById(mPlayer.UserId);
                if (dbAccountZoneData == null)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1502);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏异常2!");
                    b_Reply(b_Response);
                    return false;
                }
            }
            dbAccountZoneData.DeSerialize();
            mPlayer.SetData(dbAccountZoneData);
            #endregion

            //初始化物品数据库缓存
            var itemDataCache = mDataCacheComponent.Get<DBItemData>();
            if (itemDataCache == null)
            {
                Log.Debug("====================== 更新物品数据库缓存");
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                itemDataCache = await mDataCacheComponent.Add<DBItemData>(dBProxy2, p => p.GameUserId == mPlayer.GameUserId && p.InComponent == EItemInComponent.None && p.IsDispose == 0);
                await mDataCacheComponent.Append<DBItemData>(dBProxy2, p => p.GameUserId == mPlayer.GameUserId && p.InComponent == EItemInComponent.Stall && p.IsDispose == 0);
                // 丢失的物品，等数据加载完成后，通过邮件还给玩家
                await mDataCacheComponent.Append<DBItemData>(dBProxy2, p => p.GameUserId == mPlayer.GameUserId && p.InComponent == EItemInComponent.Lost && p.IsDispose == 0);
            }

            using ListComponent<Task<bool>> tasks = ListComponent<Task<bool>>.Create();

            async Task<bool> LoadBackpackComponent()
            {
                //添加背包组件
                BackpackComponent mBackpack = mPlayer.AddCustomComponent<BackpackComponent>();
                if ((await mBackpack.Init()) == false)
                {
                    // 背包存档损坏，无法加载
                    // 需要将玩家踢下线，防止更严重的错误
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1508);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包存档异常，请联系游戏客服");
                    Log.Error($"'BackpackComponent'数据损坏，mPlayer.USerId={mPlayer.UserId},mPlayer.GameUserId={mPlayer.GameUserId}");
                    return false;
                }
                return true;
            }

            async Task<bool> LoadWarehouseComponent()
            {
                //添加仓库组件
                WarehouseComponent mWarehouse = mPlayer.AddCustomComponent<WarehouseComponent>();
                if ((await mWarehouse.Init()) == false)
                {
                    // 仓库存档损坏，无法加载
                    // 需要将玩家踢下线，防止更严重的错误
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1903);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("仓库存档异常，请联系游戏客服");
                    Log.Error($"'WarehouseComponent'数据损坏，mPlayer.USerId={mPlayer.UserId},mPlayer.GameUserId={mPlayer.GameUserId}");
                    return false;
                }
                return true;
            }

            async Task<bool> LoadEquipmentComponent()
            {
                // 添加套装组件
                if (mPlayer.GetCustomComponent<EquipmentSetComponent>() == null)
                {
                    mPlayer.AddCustomComponent<EquipmentSetComponent>();
                }
                //添加装备组件
                EquipmentComponent mEquipComponent = mPlayer.AddCustomComponent<EquipmentComponent>();
                if ((await mEquipComponent.Init()) == false)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1511);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("装备组件数据异常!当前拥有超过1个组件");
                    Log.Error($"'EquipmentComponent'数据损坏，mPlayer.USerId={mPlayer.UserId},mPlayer.GameUserId={mPlayer.GameUserId}");
                    return false;
                }
                //宠物数据
                if ((await mPlayer.GetCustomComponent<GamePlayer>().DataUpdatePets()) == false)
                {

                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1513);
                    Log.Error($"宠物数据损坏，mPlayer.USerId={mPlayer.UserId},mPlayer.GameUserId={mPlayer.GameUserId}");
                    return false;
                }

                mEquipComponent.ApplyEquipProp(true);
                return true;
            }

            async Task<bool> LoadGameTasksComponent()
            {
                var gameTasksComponent = mPlayer.AddCustomComponent<GameTasksComponent>();
                if (await gameTasksComponent.Init() != true)
                {
                    // 任务组件异常
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1515);
                    Log.Error($"'GameTasksComponent'数据损坏，mPlayer.USerId={mPlayer.UserId},mPlayer.GameUserId={mPlayer.GameUserId}");
                    return false;
                }
                return true;
            }

            void NotifyDeferredReadyData()
            {
                mGamePlayer.NotifyOpenSkillGroup();

                BackpackComponent backpackComponent = mPlayer.GetCustomComponent<BackpackComponent>();
                backpackComponent?.NotifyAllItem();

                WarehouseComponent warehouseComponent = mPlayer.GetCustomComponent<WarehouseComponent>();
                if (warehouseComponent != null)
                {
                    mPlayer.Send(new G2C_WarehouseExtension_notice()
                    {
                        Capacity = warehouseComponent.Warehouse_DB.Capacity
                    });
                    warehouseComponent.NotifyAllItem();
                }

                EquipmentComponent equipmentComponent = mPlayer.GetCustomComponent<EquipmentComponent>();
                equipmentComponent?.NotifyAllEquip();

                GameTasksComponent gameTasksComponent = mPlayer.GetCustomComponent<GameTasksComponent>();
                if (gameTasksComponent != null)
                {
                    gameTasksComponent.NotifyAllTask();
                    using var alltasks = ListComponent<GameTask>.Create();
                    List<GameTask> taskList = alltasks;
                    gameTasksComponent.GetTasksWith(ref taskList, p => { return p.StartTask == 1; });
                    foreach (var task in taskList)
                    {
                        task.StartTask = 0;
                    }
                    if (alltasks.Count != 0)
                    {
                        gameTasksComponent.SaveDB();
                    }
                }
            }

            async Task<bool> LoadFriendComponent()
            {
                //初始化好友
                if (mPlayer.GetCustomComponent<FriendComponent>() == null)
                {
                    if ((await mPlayer.AddCustomComponent<FriendComponent>().Init(mAreaId)) == false)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1512);
                        Log.Error($"'FriendComponent'数据损坏，mPlayer.USerId={mPlayer.UserId},mPlayer.GameUserId={mPlayer.GameUserId}");
                        return false;
                    }
                }
                return true;
            }

            async Task<bool> LoadPlayerWarAllianceComponent()
            {
                //初始化战盟
                if (mPlayer.GetCustomComponent<PlayerWarAllianceComponent>() == null)
                {
                    if ((await mPlayer.AddCustomComponent<PlayerWarAllianceComponent>().OnInit(mAreaId)) == false)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1513);
                        Log.Error($"'PlayerWarAllianceComponent'数据损坏，mPlayer.USerId={mPlayer.UserId},mPlayer.GameUserId={mPlayer.GameUserId}");
                        return false;
                    }
                }
                return true;
            }

            async Task<bool> LoadPlayerMailComponent()
            {
                //初始化邮件
                if (mPlayer.GetCustomComponent<PlayerMailComponent>() == null)
                {
                    if ((await mPlayer.AddCustomComponent<PlayerMailComponent>().PlayerLoadMail(mAreaId)) == false)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1513);
                        Log.Error($"'PlayerMailComponent'数据损坏，mPlayer.USerId={mPlayer.UserId},mPlayer.GameUserId={mPlayer.GameUserId}");
                        return false;
                    }
                }
                return true;
            }

            async Task<bool> LoadPlayerShopMallComponent()
            {
                //初始化充值信息
                if (mPlayer.GetCustomComponent<PlayerShopMallComponent>() == null)
                {
                    if ((await mPlayer.AddCustomComponent<PlayerShopMallComponent>().PlayerLoadShopMall()) == false)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1513);
                        Log.Error($"'PlayerShopMallComponent'数据损坏，mPlayer.USerId={mPlayer.UserId},mPlayer.GameUserId={mPlayer.GameUserId}");
                        return false;
                    }
                }
                return true;
            }

            async Task<bool> LoadPlayerActivitComponent()
            {
                //初始化活动信息
                if (mPlayer.GetCustomComponent<PlayerActivitComponent>() == null)
                {

                    if ((await mPlayer.AddCustomComponent<PlayerActivitComponent>().PlayerLoadActivityInfo()) == false)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1513);
                        Log.Error($"'PlayerActivitComponent'数据损坏，mPlayer.USerId={mPlayer.UserId},mPlayer.GameUserId={mPlayer.GameUserId}");
                        return false;
                    }
                }
                return true;
            }

            async Task<bool> LoadPlayerTileComponent()
            {
                //初始化称号信息
                if (mPlayer.GetCustomComponent<PlayerTitle>() == null)
                {

                    if ((await mPlayer.AddCustomComponent<PlayerTitle>().Init(mAreaId)) == false)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1513);
                        Log.Error($"'PlayerTitleComponent'数据损坏，mPlayer.USerId={mPlayer.UserId},mPlayer.GameUserId={mPlayer.GameUserId}");
                        return false;
                    }
                }
                return true;
            }

            async Task<bool> LoadPlayerTreasureHouseInfo()
            {
                //初始化称号信息
                if (mPlayer.GetCustomComponent<TreasureHouseComponent>() == null)
                {

                    if (!await mPlayer.AddCustomComponent<TreasureHouseComponent>().LoadPlayerTreasureHouseInfo())
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1513);
                        Log.Error($"'PlayerTitleComponent'数据损坏，mPlayer.USerId={mPlayer.UserId},mPlayer.GameUserId={mPlayer.GameUserId}");
                        return false;
                    }
                }
                return true;
            }

            async Task<bool> LoadCumulativeRechargeComponent()
            {
                // 统计充值金额
                if (mPlayer.GetCustomComponent<CumulativeRechargeComponent>() == null)
                {
                    if ((await mPlayer.AddCustomComponent<CumulativeRechargeComponent>().Load()) == false)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1513);
                        Log.Error($"'CumulativeRechargeComponent'数据损坏，mPlayer.USerId={mPlayer.UserId},mPlayer.GameUserId={mPlayer.GameUserId}");
                        return false;
                    }
                }
                return true;
            }

            async Task<bool> LoadLotteryComponent()
            {
                // 抽奖数据加载
                if (mPlayer.GetCustomComponent<LotteryComponent>() == null)
                {
                    if ((await mPlayer.AddCustomComponent<LotteryComponent>().Init()) == false)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1513);
                        Log.Error($"'LotteryComponent'数据损坏，mPlayer.USerId={mPlayer.UserId},mPlayer.GameUserId={mPlayer.GameUserId}");
                        return false;
                    }
                }
                return true;
            }

            async Task<bool> LoadPlayerOnlineRewardComponent()
            {
                // 在线时间加载
                if (mPlayer.GetCustomComponent<PlayerOnlineRewardComponent>() == null)
                {
                    if ((await mPlayer.AddCustomComponent<PlayerOnlineRewardComponent>().LoadOnlineReward()) == false)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1513);
                        Log.Error($"'LotteryComponent'数据损坏，mPlayer.USerId={mPlayer.UserId},mPlayer.GameUserId={mPlayer.GameUserId}");
                        return false;
                    }
                }
                return true;
            }
            async Task<bool> LoadBlood()
            {
                if (mPlayer.GetCustomComponent<PlayerBloodAwakeningComponent>() == null)
                {
                    if ((await mPlayer.AddCustomComponent<PlayerBloodAwakeningComponent>().LoadPalyerBloodAwakening()) == false)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1513);
                        Log.Error($"'PlayerBloodAwakeningComponent'数据损坏，mPlayer.USerId={mPlayer.UserId},mPlayer.GameUserId={mPlayer.GameUserId}");
                        return false;
                    }
                }
                return true;
            }
            tasks.Add(LoadBlood());
            tasks.Add(LoadBackpackComponent());
            tasks.Add(LoadWarehouseComponent());
            tasks.Add(LoadEquipmentComponent());
            tasks.Add(LoadGameTasksComponent());
            tasks.Add(LoadFriendComponent());
            tasks.Add(LoadPlayerWarAllianceComponent());
            tasks.Add(LoadPlayerMailComponent());
            tasks.Add(LoadPlayerShopMallComponent());
            tasks.Add(LoadPlayerActivitComponent());
            tasks.Add(LoadPlayerTileComponent());
            tasks.Add(LoadPlayerTreasureHouseInfo());
            tasks.Add(LoadCumulativeRechargeComponent());
            tasks.Add(LoadLotteryComponent());
            //tasks.Add(LoadPlayerOnlineRewardComponent());//屏蔽在线奖励
            List<bool> result = await TaskHelper.WaitAll(tasks);
            foreach (bool ret in result)
            {
                if (ret == false)
                {
                    // 加载失败
                    b_Reply(b_Response);
                    return true;
                }
            }
            var ServerInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>();
            var info = ServerInfo?.GetStartUpInfo(OptionComponent.Options.AppType, OptionComponent.Options.AppId);
            if (info.IsPVP == 1)
            {
                mGamePlayer._PKModel = E_PKModel.Friend;
            }
            else if (info.IsPVP == 2)
            {
                mGamePlayer._PKModel = E_PKModel.Peace;
            }
            // 添加交易组件
            if (mPlayer.GetCustomComponent<ExchangeComponent>() == null)
            {
                mPlayer.AddCustomComponent<ExchangeComponent>();
            }


            // 添加合成缓存空间
            if (mPlayer.GetCustomComponent<SynthesisComponent>() == null)
            {
                mPlayer.AddCustomComponent<SynthesisComponent>().Init();
            }

            var Date = mPlayer.GetCustomComponent<GamePlayer>().Data;

            b_Reply(b_Response);

            long instanceId = mPlayer.Id;

            async Task NoticePlayerBattleCopyStatus()
            {
                await Root.MainFactory.GetCustomComponent<TimerComponent>().WaitAsync(5000);
                if (mPlayer.Id != instanceId)
                {
                    // 玩家已经下线
                    return;
                }
                if (mPlayer.OnlineStatus != EOnlineStatus.Online &&
                    mPlayer.OnlineStatus != EOnlineStatus.Ready)
                {
                    // 玩家已下线
                    return;
                }
                //推送副本状态
                BatteCopyManagerComponent batteCopyManagerCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>();
                if (batteCopyManagerCpt == null)
                {
                    return;
                }
                batteCopyManagerCpt.BroadcastPlayerBattleCopy(mPlayer.GameUserId);
            }
            mPlayer.OnlineStatus = EOnlineStatus.Online;

            long playerInstanceId = mPlayer.Id;
            SendReadyTransitionAsync().Coroutine();
            FinishPlayerReadyAsync().Coroutine();
            return true;

            async Task SendReadyTransitionAsync()
            {
                TimerComponent timerComponent = Root.MainFactory.GetCustomComponent<TimerComponent>();
                for (int attempt = 1; attempt <= 4; ++attempt)
                {
                    await timerComponent.WaitAsync(attempt == 1 ? 120 : 320);
                    if (mPlayer == null || mPlayer.IsDisposeable || mPlayer.Id != playerInstanceId)
                    {
                        return;
                    }
                    if (mPlayer.OnlineStatus != EOnlineStatus.Online &&
                        mPlayer.OnlineStatus != EOnlineStatus.Ready)
                    {
                        return;
                    }
                    if (mGamePlayer.GetCustomComponent<BuffWuDiForEnterMap>() == null)
                    {
                        Log.Info($"#RoleFlow# Ready transition stop-acked user={mPlayer.UserId} role={mPlayer.GameUserId} attempt={attempt}");
                        return;
                    }

                    // 让 ReadyResponse 先回到客户端，再补发本地进图所需的 MovePos / LoadingComplete。
                    // 这里做有限次重发，避免首次关键包偶发丢失后客户端一直卡在选角场景。
                    mPlayer.Send(new G2C_MovePos_notice()
                    {
                        GameUserId = mPlayer.GameUserId,
                        MapId = enterMapId,
                        X = enterPosX,
                        Y = enterPosY,
                        SourceX = enterPosX,
                        SourceY = enterPosY,
                        Angle = mPlayerUnitData.Angle,
                        ViewId = 1,
                        UnitType = (int)E_Identity.Hero,
                        NickName = Date.NickName,
                        ModelId = Date.PlayerTypeId,
                        OccupationLevel = Date.OccupationLevel,
                        HpValue = mGamePlayer.GetNumerialFunc(E_GameProperty.PROP_HP),
                        HpMaxValue = mGamePlayer.GetNumerialFunc(E_GameProperty.PROP_HP_MAX),
                        PkNumber = mGamePlayer.UnitData.PkPoint,
                        IsNeedMove = 0,
                        Title = Date.Title,
                        WallTitle = Date.WallTile,
                        ReincarnateCnt = Date.ReincarnateCnt,
                    });
                    Log.Info($"#RoleFlow# Ready self-movepos sent user={mPlayer.UserId} role={mPlayer.GameUserId} map={enterMapId} pos={enterPosX},{enterPosY} attempt={attempt}");

                    await timerComponent.WaitAsync(120);
                    if (mPlayer == null || mPlayer.IsDisposeable || mPlayer.Id != playerInstanceId)
                    {
                        return;
                    }
                    if (mPlayer.OnlineStatus != EOnlineStatus.Online &&
                        mPlayer.OnlineStatus != EOnlineStatus.Ready)
                    {
                        return;
                    }
                    if (mGamePlayer.GetCustomComponent<BuffWuDiForEnterMap>() == null)
                    {
                        Log.Info($"#RoleFlow# Ready loading-complete skip-acked user={mPlayer.UserId} role={mPlayer.GameUserId} attempt={attempt}");
                        return;
                    }

                    mPlayer.Send(new G2C_LoadingComplete());
                    Log.Info($"#RoleFlow# Ready loading-complete sent user={mPlayer.UserId} role={mPlayer.GameUserId} attempt={attempt}");
                }
            }

            async Task FinishPlayerReadyAsync()
            {
                try
                {
                    TimerComponent timerComponent = Root.MainFactory.GetCustomComponent<TimerComponent>();

                    for (int waitIndex = 0; waitIndex < 18; ++waitIndex)
                    {
                        if (mPlayer == null || mPlayer.IsDisposeable || mPlayer.Id != playerInstanceId)
                        {
                            return;
                        }

                        if (mGamePlayer.GetCustomComponent<BuffWuDiForEnterMap>() == null)
                        {
                            Log.Info($"#RoleFlow# Ready deferred-bootstrap acked user={mPlayer.UserId} role={mPlayer.GameUserId} waitIndex={waitIndex}");
                            break;
                        }

                        await timerComponent.WaitAsync(180);
                    }

                    if (mPlayer == null || mPlayer.IsDisposeable || mPlayer.Id != playerInstanceId)
                    {
                        return;
                    }

                    Log.Info($"#RoleFlow# Ready deferred-bootstrap start user={mPlayer.UserId} role={mPlayer.GameUserId}");
                    NotifyDeferredReadyData();

                    #region 初始化属性通知
                    {  // 经验通知
                        void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                        {
                            G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                            mBattleKVData.Key = (int)b_GameProperty;
                            mBattleKVData.Value = mGamePlayer.GetNumerial(b_GameProperty);
                            b_ChangeValue_notice.Info.Add(mBattleKVData);
                        }

                        G2C_ChangeValue_notice mChangeValue = new G2C_ChangeValue_notice();
                        mChangeValue.GameUserId = mGamePlayer.InstanceId;

                        AddPropertyNotice(mChangeValue, E_GameProperty.PROP_HP);
                        AddPropertyNotice(mChangeValue, E_GameProperty.PROP_MP);
                        AddPropertyNotice(mChangeValue, E_GameProperty.PROP_SD);
                        AddPropertyNotice(mChangeValue, E_GameProperty.PROP_AG);

                        AddPropertyNotice(mChangeValue, E_GameProperty.PROP_HP_MAX);
                        AddPropertyNotice(mChangeValue, E_GameProperty.PROP_MP_MAX);
                        AddPropertyNotice(mChangeValue, E_GameProperty.PROP_SD_MAX);
                        AddPropertyNotice(mChangeValue, E_GameProperty.PROP_AG_MAX);

                        // 力量、智力、敏捷、体力、统帅
                        AddPropertyNotice(mChangeValue, E_GameProperty.Property_Strength);
                        AddPropertyNotice(mChangeValue, E_GameProperty.Property_Willpower);
                        AddPropertyNotice(mChangeValue, E_GameProperty.Property_Agility);
                        AddPropertyNotice(mChangeValue, E_GameProperty.Property_BoneGas);
                        switch ((E_GameOccupation)mGamePlayer.Data.PlayerTypeId)
                        {
                            case E_GameOccupation.Holyteacher:
                                AddPropertyNotice(mChangeValue, E_GameProperty.Property_Command);
                                break;
                            default:
                                break;
                        }

                        G2C_BattleKVData mChildChangeValue = new G2C_BattleKVData();
                        mChildChangeValue.Key = (int)E_GameProperty.ServerTime;
                        mChildChangeValue.Value = Help_TimeHelper.GetNow();
                        mChangeValue.Info.Add(mChildChangeValue);

                        mChildChangeValue = new G2C_BattleKVData();
                        mChildChangeValue.Key = (int)E_GameProperty.Exprience;
                        mChildChangeValue.Value = mGamePlayer.Data.Exp;
                        mChangeValue.Info.Add(mChildChangeValue);
                        // 等级通知
                        mChildChangeValue = new G2C_BattleKVData();
                        mChildChangeValue.Key = (int)E_GameProperty.Level;
                        mChildChangeValue.Value = mGamePlayer.Data.Level;
                        mChangeValue.Info.Add(mChildChangeValue);
                        // 职业等级通知
                        mChildChangeValue = new G2C_BattleKVData();
                        mChildChangeValue.Key = (int)E_GameProperty.OccupationLevel;
                        mChildChangeValue.Value = mGamePlayer.Data.OccupationLevel;
                        mChangeValue.Info.Add(mChildChangeValue);

                        // 职业等级通知
                        mChildChangeValue = new G2C_BattleKVData();
                        mChildChangeValue.Key = (int)E_GameProperty.PlayerKillingMedel;
                        mChildChangeValue.Value = (int)mGamePlayer.PKModel();
                        mChangeValue.Info.Add(mChildChangeValue);

                        mChildChangeValue = new G2C_BattleKVData();
                        mChildChangeValue.Key = (int)E_GameProperty.PkNumber;
                        mChildChangeValue.Value = mGamePlayer.UnitData.PkPoint;
                        mChangeValue.Info.Add(mChildChangeValue);

                        AddPropertyNotice(mChangeValue, E_GameProperty.FreePoint);

                        mPlayer.Send(mChangeValue);
                    }
                    #endregion

                    // 加载存档，不能在新协程中加载
                    await mGamePlayer.LoadingLimitation();

                    if (mPlayer == null || mPlayer.IsDisposeable || mPlayer.Id != playerInstanceId)
                    {
                        return;
                    }

                    //推送玩家背包金币和仓库金币数
                    //推送给玩家
#pragma warning disable CS0618
                    mPlayer.Send(new G2C_WarehouseGoldChange_notice()
                    {
                        GameUserId = mPlayer.GameUserId,
                        Gold = mPlayer.GetCustomComponent<WarehouseComponent>().Warehouse_DB.Coin,
                    });
#pragma warning restore CS0618
                    mPlayer.Send(new G2C_BackpackGoldChange_notice()
                    {
                        GameUserId = mPlayer.GameUserId,
                        Gold = mGamePlayer.Data.GoldCoin
                    });

                    mapComponent.MoveSendNotice(null, mMapCellTarget, mGamePlayer);
                    mGamePlayer.DataAddPropertyBufferGotoMap(mapComponent.GetCustomComponent<BattleComponent>());

                    if (mGamePlayer.Pets != null && !mGamePlayer.Pets.IsDeath)
                    {
                        var mFindTheWay = mapComponent.GetFindTheWay2D(mGamePlayer);
                        mGamePlayer.Pets.MoveNeedTime = Help_TimeHelper.GetNow() + mGamePlayer.Pets.Config.MoSpeed;

                        mapComponent.MoveSendNotice(null, mFindTheWay, mGamePlayer.Pets);
                        //mGamePlayer.Pets.DataAddPropertyBufferGotoMap(mapComponent.GetCustomComponent<BattleComponent>());
                    }

                    NoticePlayerBattleCopyStatus().Coroutine();
                    Log.Info($"#RoleFlow# Ready deferred-bootstrap finish user={mPlayer.UserId} role={mPlayer.GameUserId}");
                    Log.PLog("Login", $"a:{mPlayer.UserId} r:{mPlayer.GameUserId} 玩家角色读取数据完成 time = {Help_TimeHelper.GetNow() - startTime}");

                    if (mPlayer == null || mPlayer.IsDisposeable || mPlayer.Id != playerInstanceId)
                    {
                        return;
                    }

                    if (mPlayer.GetCustomComponent<PlayerMailComponent>() != null)
                    {
                        Log.Info($"#RoleFlow# Ready post-start mail user={mPlayer.UserId} role={mPlayer.GameUserId}");
                        // 此函数调用完成前，不允许玩家下线。否则会造成邮件丢失
                        await mPlayer.GetCustomComponent<PlayerMailComponent>().SendServerMaill(mAreaId);
                        Log.Info($"#RoleFlow# Ready post-finish mail user={mPlayer.UserId} role={mPlayer.GameUserId}");
                    }

                    if (mPlayer == null || mPlayer.IsDisposeable || mPlayer.Id != playerInstanceId)
                    {
                        return;
                    }

                    var City = mServerArea.GetCustomComponent<CitySiegeActivities>();
                    if (City != null && City.GetSate())
                    {
                        G2C_SendPointOutMessage g2C_SendPointOutMessage = new G2C_SendPointOutMessage();
                        g2C_SendPointOutMessage.Status = 1;
                        g2C_SendPointOutMessage.Pointout = 2707;
                        mPlayer.Send(g2C_SendPointOutMessage);
                    }

                    if (mPlayer == null || mPlayer.IsDisposeable || mPlayer.Id != playerInstanceId)
                    {
                        return;
                    }

                    // TODO 临时代码 为新存档添加装备
                    if (mGamePlayer.Data.NewArchive == 1)
                    {
                        Log.Info($"#RoleFlow# Ready post-start new-archive user={mPlayer.UserId} role={mPlayer.GameUserId}");
                        {//账号第一个创建的角色赠送20W金币之后的没有
                            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                            if (dBProxy2 != null)
                            {
                                var GamePlayerList = await dBProxy2.Query<DBGamePlayerData>(P => P.UserId == mPlayer.UserId);
                                if (GamePlayerList.Count <= 1)
                                {
                                    Item newItem3 = ItemFactory.Create(320294, mPlayer.GameAreaId, 200000);
                                    mPlayer.GetCustomComponent<BackpackComponent>().AddItem(newItem3, "新存档奖励");
                                }
                            }
                        }
                        Item newItem = ItemFactory.Create(20000, mPlayer.GameAreaId);
                        mPlayer.GetCustomComponent<BackpackComponent>().AddItem(newItem, "新存档奖励");
                        int ItemId = 0;
                        switch ((E_GameOccupation)mGamePlayer.Data.PlayerTypeId)
                        {
                            case E_GameOccupation.Archer: ItemId = 40020; break;
                            case E_GameOccupation.Holyteacher:
                            case E_GameOccupation.Swordsman:
                            case E_GameOccupation.Spell:
                            case E_GameOccupation.Spellsword: ItemId = 160048; break;
                            case E_GameOccupation.Combat:
                            case E_GameOccupation.GrowLancer:
                            case E_GameOccupation.SummonWarlock: ItemId = 10036; break;
                        }
                        Item newItem2 = ItemFactory.Create(ItemId, mPlayer.GameAreaId);
                        mPlayer.GetCustomComponent<BackpackComponent>().AddItem(newItem2, "新存档奖励");
                        Log.Info($"#RoleFlow# Ready post-finish new-archive user={mPlayer.UserId} role={mPlayer.GameUserId}");
                    }

                    if (mPlayer == null || mPlayer.IsDisposeable || mPlayer.Id != playerInstanceId)
                    {
                        return;
                    }

                    DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                    var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);
                    DBAccountInfo dbLoginInfo = null;
                    if (mDBProxy != null)
                    {
                        Log.Info($"#RoleFlow# Ready post-start account-info user={mPlayer.UserId} role={mPlayer.GameUserId}");
                        var list = await mDBProxy.Query<DBAccountInfo>(p => p.Id == mPlayer.UserId);
                        if (list.Count > 0)
                        {
                            dbLoginInfo = list[0] as DBAccountInfo;
                        }
                        if (dbLoginInfo != null && dbLoginInfo.Identity == AccountIdentity.Studio)
                        {
                            mPlayer.GetCustomComponent<GamePlayer>().dBCharacterDroplimit.Restrict = 1;
                        }
                        else
                        {
                            mPlayer.GetCustomComponent<GamePlayer>().dBCharacterDroplimit.Restrict = 0;
                        }

                        if (dbLoginInfo != null)
                        {
                            mPlayer.GetCustomComponent<GamePlayer>().Code = dbLoginInfo.Code;
                        }

                        if (mPlayer.GetCustomComponent<GamePlayer>().Code == null)
                        {
                            mPlayer.GetCustomComponent<GamePlayer>().Code = new string("");
                        }
                        Log.Info($"#RoleFlow# Ready post-finish account-info user={mPlayer.UserId} role={mPlayer.GameUserId}");
                    }

                    if (mPlayer == null || mPlayer.IsDisposeable || mPlayer.Id != playerInstanceId)
                    {
                        return;
                    }

                    Log.Info($"#RoleFlow# Ready post-start events user={mPlayer.UserId} role={mPlayer.GameUserId}");
                    ETModel.EventType.PlayerReadyComplete.Instance.player = mPlayer;
                    Root.EventSystem.OnRun("PlayerReadyComplete", ETModel.EventType.PlayerReadyComplete.Instance);

                    ETModel.EventType.GamePlayerLevelUp.Instance.gamePlayer = mGamePlayer;
                    ETModel.EventType.GamePlayerLevelUp.Instance.oldLevel = mGamePlayer.Data.Level;
                    ETModel.EventType.GamePlayerLevelUp.Instance.newLevel = mGamePlayer.Data.Level;
                    Root.EventSystem.OnRun("GamePlayerLevelUp", ETModel.EventType.GamePlayerLevelUp.Instance);
                    Log.Info($"#RoleFlow# Ready post-finish events user={mPlayer.UserId} role={mPlayer.GameUserId}");
                }
                catch (Exception e)
                {
                    Log.Error($"#RoleFlow# Ready post-failed user={mPlayer.UserId} role={mPlayer.GameUserId} {e}");
                }
            }
        }
    }
}
