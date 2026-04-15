using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_LoginSystemGetGamePlayerInfoRequestHandler : AMActorRpcHandler<C2G_LoginSystemGetGamePlayerInfoRequest, G2C_LoginSystemGetGamePlayerInfoResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_LoginSystemGetGamePlayerInfoRequest b_Request, G2C_LoginSystemGetGamePlayerInfoResponse b_Response, Action<IMessage> b_Reply)
        {
            // 这条消息的 ActorId 是 UserId
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGame, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_LoginSystemGetGamePlayerInfoRequest b_Request, G2C_LoginSystemGetGamePlayerInfoResponse b_Response, Action<IMessage> b_Reply)
        {
            Log.Info($"#RoleFlow# GameGetGamePlayerInfo start actor={b_Request.ActorId} gameIdCount={b_Request.GameId?.count ?? 0}");
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

            DataCacheManageComponent mDataCacheManageComponent = mGameUser.AddCustomComponent<DataCacheManageComponent>();
            var mDataCache = mDataCacheManageComponent.Get<DBGamePlayerData>();
            if (mDataCache == null)
            {
                var mDBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mGameUser.GameAreaId);
                mDataCache = await mDataCacheManageComponent.Add<DBGamePlayerData>(mDBProxy, p => p.UserId == mGameUser.UserId
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

            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<CreateRole_InfoConfigJson>().JsonDic;

            for (int i = 0, len = b_Request.GameId.count; i < len; i++)
            {
                long mGameId = b_Request.GameId[i];

                // 查找角色
                var mGamePlayerlist = mDataCache.DataQuery(p => p.Id == mGameId
                                                             && p.UserId == mGameUser.UserId
                                                             && p.GameAreaId == mGameUser.GameAreaId
                                                             && p.IsDisposePlayer == 0, mGameUser.GameAreaId);

                if (mGamePlayerlist == null || mGamePlayerlist.Count == 0)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("数据库异常!");
                    b_Reply(b_Response);
                    return false;
                }

                // 角色数据
                DBGamePlayerData mGamePlayerData = mGamePlayerlist[0];

                // 配置不存在
                if (mJsonDic.TryGetValue(mGamePlayerData.PlayerTypeId, out var mIConfig) == false)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(308);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("数据异常, 角色类型不存在!");
                    b_Reply(b_Response);
                    return false;
                }
                G2C_LoginSystemGetGamePlayerInfoMessage message = new G2C_LoginSystemGetGamePlayerInfoMessage()
                {
                    GameId = mGamePlayerData.Id,
                    NickName = mGamePlayerData.NickName,
                    PlayerType = mGamePlayerData.PlayerTypeId,
                    Level = mGamePlayerData.Level,
                    OccupationLevel = mGamePlayerData.OccupationLevel,
                };

                //添加装备信息 bfy20230227
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)mGameUser.GameAreaId);
                // 查找角色装备信息
                List<ComponentWithId> allEquipItem = await dBProxy2.Query<DBItemData>(p => p.GameUserId == mGamePlayerData.Id
                                                                           && p.InComponent == EItemInComponent.Equipment
                                                                           && p.IsDispose == 0);
                foreach(ComponentWithId com in allEquipItem)
                {
                    DBItemData mitem = com as DBItemData;
                    int level = 0;
                    mitem.PropertyData.TryGetValue((int)EItemValue.Level, out level);
                    message.AllEquipStatus.Add(new G2C_LoginSystemEquipItemMessage()
                    {
                        GameUserId = mGamePlayerData.Id,
                        ConfigID = mitem.ConfigID,
                        ItemUID = mitem.Id,
                        SlotID = mitem.posId,
                        ItemLevel = level,
                    });
                }
                //End bfy20230227
                b_Response.GameInfos.Add(message);
            }

            Log.Info($"#RoleFlow# GameGetGamePlayerInfo finish actor={b_Request.ActorId} roleCount={b_Response.GameInfos.Count} error={b_Response.Error}");
            b_Reply(b_Response);
            return true;
        }
    }
}
