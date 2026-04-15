using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2Gate_DisconnectHandler : AMHandler<C2Gate_Disconnect>
    {
        protected override async Task<bool> CodeAsync(Session session, C2Gate_Disconnect b_Request)
        {
            session.Disconnect().Coroutine();
            return true;
        }
    }
}