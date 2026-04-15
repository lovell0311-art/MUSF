using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ETModel;
using UnityEngine.UI;



namespace ETHotfix
{
    [ObjectSystem]
    public class TopUp_7_DayComponentAwake : AwakeSystem<TopUp_7_DayComponent>
    {
        public override void Awake(TopUp_7_DayComponent self)
        {
            self.referenceCollector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.referenceCollector.GetButton("CloseBtn").onClick.AddSingleListener(()=>UIComponent.Instance.Remove(UIType.TopUp_7_Day));
            self.InitTitle();
            TopUp_7_DayComponent.Instance = self;
        }
    }

    /// <summary>
    /// 七日充值
    /// </summary>
    public class TopUp_7_DayComponent : Component
    {
       public ReferenceCollector referenceCollector;

        public static TopUp_7_DayComponent Instance;
        /// <summary>
        /// 每日充值金额
        /// </summary>
        public int topuprmb =68;

        //直接领取
        public void InitTitle_Receive() 
        {
            for (int i = 1; i <= 7; i++) 
            {
                Transform item = referenceCollector.GetImage($"Item_{i}").transform;
                int day = i;
                item.Find("TopUp").Find("Text").GetComponent<Text>().text = "领取";
                if (GlobalDataManager.SevenDaysToRechargeDic.ContainsKey(i) && GlobalDataManager.SevenDaysToRechargeDic[i])
                {
                    //已领取
                    item.Find("TopUp").Find("Text").GetComponent<Text>().text = "已领取";
                    item.Find("TopUp").GetComponent<Button>().interactable = false;
                    continue;
                }
                item.Find("TopUp").GetComponent<Button>().onClick.AddSingleListener(async () =>
               {
                   ///领取奖励

                   G2C_ShopMallReceiveResponse g2C_ShopMallBuyItemResponse = (G2C_ShopMallReceiveResponse)await SessionComponent.Instance.Session.Call(new C2G_ShopMallReceiveRequest
                   {
                       Type = topuprmb,
                       // Day = day
                   });
                   if (g2C_ShopMallBuyItemResponse.Error != 0)
                   {
                       UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ShopMallBuyItemResponse.Message);
                       
                   }
                   else
                   {
                       UIComponent.Instance.VisibleUI(UIType.UIHint, "领取成功");
                       GlobalDataManager.SevenDaysToRechargeDic[day] = true;
                       item.Find("TopUp").Find("Text").GetComponent<Text>().text = "已领取";
                       item.Find("TopUp").GetComponent<Button>().interactable = false;
                   }

               });
            }
        }
        //充值后 再领取
        public void InitTitle() 
        {
            for (int i =1; i <= 7; i++)
            {
                Transform item = referenceCollector.GetImage($"Item_{i}").transform;
                int day = i;
                item.Find("TopUp").Find("Text").GetComponent<Text>().text = "领取";
              
                if (GlobalDataManager.SevenDaysToRechargeDic.ContainsKey(i))//已充值
                {

                    //直接发送邮件 不手动领取
                    item.Find("TopUp").Find("Text").GetComponent<Text>().text = "已领取";
                    item.Find("TopUp").GetComponent<Button>().interactable = false;
                    continue;

                    #region 手动领取
                    if (GlobalDataManager.SevenDaysToRechargeDic[i])
                    {
                        //已领取
                        item.Find("TopUp").Find("Text").GetComponent<Text>().text = "已领取";
                        item.Find("TopUp").GetComponent<Button>().interactable = false;
                        continue;
                    }
                    else
                    {
                        //未领取
                        item.Find("TopUp").Find("Text").GetComponent<Text>().text = "领取";
                        item.Find("TopUp").GetComponent<Button>().onClick.AddSingleListener(async () =>
                        {
                            ///领取奖励
                            Log.DebugGreen($"请求领取奖励：{topuprmb}");
                            G2C_ShopMallReceiveResponse g2C_ShopMallBuyItemResponse = (G2C_ShopMallReceiveResponse)await SessionComponent.Instance.Session.Call(new C2G_ShopMallReceiveRequest
                            {
                                Type = topuprmb,
                                Day = day
                            });
                            if (g2C_ShopMallBuyItemResponse.Error != 0)
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ShopMallBuyItemResponse.Error.GetTipInfo());
                            }
                            else
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "领取成功");
                                GlobalDataManager.SevenDaysToRechargeDic[day] = true;
                                item.Find("TopUp").Find("Text").GetComponent<Text>().text = "已领取";
                                item.Find("TopUp").GetComponent<Button>().interactable = false;
                            }

                        });

                    } 
                    #endregion
                }
                else
                {
                    item.Find("TopUp").Find("Text").GetComponent<Text>().text = "充值"+ topuprmb+"元";
                    item.Find("TopUp").GetComponent<Button>().onClick.AddSingleListener(() => {
                        if (IsCanCToday())
                        {
                            //可充值  
                            
                            TopUpComponent.Instance.TopUp((int)E_PlayerShopQuotaType.SevenDaysTopUp).Coroutine();
                            //CZ().Coroutine();
                        }
                        else {
                            //今天已充值
                            UIComponent.Instance.VisibleUI(UIType.UIHint,"今日已充值");
                        }
                    });
                    async ETVoid CZ()
                    {
                        //充值

                        if (ETModel.Init.instance.e_SDK == E_SDK.XY_SDK)
                        {
                            C2G_CreateAnOrderResponse c2G_CreateAnOrder = (C2G_CreateAnOrderResponse)await SessionComponent.Instance.Session.Call(new C2G_CreateAnOrderRequest
                            {
                                RechargeType = (int)E_PlayerShopQuotaType.SevenDaysTopUp,

                            });
                            if (c2G_CreateAnOrder.Error != 0)
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, c2G_CreateAnOrder.Error.GetTipInfo());
                                Log.DebugRed($"g2C_RechargeResponse.Error:{c2G_CreateAnOrder.Error}");
                            }
                            else
                            {
                                Log.DebugBrown($"可以充值：{1} {c2G_CreateAnOrder.OrderId} url:{c2G_CreateAnOrder.AppCallbackUrl}");
                                //Pay($"{1}",c2G_CreateAnOrder.OrderId,topuptype.ToString(),c2G_CreateAnOrder.AppCallbackUrl);
                                SdkCallBackComponent.Instance.sdkUtility.Pay(new string[] { $"{c2G_CreateAnOrder.PayRmb}", $"{c2G_CreateAnOrder.OrderId}", $"{(int)E_TopUpType.SevenDays}", $"七日充值", $"{GlobalDataManager.EnterZoneID}", $"{GlobalDataManager.EnterZoneName}", $"{GlobalDataManager.XYUUID}", $"{UnitEntityComponent.Instance.LocaRoleUUID}", $"{UnitEntityComponent.Instance.LocalRole.RoleName}", $"{UnitEntityComponent.Instance.LocalRole.Level}", $"{c2G_CreateAnOrder.AppCallbackUrl}", "", "", $"{SdkCallBackComponent.Instance.sdkUtility.CallAllObjName}", "PaySucessCallBack", "PayFailureCallBack" });

                            }
                        }
                        else if (ETModel.Init.instance.e_SDK == E_SDK.HAO_YI_SDK)
                        {
                            //开好易充值
                            GlobalDataManager.HaoYiTopUp();
                        }
                        else if (ETModel.Init.instance.e_SDK == E_SDK.ZHIFUBAO_WECHAT)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIPay, $"{(int)E_PlayerShopQuotaType.SevenDaysTopUp}");

                            /*G2C_MyPayTopUp c2G_MyPayTopUp = (G2C_MyPayTopUp)await SessionComponent.Instance.Session.Call(new C2G_MyPayTopUp 
                            {
                                RechargeType = (int)E_PlayerShopQuotaType.SevenDaysTopUp,
                            });
                            if (c2G_MyPayTopUp.Error != 0)
                            {
                               
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
                                RechargeType = (int)E_PlayerShopQuotaType.SevenDaysTopUp,
                            });
                            if (c2G_CreateAnOrder.Error != 0)
                            {
                                Log.DebugGreen($"{c2G_CreateAnOrder.Error}");
                                UIComponent.Instance.VisibleUI(UIType.UIHint, c2G_CreateAnOrder.Error.GetTipInfo());

                            }
                            else
                            {

                                string[] strings = new string[6];
                                strings[0] = c2G_CreateAnOrder.OrderId;
                                strings[1] = $"{c2G_CreateAnOrder.PayRmb}";
                                strings[2] = $"{GlobalDataManager.EnterZoneName}";
                                strings[3] = $"{UnitEntityComponent.Instance.LocaRoleUUID}";
                                strings[4] = $"{GlobalDataManager.ShouQUUID}";
                                strings[5] = "";

                                SdkCallBackComponent.Instance.sdkUtility.Pay(strings);

                            }
                        }
                        else if (ETModel.Init.instance.e_SDK == E_SDK.TIKTOK_SDK)
                        {
                            C2G_DouYinCreateAnOrderResponse c2G_CreateAnOrder = (C2G_DouYinCreateAnOrderResponse)await SessionComponent.Instance.Session.Call(new C2G_DouYinCreateAnOrderRequest
                            {
                                RechargeType = (int)E_PlayerShopQuotaType.SevenDaysTopUp,
                                ControlInfo = SdkCallBackComponent.Instance.sdkUtility.GetRiskControlInfo(),//风控参数
                            });
                            if (c2G_CreateAnOrder.Error != 0)
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, c2G_CreateAnOrder.Error.GetTipInfo());

                            }
                            else
                            {
                                Log.DebugGreen($"请求支付：{topuprmb * 10 * 10} 分");
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
                        else
                        {
                            G2C_RechargeResponse g2C_RechargeResponse = (G2C_RechargeResponse)await SessionComponent.Instance.Session.Call(new C2G_RechargeRequest
                            {

                                Page = (int)E_PlayerShopQuotaType.SevenDaysTopUp,
                            });
                            if (g2C_RechargeResponse.Error != 0)
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_RechargeResponse.Error.GetTipInfo());
                                Log.DebugRed($"g2C_RechargeResponse.Error:{g2C_RechargeResponse.Error}");
                            }
                            else
                            {
                                //item.Find("TopUp").Find("Text").GetComponent<Text>().text = "领取";

                                UIComponent.Instance.VisibleUI(UIType.UIHint, $"成功充值 {topuprmb} 元");
                                //直接发送邮件 不手动领取
                                item.Find("TopUp").Find("Text").GetComponent<Text>().text = "已领取";
                                item.Find("TopUp").GetComponent<Button>().interactable = false;
                            }

                        }
                    }
                }
            }
        }
        public bool IsCanCToday() {
            string todayTimeStr = System.DateTime.Now.ToString("yyyy/M/d");
            string timeStr = null;
            for (int i = 1; i <= 7; i++)
            {
                if (GlobalDataManager.SevenDaysToRechargeDic2.ContainsKey(i))
                {
                    timeStr = GlobalDataManager.SevenDaysToRechargeDic2[i];
                    if (timeStr == todayTimeStr)
                    {            
                        return false;
                    }
                }
            }
            return true;
        }
        //获取奖励物品 信息
        public (string name, string resname) GetItemInfo(int day) => day switch
        {
            1=>("恶魔门票", "TopUp_emomenpiao"),
            2=>("血色门票", "TopUp_xuesemenpiao"),
            3=>("恶魔门票", "TopUp_emomenpiao"),
            4=>("血色门票", "TopUp_xuesemenpiao"),
            5=>("昆顿印记", "TopUp_kundunyinji"),
            6=>("藏宝图", "TopUp_cangbaotu"),
           7=>("洛克之羽", "TopUp_luokezhiyu"),
            _=>(null,null)
        };


    public void RegisterReceiveRequest(int day)
        {
          
            if (GlobalDataManager.SevenDaysToRechargeDic.ContainsKey(day) && GlobalDataManager.SevenDaysToRechargeDic[day])
            {
                return;
            }
            Transform item = referenceCollector.GetImage($"Item_{day}").transform;
            item.Find("TopUp").Find("Text").GetComponent<Text>().text = "领取";
            item.Find("TopUp").GetComponent<Button>().onClick.AddSingleListener(async () =>
            {
                ///领取奖励

                G2C_ShopMallReceiveResponse g2C_ShopMallBuyItemResponse = (G2C_ShopMallReceiveResponse)await SessionComponent.Instance.Session.Call(new C2G_ShopMallReceiveRequest
                {
                    Type = 6,
                    Day = day,
                });
                if (g2C_ShopMallBuyItemResponse.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ShopMallBuyItemResponse.Error.GetTipInfo());
                 //   Log.DebugRed($"g2C_ShopMallBuyItemResponse.Error:{g2C_ShopMallBuyItemResponse.Error}");
                }
                else
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "领取成功");
                    item.Find("TopUp").Find("Text").GetComponent<Text>().text = "已领取";
                    item.Find("TopUp").GetComponent<Button>().interactable = false;
                    GlobalDataManager.SevenDaysToRechargeDic[day] = true;
                }

            });
        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
            TopUp_7_DayComponent.Instance = null;

        }
    }
}
