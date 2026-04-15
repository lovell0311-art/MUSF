using ETModel;
using System.Linq;

namespace ETHotfix
{
    /// <summary>
    /// 物品掉落
    /// </summary>
    [MessageHandler]
    public class G2C_ItemDrop_notice_Handler : AMHandler<G2C_ItemDrop_notice>
    {
        protected override void Run(ETModel.Session session, G2C_ItemDrop_notice message)
        {
         //   for (int i = 0, length=message.Info.Count; i < length; i++)
           foreach (var dropData in message.Info)
            {
               // G2C_ItemDropData dropData = message.Info[i];
              
               /* if (KnapsackItemsManager.DisDropItemList.Contains(dropData.InstanceId)) 
                {
                    Log.DebugBrown($"当前装备已经销毁：{dropData.InstanceId}");
                    continue;
                };*/
                if (UnitEntityComponent.Instance.KnapsackItemEntityDic.ContainsKey(dropData.InstanceId)) continue;

                var node = AstarComponent.Instance.GetNode(dropData.PosX, dropData.PosY);
                KnapsackItemEntity knapsackData = UnitEntityFactory.CreatKnapsackItemEntity(new ItemDropDataInfo
                {
                    InstanceId = dropData.InstanceId,
                    Key = dropData.Key,
                    Value = dropData.Value,
                    PosX = dropData.PosX,
                    PosY = dropData.PosY,
                    Quality = dropData.Quality,
                    ProtectTick = dropData.ProtectTick,
                    KillerId = dropData.KillerId.ToList(),
                    Level = dropData.Level,
                    SetId = dropData.SetId,
                    CreatType=dropData.CreateType,
                },
                   node
                );
                //|| knapsackData.dropData.KillerId.Contains(UnitEntityComponent.Instance.LocaRoleUUID) == false
                if (KnapsackItemsManager.IsPackBackpack || dropData.CreateType == 1 ) continue; //正在整理背包、玩家丢弃的物品、玩家不是拾取

               

                //本地玩家挂机 并且在可拾取范围内、当前没有整理背包
                if (IsCanPick(node))
                { 
                    if (RoleOnHookComponent.Instance.IsCanAutoPick(knapsackData))
                    {
                        knapsackData.AutoPickUp();
                    }
                }
            }

           /* //自动拾取
            if (RoleOnHookComponent.Instance != null)
            {
                RoleOnHookComponent.Instance.AutoPickUpItem();
            }*/
        }

            
            bool IsCanPick(AstarNode astar) 
            {
                if (UnitEntityComponent.Instance.LocalRole.CurrentNodePos == null) return true;
                else
                {
                
                    return PositionHelper.Distance(UnitEntityComponent.Instance.LocalRole.CurrentNodePos, astar) <= 10;
                }
            }
        }
    
}
