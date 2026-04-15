using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
namespace ETHotfix
{
    /// <summary>
    /// 推送客户端背包中有物品位置变动
    /// </summary>
    [MessageHandler]
    public class G2C_ItemsLocationChangeBackpack_notice_Handler : AMHandler<G2C_ItemsLocationChangeBackpack_notice>
    {
        protected override void Run(ETModel.Session session, G2C_ItemsLocationChangeBackpack_notice message)
        {
            //改变物品的 位置信息
            if (KnapsackItemsManager.KnapsackItems.TryGetValue(message.ItemUUID, out KnapsackDataItem dataItem))
            {
                dataItem.PosInBackpackX = message.PosInBackpackX;
                dataItem.PosInBackpackY = message.PosInBackpackY;
                KnapsackTools.AddEquip(dataItem);
            }
        }
    }
}
