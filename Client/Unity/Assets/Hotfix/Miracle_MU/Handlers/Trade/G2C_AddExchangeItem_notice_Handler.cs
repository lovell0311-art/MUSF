using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 有物品进入交易栏内
    /// </summary>
    [MessageHandler]
    public class G2C_AddExchangeItem_notice_Handler : AMHandler<G2C_AddExchangeItem_notice>
    {
        protected override void Run(ETModel.Session session, G2C_AddExchangeItem_notice message)
        {
            if (message.PlayerGameUserId == UnitEntityComponent.Instance.LocaRoleUUID) return;
            UIKnapsackComponent uIKnapsack = UIComponent.Instance.Get(UIType.UIKnapsack).GetComponent<UIKnapsackComponent>();
            KnapsackDataItem knapsackDataItem = ComponentFactory.CreateWithId<KnapsackDataItem>(message.ItemUUID);
            knapsackDataItem.GameUserId = message.PlayerGameUserId;//玩家的UID
            knapsackDataItem.UUID = message.ItemUUID;//装备的UID
            knapsackDataItem.ConfigId = message.ItemConfigID;//装备配置表id
            knapsackDataItem.PosInBackpackX = message.PosInBackpackX;//装备在背包中的起始格子 坐标
            knapsackDataItem.PosInBackpackY = message.PosInBackpackY;
            knapsackDataItem.SetProperValue(E_ItemValue.Quantity, message.ItemQuantity);//装备的数量
            knapsackDataItem.SetProperValue(E_ItemValue.Level, message.ItemLevel);//装备的等级
            uIKnapsack.OtherTradeItemDic[message.ItemUUID] = knapsackDataItem;
            uIKnapsack.AddItem(knapsackDataItem,type:E_Grid_Type.Trade_Other);

            UIComponent.Instance.VisibleUI(UIType.UIHint,"交易物品已更改 请确认！");
        }
    }
}