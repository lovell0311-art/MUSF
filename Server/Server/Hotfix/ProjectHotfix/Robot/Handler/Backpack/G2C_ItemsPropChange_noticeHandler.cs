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
    public class G2C_ItemsPropChange_noticeHandler : AMHandler<G2C_ItemsPropChange_notice>
    {
        protected async override Task<bool> BeforeCodeAsync(Session session, G2C_ItemsPropChange_notice message)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, session.InstanceId))
            {
                return await NextCodeAsync(session, message);
            }
        }
        protected async override Task<bool> CodeAsync(Session session, G2C_ItemsPropChange_notice message)
        {
            Scene clientScene = session.ClientScene();
            RobotItemManager robotItemManager = clientScene.GetComponent<RobotItemManager>();
            if (robotItemManager.AllItems.TryGetValue(message.ItemUUID,out RobotItem item))
            {
                NumericComponent numeric = item.GetComponent<NumericComponent>();
                foreach(var kv in message.PropList)
                {
                    numeric.Set(kv.PropID, kv.Value);
                }
            }
            return false;
        }
    }
}
