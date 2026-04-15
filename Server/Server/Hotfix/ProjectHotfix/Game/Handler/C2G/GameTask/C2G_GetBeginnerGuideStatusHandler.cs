
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_GetBeginnerGuideStatusHandler : AMActorRpcHandler<C2G_GetBeginnerGuideStatus,
        G2C_GetBeginnerGuideStatus>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_GetBeginnerGuideStatus b_Request, G2C_GetBeginnerGuideStatus b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_GetBeginnerGuideStatus b_Request, G2C_GetBeginnerGuideStatus b_Response, Action<IMessage> b_Reply)
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

            b_Response.Value = gamePlayer.Data.BeginnerGuideStatus;

            b_Reply(b_Response);
            return true;

        }
    }
}