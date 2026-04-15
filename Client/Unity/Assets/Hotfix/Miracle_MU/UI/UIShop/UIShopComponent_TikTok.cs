using ETModel;
using ILRuntime.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

namespace ETHotfix
{
    /// <summary>
    /// 抖音SDK充值
    /// </summary>
    public partial class UIShopComponent
    {
        public void Init_TopUp_TikTokSdk()
        {
            Transform TopUps = collector.GetGameObject("TopUps").transform;
            for (int i = 0, length = TopUps.childCount; i < length; i++)
            {
                Button button = TopUps.GetChild(i).GetComponent<Button>();
                var value = (int)E_PlayerShopQuotaType.StoreRechargeI + i;

                button.onClick.AddSingleListener(async () =>
                {
                    Log.DebugRed($"请求充值：{value * 10 * 10} 分");
                 
                    C2G_DouYinCreateAnOrderResponse c2G_CreateAnOrder = (C2G_DouYinCreateAnOrderResponse)await SessionComponent.Instance.Session.Call(new C2G_DouYinCreateAnOrderRequest
                    {
                        RechargeType = value,
                        ControlInfo = SdkCallBackComponent.Instance.sdkUtility.GetRiskControlInfo(),//风控参数
                    }) ;
                    if (c2G_CreateAnOrder.Error != 0)
                    {
                        Log.DebugGreen($"{c2G_CreateAnOrder.Error}");
                        UIComponent.Instance.VisibleUI(UIType.UIHint, c2G_CreateAnOrder.Error.GetTipInfo());

                    }
                    else
                    {
                        Log.DebugGreen($"请求支付：{value * 10 * 10} 分 {c2G_CreateAnOrder.OrderId}");
                        string[] strings = new string[8];
                        strings[0] = c2G_CreateAnOrder.OrderId;
                        strings[1] = $"{c2G_CreateAnOrder.PayRmb * 10 * 10}";
                        strings[2] = "商城充值";
                        strings[3] = "商城充值";
                        strings[4] = $"商城道具";
                        strings[5] = c2G_CreateAnOrder.AppCallbackUrl;
                        strings[6] = "PaySucessCallBack";
                        strings[7] = "PayFailureCallBack";
                       
                        SdkCallBackComponent.Instance.sdkUtility.Pay(strings);
                       
                    }

                });
            }
        }
    }
}