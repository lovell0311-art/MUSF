using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Linq;
using System.Threading.Tasks;
using Player = ETModel.Player;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using Microsoft.CodeAnalysis.Operations;
using CSharpx;

namespace ETHotfix
{
    public static class BattleCopyComponentSystem
    {
        public static async Task Run(this BattleCopyComponent self)
        {
            //开始时间轮循
            string name = self.GetCopyName(self.Type);

            while (!self.IsDisposeable)
            {
                /*if (self.IsTimelimit)
                {
                    self.StartGame();
                    break;//古战场
                }*/
                self.CalculateNextOpenTimer();

                //测试 ==============================================>
                DateTime currentDT = DateTime.Now;//new DateTime(2022, 12, 15, 12, 9, 0);
                long timeNow = Help_TimeHelper.ConvertDateTimeToLong(currentDT);
                //=================================================>
                if (self.copyState == CopyState.Close)
                {
                    await Root.MainFactory.GetCustomComponent<TimerComponent>().WaitAsyncSetTime(timeNow+50000000, timeNow);
                    continue;
                }

                if (self.copyState == CopyState.Prepare) goto Prepare;

                await Root.MainFactory.GetCustomComponent<TimerComponent>().WaitAsyncSetTime(self.prepareTimer, timeNow);
                //self.ChangeState(timeNow);
                self.ChangeState();
                //Log.Debug($"{name}副本还有5分钟即将开启 ，可以进入战斗地图进行等待");

                //DateTime startTime = Help_TimeHelper.ConvertStringToDateTime(self.startTimer);
                //Log.Debug($"{name}副本将在 {startTime.ToString()} 开启");

                //Prepare: await Root.MainFactory.GetCustomComponent<TimerComponent>().WaitAsync(15000);
                //Prepare: await Root.MainFactory.GetCustomComponent<TimerComponent>().WaitAsyncSetTime(self.startTimer, timeNow);
                Prepare: await Root.MainFactory.GetCustomComponent<TimerComponent>().WaitAsyncSetTime(self.startTimer, self.prepareTimer);
                //Log.Debug($"{name}副本已经开启 ！！！");
                //self.ChangeState(timeNow);
                self.ChangeState();
                self.StartGame();

                //await Root.MainFactory.GetCustomComponent<TimerComponent>().WaitAsyncSetTime(self.endTimer, timeNow);
                await Root.MainFactory.GetCustomComponent<TimerComponent>().WaitAsyncSetTime(self.endTimer, self.startTimer);
                //Log.Debug($"{name}副本已经关闭 ！！！");
                //self.ChangeState(timeNow);
                self.ChangeState();
                self.CopyGameOver();

                await Root.MainFactory.GetCustomComponent<TimerComponent>().WaitAsync(10000);
                self.ClearRoom();

            }
        }

