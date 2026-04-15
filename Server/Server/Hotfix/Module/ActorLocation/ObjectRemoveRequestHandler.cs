using System;
using ETModel;
using System.Threading.Tasks;
namespace ETHotfix
{
	[MessageHandler(AppType.Location)]
	public class ObjectRemoveRequestHandler : AMRpcHandler<ObjectRemoveRequest, ObjectRemoveResponse>
	{
		protected override async Task Run(Session session, ObjectRemoveRequest request, ObjectRemoveResponse response, Action reply)
		{
			await Game.Scene.GetComponent<LocationComponent>().Remove(request.Key);
			reply();
			await Task.CompletedTask;
		}
	}
}