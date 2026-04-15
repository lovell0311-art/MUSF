using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_LoginSystemDeleteGamePlayerRequestHandler : AMActorRpcHandler<C2G_LoginSystemDeleteGamePlayerRequest, G2C_LoginSystemDeleteGamePlayerResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_LoginSystemDeleteGamePlayerRequest b_Request, G2C_LoginSystemDeleteGamePlayerResponse b_Response, Action<IMessage> b_Reply)
        {
            // 这条消息的 ActorId 是 UserId
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGame, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_LoginSystemDeleteGamePlayerRequest b_Request, G2C_LoginSystemDeleteGamePlayerResponse b_Response, Action<IMessage> b_Reply)
        {
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
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)mGameUser.GameAreaId);

            DataCacheManageComponent mDataCacheManageComponent = mGameUser.AddCustomComponent<DataCacheManageComponent>();
            var mDataCache = mDataCacheManageComponent.Get<DBGamePlayerData>();
            if (mDataCache == null)
            {
                mDataCache = await mDataCacheManageComponent.Add<DBGamePlayerData>(dBProxy2, p => p.UserId == mGameUser.UserId
                                                                                               && p.GameAreaId == mGameUser.GameAreaId
                                                                                               && p.IsDisposePlayer == 0, mGameUser.GameAreaId);
            }
            if (mDataCache.ContainsKey(mGameUser.GameAreaId) == false)
            {
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
            var mDatalist = mDataCache.DataQuery(p => p.Id == b_Request.GameId 
                                                    && p.GameAreaId == mGameUser.GameAreaId
                                                    && p.UserId == mGameUser.UserId
                                                    && p.IsDisposePlayer == 0, mGameUser.GameAreaId);

            if (mDatalist == null || mDatalist.Count <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("数据库异常!");
                b_Reply(b_Response);
                return false;
            }

            var mData = mDatalist[0];
            mData.IsDisposePlayer = 1;

            bool mSaveResult = await dBProxy2.Save(mData);
            if (mSaveResult == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(307);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("删除失败!");
            }

            b_Reply(b_Response);
            return true;
        }
    }
}