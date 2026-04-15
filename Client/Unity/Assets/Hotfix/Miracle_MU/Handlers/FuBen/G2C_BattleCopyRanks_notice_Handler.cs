using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_BattleCopyRanks_notice_Handler : AMHandler<G2C_BattleCopyRanks_notice>
    {
        protected override void Run(ETModel.Session session, G2C_BattleCopyRanks_notice message)
        {

            G2C_RedCastleSettlement_notice g2C_RedCastleSettlement = new G2C_RedCastleSettlement_notice();
            g2C_RedCastleSettlement.BatteCopyRankDatas = message.BatteCopyRankDatas;
            UIMainComponent.Instance.ExitFuBenBtn.gameObject.SetActive(false);
            UIComponent.Instance.VisibleUI(UIType.UIFuBenPaiHangBang);
            UIComponent.Instance.Get(UIType.UIFuBenPaiHangBang).GetComponent<UIFuBenPaiHangBangComponent>().SetValue(g2C_RedCastleSettlement);
        }
    }

}
