using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System;

namespace ETHotfix
{
    [ObjectSystem]
    public class TopUpComponentAwake : AwakeSystem<TopUpComponent>
    {
        public override void Awake(TopUpComponent self)
        {
            TopUpComponent.Instance = self;
        }
    }
    /// <summary>
    /// 充值组件
    /// </summary>
    public class TopUpComponent : Component
    {
        public static TopUpComponent Instance;

        /// <summary>
        /// 请求充值
        /// </summary>
        /// <param name="topType"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public async ETVoid TopUp(int topType,Action action=null)
        {
           // Log.DebugGreen($"请求充值：{topType}->{(E_PlayerShopQuotaType)topType}");
            if (ETModel.Init.instance.e_SDK == E_SDK.XY_SDK)
            {
                C2G_CreateAnOrderResponse c2G_CreateAnOrder = (C2G_CreateAnOrderResponse)await SessionComponent.Instance.Session.Call(new C2G_CreateAnOrderRequest
                {
                    RechargeType = topType,

                });
                if (c2G_CreateAnOrder.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, c2G_CreateAnOrder.Error.GetTipInfo());
                    //  Log.DebugRed($"g2C_RechargeResponse.Error:{c2G_CreateAnOrder.Error}");
                }
                else
                {
                    //  Log.DebugBrown($"可以充值：{2000} {c2G_CreateAnOrder.OrderId} url:{c2G_CreateAnOrder.AppCallbackUrl}");
                    //Pay($"{1}",c2G_CreateAnOrder.OrderId,topuptype.ToString(),c2G_CreateAnOrder.AppCallbackUrl);
                    SdkCallBackComponent.Instance.sdkUtility.Pay(new string[] { $"{c2G_CreateAnOrder.PayRmb}", $"{c2G_CreateAnOrder.OrderId}", $"{(int)E_TopUpType.ActivityTopUp}", $"{(E_PlayerShopQuotaType)topType}", $"{GlobalDataManager.EnterZoneID}", $"{GlobalDataManager.EnterZoneName}", $"{GlobalDataManager.XYUUID}", $"{UnitEntityComponent.Instance.LocaRoleUUID}", $"{UnitEntityComponent.Instance.LocalRole.RoleName}", $"{UnitEntityComponent.Instance.LocalRole.Level}", $"{c2G_CreateAnOrder.AppCallbackUrl}", "", "", $"{SdkCallBackComponent.Instance.sdkUtility.CallAllObjName}", "PaySuccess", "PayFaulure" });//额外参数 为传

                }

            }
            else if (ETModel.Init.instance.e_SDK == E_SDK.ZHIFUBAO_WECHAT)
            {
                UIComponent.Instance.VisibleUI(UIType.UIPay, topType);

                /*  G2C_MyPayTopUp c2G_MyPayTopUp = (G2C_MyPayTopUp)await SessionComponent.Instance.Session.Call(new C2G_MyPayTopUp
                  {
                      RechargeType = topType,
                  });
                  if (c2G_MyPayTopUp.Error != 0)
                  {
                      Log.DebugRed($"c2G_MyPayTopUp.Error:{c2G_MyPayTopUp.Error}");
                      UIComponent.Instance.VisibleUI(UIType.UIHint, c2G_MyPayTopUp.Error.GetTipInfo());
                  }
                  else
                  {
                      SdkCallBackComponent.Instance.sdkUtility.Pay(new string[] { c2G_MyPayTopUp.OrderStr});
                  }*/
            }
            else if (ETModel.Init.instance.e_SDK == E_SDK.SHOU_Q)
            {
                G2C_ShouQCreateAnOrder c2G_CreateAnOrder = (G2C_ShouQCreateAnOrder)await SessionComponent.Instance.Session.Call(new C2G_ShouQCreateAnOrder
                {
                    RechargeType = topType,
                });
                if (c2G_CreateAnOrder.Error != 0)
                {
                    Log.DebugGreen($"{c2G_CreateAnOrder.Error}");
                    UIComponent.Instance.VisibleUI(UIType.UIHint, c2G_CreateAnOrder.Error.GetTipInfo());

                }
                else
                {

                    string[] strings = new string[6];
                    strings[0] = c2G_CreateAnOrder.OrderId;//订单号
                    strings[1] = $"{c2G_CreateAnOrder.PayRmb}";//支付金额
                    strings[2] = $"{GlobalDataManager.EnterZoneName}";//游戏区服名字
                    strings[3] = $"{UnitEntityComponent.Instance.LocaRoleUUID}";//登录信息中的uuid [传入已解密的uid]
                    strings[4] = $"{GlobalDataManager.ShouQUUID}";//
                    strings[5] = c2G_CreateAnOrder.ExInfo;//额外信息

                    SdkCallBackComponent.Instance.sdkUtility.Pay(strings);

                }
                
            }
            else if (ETModel.Init.instance.e_SDK == E_SDK.TIKTOK_SDK)
            {
                C2G_DouYinCreateAnOrderResponse c2G_CreateAnOrder = (C2G_DouYinCreateAnOrderResponse)await SessionComponent.Instance.Session.Call(new C2G_DouYinCreateAnOrderRequest
                {
                    RechargeType = topType,
                    ControlInfo = SdkCallBackComponent.Instance.sdkUtility.GetRiskControlInfo(),//风控参数
                });
                if (c2G_CreateAnOrder.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, c2G_CreateAnOrder.Error.GetTipInfo());

                }
                else
                {
                    // Log.DebugGreen($"请求支付：{topuprmb * 10 * 10} 分");
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
            }
            else if (ETModel.Init.instance.e_SDK == E_SDK.HaXi)
            {
                Log.Debug("哈希网络支付");

                G2C_MagicBoxCreateOrder c2G_CreateAnOrder = (G2C_MagicBoxCreateOrder)await SessionComponent.Instance.Session.Call(new C2G_MagicBoxCreateOrder
                {
                    RechargeType = topType,
                });
                if (c2G_CreateAnOrder.Error != 0)
                {
                    Log.DebugGreen($"{c2G_CreateAnOrder.Error}");
                    UIComponent.Instance.VisibleUI(UIType.UIHint, c2G_CreateAnOrder.Error.GetTipInfo());
                }
                else
                {
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    dict.Add("ExtInfo", c2G_CreateAnOrder.ServerAddres);
                    dict.Add("ProductId", (topType).ToString());
                    dict.Add("ProductName", "充值魔晶");
                    dict.Add("ProductDesc", "魔晶");
                    dict.Add("ProductCount", (c2G_CreateAnOrder.Money).ToString());
                    dict.Add("ProductPrice", (c2G_CreateAnOrder.Money * 100).ToString());
                    //dict.Add("ProductPrice", (c2G_CreateAnOrder.Money ).ToString());
                    dict.Add("CurrencyName", "人民币");
                    dict.Add("ExchangeRate", "1");
                    dict.Add("RoleId", UnitEntityComponent.Instance.LocalRole.Id.ToString());
                    dict.Add("RoleName", UnitEntityComponent.Instance.LocalRole.RoleName);
                    dict.Add("ServerId", GlobalDataManager.EnterZoneID.ToString());
                    dict.Add("ServerName", GlobalDataManager.EnterZoneName);

                    string[] strings = new string[1];
                    strings[0] = JsonHelper.ToJson(dict);//订单号

                    SdkCallBackComponent.Instance.sdkUtility.Pay(strings);


                }
            }
            else if (ETModel.Init.instance.e_SDK == E_SDK.HAO_YI_SDK)
            {
                //开好易充值
                 GlobalDataManager.HaoYiTopUp();
            }
            else
            {
                //开好易充值
               GlobalDataManager.HaoYiTopUp();
                ////默认充值
                //G2C_RechargeResponse g2C_RechargeResponse = (G2C_RechargeResponse)await SessionComponent.Instance.Session.Call(new C2G_RechargeRequest
                //{
                //    Page = topType,
                //});
                //if (g2C_RechargeResponse.Error != 0)
                //{
                //    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_RechargeResponse.Error.GetTipInfo());
                //    Log.DebugRed($"g2C_RechargeResponse.Error:{g2C_RechargeResponse.Error}");
                //}
                //else
                //{
                //    UIComponent.Instance.VisibleUI(UIType.UIHint, $"充值成功");
                //}
            }


        }
    }
}