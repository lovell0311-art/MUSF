using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    public static class TopUpRewardsGlob
    {
        /// <summary>
        /// 充值金额
        /// </summary>
        public static int TotalAmount;
        /// <summary>
        /// 已领取的充值礼包
        /// </summary>
        public static List<int> Configids = new List<int>();
    }
    public partial class UITopUpRewardsComponent
    {
        public async ETVoid OpenCumulativeRecharge()
        {
            TopUpRewardsGlob.Configids.Clear();
            G2C_OpenCumulativeRecharge g2C_OpenCumulative = (G2C_OpenCumulativeRecharge)await SessionComponent.Instance.Session.Call(new C2G_OpenCumulativeRecharge() { });
            if (g2C_OpenCumulative.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenCumulative.Error.GetTipInfo());
            }
            else
            {
                TopUpRewardsGlob.TotalAmount = g2C_OpenCumulative.TotalAmount;
                TopUpAmount.text = "当前充值金额: " + g2C_OpenCumulative.TotalAmount.ToString();
                foreach (var item in g2C_OpenCumulative.ConfigIds)
                {
                    TopUpRewardsGlob.Configids.Add(item);
                }
                InitTopUpInfo();
            }
        }
        public async ETVoid ReceiveRechargeGiftPack()
        {
            G2C_ReceiveRechargeGiftPack g2C_Receive = (G2C_ReceiveRechargeGiftPack)await SessionComponent.Instance.Session.Call(new C2G_ReceiveRechargeGiftPack()
            {
                ConfigId = TypeId,
                Id2 = twoId
            });
            if (g2C_Receive.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Receive.Error.GetTipInfo());
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "领取成功");
                for (int i = 0, length = uITitleInfos.Count; i < length; i++)
                {
                    if (oneId == uITitleInfos[i].id)
                    {
                        uITitleInfos[i].isGet = true;
                        break;
                    }
                }
                TopUpRewardsGlob.Configids.Add(TypeId);
            }
            SetSelectIsShow(false);
            //GetItemPanel.gameObject.SetActive(false);
        }
    }

}