        public static void Create(this BattleCopyComponent self, BattleCopy_OpenConfig config)
        {
            int type = config.Id;
            self.Type = type;
            /*if (config.Timelimit == 1)//古战场
            {
                self.copyState = CopyState.Open;
                self.IsTimelimit = true;
                return;
            }*/
            self.DurationnTime = int.Parse(config.Duration);

            if (config.OpenTime1 != "0") self.openTimeList.Add(config.OpenTime1);
            if (config.OpenTime2 != "0") self.openTimeList.Add(config.OpenTime2);
            if (config.OpenTime3 != "0") self.openTimeList.Add(config.OpenTime3);
            if (config.OpenTime4 != "0") self.openTimeList.Add(config.OpenTime4);
            if (config.OpenTime5 != "0") self.openTimeList.Add(config.OpenTime5);
            if (config.OpenTime6 != "0") self.openTimeList.Add(config.OpenTime6);
            if (config.OpenTime7 != "0") self.openTimeList.Add(config.OpenTime7);
            if (config.OpenTime8 != "0") self.openTimeList.Add(config.OpenTime8);
            if (config.OpenTime9 != "0") self.openTimeList.Add(config.OpenTime9);
            if (config.OpenTime10 != "0") self.openTimeList.Add(config.OpenTime10);
            if (config.OpenTime11 != "0") self.openTimeList.Add(config.OpenTime11);
            if (config.OpenTime12 != "0") self.openTimeList.Add(config.OpenTime12);
            if (config.OpenTime13 != "0") self.openTimeList.Add(config.OpenTime13);
            if (config.OpenTime14 != "0") self.openTimeList.Add(config.OpenTime14);
            if (config.OpenTime15 != "0") self.openTimeList.Add(config.OpenTime15);
            if (config.OpenTime16 != "0") self.openTimeList.Add(config.OpenTime16);
            if (config.OpenTime17 != "0") self.openTimeList.Add(config.OpenTime17);
            if (config.OpenTime18 != "0") self.openTimeList.Add(config.OpenTime18);
            if (config.OpenTime19 != "0") self.openTimeList.Add(config.OpenTime19);
            if (config.OpenTime20 != "0") self.openTimeList.Add(config.OpenTime20);
            if (config.OpenTime21 != "0") self.openTimeList.Add(config.OpenTime21);
            if (config.OpenTime22 != "0") self.openTimeList.Add(config.OpenTime22);
            if (config.OpenTime23 != "0") self.openTimeList.Add(config.OpenTime23);
            if (config.OpenTime24 != "0") self.openTimeList.Add(config.OpenTime24);

            int count = self.openTimeList.Count;
            self.timeKeys = new int[count];
            self.timeValues = new int[count];

            for (int i = 0; i < count; i++)
            {
                string timeStr = self.openTimeList[i];
                // timeStr 格式 23 +10
                int hour = int.Parse(timeStr.Split('+')[0]);
                int minute = int.Parse(timeStr.Split('+')[1]);
                self.timeKeys[i] = hour;
                self.timeValues[i] = minute;
            }

            self.copyState = CopyState.Wait;
            if (type == (int)CopyType.CangBaoTu)//藏宝图常开
            {
                self.copyState = CopyState.Open;
            }
        }

