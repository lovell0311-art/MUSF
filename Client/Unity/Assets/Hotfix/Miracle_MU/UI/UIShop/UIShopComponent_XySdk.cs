using ETModel;
using ILRuntime.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{

    /// <summary>
    /// XYSDK ≥‰÷µ
    /// </summary>
    public partial class UIShopComponent
    {
        public void Init_TopUp_XySdk()
        {
            Transform TopUps = collector.GetGameObject("TopUps").transform;
            for (int i = 0, length = TopUps.childCount; i < length; i++)
            {
                Button button = TopUps.GetChild(i).GetComponent<Button>();
                var value = (int)E_PlayerShopQuotaType.StoreRechargeI + i;

                button.onClick.AddSingleListener(async () =>
                {
                    C2G_CreateAnOrderResponse c2G_CreateAnOrder = (C2G_CreateAnOrderResponse)await SessionComponent.Instance.Session.Call(new C2G_CreateAnOrderRequest
                    {
                      RechargeType = value,
                    });
                    if (c2G_CreateAnOrder.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, c2G_CreateAnOrder.Error.GetTipInfo());

                    }
                    else
                    {
                        SdkCallBackComponent.Instance.sdkUtility.Pay(new string[] { $"{c2G_CreateAnOrder.PayRmb}", $"{c2G_CreateAnOrder.OrderId}", $"{topuptype}", $"{GetTypeName(topuptype)}", $"{GlobalDataManager.EnterZoneID}", $"{GlobalDataManager.EnterZoneName}", $"{GlobalDataManager.XYUUID}", $"{UnitEntityComponent.Instance.LocaRoleUUID}", $"{UnitEntityComponent.Instance.LocalRole.RoleName}", $"{UnitEntityComponent.Instance.LocalRole.Level}", $"{c2G_CreateAnOrder.AppCallbackUrl}", "", "", $"{SdkCallBackComponent.Instance.sdkUtility.CallAllObjName}", "PaySuccess", "PayFaulure" });

                    }

                });
            }
        }
    }
}