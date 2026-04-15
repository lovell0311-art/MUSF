using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 有物品在交易栏移动
    /// </summary>
    [MessageHandler]
    public class G2C_MoveExchangeItem_notice_Handler : AMHandler<G2C_MoveExchangeItem_notice>
    {
        protected override void Run(ETModel.Session session, G2C_MoveExchangeItem_notice message)
        {

            if (UIComponent.Instance.Get(UIType.UIKnapsack).GetComponent<UIKnapsackComponent>() is UIKnapsackComponent uIKnapsack)
            {
                uIKnapsack.ChangeTradeItemPos(message.ItemUUID, message.PosInBackpackX, message.PosInBackpackY);
            }
        }
    }
}