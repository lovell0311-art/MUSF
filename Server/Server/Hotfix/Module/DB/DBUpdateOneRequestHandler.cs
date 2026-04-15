using System;
using ETModel;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    [MessageHandler(AppType.DB)]
    public class DBUpdateOneRequestHandler : AMRpcHandler<DBUpdateOneRequest, DBUpdateOneResponse>
    {
        protected override async Task<bool> CodeAsync(Session session, DBUpdateOneRequest request, DBUpdateOneResponse response, Action<IMessage> reply)
        {
            DBComponent dbComponent = Root.MainFactory.GetCustomComponent<DBComponent>();

            var mResult = await dbComponent.UpdateOne(request.Filter, request.Updates,request.CollectionName);
            if (mResult == false)
            {
                response.Error = ErrorCodeHotfix.ERR_MongodbUpdateError;
            }

            reply(response);
            return true;
        }
    }
}