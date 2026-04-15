using System;
using ETModel;
using System.Threading.Tasks;
using CustomFrameWork.Component;
using CustomFrameWork;

namespace ETHotfix
{
	[MessageHandler(AppType.DB)]
	public class DBQueryBatchRequestHandler : AMRpcHandler<DBQueryBatchRequest, DBQueryBatchResponse>
	{
		protected override async Task<bool> CodeAsync(Session session, DBQueryBatchRequest request, DBQueryBatchResponse response, Action<IMessage> reply)
		{
			DBComponent dbComponent = Root.MainFactory.GetCustomComponent<DBComponent>();
			response.Components = await dbComponent.GetBatch(request.CollectionName, request.IdList);

			reply(response);
			return true;
		}
	}
}