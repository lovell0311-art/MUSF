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
    public class G2C_BattlePickUpDropItem_noticeHandler : AMHandler<G2C_BattlePickUpDropItem_notice>
    {
        protected async override Task<bool> BeforeCodeAsync(Session session, G2C_BattlePickUpDropItem_notice message)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, session.InstanceId))
            {
                return await NextCodeAsync(session, message);
            }
        }
        protected async override Task<bool> CodeAsync(Session session, G2C_BattlePickUpDropItem_notice message)
        {
            Scene clientScene = session.ClientScene();
            RobotMapComponent robotMap = clientScene.GetComponent<RobotMapComponent>();
            foreach(long uid in message.InstanceId)
            {
                Unit item = UnitHelper.GetUnitFromClientScene(clientScene, uid);
                if(item == null)
                {
                    Log.Warning($"[{clientScene.Name}] Unit Item已经离开视野:{uid}");
                    continue;
                }
                robotMap.CurrentMap.UnitLeave(item);
            }
            return false;
        }
    }
}
