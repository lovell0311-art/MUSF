//using CustomFrameWork.Component;
//using ETModel;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//namespace ETHotfix
//{
//    [MessageHandler(AppType.Gate)]
//    public class C2G_GetPlayerInfoByGameUserIdRequestHandler2 : AMActorRpcHandler<C2G_GetPlayerInfoByGameUserIdRequest, G2C_GetPlayerInfoByGameUserIdResponse>
//    {
//        protected override async Task<bool> CodeAsync(Session session, C2G_GetPlayerInfoByGameUserIdRequest b_Request, G2C_GetPlayerInfoByGameUserIdResponse b_Response, Action<IMessage> b_Reply)
//        {
//            var netConnectGate = session.GetComponent<SessionGateUserComponent>();
//            if (netConnectGate == null || netConnectGate.TransferServerId == 0)
//            {
//                Log.Error($"玩家找不到或者ID为0 {netConnectGate.TransferServerId == 0}\n GameRouteMessage {b_Request.GetType().Name} : {Newtonsoft.Json.JsonConvert.SerializeObject(b_Request)}");
//                b_Response.Error = CustomFrameWork.Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
//                b_Reply(b_Response);
//                return false;
//            }
//            int rpcId = b_Request.RpcId; // 这里要保存客户端的rpcId

//            b_Request.ActorId = netConnectGate.UserId;
//            var ipEndPoint = CustomFrameWork.Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(netConnectGate.TransferServerId);
//            //var ipEndPoint = StartConfigComponent.Instance.GetInnerAddress(netConnectGate.TransferServerId);
//            Session targetSession = Game.Scene.GetComponent<NetInnerComponent>().Get(ipEndPoint.ServerInnerIP);
//            IResponse response = await targetSession.Call(b_Request);

//            response.RpcId = rpcId;
//            b_Reply(response);
//            return true;
//        }
//    }
//}