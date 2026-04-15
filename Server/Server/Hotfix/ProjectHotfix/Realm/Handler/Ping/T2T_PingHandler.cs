using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Threading.Tasks;

namespace ETHotfix
{
    [MessageHandler(AppType.Realm)]
    public class T2T_PingHandler2 : AMHandler<T2T_Ping>
    {
        protected override async Task<bool> CodeAsync(Session session, T2T_Ping request)
        {
            Root.MainFactory.GetCustomComponent<PingComponent>().UpdatePingTick(session).Coroutine();
            return true;
        }
    }
}