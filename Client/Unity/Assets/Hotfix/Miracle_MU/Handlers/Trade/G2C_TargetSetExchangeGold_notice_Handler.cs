using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_TargetSetExchangeGold_notice_Handler : AMHandler<G2C_TargetSetExchangeGold_notice>
    {
        protected override void Run(ETModel.Session session, G2C_TargetSetExchangeGold_notice message)
        {
            if (UIComponent.Instance.Get(UIType.UIKnapsack).GetComponent<UIKnapsackComponent>() is UIKnapsackComponent knapsackComponent)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "交易物品已更改，请确认!");
                knapsackComponent.ChangeGlodCoin(message.Gold);
            }
        }
    }
}
