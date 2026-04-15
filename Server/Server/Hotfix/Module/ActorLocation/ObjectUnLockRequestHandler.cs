using System;
using ETModel;
using System.Threading.Tasks;
namespace ETHotfix
{
	[MessageHandler(AppType.Location)]
	public class ObjectUnLockRequestHandler : AMRpcHandler<ObjectUnLockRequest, ObjectUnLockResponse>
	{
		protected override async Task Run(Session session, ObjectUnLockRequest request, ObjectUnLockResponse response, Action reply)
		{
			Game.Scene.GetComponent<LocationComponent>().UnLock(request.Key, request.OldInstanceId, request.InstanceId);
			reply();
			await Task.CompletedTask;
		}
	}
}