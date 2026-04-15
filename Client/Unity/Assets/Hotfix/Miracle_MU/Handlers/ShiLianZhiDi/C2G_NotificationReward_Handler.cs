using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class C2G_NotificationReward_Handler : AMHandler<C2G_NotificationReward>
    {
        protected override void Run(ETModel.Session session, C2G_NotificationReward message)
        {
            UIShiLianZhiDiComponent shiLianZhiDiComponent = UIComponent.Instance.VisibleUI(UIType.UIShiLianZhiDi).GetComponent<UIShiLianZhiDiComponent>();
            shiLianZhiDiComponent.ChangePanel(E_ShiLianType.Success);
            shiLianZhiDiComponent.ShowSuccess(message.Cnt);

        }
    }
}