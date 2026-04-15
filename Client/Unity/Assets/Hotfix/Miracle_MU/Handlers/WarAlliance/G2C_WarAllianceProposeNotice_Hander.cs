using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_WarAllianceProposeNotice_Hander : AMHandler<G2C_WarAllianceProposeNotice>
    {
        protected override void Run(ETModel.Session session, G2C_WarAllianceProposeNotice message)
        {
            UIComponent.Instance.VisibleUI(UIType.UIHint, "您已被踢出战盟");
            WarAllianceDatas.IsJoinWar = false;
            UIComponent.Instance.Get(UIType.UIWarAlliance)?.GetComponent<UIWarAllianceComponent>()?.Dispose();
        }
    }

}
