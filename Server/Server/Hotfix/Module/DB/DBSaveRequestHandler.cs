using System;
using ETModel;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    [MessageHandler(AppType.DB)]
    public class DBSaveRequestHandler : AMRpcHandler<DBSaveRequest, DBSaveResponse>
    {
        protected override async Task<bool> CodeAsync(Session session, DBSaveRequest request, DBSaveResponse response, Action<IMessage> reply)
        {
            DBComponent dbComponent = Root.MainFactory.GetCustomComponent<DBComponent>();
            if (string.IsNullOrEmpty(request.CollectionName))
            {
                request.CollectionName = request.Component.GetType().Name;
            }

            var mResult = await dbComponent.Add(request.Component, request.CollectionName);
            if (mResult == false)
            {
                response.Error = ErrorCodeHotfix.ERR_MongodbSaveError;
            }

            reply(response);
            return true;
        }
    }
}