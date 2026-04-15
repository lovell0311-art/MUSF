using System;
using System.Collections.Generic;
using ETModel;
using System.Threading.Tasks;
using CustomFrameWork.Component;
using CustomFrameWork;

namespace ETHotfix
{
    [MessageHandler(AppType.DB)]
    public class DBQueryJsonRequestHandler : AMRpcHandler<DBQueryJsonRequest, DBQueryJsonResponse>
    {
        protected override async Task<bool> CodeAsync(Session session, DBQueryJsonRequest request, DBQueryJsonResponse response, Action<IMessage> reply)
        {
            List<ComponentWithId> components = await Root.MainFactory.GetCustomComponent<DBComponent>().GetJson(request.CollectionName, request.Json, request.Count);
            response.Components = components;

            reply(response);
            return true;
        }
    }
}