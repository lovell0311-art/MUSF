using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 扩建仓库
    /// </summary>
    [MessageHandler]
    public class G2C_WarehouseExtension_notice_Handler : AMHandler<G2C_WarehouseExtension_notice>
    {
        protected override void Run(ETModel.Session session, G2C_WarehouseExtension_notice message)
        {
           // Log.DebugBrown($"扩建仓库:{message.Capacity}  行数：{message.Capacity / UIKnapsackComponent.LENGTH_WareHouse_X}");
            KnapsackItemsManager.WareHouseRows = message.Capacity/UIKnapsackComponent.LENGTH_WareHouse_X;
        }
    }
}
