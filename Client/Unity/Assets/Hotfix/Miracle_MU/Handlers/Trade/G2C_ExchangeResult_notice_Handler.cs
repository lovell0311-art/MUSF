using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 交易结果 交易被终止时也推送此通知，交易结果为false
    /// </summary>
    [MessageHandler]
    public class G2C_ExchangeResult_notice_Handler : AMHandler<G2C_ExchangeResult_notice>
    {
        protected override void Run(ETModel.Session session, G2C_ExchangeResult_notice message)
        {
           
            if (message.State)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "交易成功");
                UIKnapsackComponent.Instance.ClearTrade();
                UIKnapsackComponent.Instance.CloseKnapsack();
            }
            else
            {
               
                UIComponent.Instance.VisibleUI(UIType.UIHint, $"{message.ErrorMessage}");
               // UIKnapsackComponent.Instance?.ClearOtherTrade();
                if (message.ErrorMessage.Contains("主动取消交易!")||message.ErrorMessage.Contains("对方已下线，交易终止!"))
                {
                    if (UIComponent.Instance.Get(UIType.UIKnapsack)?.GetComponent<UIKnapsackComponent>() is UIKnapsackComponent knapsackComponent)
                    {
                        knapsackComponent.ClearTrade();
                        knapsackComponent.CloseKnapsack();
                    }
                }
            }
        }
    }
}
