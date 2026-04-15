using System;
using ETModel;
using System.Threading.Tasks;
using CustomFrameWork.Component;
using CustomFrameWork;

namespace ETHotfix
{
	[MessageHandler(AppType.DB)]
	public class DBSaveBatchRequestHandler : AMRpcHandler<DBSaveBatchRequest, DBSaveBatchResponse>
	{
		protected override async Task<bool> CodeAsync(Session session, DBSaveBatchRequest request, DBSaveBatchResponse response, Action<IMessage> reply)
		{
			DBComponent dbComponent = Root.MainFactory.GetCustomComponent<DBComponent>();

			if (string.IsNullOrEmpty(request.CollectionName))
			{
				request.CollectionName = request.Components[0].GetType().Name;
			}

			await dbComponent.AddBatch(request.Components, request.CollectionName);

			reply(response);
			return true;
		}
	}
}