        public static MapComponent JoinRoom(this BattleCopyComponent self, int level, Player player, string name, int mapId, C_ServerArea mServerArea)
        {
            long userId = player.GameUserId;
            if (self.copyRankDataDic.ContainsKey(userId)) return null;

            if (!self.battleCopyRoomDic.ContainsKey(level))
            {
                BattleCopyRoom battleCopyRoom = Root.CreateBuilder.GetInstance<BattleCopyRoom, BattleCopyComponent>(self);
                MapManageComponent mapManageComponent = mServerArea.GetCustomComponent<MapManageComponent>();
                battleCopyRoom.mapComponent = mapManageComponent.Copy(mapId);
                battleCopyRoom.level = level;
                battleCopyRoom.round = 0;
                battleCopyRoom.mapComponent.Awake(mapManageComponent);
                Dictionary<int,BattleCopyRoom> battleCopyRooms = new Dictionary<int, BattleCopyRoom>();
                battleCopyRooms.Add(0,battleCopyRoom);
                self.battleCopyRoomDic.Add(level,battleCopyRooms);
            }
            self.battleCopyRoomDic.TryGetValue(level, out Dictionary<int, BattleCopyRoom> roomList);

            int roomIndex = -1;
            BattleCopyRoom mCopyRoom = null;

            foreach (var room in roomList)
            {
                if (!room.Value.JoinRoom(userId, name)) continue;
                roomIndex = room.Key;
                mCopyRoom = room.Value;
                break;
            }
            if (roomIndex == -1)
            {
                mCopyRoom = Root.CreateBuilder.GetInstance<BattleCopyRoom, BattleCopyComponent>(self);
                mCopyRoom.level = level;
                mCopyRoom.round = 0;
                MapManageComponent mapManageComponent = mServerArea.GetCustomComponent<MapManageComponent>();
                mCopyRoom.mapComponent = mapManageComponent.Copy(mapId);
                mCopyRoom.mapComponent.Awake(mapManageComponent);
                mCopyRoom.JoinRoom(userId, name);
                roomIndex = roomList.Count;
                mCopyRoom.Index = roomIndex;
                roomList.Add(roomIndex,mCopyRoom);
                
            }

            CopyRankData copyRankData = new CopyRankData()
            {
                Level = level,
                Index = roomIndex,
            };
            self.copyRankDataDic.Add(userId, copyRankData);
            Log.PLog($"JoinRoom Player:{userId} Level:{level}Index:{roomIndex}");
            self.players.Add(userId, player);
            return mCopyRoom.mapComponent;
        }
        /// <summary>
        /// 由于QuitRoom和QuitMap有顺序关系，删除副本玩家需要单独处理，
        /// </summary>
        /// <param name="self"></param>
        /// <param name="gameUserId"></param>
        public static void RemoveUser(this BattleCopyComponent self, long gameUserId)
        {
            if(self.copyRankDataDic.TryGetValue(gameUserId,out var Info ))
                Log.PLog($"RemoveUser Player:{gameUserId} Level:{Info.Level}Index:{Info.Index}");

            self.copyRankDataDic.Remove(gameUserId);
            self.players.Remove(gameUserId);
        }
        public static void DelSend(this BattleCopyComponent self,long gameUserId)
        {
            if (!self.copyRankDataDic.ContainsKey(gameUserId)) return ;
            if (!self.players.ContainsKey(gameUserId)) return ;
            CopyRankData copyRankData = self.copyRankDataDic[gameUserId];
            long level = copyRankData.Level;
            int index = copyRankData.Index;

            self.battleCopyRoomDic.TryGetValue(level, out Dictionary<int,BattleCopyRoom> roomList);
            Log.Debug($"数量：{roomList.Count} Index:{index}");
            BattleCopyRoom copyRoom = roomList[index];
            copyRoom.DleSend(gameUserId);
        }
        public static bool QuitRoom(this BattleCopyComponent self, long gameUserId)
        {
            if (!self.copyRankDataDic.ContainsKey(gameUserId)) return false;
            if (!self.players.ContainsKey(gameUserId)) return false;
            CopyRankData copyRankData = self.copyRankDataDic[gameUserId];
            long level = copyRankData.Level;
            int index = copyRankData.Index;

            self.battleCopyRoomDic.TryGetValue(level, out Dictionary<int,BattleCopyRoom> roomList);
            Log.Debug($"数量：{roomList.Count} Index:{index}");
            BattleCopyRoom copyRoom = roomList[index];
            copyRoom.QuitRoom(gameUserId);

            Player player = self.players[gameUserId];
            if ((CopyType)self.Type == CopyType.RedCastle)  //血色城堡退出时若有大天使之剑则掉落
            {
                BackpackComponent backpack = player.GetCustomComponent<BackpackComponent>();
                var itemList = backpack.GetAllItemByConfigID(320304);
                if (itemList != null)
                {
                    //for (int i = 0; i < itemList.Count; i++)
                    foreach(var item in itemList)
                    {
                        //Item item = itemList[i];
                        Item itemInfo = item.Value;
                        if (itemInfo != null)
                        {
                            //backpack.UseItem(itemInfo.ItemUID, "退出副本强制消耗");
                            GamePlayer mGameplayer = player.GetCustomComponent<GamePlayer>();
                            MapComponent mMapComponent = copyRoom.mapComponent;
                            var mFindTheWay = mMapComponent.GetFindTheWay2D(mGameplayer);
                            if (mFindTheWay == null || mFindTheWay.IsStaticObstacle == true)
                            {
                                Log.Info($"mMapComponent.Id:{mMapComponent.Id} MapId:{mMapComponent.MapId}");
                                Log.Error(message: $"大天使之剑掉落失败MapId:{mGameplayer.UnitData.Index}X:{mGameplayer.UnitData.X}Y:{mGameplayer.UnitData.Y}");
                                return false;
                            }
                            backpack.DiscardItemToGround(itemInfo.ItemUID, mMapComponent, mGameplayer.UnitData.X, mGameplayer.UnitData.Y, EMapItemCreateType.Other, "退出副本强制掉落");
                        }
                    }
                }

            }
            //self.copyRankDataDic.Remove(gameUserId);
            //self.players.Remove(gameUserId);
            return true;
        }

        public static void StartGame(this BattleCopyComponent self)
        {
            //for (int i = 0; i < self.battleCopyRoomDic.Count; i++)
            foreach(var Rom in self.battleCopyRoomDic)
            {
                //List<BattleCopyRoom> battleCopyRooms = self.battleCopyRoomDic.ElementAt(i).Value;
                //for (int j = 0; j < battleCopyRooms.Count; j++)
                foreach(var Room in Rom.Value)
                {
                    //battleCopyRooms[i].StateGameAsync();
                    Room.Value.StateGameAsync().Coroutine();
                }
            }
        }

