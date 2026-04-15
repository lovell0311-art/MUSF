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
    public class G2C_AttackStart_noticeHandler : AMHandler<G2C_AttackStart_notice>
    {
        protected async override Task<bool> BeforeCodeAsync(Session session, G2C_AttackStart_notice message)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, session.InstanceId))
            {
                return await NextCodeAsync(session, message);
            }
        }
        protected async override Task<bool> CodeAsync(Session session, G2C_AttackStart_notice message)
        {
            Scene clientScene = session.ClientScene();
            Unit localUnit = clientScene.GetComponent<PlayerComponent>().LocalUnit;
            if (message.AttackSource == localUnit.Id)
            {
                // 攻击者是本地玩家
                if (message.AttackType != 0)
                {
                    // 技能攻击
                    localUnit.GetComponent<NumericComponent>().Set((int)E_GameProperty.PROP_MP, message.MpValue);
                    localUnit.GetComponent<NumericComponent>().Set((int)E_GameProperty.PROP_AG, message.AG);
                }
                localUnit.GetComponent<AttackComponent>().NextAttackTime = message.Ticks;

                clientScene.GetComponent<ObjectWait>().Notify(new Wait_AttackStartNotice()
                {
                    message = message,
                });
            }
            return false;
        }
    }
}
