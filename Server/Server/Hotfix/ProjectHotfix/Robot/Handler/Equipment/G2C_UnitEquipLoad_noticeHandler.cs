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
    public class G2C_UnitEquipLoad_noticeHandler : AMHandler<G2C_UnitEquipLoad_notice>
    {
        protected async override Task<bool> BeforeCodeAsync(Session session, G2C_UnitEquipLoad_notice message)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, session.InstanceId))
            {
                return await NextCodeAsync(session, message);
            }
        }
        protected async override Task<bool> CodeAsync(Session session, G2C_UnitEquipLoad_notice message)
        {
            Scene clientScene = session.ClientScene();
            RobotMapComponent robotMap = clientScene.GetComponent<RobotMapComponent>();

            MultiMap<long, Struct_ItemInSlot_Status> allItems = new MultiMap<long, Struct_ItemInSlot_Status>();
            foreach(var itemInfo in message.AllEquipStatus)
            {
                allItems.Add(itemInfo.GameUserId, itemInfo);
            }
            foreach (var kv in allItems.GetDictionary())
            {
                Unit targetUnit = UnitHelper.GetUnitFromClientScene(clientScene,kv.Key);
                if(targetUnit == null)
                {
                    Log.Warning($"没找到指定unit={kv.Key}");
                    return false;
                }
                RobotEquipmentComponent equipment = targetUnit.GetComponent<RobotEquipmentComponent>();
                foreach (var itemInfo in kv.Value)
                {
                    RobotItem item = RobotItemFactory.Create(clientScene,itemInfo.ItemUID, (int)itemInfo.ConfigID);
                    if (item == null) continue;

                    NumericComponent numeric = item.GetComponent<NumericComponent>();
                    numeric.SetNoEvent((int)EItemValue.Level, itemInfo.ItemLevel);

                    if (!equipment.EquipItem(item,(EquipPosition)itemInfo.SlotID))
                    {
                        Log.Warning($"[{clientScene.Name}] 穿戴装备失败,itemUid={item.Id}");
                        continue;
                    }
                    item.GameUserId = itemInfo.GameUserId;
                }

                targetUnit.GetComponent<ObjectWait>().Notify(new Wait_Unit_EquipItem());
            }
            return false;
        }
    }
}
