using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_LoginSystemDeleteGamePlayerRequestHandler2 : AMActorRpcHandler<C2G_LoginSystemDeleteGamePlayerRequest, G2C_LoginSystemDeleteGamePlayerResponse>
    {
        protected override async Task<bool> CodeAsync(Session session, C2G_LoginSystemDeleteGamePlayerRequest b_Request, G2C_LoginSystemDeleteGamePlayerResponse b_Response, Action<IMessage> b_Reply)
        {
            var netConnectGate = session.GetComponent<SessionGateUserComponent>();
            if (netConnectGate == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(112);
                b_Reply(b_Response);
                return false;
            }
            if (netConnectGate.TransferServerId == 0)
            {
                Log.Error($"玩家找不到或者ID为0 {netConnectGate.TransferServerId == 0}\n GameRouteMessage {b_Request.GetType().Name} : {Newtonsoft.Json.JsonConvert.SerializeObject(b_Request)}");
                return false;
            }
            int rpcId = b_Request.RpcId; // 这里要保存客户端的rpcId
            long instanceId = session.InstanceId;

            b_Request.ActorId = netConnectGate.UserId;
            var ipEndPoint = CustomFrameWork.Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(netConnectGate.TransferServerId);
            //var ipEndPoint = StartConfigComponent.Instance.GetInnerAddress(netConnectGate.TransferServerId);
            Session targetSession = Game.Scene.GetComponent<NetInnerComponent>().Get(ipEndPoint.ServerInnerIP);
            IResponse response = await targetSession.Call(b_Request);

            response.RpcId = rpcId;
            // session可能已经断开了，所以这里需要判断
            if (session.InstanceId == instanceId)
            {
                session.Reply(response);
            }

            return true;
        }
    }
}