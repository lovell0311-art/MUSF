using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;


namespace ETHotfix
{

    /// <summary>
    /// 通知 仓库中物品 位置变动
    /// </summary>
    [MessageHandler]
    public class G2C_MoveWarehouseItem_notice_Handler : AMHandler<G2C_MoveWarehouseItem_notice>
    {
        protected override void Run(ETModel.Session session, G2C_MoveWarehouseItem_notice message)
        {
            if (KnapsackItemsManager.WareHouseItems.TryGetValue(message.ItemUUID, out KnapsackDataItem dataItem))
            {
                dataItem.PosInBackpackX = message.PosInBackpackX;
                dataItem.PosInBackpackY = message.PosInBackpackY % 11; 
            }

          
        }
    }
}