        public static void CopyGameOver(this BattleCopyComponent self)
        {
            //让副本的所有怪物停止攻击
            //for (int i = 0; i < self.battleCopyRoomDic.Count; i++)
            foreach(var Room in self.battleCopyRoomDic)
            {
                //List<BattleCopyRoom> battleCopyRooms = self.battleCopyRoomDic.ElementAt(i).Value;
                //for (int j = 0; j < battleCopyRooms.Count; j++)
                foreach(var Rom in Room.Value)
                {
                    //BattleCopyRoom battleCopyRoom = battleCopyRooms[i];
                    //battleCopyRoom.mapComponent.OnlyStopUpdate();
                    //CopyType copyType = (CopyType)battleCopyRoom.type;
                    Rom.Value.mapComponent.OnlyStopUpdate();
                    CopyType copyType = (CopyType)Rom.Value.type; 
                    if (copyType == CopyType.DemonSquae)
                    {
                        //battleCopyRoom.mapComponent.OnlyStopUpdate();
                        //battleCopyRoom.CalculateDemonSquareRank();
                        Rom.Value.CalculateDemonSquareRank();
                    }
                    else if (copyType == CopyType.RedCastle)
                    {
                        //battleCopyRoom.RedCastleSettlement();
                        Rom.Value.RedCastleSettlement();
                    }
                }
            }
        }

        private static void ChangeState(this BattleCopyComponent self, long timeNow = 0)
        {
            if (self.copyState == CopyState.Wait)
            {
                self.copyState = CopyState.Prepare;
            }
            else if (self.copyState == CopyState.Prepare)
            {
                self.copyState = CopyState.Open;
            }
            else if (self.copyState == CopyState.Open)
            {
                self.copyState = CopyState.Close;
            }
            else if (self.copyState == CopyState.Close)
            {
                self.copyState = CopyState.Wait;
            }

            self.Broadcast(timeNow);
        }

        private static string GetCopyName(this BattleCopyComponent self, int type)
        {
            switch (type)
            {
                case 1: return "恶魔广场";
                case 2: return "血色城堡";
                case 3: return "古战场";

                default: return "";
            }
        }

        public static void CalculateNextOpenTimer(this BattleCopyComponent self)
        {
            DateTime currentDT = DateTime.Now;
            //DateTime currentDT = new DateTime(2022, 12, 15, 12, 9, 0); //TEST

            int day = currentDT.Day;
            int year = currentDT.Year;
            int month = currentDT.Month;
            int currentHour = currentDT.Hour;

            List<string> openTimes = self.openTimeList;
            int length = self.timeKeys.Length;
            for (int i = 0; i < length; i++)
            {
                int hour = self.timeKeys[i];
                int minute = self.timeValues[i];

                DateTime openDateTime = new DateTime(year, month, day, hour, minute, 0);
                //DateTime openDateTime = new DateTime(2022, 12, 15, 12, 10, 0);  //TEST

                if (currentHour == 23) openDateTime = openDateTime.AddDays(1);

                DateTime prepareDateTime = openDateTime.AddMinutes(-5);
                DateTime overDateTime = openDateTime.AddMinutes(15);

                //如果当前时间已经过了开始时间  或是大于 结束时间 直接下一个
                if (DateTime.Compare(currentDT, openDateTime) >= 0 || DateTime.Compare(currentDT, overDateTime) >= 0)
                {
                    continue;
                }

                self.prepareTimer = Help_TimeHelper.ConvertDateTimeToLong(prepareDateTime);
                self.startTimer = Help_TimeHelper.ConvertDateTimeToLong(openDateTime);
                self.endTimer = Help_TimeHelper.ConvertDateTimeToLong(overDateTime);

                if (DateTime.Compare(currentDT, prepareDateTime) < 0)
                {
                    //如果当前时间小于准备时间
                    self.copyState = CopyState.Wait;
                    Log.Debug($"当前时间{currentDT.ToString()}  小于  {prepareDateTime},进入等待状态");

                    return;
                }

                if (DateTime.Compare(currentDT, prepareDateTime) >= 0 && DateTime.Compare(currentDT, openDateTime) < 0)
                {
                    //当前时间过了准备时间 并小于开始时间
                    self.copyState = CopyState.Prepare;
                    Log.Debug($"当前时间{currentDT.ToString()}  大于  {prepareDateTime},进入准备状态");
                    return;
                }
            }
        }

