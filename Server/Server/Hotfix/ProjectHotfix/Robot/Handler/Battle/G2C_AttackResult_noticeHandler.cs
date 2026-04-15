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
    public class G2C_AttackResult_noticeHandler : AMHandler<G2C_AttackResult_notice>
    {
        protected async override Task<bool> BeforeCodeAsync(Session session, G2C_AttackResult_notice message)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, session.InstanceId))
            {
                return await NextCodeAsync(session, message);
            }
        }
        protected async override Task<bool> CodeAsync(Session session, G2C_AttackResult_notice message)
        {
            if (message.HurtValueType == 1) return false; // 啥都没有
            if (message.HurtValueType == 6) return false; // 技能终止
            Scene clientScene = session.ClientScene();
            Unit targetUnit = UnitHelper.GetUnitFromClientScene(clientScene, message.AttackTarget);
            if (targetUnit == null) return false;
            NumericComponent numerice = targetUnit.GetComponent<NumericComponent>();
            if(numerice != null)
            {
                numerice.Set((int)E_GameProperty.PROP_HP_MAX, message.HpMaxValue);
                numerice.Set((int)E_GameProperty.PROP_HP, message.HpValue);
            }
            return false;
        }
    }
}
