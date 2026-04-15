using System.Threading.Tasks;
using System;
using ETModel;
using ETModel.Robot;
using CustomFrameWork;
using CustomFrameWork.Component;
using MongoDB.Bson;
using UnityEngine;

namespace ETHotfix.Robot
{
    [MessageHandler(AppType.Robot)]
    public class G2C_UnitLeaveView_noticeHandler : AMHandler<G2C_UnitLeaveView_notice>
    {
        protected async override Task<bool> BeforeCodeAsync(Session session, G2C_UnitLeaveView_notice message)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, session.InstanceId))
            {
                return await NextCodeAsync(session, message);
            }
        }
        protected async override Task<bool> CodeAsync(Session session, G2C_UnitLeaveView_notice message)
        {
            Scene clientScene = session.ClientScene();
            RobotMapComponent robotMap = clientScene.GetComponent<RobotMapComponent>();
            Unit localUnit = clientScene.GetComponent<PlayerComponent>().LocalUnit;
            if(localUnit == null)
            {
                Log.Warning($"[{clientScene.Name}] 收到移动包前，还没收到 G2C_StartGameGamePlayerResponse 消息");
                return false;
            }
            // TODO 单位离开视野
            foreach(long unitId in message.LeavedUnitId)
            {
                Log.Info($"[{clientScene.Name}] 收到离开视野消息,unitId={unitId}");
                Unit unit = robotMap.CurrentMap.GetUnit(unitId);
                if (unit == null)
                {
                    //Log.Warning($"[{clientScene.Name}] 收到重复离开视野消息,unitId={unitId}");
                    continue;
                }
                unit.Dispose();
            }
           
            return false;
        }
    }
}
