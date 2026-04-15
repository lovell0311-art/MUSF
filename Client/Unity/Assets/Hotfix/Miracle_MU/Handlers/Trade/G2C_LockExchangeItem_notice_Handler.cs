using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 玩家锁定交易物品
    /// </summary>
    [MessageHandler]
    public class G2C_LockExchangeItem_notice_Handler : AMHandler<G2C_LockExchangeItem_notice>
    {
        protected override void Run(ETModel.Session session, G2C_LockExchangeItem_notice message)
        {
            if (message.GameUserId == UnitEntityComponent.Instance.LocaRoleUUID) return;
            if (UIComponent.Instance.Get(UIType.UIKnapsack).GetComponent<UIKnapsackComponent>() is UIKnapsackComponent uIKnapsack)
            {
                uIKnapsack.ChangeOtherTradeStatus(message.LockState);
            }
        }
    }
}
