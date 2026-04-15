using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_StartGameGamePlayerRequestHandler : AMActorRpcHandler<C2G_StartGameGamePlayerRequest, G2C_StartGameGamePlayerResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session session, C2G_StartGameGamePlayerRequest b_Request, G2C_StartGameGamePlayerResponse b_Response, Action<IMessage> b_Reply)
        {
            // b_Request.ActorId = UserId
            // 这条消息的 ActorId 是 UserId
            // b_Request.GameUserId 是 ActorId
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.GameUserId))
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGame, b_Request.ActorId))
                {
                    return await base.BeforeCodeAsync(session, b_Request, b_Response, b_Reply);
                }
            }
        }

        protected override async Task<bool> Run(C2G_StartGameGamePlayerRequest b_Request, G2C_StartGameGamePlayerResponse b_Response, Action<IMessage> b_Reply)
        {
            if (Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().ServerStopping)
            {
                // 服务器正在关闭，禁止进入
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(114);
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
            GameUser mGameUser = Root.MainFactory.GetCustomComponent<GameUserComponent>().GetPlayer(b_Request.ActorId);
            if (mGameUser == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(120);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
                b_Reply(b_Response);
                return false;
            }
            if (mGameUser.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(121);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
                b_Reply(b_Response);
                return false;
            }

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();

            DataCacheManageComponent mDataCacheComponent = mGameUser.AddCustomComponent<DataCacheManageComponent>();
            var mDataCache = mDataCacheComponent.Get<DBGamePlayerData>();
            if (mDataCache == null)
            {
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)mGameUser.GameAreaId);
                mDataCache = await mDataCacheComponent.Add<DBGamePlayerData>(dBProxy2, p => p.UserId == mGameUser.UserId
                                                                                         && p.GameAreaId == mGameUser.GameAreaId
                                                                                         && p.IsDisposePlayer == 0, mGameUser.GameAreaId);
            }
            if (mDataCache.ContainsKey(mGameUser.GameAreaId) == false)
            {
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mGameUser.GameAreaId);
                var mInitResult = await mDataCache.DataQueryInit(dBProxy2, p => p.UserId == mGameUser.UserId
                                                                             && p.GameAreaId == mGameUser.GameAreaId
                                                                             && p.IsDisposePlayer == 0, mGameUser.GameAreaId);
                if (mInitResult == false)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("角色不存在!");
                    b_Reply(b_Response);
                    return false;
                }
            }

            // 查找角色
            var mDatalist = mDataCache.DataQuery(p => p.Id == b_Request.GameUserId
                                                    && p.UserId == mGameUser.UserId
                                                    && p.GameAreaId == mGameUser.GameAreaId
                                                    && p.IsDisposePlayer == 0, mGameUser.GameAreaId);

            if (mDatalist == null || mDatalist.Count <= 0)
            {
                Log.Error($"有多条角色数据 UserID:{mGameUser.UserId} GameUserId:{b_Request.GameUserId}");
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1513);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("数据库异常!");
                b_Reply(b_Response);
                return false;
            }
            var mData = mDatalist[0];
            mData.DeSerialize();



            PlayerManageComponent mPlayerManageComponent = Root.MainFactory.GetCustomComponent<PlayerManageComponent>();
            Player stalePlayer = mPlayerManageComponent.UnsafeGet(mGameUser.GameAreaId, b_Request.GameUserId);
            if (stalePlayer != null && stalePlayer.OnlineStatus != EOnlineStatus.Online)
            {
                Log.Warning($"#RoleFlow# GameStartGame clear stale player userId={mGameUser.UserId} gameUserId={b_Request.GameUserId} status={stalePlayer.OnlineStatus}");

                Player removedPlayer = mPlayerManageComponent.Remove(mGameUser.GameAreaId, b_Request.GameUserId);
                if (ReferenceEquals(mGameUser.Player, stalePlayer))
                {
                    mGameUser.Player = null;
                }

                if (removedPlayer != null && removedPlayer.IsDisposeable == false)
                {
                    removedPlayer.Dispose();
                }
            }

            Player mPlayer = mPlayerManageComponent.AddPlayerByUserID(mGameUser.GameAreaId, b_Request.GameUserId);
            mPlayer.UserId = mGameUser.UserId;
            mPlayer.GateServerId = mGameUser.GateServerId;
            mPlayer.SourceGameAreaId = (int)b_Request.AppendData;
            mPlayer.OnlineStatus = EOnlineStatus.StartGame;
            mGameUser.Player = mPlayer;

            bool isCreatePlayer = false;
            DataCacheManageComponent mDataCacheComponent2 = mPlayer.AddCustomComponent<DataCacheManageComponent>();
            // 初始化玩家移动信息
            var mDataCache2 = mDataCacheComponent2.Get<DBPlayerUnitData>();
            if (mDataCache2 == null)
            {
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                mDataCache2 = await HelpDb_DBPlayerUnitData.Init(mPlayer, mDataCacheComponent2, dBProxy2);
            }
            var mDatalist2 = mDataCache2.DataQuery(p => p.GameUserId == b_Request.GameUserId);
            if (mDatalist2.Count == 0)
            {
                isCreatePlayer = true;

                var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<CreateRole_InfoConfigJson>().JsonDic;

                if (mJsonDic.TryGetValue(mData.PlayerTypeId, out var mConfig) == false)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(305);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("数据异常!");
                    b_Reply(b_Response);
                    return true;
                }

                int tempRandPosX = 0;
                int tempRandPosY = 0;
                var mMapComponent = mServerArea.GetCustomComponent<MapManageComponent>().GetMapByMapIndex(mConfig.InitMap);
                var mSafeAreaValues = mMapComponent.SpawnSafeFindTheWayDic.Values.ToArray();
                if (mSafeAreaValues.Length > 0)
                {
                    int mRandomIndex = Help_RandomHelper.Range(0, mSafeAreaValues.Length);
                    var mRandemValueDic = mSafeAreaValues[mRandomIndex];

                    var mSafeAreaDicKeys = mRandemValueDic.Keys.ToArray();
                    int mRandomKeyIndex = Help_RandomHelper.Range(0, mSafeAreaDicKeys.Length);
                    var mRandemKeyValue = mSafeAreaDicKeys[mRandomKeyIndex];
                    var mRandemValue = mRandemValueDic[mRandemKeyValue];

                    tempRandPosX = mRandemValue.X;
                    tempRandPosY = mRandemValue.Y;
                }
                DBPlayerUnitData mUnitDataTemp = new DBPlayerUnitData()
                {
                    Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId),
                    GameUserId = b_Request.GameUserId,
                    index = mConfig.InitMap,
                    x = tempRandPosX,
                    y = tempRandPosY,
                    Hp = 100,
                };

                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                bool mSaveResult = await dBProxy.Save(mUnitDataTemp);
                if (mSaveResult == false)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1514);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("删除失败!");
                    b_Reply(b_Response);
                    return false;
                }
                mDataCache2.DataAdd(mUnitDataTemp);

                mDatalist2 = mDataCache2.DataQuery(p => p.GameUserId == b_Request.GameUserId);
            }

            if (mDatalist2.Count <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1500);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏异常!");
                b_Reply(b_Response);
                return false;
            }

            DBPlayerUnitData mUnitData = mDatalist2[0];

            GamePlayer mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            if (mGamePlayer == null)
            {
                mGamePlayer = mPlayer.AddCustomComponent<GamePlayer>();
                mGamePlayer.Player = mPlayer;
            }

            mGamePlayer.SetData(mData, mUnitData);
            mGamePlayer.AfterAwake();
            mGamePlayer.SetInstanceId(mUnitData.GameUserId);

            if (isCreatePlayer)
            {
                mUnitData.Hp = mGamePlayer.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                mUnitData.Mp = mGamePlayer.GetNumerialFunc(E_GameProperty.PROP_MP_MAX);
                mUnitData.SD = mGamePlayer.GetNumerialFunc(E_GameProperty.PROP_SD_MAX);
                mUnitData.AG = mGamePlayer.GetNumerialFunc(E_GameProperty.PROP_AG_MAX);

                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
                mWriteDataComponent.Save(mUnitData, dBProxy).Coroutine();
            }

            Log.PLog("Login", $"a:{mPlayer.UserId} r:{mPlayer.GameUserId} 玩家角色上线");

            // 游戏上线日志
            DbLoginLog dbChangePasswordLog = new DbLoginLog()
            {
                LoginLogType = ELoginLogType.GameOnline,
                UserId = mPlayer.UserId,
                GameUserId = mPlayer.GameUserId,
                GameServerId = OptionComponent.Options.AppId,
                CreateTime = TimeHelper.Now(),
            };
            DBLogHelper.Write(dbChangePasswordLog);

            b_Response.GameUserId = mPlayer.GameUserId;

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
                    string Response = await RealName.LoginOrOut(mPlayer.GameUserId.ToString(), 1, 0, "", dbLoginInfo.Pi);
                    LoginOutInfo loginOutInfo  = Help_JsonSerializeHelper.DeSerialize<LoginOutInfo>(Response);
                    if (loginOutInfo.errcode != 0 )
                    {
                        Log.PLog($"游戏行为上报 PlayersId:{mPlayer.GameUserId}errcode:{loginOutInfo.errcode} errmsg:{loginOutInfo.errmsg}Data:{loginOutInfo.data}");
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1513);
                        b_Reply(b_Response);
                        return false;
                    }
                }
            }
            b_Reply(b_Response);
            return true;
        }
    }
}