        public static void ClearRoom(this BattleCopyComponent self)
        {

            for (int i = 0; i < self.battleCopyRoomDic.Count; i++)
            {
                long key = self.battleCopyRoomDic.ElementAt(i).Key;
                if (key >= 8) continue;//不处理藏宝图副本

                Dictionary<int,BattleCopyRoom> battleCopyRooms = self.battleCopyRoomDic[key];
                foreach(var Room in battleCopyRooms)
                {
                    Room.Value.ClearRoom();
                    Room.Value.Dispose();
                }
                battleCopyRooms.Clear();

                self.battleCopyRoomDic[key].Clear();
            }
            /*foreach (var item in self.players)
            {
                self.QuitRoom(item.Key);
            }*/
            //self.battleCopyRoomDic.Clear();
            //self.copyRankDataDic.Clear();
            //self.players.Clear();

        }

        public static void Broadcast(this BattleCopyComponent self, long timeNow = 0, long targetGameUserID = 0)
        {
            int mAreaId = self.Parent.Parent.GameAreaId;
            var playerDic = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().GetAllByZone(mAreaId);
            if (playerDic == null) return;
            if (targetGameUserID != 0)
            {
                if (!playerDic.ContainsKey(targetGameUserID))
                {
                    return;
                }
            }

            //Player[] PlayeDic = playerDic.Values.ToArray();
            self.message.MapType = self.Type ;
            self.message.State = (int)self.copyState;//new RepeatedField<int>() { 0, 0 };
            //self.message.LeftSeconds = new RepeatedField<long> { 0, 0 };
            if (timeNow == 0) timeNow = Help_TimeHelper.GetCurrenTimeStamp();
            if (self.copyState == CopyState.Prepare)
            {
                self.message.LeftSeconds = Help_TimeHelper.GetSecond(timeNow, self.startTimer);
            }
            else if (self.copyState == CopyState.Open)
            {
                self.message.LeftSeconds = Help_TimeHelper.GetSecond(timeNow, self.endTimer);
            }
            /*CopyState copyState = self.copyState;
            if (timeNow == 0) timeNow = Help_TimeHelper.GetCurrenTimeStamp();

            switch (self.Type)
            {
                case 1:
                    self.message.State[0] = (int)self.copyState;
                    if (copyState == CopyState.Prepare)
                    {
                        self.message.LeftSeconds[0] = Help_TimeHelper.GetSecond(timeNow, self.startTimer);
                    }
                    else if (copyState == CopyState.Open)
                    {
                        self.message.LeftSeconds[0] = Help_TimeHelper.GetSecond(timeNow, self.endTimer);
                    }
                    break;
                case 2:
                    self.message.State[1] = (int)self.copyState;
                    if (copyState == CopyState.Prepare)
                    {
                        self.message.LeftSeconds[1] = Help_TimeHelper.GetSecond(timeNow, self.startTimer);
                    }
                    else if (copyState == CopyState.Open)
                    {
                        self.message.LeftSeconds[1] = Help_TimeHelper.GetSecond(timeNow, self.endTimer);
                    }
                    break ;
            }*/

            //Log.Debug("广播给所有玩家副本信息："+ playerDic.Count);
            if (targetGameUserID > 0)
            {
                var player = playerDic[targetGameUserID];
                if (self.Parent.Parent.SourceId == player.SourceGameAreaId)
                    player.Send(self.message);
            }
            else
            {
                foreach (var player in playerDic.Values)
                {
                    if (self.Parent.Parent.SourceId == player.SourceGameAreaId)
                        player.Send(self.message);
                }
            }

        }

        public static bool QuitMap(this BattleCopyComponent self, long ActorId, DBProxyManagerComponent mDBProxyManagerComponent, C_ServerArea mServerArea)
        {
            int mAreaId = mServerArea.GameAreaId;
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, ActorId);
            if (mPlayer == null)
            {
                Log.Error("玩家没有找到");
                return false;
            }
            return self.QuitMap(mPlayer, mDBProxyManagerComponent, mServerArea);
        }

