using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Threading.Tasks;

namespace ETHotfix
{
    [MessageHandler(AppType.Robot)]
    public class T2T_PingHandler3 : AMHandler<T2T_Ping>
    {
        protected override async Task<bool> CodeAsync(Session session, T2T_Ping request)
        {
            session.Send(request);
            return true;
        }
    }
}