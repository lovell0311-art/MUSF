
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_ReadyRequestHandler2 : AMActorRpcHandler<C2G_ReadyRequest, G2C_ReadyResponse>
    {
        protected override Task<bool> BeforeCodeAsync(Session b_Connect, C2G_ReadyRequest b_Request, G2C_ReadyResponse b_Response, Action<IMessage> b_Reply)
        {
            return base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
        }

        protected override async Task<bool> CodeAsync(Session session, C2G_ReadyRequest b_Request, G2C_ReadyResponse b_Response, Action<IMessage> b_Reply)
        {
            SessionGateUserComponent mConnectGate = session.GetComponent<SessionGateUserComponent>();
            if (mConnectGate == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(112);
                b_Reply(b_Response);
                return false;
            }
            GatePlayer mGatePlayer = Root.MainFactory.GetCustomComponent<GatePlayerComponent>().Get(mConnectGate.GameUserId);
            if (mGatePlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(120);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家不存在!");
                b_Reply(b_Response);
                return false;
            }
            
            b_Request.ActorId = mConnectGate.GameUserId;
            b_Request.AppendData = (long)((uint)mConnectGate.GameAreaId << 16 | (uint)mConnectGate.GameAreaLineId);
            int mCallTagId = b_Request.RpcId;

            var ipEndPoint = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(mConnectGate.GameServerId);
            Session mGameSession = Game.Scene.GetComponent<NetInnerComponent>().Get(ipEndPoint.ServerInnerIP);
            var mGameServerResult = await mGameSession.Call(b_Request);
            if (mGameServerResult == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21004);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("没有匹配到有效的Game服务器!");
                b_Reply(b_Response);
                return false;
            }

            mGameServerResult.RpcId = mCallTagId;
            b_Reply(mGameServerResult);
            return true;
        }
    }
}