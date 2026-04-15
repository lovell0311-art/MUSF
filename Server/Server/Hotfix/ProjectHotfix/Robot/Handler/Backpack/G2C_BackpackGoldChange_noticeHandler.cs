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
    public class G2C_BackpackGoldChange_noticeHandler : AMHandler<G2C_BackpackGoldChange_notice>
    {
        protected async override Task<bool> BeforeCodeAsync(Session session, G2C_BackpackGoldChange_notice message)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, session.InstanceId))
            {
                return await NextCodeAsync(session, message);
            }
        }
        protected async override Task<bool> CodeAsync(Session session, G2C_BackpackGoldChange_notice message)
        {
            Scene clientScene = session.ClientScene();
            Unit targetUnit = UnitHelper.GetUnitFromClientScene(clientScene,message.GameUserId);
            if(targetUnit == null)
            {
                Log.Warning($"[{clientScene.Name}] not found unit {message.GameUserId}");
                return false;
            }
            NumericComponent numeric = targetUnit.GetComponent<NumericComponent>();
            numeric.Set((int)E_GameProperty.GoldCoin, message.Gold);
            return false;
        }
    }
}
