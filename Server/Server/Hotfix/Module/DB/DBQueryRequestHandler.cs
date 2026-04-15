using System;
using ETModel;
using System.Threading.Tasks;
using CustomFrameWork.Component;
using CustomFrameWork;

namespace ETHotfix
{
	[MessageHandler(AppType.DB)]
	public class DBQueryRequestHandler : AMRpcHandler<DBQueryRequest, DBQueryResponse>
	{
		protected override async Task<bool> CodeAsync(Session session, DBQueryRequest request, DBQueryResponse response, Action<IMessage> reply)
		{
			ComponentWithId component = await Root.MainFactory.GetCustomComponent<DBComponent>().Get(request.CollectionName, request.Id);

			response.Component = component;

			reply(response);
			return true;
		}
	}
}