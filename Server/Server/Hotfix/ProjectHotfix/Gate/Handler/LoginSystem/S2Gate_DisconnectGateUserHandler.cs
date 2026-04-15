using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Threading.Tasks;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class S2Gate_DisconnectGateUserHandler : AMRpcHandler<S2Gate_DisconnectGateUser, Gate2S_DisconnectGateUser>
    {
        protected override async Task<bool> CodeAsync(Session session, S2Gate_DisconnectGateUser b_Request, Gate2S_DisconnectGateUser b_Response, Action<IMessage> b_Reply)
        {
            GateUser gateUser = Root.MainFactory.GetCustomComponent<GateUserComponent>().GetUserByUserId(b_Request.UserId);
            if(gateUser != null)
            {
                if(gateUser.ClientSession != null &&
                    gateUser.ClientSession.IsDisposed == false &&
                    gateUser.ClientSession.InstanceId == gateUser.CSessionInstanceId)
                {
                    if((DisconnectType)b_Request.DisconnectType == DisconnectType.RepeatLogin)
                    {
                        // 发送顶号通知消息
                        gateUser.ClientSession.Send(new Gate2C_LoginFromOtherDevices() { });
                    }else if((DisconnectType)b_Request.DisconnectType == DisconnectType.ServerShutdown)
                    {
                        gateUser.ClientSession.Send(new Gate2C_ServerShutdown() { });
                    }
                    else
                    {
                        gateUser.ClientSession.Send(new Gate2C_KickOffline() { 
                            DisconnectType = b_Request.DisconnectType,
                            BanTillTime = b_Request.BanTillTime,
                            Reason = b_Request.Reason,
                        });
                    }
                }
                await gateUser.KickGateUser();
            }
            else
            {
                await GateUserHelper.KickUser(b_Request.UserId);
            }
            b_Reply(b_Response);
            return true;
        }
    }
}