using System;
using ETModel;
using System.Threading.Tasks;
namespace ETHotfix
{
	[MessageHandler(AppType.Location)]
	public class ObjectLockRequestHandler : AMRpcHandler<ObjectLockRequest, ObjectLockResponse>
	{
		protected override async Task Run(Session session, ObjectLockRequest request, ObjectLockResponse response, Action reply)
		{
			Game.Scene.GetComponent<LocationComponent>().Lock(request.Key, request.InstanceId, request.Time);
			reply();
		}
	}
}