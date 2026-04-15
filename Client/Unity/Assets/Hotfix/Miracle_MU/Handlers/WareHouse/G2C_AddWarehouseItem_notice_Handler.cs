using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 推送客户端仓库中有物品进入 选择角色进入游戏场景时会推送玩家仓库里现有物品
    /// </summary>
    [MessageHandler]
    public class G2C_AddWarehouseItem_notice_Handler : AMHandler<G2C_AddWarehouseItem_notice>
    {
        protected override void Run(ETModel.Session session, G2C_AddWarehouseItem_notice message)
        {
            for (int i = 0,length=message.AllItems.Count; i < length; i++)
            {
                
                var item = message.AllItems[i];
               
                if (KnapsackItemsManager.WareHouseItems.ContainsKey(item.ItemUID))
                {
                   
                    continue;
                }
            //    Log.DebugYellow($"仓库新加物品：{item.ConfigID} item.PosInBackpackX:{item.PosInBackpackX}  item.PosInBackpackY:{item.PosInBackpackY} y:{item.PosInBackpackY % 11}");
                KnapsackDataItem knapsackDataItem = ComponentFactory.CreateWithId<KnapsackDataItem>(item.ItemUID);
                knapsackDataItem.GameUserId = item.GameUserId;
                knapsackDataItem.UUID = item.ItemUID;
                knapsackDataItem.ConfigId = item.ConfigID;
                knapsackDataItem.ItemType = item.Type;
                knapsackDataItem.PosInBackpackX=item.PosInBackpackX;
                knapsackDataItem.Page= (item.PosInBackpackY / 11) + 1;//当前物品 所属的页数
                knapsackDataItem.PosInBackpackY = item.PosInBackpackY % 11;// 物品在 page 页中 所在位置的 Y

                knapsackDataItem.X = item.Width;
                knapsackDataItem.Y = item.Height;
                knapsackDataItem.SetProperValue(E_ItemValue.Quantity, item.Quantity);
                knapsackDataItem.SetProperValue(E_ItemValue.Level, item.ItemLevel);
                KnapsackItemsManager.WareHouseItems[item.ItemUID] = knapsackDataItem;

                
                Game.EventCenter.EventTrigger<KnapsackDataItem>(EventTypeId.WARE_ADD_ITEM,knapsackDataItem);
            }

            
        }
    }
}
