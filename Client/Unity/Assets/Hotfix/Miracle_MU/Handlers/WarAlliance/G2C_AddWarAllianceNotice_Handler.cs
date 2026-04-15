using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{

    /// <summary>
    /// 申请战盟 通知
    /// </summary>
    [MessageHandler]
    public class G2C_AddWarAllianceNotice_Handler : AMHandler<G2C_AddWarAllianceNotice>
    {
        protected override void Run(ETModel.Session session, G2C_AddWarAllianceNotice message)
        {
            //红点通知
            RedDotManagerComponent.RedDotManager.Set(E_RedDotDefine.Root_WarAlliance_WarApply, 1);
            //WarAllianceDatas.IsJoinWar = true;
            UIMainComponent.Instance?.RedDotFriendCheack();
            UIComponent.Instance.Get(UIType.UIWarAlliance)?.GetComponent<UIWarAllianceComponent>()?.RefreachApply();
        }
    }
}