        public static bool QuitMap(this BattleCopyComponent self, Player mPlayer, DBProxyManagerComponent mDBProxyManagerComponent, C_ServerArea mServerArea)
        {
            if (mPlayer == null)
            {
                Log.Error("玩家没有找到");
                return false;
            }

            DBProxyComponent dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
            DataCacheManageComponent mDataCacheManageComponent = mPlayer.GetCustomComponent<DataCacheManageComponent>();
            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            DBPlayerUnitData mPlayerUnitData = mGamePlayer.UnitData;

            MapComponent mapComponent = self.Get(mPlayer.GameUserId);
            if (mapComponent == null)
            {
                Log.Error($"找不到自己所在的地图GamePlayer{mPlayer.GameUserId}MapID{mPlayerUnitData.Index}X{mPlayerUnitData.X}Y{mPlayerUnitData.Y}");
                return false;
            }
            if (mapComponent.TryGetPosX(mPlayerUnitData.X) == false)
            {
                Log.Error($"找不到自己在地图的位置GamePlayer{mPlayer.GameUserId}MapID{mPlayerUnitData.Index}X{mPlayerUnitData.X}Y{mPlayerUnitData.Y}");
                return false;
            }
            var mFindTheWaySource = mapComponent.GetFindTheWay2D(mPlayerUnitData.X, mPlayerUnitData.Y);
            if (mFindTheWaySource == null)
            {
                Log.Error($"找不到自己在地图的位置GamePlayer{mPlayer.GameUserId}MapID{mPlayerUnitData.Index}X{mPlayerUnitData.X}Y{mPlayerUnitData.Y}");
                return false;
            }


            // 直接去到冰风谷传送点
            var mTransferPointId = 400;
            var mJsonMapDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Map_TransferPointConfigJson>().JsonDic;
            if (mJsonMapDic.TryGetValue(mTransferPointId, out var _TransferPointConfig) == false)
            {
                Log.Error("传送点不存在");
                return false;
            }
            if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(_TransferPointConfig.MapId, out var mTargetMapComponent) == false)
            {
                Log.Error("地图数据异常");
                return false;
            }

            if (mTargetMapComponent.TransferPointFindTheWayDic.TryGetValue(mTransferPointId, out var mTransferPointlist) == false)
            {
                Log.Error("传送点初始化异常");
                return false;
            }
            if (mTransferPointlist == null || mTransferPointlist.Count == 0)
            {
                Log.Error("传送点初始化异常");
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
//             mPlayer.Send(new G2C_MovePos_notice()
//             {
//                 UnitType = (int)E_Identity.Hero,
//                 GameUserId = mGamePlayer.InstanceId,
//                 MapId = mGamePlayer.UnitData.Index,
//                 X = mPlayerUnitData.X,
//                 Y = mPlayerUnitData.Y,
//                 Angle = mPlayerUnitData.Angle,
//                 IsNeedMove = 0
//             });

            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
            mWriteDataComponent.Save(mPlayerUnitData, dBProxy).Coroutine();

            return true;
        }

        public static void RecordCoin(this BattleCopyComponent self, long gameUserId, int coin)
        {
            if (!self.copyRankDataDic.ContainsKey(gameUserId)) return;
            self.copyRankDataDic[gameUserId].SetCoin(coin);
        }

