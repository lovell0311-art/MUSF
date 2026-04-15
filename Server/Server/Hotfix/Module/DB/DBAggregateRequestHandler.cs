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
    public class DBAggregateRequestHandler : AMRpcHandler<DBAggregateRequest, DBAggregateResponse>
    {
        protected override async Task<bool> CodeAsync(Session session, DBAggregateRequest request, DBAggregateResponse response, Action<IMessage> reply)
        {
            DBComponent dbComponent = Root.MainFactory.GetCustomComponent<DBComponent>();

            List<BsonDocument> result = await dbComponent.Aggregate(request.PipeLine, request.CollectionName);
            response.Result = result;

            reply(response);
            return true;
        }
    }
}