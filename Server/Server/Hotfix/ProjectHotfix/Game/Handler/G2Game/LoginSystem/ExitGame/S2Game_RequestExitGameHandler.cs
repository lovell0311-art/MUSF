using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TencentCloud.Bri.V20190328.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class S2Game_RequestExitGameHandler : AMRpcHandler<S2Game_RequestExitGame, Game2S_RequestExitGame>
    {
        protected override async Task<bool> Run(S2Game_RequestExitGame b_Request, Game2S_RequestExitGame b_Response, Action<IMessage> b_Reply)
        {
            GameUser gameUser = Root.MainFactory.GetCustomComponent<GameUserComponent>().GetPlayer(b_Request.UserId);
            if (gameUser == null)
            {
                b_Reply(b_Response);
                return true;
            }
            await gameUser._OfflineAsync();
            // 等待两帧
            await ETModel.ET.TimerComponent.Instance.WaitFrameAsync();
            await ETModel.ET.TimerComponent.Instance.WaitFrameAsync();
            b_Reply(b_Response);
            return true;
        }
    }


    public static class OfflineHelper
    {
        /// <summary>
        /// 下线逻辑
        /// </summary>
        private static void OfflineLogic(GameUser mGameUser, Player mPlayer, C_ServerArea mServerArea)
        {
            // 玩家下线时，会先执行当前函数中的代码
            // 写下线逻辑时不需要考虑异步操作（上下文切换后，如操作其他玩家，状态变动问题），当单线程写就可以

            // 调用此函数前，已经使用 ActorId、LoginGame 协程锁

            // mGameUser、mPlayer 中的 DataCacheManageComponent 会在数据，会在完全下线后落地

            PlayerManageComponent mPlayerManageComponent = Root.MainFactory.GetCustomComponent<PlayerManageComponent>();

            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();

            #region 初次进入游戏
            if (mGamePlayer != null && mGamePlayer.Data.NewArchive == 1)
            {
                // 角色存档第一次进入游戏
                mGamePlayer.Data.NewArchive = 0;

                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
                mWriteDataComponent.Save(mGamePlayer.Data, dBProxy2).Coroutine();
            }
            C_DataCache<DBAccountZoneData> dataCacheAccountData = mPlayer.GetCustomComponent<DataCacheManageComponent>().Get<DBAccountZoneData>();
            if (dataCacheAccountData != null)
            {
                DBAccountZoneData dbAccountData = dataCacheAccountData.DataQueryById(mPlayer.UserId);
                if (dbAccountData != null && dbAccountData.NewAccount == 1)
                {
                    // 账号第一次进入游戏
                    dbAccountData.NewAccount = 0;

                    DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                    var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                    var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
                    mWriteDataComponent.Save(dbAccountData, dBProxy2).Coroutine();
                }
            }
            #endregion


            //在副本时退出副本
            if (BatteCopyManagerComponent.BattleCopyMapIDList.Contains(mGamePlayer.UnitData.Index))
            {
                bool result = mServerArea.GetCustomComponent<BatteCopyManagerComponent>().PlayerDeathHandler(mGamePlayer, mServerArea);
                Log.Debug("离线退出副本:" + result);
            }
            mServerArea.GetCustomComponent<BatteCopyManagerComponent>().PlayerDeathTrialTowerHandler(mGamePlayer, mServerArea);
            //防止意外掉线，清理藏宝阁缓存
            { 
                G2M_DeletePlayerTreasureHouse g2M_DeletePlayerTreasureHouse = new G2M_DeletePlayerTreasureHouse { GameUserID = mPlayer.GameUserId };
                mPlayer.GetSessionMGMT().Send(g2M_DeletePlayerTreasureHouse);
            }
            string NickName = mGamePlayer.Data.NickName;
            //if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(mGamePlayer.UnitData.Index, out var mapComponent))
            var mapComponent = mGamePlayer.CurrentMap;
            if (mapComponent != null)
            {
                G2C_PlayerSessionDisconnect_notice mPlayerSessionDisconnectNotice = new G2C_PlayerSessionDisconnect_notice();
                mPlayerSessionDisconnectNotice.InstanceId = mGamePlayer.InstanceId;
                mPlayerSessionDisconnectNotice.PetsID = mGamePlayer.Pets != null ? mGamePlayer.Pets.InstanceId : 0;
                mapComponent.SendNotice(mGamePlayer, mPlayerSessionDisconnectNotice);

                var mFindTheWay = mapComponent.GetFindTheWay2D(mGamePlayer);
                var mMapCellField = mGamePlayer.CurrentCell;
                if (mMapCellField != null)
                {
                    mapComponent.Leave(mGamePlayer);
                    Log.Debug("移除地图中的缓存");
                    if (mMapCellField.MapStallDic.TryGetValue(mPlayer.GameUserId, out var mMapStall))
                    {
                        mMapCellField.MapStallDic.Remove(mPlayer.GameUserId);

                        if (mMapStall.IsStalling)
                        {
                            G2C_BaiTanClose_notice mBaiTanCloseMessage = new G2C_BaiTanClose_notice();
                            mBaiTanCloseMessage.BaiTanInstanceId.Add(mPlayer.GameUserId);
                            mapComponent.SendNotice(mGamePlayer, mBaiTanCloseMessage);
                        }
                        mMapStall.Dispose();
                    }
                    if (mGamePlayer.Summoned != null && mGamePlayer.Summoned.IsDisposeable == false)
                    {
                        mGamePlayer.Summoned.IsDeath = true;
                        mGamePlayer.Summoned.IsReallyDeath = true;
                    }
                    if (mGamePlayer.HolyteacherSummoned != null && mGamePlayer.HolyteacherSummoned.IsDisposeable == false)
                    {
                        mGamePlayer.HolyteacherSummoned.IsDeath = true;
                        mGamePlayer.HolyteacherSummoned.IsReallyDeath = true;
                    }
                }
            }

            if (mGamePlayer.Pets != null)
            {
                var mapComponent1 = mGamePlayer.Pets.CurrentMap;
                if (mapComponent1 != null)
                {
                    mapComponent1.Leave(mGamePlayer.Pets);
                    mGamePlayer.Pets.IsDeath = true;
                }
            }
            //移除聊天室中的缓存
            ChatManageComponent chatManager = Root.MainFactory.GetCustomComponent<ChatManageComponent>();
            if (chatManager != null)
            {
                chatManager.PlayerDispose(mPlayer);
            }

            //离线自动离开组队
            TeamManageComponent teamManager = Root.MainFactory.GetCustomComponent<TeamManageComponent>();
            if (teamManager != null)
            {
                teamManager.PlayerDispose(mPlayer);
            }

            //离线终止交易
            ExchangeComponent exchangeComponent = mPlayer.GetCustomComponent<ExchangeComponent>();
            if (exchangeComponent != null)
            {
                long targetPlayerID = exchangeComponent.ExchangeTargetGameUserId;
                if (targetPlayerID > 0)
                {
                    Player targetPlayer = mPlayerManageComponent.Get((int)mPlayer.GameAreaId, exchangeComponent.ExchangeTargetGameUserId);
                    if (targetPlayer != null)
                    {
                        targetPlayer.GetCustomComponent<ExchangeComponent>().EndExchange();
                        exchangeComponent.EndExchange();
                        targetPlayer.Send(new G2C_ExchangeResult_notice()
                        {
                            State = false,
                            ErrorMessage = "对方已下线，交易终止!"
                        });
                    }
                }
            }

            //离线将合成缓存区物品放入背包(放不进的会掉落)
            var synthesisComponent = mPlayer.GetCustomComponent<SynthesisComponent>();
            if (synthesisComponent != null)
            {
                synthesisComponent.PlayerDispose();
            }


            var ZhanMeng = mPlayer.GetCustomComponent<PlayerWarAllianceComponent>();
            if (ZhanMeng != null && ZhanMeng.WarAllianceID != 0)
            {
                ZhanMeng.UpDateWarAlliancePlayerInfo(1);
            }

            var City = mServerArea.GetCustomComponent<CitySiegeActivities>();
            if (City != null && City.GetSate())
            {
                if (City.SupremeThrone == mPlayer.GameUserId)
                {
                    City.LeaveTiem = Help_TimeHelper.GetNowSecond();
                    G2C_SendPointOutMessage g2C_SendPointOutMessage = new G2C_SendPointOutMessage();
                    g2C_SendPointOutMessage.Status = 2;
                    g2C_SendPointOutMessage.Pointout = 0;
                    g2C_SendPointOutMessage.PlayerName = "";
                    g2C_SendPointOutMessage.WarName = "";
                    g2C_SendPointOutMessage.TitleName = 0;
                    g2C_SendPointOutMessage.Time = 0;
                    City.SendMessage(g2C_SendPointOutMessage);
                }
            }
        }

        /// <summary>
        /// 下线逻辑，异步
        /// </summary>
        public static async Task OfflineLogicAsync(GameUser mGameUser, Player mPlayer, C_ServerArea mServerArea)
        {
            // 玩家下线时，当 OfflineLogic 执行完成后，会调用当前函数

            // 注意！注意！注意！注意！注意！注意！注意！注意！
            // 注意！注意！注意！注意！注意！注意！注意！注意！
            // 注意！注意！注意！注意！注意！注意！注意！注意！
            // 尽可能不要在此方法中写下线逻辑
            // 如无法避免在此方法中调用下线逻辑，务必要考虑清楚异步操作时，可能发生的问题。

            // 比如：
            // 操作其他玩家，对方玩家是不是正在下线。
            // 刚获取其他玩家时，状态是正常的。中途调用 await 后，玩家就下线了。
            // 这时需要怎么办？下线逻辑可以中断吗？修改需要存入数据库的数据会不会因为这种情况丢失？

            // 下线逻辑尽量避免使用异步

            // 调用此函数前，已经使用 ActorId、LoginGame 协程锁



        }


        #region 没看明白时，禁止修改此代码
        public static async Task<bool> _OfflineAsync(this GameUser gameUser)
        {
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(gameUser.AppendData);
            if (mServerArea == null)
            {
                return true;
            }
            Player mPlayer = null;
            #region 情况1: 请求了 G2Game_EnterGameAreaMessage ，没请求 C2G_StartGameGamePlayerRequest
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGame, gameUser.UserId))
            {
                // 情况1: 请求了 G2Game_EnterGameAreaMessage ，没请求 C2G_StartGameGamePlayerRequest
                if (gameUser.IsDisposeable) return true;
                mPlayer = gameUser.Player;
                if (mPlayer == null)
                {
                    // 玩家只进入了选角色界面，没进入场景
                    Log.PLog("Login", $"a:{gameUser.UserId} 玩家断开连接");
                    Root.MainFactory.GetCustomComponent<GameUserComponent>().Remove(gameUser.UserId);
                    return true;
                }
            }
            #endregion

            long instanceId = mPlayer.Id;

            long gameUserId = mPlayer.GameUserId;
            long userId = gameUser.UserId;


            #region 情况3: 玩家正常在线
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, gameUserId))
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGame, userId))
            {
                // 情况3: 玩家正常在线
                if (mPlayer.Id != instanceId)
                {
                    Log.Error($"角色 实例Id 更改  {instanceId} : {mPlayer.Id}");
                    return true;
                }
                // 情况2: 玩家没有正常进入游戏 或 下线失败，处于其他状态
                PlayerManageComponent mPlayerManageComponent = Root.MainFactory.GetCustomComponent<PlayerManageComponent>();
                if (mPlayer.OnlineStatus != EOnlineStatus.Online)
                {
                    mPlayer.OnlineStatus = EOnlineStatus.Offline;
                    if (mPlayerManageComponent != null)
                    {
                        mPlayerManageComponent.Remove((int)mPlayer.GameAreaId, mPlayer.GameUserId);
                        Log.PLog("Login", $"a:{mPlayer.UserId} r:{mPlayer.GameUserId} 玩家角色下线");
                        mPlayer.Dispose();
                        Log.PLog("Login", $"a:{gameUser.UserId} 玩家断开连接");
                        Root.MainFactory.GetCustomComponent<GameUserComponent>().Remove(gameUser.UserId);
                    }
                    return true;
                }

                long startTime = Help_TimeHelper.GetNow();
                // TODO 玩家下线逻辑
                mPlayer.OnlineStatus = EOnlineStatus.Offline;
                Log.PLog("Login", $"a:{mPlayer.UserId} r:{mPlayer.GameUserId} 玩家角色开始下线");

                // 情况3: 玩家正常在线
                if (mPlayerManageComponent != null)
                {
                    mPlayerManageComponent.Remove((int)mPlayer.GameAreaId, mPlayer.GameUserId);

                    OfflineLogic(gameUser, mPlayer, mServerArea);

                    // 暂停一帧，使异步方法有问题时，早点发现
                    await ETModel.ET.TimerComponent.Instance.WaitFrameAsync();

                    await OfflineLogicAsync(gameUser, mPlayer, mServerArea);

                    // TODO 落地数据
                    var mDataCacheManageComponent = mPlayer.GetCustomComponent<DataCacheManageComponent>();
                    if (mDataCacheManageComponent != null)
                    {
                        await mDataCacheManageComponent.ForceSave();
                    }
                    mDataCacheManageComponent = gameUser.GetCustomComponent<DataCacheManageComponent>();
                    if (mDataCacheManageComponent != null)
                    {
                        await mDataCacheManageComponent.ForceSave();
                    }
                    Log.PLog("Login", $"a:{mPlayer.UserId} r:{mPlayer.GameUserId} 玩家角色下线 time = {Help_TimeHelper.GetNow() - startTime}");

                    // 游戏下线日志
                    DbLoginLog dbChangePasswordLog = new DbLoginLog()
                    {
                        LoginLogType = ELoginLogType.GameOffline,
                        UserId = mPlayer.UserId,
                        GameUserId = mPlayer.GameUserId,
                        GameServerId = OptionComponent.Options.AppId,
                        CreateTime = TimeHelper.Now(),
                    };
                    DBLogHelper.Write(dbChangePasswordLog);
                    if (RealName.EnableOrNot)
                    {
                        DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                        var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);
                        DBAccountInfo dbLoginInfo = null;
                        if (mDBProxy != null)
                        {
                            {
                                var list = await mDBProxy.Query<DBAccountInfo>(p => p.Id == mPlayer.UserId);
                                if (list.Count > 0)
                                {
                                    dbLoginInfo = list[0] as DBAccountInfo;
                                }
                            }
                        }
                        if (dbLoginInfo != null)
                        {
                            string Response = await RealName.LoginOrOut(mPlayer.GameUserId.ToString(), 0, 0, "", dbLoginInfo.Pi);
                            LoginOutInfo loginOutInfo = Help_JsonSerializeHelper.DeSerialize<LoginOutInfo>(Response);
                            if (loginOutInfo.errcode != 0)
                            {
                                Log.PLog($"游戏行为上报 PlayersId:{mPlayer.GameUserId}errcode:{loginOutInfo.errcode} errmsg:{loginOutInfo.errmsg}Data:{loginOutInfo.data}");
                            }
                        }
                    }
                    mPlayer.Dispose();
                    Log.PLog("Login",$"a:{gameUser.UserId} 玩家断开连接");
                    Root.MainFactory.GetCustomComponent<GameUserComponent>().Remove(gameUser.UserId);
                }
            }
            #endregion
            return true;
        }

        #endregion


    }

}