        public static int RecordScore(this BattleCopyComponent self, long gameUserId, int exp, int level)
        {
            if (!self.copyRankDataDic.ContainsKey(gameUserId)) return exp;
            //击杀怪物获得经验=怪物的正常经验*经验倍数，经验倍数暂定为2
            int newExp = exp * self.config.EXPMultiple;
            //击杀怪物获得分数 = 怪物等级 * 等级系数，等级系数暂定为2
            int score = level * self.config.ScoreRate;

            self.copyRankDataDic[gameUserId].Set(newExp, score);

            if (self.players.ContainsKey(gameUserId))
            {
                //var mPlayer = self.players[gameUserId];
                ETModel.EventType.DemonPlazaGainsPoints.Instance.player = self.players[gameUserId];
                ETModel.EventType.DemonPlazaGainsPoints.Instance.transferPoint = score;
                Root.EventSystem.OnRun("GamePlayerDemonPlazaGainsPoints", ETModel.EventType.DemonPlazaGainsPoints.Instance);
            }
            return newExp;
        }
        public static int GetCopyLV(this BattleCopyComponent self, int type,int Lv)
        {
            CopyType copyType = (CopyType)type;
            switch (copyType)
            {
                case CopyType.DemonSquae:
                    
                        if (Lv >= 50 && Lv <= 179) return 1;
                        if (Lv >= 180 && Lv <= 269) return 2;
                        if (Lv >= 270 && Lv <= 349) return 3;
                        if (Lv >= 350 && Lv <= 399) return 4;
                        if (Lv >= 400 && Lv <= 500) return 5;
                        if (Lv >= 501 && Lv <= 650) return 6;
                        if (Lv >= 651 && Lv <= 800) return 7;
                    
                    break;
                case CopyType.RedCastle:
                    
                        if (Lv >= 50 && Lv <= 179) return 1;
                        if (Lv >= 180 && Lv <= 269) return 2;
                        if (Lv >= 270 && Lv <= 349) return 3;
                        if (Lv >= 350 && Lv <= 399) return 4;
                        if (Lv >= 400 && Lv <= 500) return 5;
                        if (Lv >= 501 && Lv <= 650) return 6;
                        if (Lv >= 651 && Lv <= 800) return 7;
                     break;
            }

            return 1;
        }
        public static bool DecisionPlayerLevel(this BattleCopyComponent self, int type, int level, DBGamePlayerData mPlayerData)
        {
            int needLevelUp = 0; int needLvDown = 0;
            CopyType copyType = (CopyType)type;
            switch (copyType)
            {
                case CopyType.DemonSquae:
                    {
                        switch (level)
                        {
                            case 1: needLevelUp = 50; needLvDown = 179; break;
                            case 2: needLevelUp = 180; needLvDown = 269; break;
                            case 3: needLevelUp = 270; needLvDown = 349; break;
                            case 4: needLevelUp = 350; needLvDown = 399; break;
                            case 5: needLevelUp = 400; needLvDown = 500; break;
                            case 6: needLevelUp = 501; needLvDown = 650; break;
                            case 7: needLevelUp = 651; needLvDown = 800; break;
                        }
                        break;
                    }
                case CopyType.RedCastle:
                    {
                        switch (level)
                        {
                            case 1: needLevelUp = 50; needLvDown = 179; break;
                            case 2: needLevelUp = 180; needLvDown = 269; break;
                            case 3: needLevelUp = 270; needLvDown = 349; break;
                            case 4: needLevelUp = 350; needLvDown = 399; break;
                            case 5: needLevelUp = 400; needLvDown = 500; break;
                            case 6: needLevelUp = 501; needLvDown = 650; break;
                            case 7: needLevelUp = 651; needLvDown = 800; break;
                        }
                        break;
                    }
                /*case CopyType.AncientBattlefield://古战场
                    {
                        switch (level)
                        {
                            case 1: needLevelUp = 50; needLvDown = 2000; break;
                                case 2: needLevelUp = 180; needLvDown = 269; break;
                                case 3: needLevelUp = 270; needLvDown = 349; break;
                                case 4: needLevelUp = 350; needLvDown = 399; break;
                                case 5: needLevelUp = 400; needLvDown = 500; break;
                                case 6: needLevelUp = 501; needLvDown = 650; break;
                                case 7: needLevelUp = 651; needLvDown = 800; break;
                        }
                        break;
                    }*/
            }

            if (mPlayerData.Level < needLevelUp || mPlayerData.Level > needLvDown)
            {
                return false;
            }

            return true;
        }
        public static Item GetDecisionTicket(this BattleCopyComponent self, BackpackComponent backpackCpt, int type)
        {
            CopyType copyType = (CopyType)type;
            int ItemID = 0;
            switch (copyType)
            {
                case CopyType.DemonSquae:
                    ItemID= 320014;
                    break;
                case CopyType.RedCastle:
                    ItemID= 320015;
                    break;
            }
            var ticket = backpackCpt.GetAllItemByConfigID(ItemID);
            if (ticket != null && ticket.Count >= 1)
            { 
                return ticket.ElementAt(0).Value;
            }
            return null;
        }
        public static int DecisionTicket(this BattleCopyComponent self, int type)
        {
            CopyType copyType = (CopyType)type;
            switch (copyType)
            {
                case CopyType.DemonSquae:
                    return 320099;

                case CopyType.RedCastle:
                    return 320303;
                /*case CopyType.AncientBattlefield:
                    return 1;//古战场 接口特殊处理*/
            }
            return 0;
        }
        public static bool CheckChallengeNum(this BattleCopyComponent self, int type, PlayerShopMallComponent Info)
        {
            CopyType copyType = (CopyType)type;
            switch (copyType)
            {
                case CopyType.DemonSquae:
                    break;
                case CopyType.RedCastle:
                    break;
                /*case CopyType.AncientBattlefield:
                    return Info.GetPlayerShopState(DeviationType.MaxMonthlyCard);*/

            }
            return false;
        }

