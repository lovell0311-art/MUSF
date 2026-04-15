using System.Threading.Tasks;
using ETModel;
using ETModel.Robot;
using ETModel.Robot.WaitType;
using CustomFrameWork;
using CustomFrameWork.Component;
using MongoDB.Bson;
using UnityEngine;

namespace ETHotfix.Robot
{
    [MessageHandler(AppType.Robot)]
    public class G2C_ItemDrop_noticeHandler : AMHandler<G2C_ItemDrop_notice>
    {
        protected async override Task<bool> BeforeCodeAsync(Session session, G2C_ItemDrop_notice message)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, session.InstanceId))
            {
                return await NextCodeAsync(session, message);
            }
        }
        protected async override Task<bool> CodeAsync(Session session, G2C_ItemDrop_notice message)
        {
            Scene clientScene = session.ClientScene();
            RobotMapComponent robotMap = clientScene.GetComponent<RobotMapComponent>();
            foreach(var itemData in message.Info)
            {
                Unit item = UnitFactory.CreateItem(clientScene, itemData.InstanceId, itemData);
                if (item == null) continue;
                robotMap.CurrentMap.UnitEnter(item, new Vector2Int(itemData.PosX, itemData.PosY));
            }
            return false;
        }
    }
}
