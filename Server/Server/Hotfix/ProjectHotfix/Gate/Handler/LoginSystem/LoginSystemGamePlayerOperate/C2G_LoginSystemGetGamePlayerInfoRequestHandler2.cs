using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;


namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_LoginSystemGetGamePlayerInfoRequestHandler2 : AMActorRpcHandler<C2G_LoginSystemGetGamePlayerInfoRequest, G2C_LoginSystemGetGamePlayerInfoResponse>
    {
        protected override async Task<bool> CodeAsync(Session session, C2G_LoginSystemGetGamePlayerInfoRequest b_Request, G2C_LoginSystemGetGamePlayerInfoResponse b_Response, Action<IMessage> reply)
        {
            Log.Info($"#RoleFlow# GateGetGamePlayerInfo start session={session?.Id ?? 0} rpc={b_Request.RpcId} gameIdCount={b_Request.GameId?.count ?? 0}");
            var netConnectGate = session.GetComponent<SessionGateUserComponent>();
            if (netConnectGate == null) return false;
            if (netConnectGate.TransferServerId == 0)
            {
                Log.Error($"玩家ID为0 {netConnectGate.TransferServerId == 0}\n GameRouteMessage {b_Request.GetType().Name} : {Newtonsoft.Json.JsonConvert.SerializeObject(b_Request)}");
                return false;
            }
            int rpcId = b_Request.RpcId; // 这里要保存客户端的rpcId
            long instanceId = session.InstanceId;

            b_Request.ActorId = netConnectGate.UserId;
            var ipEndPoint = CustomFrameWork.Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(netConnectGate.TransferServerId);
            //var ipEndPoint = StartConfigComponent.Instance.GetInnerAddress(netConnectGate.TransferServerId);
            Session targetSession = Game.Scene.GetComponent<NetInnerComponent>().Get(ipEndPoint.ServerInnerIP);
            IResponse response = await targetSession.Call(b_Request);
            Log.Info($"#RoleFlow# GateGetGamePlayerInfo finish session={session?.Id ?? 0} rpc={rpcId} responseType={response?.GetType().Name} error={(response as IResponse)?.Error ?? 0}");

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
