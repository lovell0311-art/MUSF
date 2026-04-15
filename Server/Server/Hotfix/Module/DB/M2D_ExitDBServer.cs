using System;
using System.Collections.Generic;
using ETModel;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using MongoDB.Bson;

namespace ETHotfix
{
    [MessageHandler(AppType.DB)]
    public class M2D_ExitDBServerRequestHandler : AMHandler<M2D_ExitDBServer>
    {
        protected override async Task<bool> CodeAsync(Session session, M2D_ExitDBServer request)
        {

            System.Environment.Exit(0);
            return true;
        }
    }
}