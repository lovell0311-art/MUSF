using System;
using ETModel;
using System.Threading.Tasks;
using CustomFrameWork.Component;
using CustomFrameWork;

namespace ETHotfix.Handler
{
    [MessageHandler(AppType.AllServer)]
    public class C2R_PingHandler : AMRpcHandler<C2R_Ping, R2C_Ping>
    {
        protected override async Task<bool> CodeAsync(Session session, C2R_Ping request, R2C_Ping response, Action<IMessage> reply)
        {
            PingComponent pingCom = Root.MainFactory.GetCustomComponent<PingComponent>();
            if(pingCom != null)
            {
                pingCom.ReceiveTickDic[session.Id] = Help_TimeHelper.GetNowSecond();
            }
            reply(response);
            return true;
        }
    }
}