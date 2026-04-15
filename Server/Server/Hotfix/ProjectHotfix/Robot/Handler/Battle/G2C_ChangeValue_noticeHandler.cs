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
    public class G2C_ChangeValue_noticeHandler : AMHandler<G2C_ChangeValue_notice>
    {
        protected async override Task<bool> BeforeCodeAsync(Session session, G2C_ChangeValue_notice message)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, session.InstanceId))
            {
                return await NextCodeAsync(session, message);
            }
        }
        protected async override Task<bool> CodeAsync(Session session, G2C_ChangeValue_notice message)
        {
            Scene clientScene = session.ClientScene();
            Unit targetUnit = UnitHelper.GetUnitFromClientScene(clientScene, message.GameUserId);
            if (targetUnit == null) return false;
            NumericComponent numerice = targetUnit.GetComponent<NumericComponent>();
            if(numerice != null)
            {
                foreach(var kv in message.Info)
                {
                    numerice.Set(kv.Key, kv.Value);
                }
            }
            return false;
        }
    }
}
