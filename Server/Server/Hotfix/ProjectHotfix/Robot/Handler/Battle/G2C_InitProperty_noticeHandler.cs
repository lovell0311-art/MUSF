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
    public class G2C_InitProperty_noticeHandler : AMHandler<G2C_InitProperty_notice>
    {
        protected async override Task<bool> BeforeCodeAsync(Session session, G2C_InitProperty_notice message)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, session.InstanceId))
            {
                return await NextCodeAsync(session, message);
            }
        }
        protected async override Task<bool> CodeAsync(Session session, G2C_InitProperty_notice message)
        {
            Scene clientScene = session.ClientScene();
            Unit localUnit = UnitHelper.GetLocalUnitFromClientScene(clientScene);
            if (localUnit == null) return false;
            NumericComponent numerice = localUnit.GetComponent<NumericComponent>();
            numerice.Set((int)E_GameProperty.PROP_HP, message.Hp);
            numerice.Set((int)E_GameProperty.PROP_MP, message.Mp);
            numerice.Set((int)E_GameProperty.PROP_AG, message.AG);
            numerice.Set((int)E_GameProperty.PROP_SD, message.SD);
            numerice.Set((int)E_GameProperty.PROP_HP_MAX, message.HpMax);
            numerice.Set((int)E_GameProperty.PROP_MP_MAX, message.MpMax);
            numerice.Set((int)E_GameProperty.PROP_AG_MAX, message.AGMax);
            numerice.Set((int)E_GameProperty.PROP_SD_MAX, message.SDMax);
            return false;
        }
    }
}
