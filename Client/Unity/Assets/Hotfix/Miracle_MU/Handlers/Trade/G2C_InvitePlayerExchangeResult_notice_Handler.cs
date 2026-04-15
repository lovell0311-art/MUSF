using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 交易邀请被拒绝或同意后推送  同意则打开交易窗口
    /// </summary>
    [MessageHandler]
    public class G2C_InvitePlayerExchangeResult_notice_Handler : AMHandler<G2C_InvitePlayerExchangeResult_notice>
    {
        protected override void Run(ETModel.Session session, G2C_InvitePlayerExchangeResult_notice message)
        {
         
            if (message.IsAgree)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, $"<color=red>{message.PlayerName}</color> 同意交易");
                //显示交易面板
                if (UIComponent.Instance.Get(UIType.UIKnapsack) != null) UIKnapsackComponent.Instance.CloseKnapsack();
                UIComponent.Instance.VisibleUI(UIType.UIKnapsack, E_KnapsackState.KS_Trade);
                UIComponent.Instance.Get(UIType.UIKnapsack).GetComponent<UIKnapsackComponent>().SetOtherInfo(message.PlayerName,message.PlayerWarAllianceName,message.PlayerLevel);
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, $"<color=red>{message.PlayerName}</color> 拒绝交易");
            }
        }
    }
}
