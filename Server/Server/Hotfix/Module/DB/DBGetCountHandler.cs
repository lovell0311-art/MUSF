using System;
using ETModel;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    [MessageHandler(AppType.DB)]
    public class DBGetCountHandler : AMRpcHandler<DBGetCountRequest, DBGetCountResponse>
    {
        protected override async Task<bool> CodeAsync(Session session, DBGetCountRequest request, DBGetCountResponse response, Action<IMessage> reply)
        {
            DBComponent dbComponent = Root.MainFactory.GetCustomComponent<DBComponent>();
            response.RowCount = await dbComponent.GetCount(request.CollectionName, request.Json);

            reply(response);
            return true;
        }
    }
}