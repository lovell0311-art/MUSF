using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{
    
    public partial class UIShopComponent
    {
        public void Init_TopUp_ZhiFuBaoSdk()
        {

            /* SdkCallBackComponent.Instance.PaySucessCallBack = (s) => 
             {
                 Log.DebugRed($"支付回调：{s}");
             };*/
            //--------旧的
            //Transform TopUps = collector.GetGameObject("TopUps").transform;
            //for (int i = 0, length = TopUps.childCount; i < length; i++)
            //{
            //    Button button = TopUps.GetChild(i).GetComponent<Button>();
            //    var value = (int)E_PlayerShopQuotaType.StoreRechargeI + i;
            //    TopUpComponent.Instance.TopUp(value).Coroutine();
            //    button.onClick.AddSingleListener(async () =>
            //    {
            //        /* G2C_MyPayTopUp c2G_MyPayTopUp = (G2C_MyPayTopUp)await SessionComponent.Instance.Session.Call(new C2G_MyPayTopUp
            //         {
            //             RechargeType = value,
            //         });
            //         if (c2G_MyPayTopUp.Error != 0)
            //         {

            //             UIComponent.Instance.VisibleUI(UIType.UIHint, c2G_MyPayTopUp.Error.GetTipInfo());
            //         }
            //         else
            //         {
            //             SdkCallBackComponent.Instance.sdkUtility.Pay(new string[] { c2G_MyPayTopUp.OrderStr });

            //         }*/
            //        Log.DebugRed($"请求支付：{value}");
            //        UIComponent.Instance.VisibleUI(UIType.UIPay,value);


            //       /* G2C_MyV4PayTopUp c2G_MyV4PayTopUp = (G2C_MyV4PayTopUp)await SessionComponent.Instance.Session.Call(new C2G_MyV4PayTopUp
            //        {
            //            RechargeType = value,
            //            PayType = "wechatLiteH5"
            //            //PayType="alipayWap"
            //        });
            //        if (c2G_MyV4PayTopUp.Error != 0)
            //        {

            //            UIComponent.Instance.VisibleUI(UIType.UIHint, c2G_MyV4PayTopUp.Error.GetTipInfo());
            //        }
            //        else
            //        {
            //            Log.DebugGreen($"{c2G_MyV4PayTopUp.OrderStrURL}");
            //            Application.OpenURL(c2G_MyV4PayTopUp.OrderStrURL);
            //        }*/

            //    });
            //}
            //--------------


            Transform TopUps = collector.GetGameObject("TopUps").transform;
            for (int i = 0, length = TopUps.childCount; i < length; i++)
            {
                Button button = TopUps.GetChild(i).GetComponent<Button>();
                var value = (int)E_PlayerShopQuotaType.StoreRechargeI + i;

                if (ETModel.Init.instance.e_SDK == E_SDK.ZHIFUBAO_WECHAT)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIPay, $"{value}");
                }
                else
                {
                    TopUpComponent.Instance.TopUp(value).Coroutine();

                }

                button.onClick.AddSingleListener(async () =>
                {

                    if (ETModel.Init.instance.e_SDK == E_SDK.ZHIFUBAO_WECHAT)
                    {
                        //UIComponent.Instance.VisibleUI(UIType.UIPay,value);
                        UIComponent.Instance.VisibleUI(UIType.UIPay, $"{value}");
                    }
                    else
                    {
                        TopUpComponent.Instance.TopUp(value).Coroutine();

                    }
                });
            }

        }
    }
}
