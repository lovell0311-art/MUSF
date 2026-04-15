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
    public class G2C_ItemsIntoBackpack_noticeHandler : AMHandler<G2C_ItemsIntoBackpack_notice>
    {
        protected async override Task<bool> BeforeCodeAsync(Session session, G2C_ItemsIntoBackpack_notice message)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, session.InstanceId))
            {
                return await NextCodeAsync(session, message);
            }
        }
        protected async override Task<bool> CodeAsync(Session session, G2C_ItemsIntoBackpack_notice message)
        {
            Scene clientScene = session.ClientScene();
            RobotMapComponent robotMap = clientScene.GetComponent<RobotMapComponent>();
            MultiMap<long, Struct_ItemInBackpack_Status> allItems = new MultiMap<long, Struct_ItemInBackpack_Status>();
            foreach(var itemInfo in message.AllItems)
            {
                allItems.Add(itemInfo.GameUserId, itemInfo);
            }
            foreach (var kv in allItems.GetDictionary())
            {
                Unit targetUnit = UnitHelper.GetUnitFromClientScene(clientScene,kv.Key);
                if(targetUnit == null)
                {
                    Log.Warning($"[{clientScene.Name}] 没找到指定unit={kv.Key}");
                    return false;
                }
                RobotBackpackComponent backpack = targetUnit.GetComponent<RobotBackpackComponent>();
                foreach (var itemInfo in kv.Value)
                {
                    RobotItem item = RobotItemFactory.Create(clientScene,itemInfo.ItemUID, itemInfo.ConfigID);
                    if (item == null) continue;

                    NumericComponent numeric = item.GetComponent<NumericComponent>();
                    numeric.SetNoEvent((int)EItemValue.Quantity, itemInfo.Quantity);
                    numeric.SetNoEvent((int)EItemValue.Level, itemInfo.ItemLevel);

                    if (!backpack.AddItem(item, itemInfo.PosInBackpackX, itemInfo.PosInBackpackY))
                    {
                        Log.Warning($"[{clientScene.Name}] 添加进背包失败,itemUid={item.Id}");
                        continue;
                    }
                    item.GameUserId = itemInfo.GameUserId;
                    item.InComponent = EItemInComponent.Backpack;
                }
                targetUnit.GetComponent<ObjectWait>().Notify(new Wait_Unit_ItemIntoBackpack());
            }
            return false;
        }
    }
}
