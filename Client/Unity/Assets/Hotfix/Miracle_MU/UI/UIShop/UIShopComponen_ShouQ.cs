using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIShopComponent
    {
        public void Init_TopUp_ShouQSdk()
        {
            Transform TopUps = collector.GetGameObject("TopUps").transform;
            for (int i = 0, length = TopUps.childCount; i < length; i++)
            {
                Button button = TopUps.GetChild(i).GetComponent<Button>();
                var value = (int)E_PlayerShopQuotaType.StoreRechargeI + i;

                button.onClick.AddSingleListener(async () =>
                {
                    Log.DebugRed($"请求充值：{value} 元");

                    G2C_ShouQCreateAnOrder c2G_CreateAnOrder = (G2C_ShouQCreateAnOrder)await SessionComponent.Instance.Session.Call(new C2G_ShouQCreateAnOrder
                    {
                        RechargeType = value,
                    });
                    if (c2G_CreateAnOrder.Error != 0)
                    {
                        Log.DebugGreen($"{c2G_CreateAnOrder.Error}");
                        UIComponent.Instance.VisibleUI(UIType.UIHint, c2G_CreateAnOrder.Error.GetTipInfo());

                    }
                    else
                    {
                        Log.DebugGreen($"请求支付：{value * 10 * 10} 分 {c2G_CreateAnOrder.OrderId}");
                        string[] strings = new string[6];
                        strings[0] = c2G_CreateAnOrder.OrderId;
                        strings[1] = $"{c2G_CreateAnOrder.PayRmb}";
                        strings[2] = $"{GlobalDataManager.EnterZoneName}";
                        strings[3] = $"{UnitEntityComponent.Instance.LocaRoleUUID}";
                        strings[4] = $"{GlobalDataManager.ShouQUUID}";
                        strings[5] = "";

                        SdkCallBackComponent.Instance.sdkUtility.Pay(strings);

                    }

                });
            }
        }
    }
}
