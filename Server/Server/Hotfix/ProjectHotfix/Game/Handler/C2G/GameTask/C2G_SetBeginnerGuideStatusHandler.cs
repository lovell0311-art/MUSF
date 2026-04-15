
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_SetBeginnerGuideStatusHandler : AMActorRpcHandler<C2G_SetBeginnerGuideStatus,
        G2C_SetBeginnerGuideStatus>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_SetBeginnerGuideStatus b_Request, G2C_SetBeginnerGuideStatus b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_SetBeginnerGuideStatus b_Request, G2C_SetBeginnerGuideStatus b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
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

            GamePlayer gamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            gamePlayer.Data.BeginnerGuideStatus = b_Request.Value;
            mPlayer.PLog($"设置新手引导进度 b_Request.Value {gamePlayer.Data.BeginnerGuideStatus} => {b_Request.Value}");

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
            mWriteDataComponent.Save(gamePlayer.Data, dBProxy).Coroutine();

            b_Reply(b_Response);
            return true;

        }
    }
}