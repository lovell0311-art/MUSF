using System;
using ETModel;
using System.Threading.Tasks;
namespace ETHotfix
{
	[MessageHandler(AppType.Location)]
	public class ObjectAddRequestHandler : AMRpcHandler<ObjectAddRequest, ObjectAddResponse>
	{
		protected override async Task Run(Session session, ObjectAddRequest request, ObjectAddResponse response, Action reply)
		{
			await Game.Scene.GetComponent<LocationComponent>().Add(request.Key, request.InstanceId);
			reply();
		}
	}
}