using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CustomFrameWork;
using ETModel;
using System.Linq;


namespace ETHotfix
{
    public static class Help_GameServerHelper
    {
        /// <summary>
        /// 关闭服务器
        /// </summary>
        /// <returns></returns>
        public static async Task Shutdown()
        {
            Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().ServerStopping = true;


            #region 下线账号
            // TODO 下线全部玩家
            Log.Console($"开始下线所有账号");
            GameUserComponent gameUserManager = Root.MainFactory.GetCustomComponent<GameUserComponent>();
            for (int i = 0; i < 12; ++i)
            {
                List<GameUser> gameUserList = gameUserManager.GetGameUsers();
                foreach (GameUser gameUser in gameUserList)
                {
                    gameUser.OfflineFromGate(DisconnectType.ServerShutdown).Coroutine();
                }
                for(int j = 0;j<5;++i)
                {
                    await ETModel.ET.TimerComponent.Instance.WaitAsync(1000);
                    if(gameUserManager.GetUserCount() == 0)
                    {
                        // 下线完成
                        break;
                    }
                }
                if (gameUserManager.GetUserCount() == 0)
                {
                    // 下线完成
                    break;
                }
            }
            
            while(gameUserManager.GetUserCount() > 0)
            {
                // 极端情况，通过Gate没能将玩家全部下线
                Log.Warning($"还有 {gameUserManager.GetUserCount()} 玩家没有下线");
                List<GameUser> gameUserList = gameUserManager.GetGameUsers();
                foreach (GameUser gameUser in gameUserList)
                {
                    if (gameUser.IsDisposeable) continue;
                    await OfflineHelper._OfflineAsync(gameUser);
                }
                await ETModel.ET.TimerComponent.Instance.WaitAsync(1000);
            }
            Log.Console($"所有账号下线完成");
            #endregion

            #region 销毁MapEntity
            // TODO 所有账号下线完成
            Log.Console($"开始销毁所有 MapEntity");
            ServerAreaManagerComponent serverAreaManager = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>();
            foreach(var serverArea in serverAreaManager.GameAreaOnServerDic.Values)
            {
                // 世界地图
                MapManageComponent mapManage = serverArea.GetCustomComponent<MapManageComponent>();
                foreach(MapComponent map in mapManage.keyValuePairs.Values)
                {
                    MapEntity[] allEntity = map.AllEntities.Values.ToArray();
                    foreach(MapEntity entity in allEntity)
                    {
                        entity.Dispose();
                    }
                }
                // 副本地图
                BatteCopyManagerComponent batteCopyManager = serverArea.GetCustomComponent<BatteCopyManagerComponent>();
                foreach (BattleCopyComponent batteCopy in batteCopyManager.battleCopyMap.Values)
                {
                    foreach(var roomList in batteCopy.battleCopyRoomDic.Values)
                    {
                        foreach(var room in roomList)
                        {
                            if(room.Value.mapComponent != null)
                            {
                                MapEntity[] allEntity2 = room.Value.mapComponent.AllEntities.Values.ToArray();
                                foreach (MapEntity entity in allEntity2)
                                {
                                    entity.Dispose();
                                }
                            }
                        }
                    }
                }
            }
            Log.Console($"所有 MapEntity 已销毁");
            #endregion

            #region 落地数据
            // TODO 落地数据
            Log.Console($"开始落地数据");

            DBMongodbProxySaveManageComponent DBMongodbProxySaveManage = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>();
            C_MongodbProxySave[] mongodbProxySave = DBMongodbProxySaveManage._Instance.Values.ToArray();
            foreach(C_MongodbProxySave proxySave in mongodbProxySave)
            {
                await proxySave.SaveAllAsync();
            }

            Log.Console($"数据落地完成");
            #endregion

            NLog.LogManager.Shutdown();
            System.Environment.Exit(0);
        }

        public static async Task ShutdownWait(long time)
        {
            Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().ServerStopping = true;

            long tillTime = Help_TimeHelper.GetNow() + time;
            GameUserComponent gameUserManager = Root.MainFactory.GetCustomComponent<GameUserComponent>();
            List<GameUser> gameUserList = gameUserManager.GetGameUsers();
            foreach (GameUser gameUser in gameUserList)
            {
                gameUser.Player?.Send(new Game2C_ServerShutdownTime()
                {
                    ShutdownTillTime = tillTime
                });
            }
            for (long t = time; t > 0; t -= 1000)
            {
                Log.Warning($"{t / 1000} 秒后关闭服务器");
                await ETModel.ET.TimerComponent.Instance.WaitAsync(1000);
            }

            await Shutdown();
        }

    }
}
