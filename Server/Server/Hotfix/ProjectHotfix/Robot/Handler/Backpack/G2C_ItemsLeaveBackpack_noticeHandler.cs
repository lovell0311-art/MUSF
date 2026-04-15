using System.Threading.Tasks;
using ETModel;
using ETModel.Robot;
using ETModel.Robot.WaitType;
using CustomFrameWork;
using CustomFrameWork.Component;
using MongoDB.Bson;

namespace ETHotfix.Robot
{
    [MessageHandler(AppType.Robot)]
    public class G2C_ItemsLeaveBackpack_noticeHandler : AMHandler<G2C_ItemsLeaveBackpack_notice>
    {
        protected async override Task<bool> BeforeCodeAsync(Session session, G2C_ItemsLeaveBackpack_notice message)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, session.InstanceId))
            {
                return await NextCodeAsync(session, message);
            }
        }
        protected async override Task<bool> CodeAsync(Session session, G2C_ItemsLeaveBackpack_notice message)
        {
            Scene clientScene = session.ClientScene();
            Unit targetUnit = UnitHelper.GetUnitFromClientScene(clientScene, message.GameUserId);
            if(targetUnit == null)
            {
                Log.Warning($"[{clientScene.Name}] 没找到指定unit={message.GameUserId}");
                return false;
            }
            RobotBackpackComponent backpack = targetUnit.GetComponent<RobotBackpackComponent>();
            backpack.DeleteItem(message.LeaveItemUUID);
            targetUnit.GetComponent<ObjectWait>().Notify(new Wait_Unit_ItemLeaveBackpack());
            return false;
        }
    }
}