        public static void DeductionChallengeNum(this BattleCopyComponent self, int type, DBBattleCopyData battleCopyData)
        {
            CopyType copyType = (CopyType)type;
            switch (copyType)
            {
                case CopyType.DemonSquae:
                    battleCopyData.demonSquaeNum--;
                    if (battleCopyData.demonSquaeNum < 0) battleCopyData.demonSquaeNum = 0;
                    break;
                case CopyType.RedCastle:
                    battleCopyData.redCastleNum--;
                    if (battleCopyData.redCastleNum < 0) battleCopyData.redCastleNum = 0;
                    break;
                /*case CopyType.AncientBattlefield:
                    //古战场 不限制次数
                    break;*/
            }
        }

        public static int DecisionTransferPointId(this BattleCopyComponent self, int type)
        {
            CopyType copyType = (CopyType)type;
            switch (copyType)
            {
                case CopyType.DemonSquae:
                    return 101;

                case CopyType.RedCastle:
                    return 100;
                /*case CopyType.AncientBattlefield://古战场
                    return 102;*/
                case CopyType.CangBaoTu:
                    return 110;
            }
            return 0;
        }

        public static int RecordKilledMonsters(this BattleCopyComponent self, long gameUserId, long monsterId, int exp)
        {
            if (!self.copyRankDataDic.ContainsKey(gameUserId)) return exp;

            long level = self.copyRankDataDic[gameUserId].Level;
            int index = self.copyRankDataDic[gameUserId].Index;
            Log.PLog($"RemoveUser Player:{gameUserId} Level:{level}Index:{index}");
            BattleCopyRoom BattleCopyRoom = self.battleCopyRoomDic[level][index];
            if (BattleCopyRoom == null)
            {
                Log.Error("记录杀怪数量是 找不到副本房间");
                return exp;
            }

            BattleCopyRoom.RecordKilledMonsters(gameUserId, monsterId);

            //击杀怪物获得经验=怪物的正常经验*经验倍数，经验倍数暂定为2
            int newExp = exp * self.config.EXPMultiple;
            return newExp;
        }

        public static async Task RedCastleSettlementAsync(this BattleCopyComponent self, BattleCopyRoom battleCopyRoom)
        {

            battleCopyRoom.RedCastleSettlement();
            battleCopyRoom.RedBaoShi();
            int level = battleCopyRoom.level;
            int Index = battleCopyRoom.Index;
            /*DateTime nowTime = DateTime.Now;
            nowTime.AddSeconds(20);
            long FinalCloseTimer = 0;
            long closeTimer = Help_TimeHelper.ConvertDateTimeToLong(nowTime);
            if (closeTimer < self.endTimer)
            {
                FinalCloseTimer = closeTimer;
            }
            else
            {
                FinalCloseTimer = self.endTimer;
            }

            DateTime finalTimeDate = Help_TimeHelper.ConvertStringToDateTime(FinalCloseTimer);
            Log.Debug($"血色城堡房间将在 {finalTimeDate.ToString()} 后关闭，请尽快退出房间");*/
            await Root.MainFactory.GetCustomComponent<TimerComponent>().WaitAsync(1000);
            battleCopyRoom.ClearRoom();
            battleCopyRoom.Dispose();
            self.battleCopyRoomDic[level].Remove(Index);
  
        }

        public static MapComponent Get(this BattleCopyComponent self, long userId)
        {
            if (!self.copyRankDataDic.ContainsKey(userId)) return null;
            long level = self.copyRankDataDic[userId].Level;
            int index = self.copyRankDataDic[userId].Index;
            Log.PLog($"RemoveUser Player:{userId} Level:{level}Index:{index}");
            return self.battleCopyRoomDic[level][index].mapComponent;
        }
    }
}
