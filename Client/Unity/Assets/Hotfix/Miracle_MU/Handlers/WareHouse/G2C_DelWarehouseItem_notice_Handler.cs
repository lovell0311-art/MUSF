using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 移除仓库中的物品
    /// </summary>
    [MessageHandler]
    public class G2C_DelWarehouseItem_notice_Handler : AMHandler<G2C_DelWarehouseItem_notice>
    {
        protected override void Run(ETModel.Session session, G2C_DelWarehouseItem_notice message)
        {
            ///仓库 中的物品离开
            if (KnapsackItemsManager.WareHouseItems.ContainsKey(message.LeaveItemUUID))
            {
                KnapsackItemsManager.WareHouseItems[message.LeaveItemUUID].Dispose();
                KnapsackItemsManager.WareHouseItems.Remove(message.LeaveItemUUID);
            }

         
        }
    }
}
