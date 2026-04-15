using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System;
using UnityEngine.UI;



namespace ETHotfix
{

    [ObjectSystem]
    public class UILimitTopUpComponentAwake : AwakeSystem<UILimitTopUpComponent>
    {
        public override void Awake(UILimitTopUpComponent self)
        {
            self.referenceCollector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.referenceCollector.GetButton("CloseBtn").onClick.AddSingleListener(() => UIComponent.Instance.Remove(UIType.UILimitTopUp));
            self.goodsTarget = self.referenceCollector.GetGameObject("goodsTarget").transform;
            self.modepos = self.referenceCollector.GetImage("targetPos").transform;
             
            self.referenceCollector.GetButton("TopUpBtn").onClick.AddSingleListener(async () =>
            {
                TopUpComponent.Instance.TopUp((int)E_PlayerShopQuotaType.AwardFlag).Coroutine();

                return;
                if (ETModel.Init.instance.e_SDK == E_SDK.XY_SDK)
                {
                    C2G_CreateAnOrderResponse c2G_CreateAnOrder = (C2G_CreateAnOrderResponse)await SessionComponent.Instance.Session.Call(new C2G_CreateAnOrderRequest
                    {
                        RechargeType=(int)E_PlayerShopQuotaType.AwardFlag,
                     
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
                        SdkCallBackComponent.Instance.sdkUtility.Pay(new string[] { $"{c2G_CreateAnOrder.PayRmb}", $"{c2G_CreateAnOrder.OrderId}", $"{(int)E_TopUpType.ActivityTopUp}", $"活动充值", $"{GlobalDataManager.EnterZoneID}", $"{GlobalDataManager.EnterZoneName}", $"{GlobalDataManager.XYUUID}", $"{UnitEntityComponent.Instance.LocaRoleUUID}", $"{UnitEntityComponent.Instance.LocalRole.RoleName}", $"{UnitEntityComponent.Instance.LocalRole.Level}", $"{c2G_CreateAnOrder.AppCallbackUrl}", "", "", $"{SdkCallBackComponent.Instance.sdkUtility.CallAllObjName}", "PaySuccess", "PayFaulure" });

                    }

                }
                else if (ETModel.Init.instance.e_SDK == E_SDK.ZHIFUBAO_WECHAT)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIPay, (int)E_PlayerShopQuotaType.AwardFlag);

                  /*  G2C_MyPayTopUp c2G_MyPayTopUp = (G2C_MyPayTopUp)await SessionComponent.Instance.Session.Call(new C2G_MyPayTopUp
                    {
                        RechargeType = (int)E_PlayerShopQuotaType.AwardFlag,
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
                        RechargeType = (int)E_PlayerShopQuotaType.AwardFlag,
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
                        RechargeType = (int)E_PlayerShopQuotaType.AwardFlag,
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
                        strings[1] = $"{c2G_CreateAnOrder.PayRmb* 10 * 10}";
                        strings[2] = "商城充值";
                        strings[3] = "商城充值";
                        strings[4] = $"商城道具";
                        strings[5] = c2G_CreateAnOrder.AppCallbackUrl;
                        strings[6] = "PaySucessCallBack";
                        strings[7] = "PayFailureCallBack";

                        SdkCallBackComponent.Instance.sdkUtility.Pay(strings);
                    }
                }
                else if (ETModel.Init.instance.e_SDK==E_SDK.HAO_YI_SDK)
                {
                    self.referenceCollector.GetButton("TopUpBtn").onClick.AddSingleListener(async () =>
                    {
                        ///领取奖励

                        G2C_ShopMallReceiveResponse g2C_ShopMallBuyItemResponse = (G2C_ShopMallReceiveResponse)await SessionComponent.Instance.Session.Call(new C2G_ShopMallReceiveRequest
                        {
                            Type = self.payment
                        });
                        if (g2C_ShopMallBuyItemResponse.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ShopMallBuyItemResponse.Message);

                        }
                        else
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "领取成功");
                        }
                    });
                    //开好易充值
                   // GlobalDataManager.HaoYiTopUp();
                }
                else
                {
                    G2C_RechargeResponse g2C_RechargeResponse = (G2C_RechargeResponse)await SessionComponent.Instance.Session.Call(new C2G_RechargeRequest
                    {
                       
                        Page = (int)E_PlayerShopQuotaType.AwardFlag,
                    });
                    if (g2C_RechargeResponse.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_RechargeResponse.Error.GetTipInfo());
                        Log.DebugRed($"g2C_RechargeResponse.Error:{g2C_RechargeResponse.Error}");
                    }
                    else
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"充值成功");
                    }
                }


            });

            self.good= ResourcesComponent.Instance.LoadGameObject(self.goodsResName.StringToAB(), self.goodsResName);
            self.good.transform.SetParent(self.goodsTarget,false);
            self.good.transform.localPosition = Vector3.zero;
            self.good.transform.localRotation = Quaternion.identity;
            self.Infos = self.referenceCollector.GetImage("Infos").transform;
            self.itemInfo = self.Infos.GetChild(0);

            self.LimitTimeTxt = self.referenceCollector.GetText("LimitTimeTxt");
            var info = ConfigComponent.Instance.GetItem<Activity_InfoConfig>(3);
            //获取时间戳
            DateTime starttime = Convert.ToDateTime(info.OpenTime);
            DateTime endtime = Convert.ToDateTime(info.EndTime);
          //  DateTime starttime = Convert.ToDateTime("2023-10-31 23:00:00");
         //   DateTime endtime = Convert.ToDateTime("2023-10-31 23:00:00");
            if (DateTime.Compare(DateTime.Now, endtime) > 0)
            {
                
                self.LimitTimeTxt.text = "限时活动已结束";
            }
            else
            {
                self.span = endtime.Subtract(DateTime.Now);
             //   Log.DebugYellow($"{self.span.Days}天{self.span.Hours}时{self.span.Minutes}分{self.span.Seconds}秒");
                self.LimitTimeTxt.text = $"{self.span.Days}天{self.span.Hours}时{self.span.Minutes}分{self.span.Seconds}秒";
            }

            
            self.dataItem = ComponentFactory.Create<KnapsackDataItem>();
            self.dataItem.ConfigId = 340001;
            self.dataItem.ConfigId.GetItemInfo_Out(out self.dataItem.item_Info);
          
            self.InitAtrEvent();
        }
    }
    [ObjectSystem]
    public class UILimitTopUpComponentUpdate : UpdateSystem<UILimitTopUpComponent>
    {
        public override void Update(UILimitTopUpComponent self)
        {
            if (self.span.TotalSeconds <= 0)
            {
                self.LimitTimeTxt.text = "限时活动已结束";
                return;
            }
            if (Time.time >= self.nexttime)
            {
                self.span = self.span.Subtract(self.subSpane);
                self.nexttime = Time.time+1;
                self.LimitTimeTxt.text = $"{self.span.Days}天{self.span.Hours}时{self.span.Minutes}分{self.span.Seconds}秒";
            }
        }
    }
    /// <summary>
    /// 限时充值（送旗帜）
    /// </summary>
    public class UILimitTopUpComponent : Component
    {
        public ReferenceCollector referenceCollector;
        public Transform goodsTarget;
        public string goodsResName = "Shop_JiNianQiZhi";
        public Text LimitTimeTxt;

        public TimeSpan span;
        public float nexttime = 1;
        public TimeSpan subSpane = new TimeSpan(0, 0, 1);

        public KnapsackDataItem dataItem;
      
        public Transform modepos;
        List<String> ItemAtrList = new List<String>();

        public GameObject good;

        public Transform Infos,itemInfo;

        public int payment = 588;//支付金额
        public void InitAtrEvent()
        {
            UGUITriggerProxy triggerProxy = modepos.GetComponent<UGUITriggerProxy>();
           
            triggerProxy.OnPointerClickEvent = () =>
            {
                ShowAtr();
                if (Infos.gameObject.activeSelf)
                {
                    Infos.gameObject.SetActive(false);
                }
                else
                {
                    Infos.gameObject.SetActive(true);
                }
            };
           ShowAtr();
        }
        public void ShowAtr()
        {
           
            if (ItemAtrList == null)
            {
                ItemAtrList = new List<string>(100);
            }
            ItemAtrList.Clear();
            dataItem.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);

            dataItem.GetItemName(ref ItemAtrList);//装备 名字
            dataItem.GetItemCount(ref ItemAtrList);//数量

            if (dataItem.ConfigId == 260012)//黑王马
            {
                dataItem.GetHeiWangMaAtrs(ref ItemAtrList);
            }
            else if (dataItem.ConfigId == 260011)//炎狼兽之角 +幻影
            {
                dataItem.GetYangLangShouZhiJiaoHuanYingAtrs(ref ItemAtrList);
            }
            else if (dataItem.IsTreasureItem())
            {
                dataItem.GetTreasureAtrs(ref ItemAtrList);
            }
            else
            {
                dataItem.GetBaseAtrs(ref ItemAtrList);//基本属性(读表)
                dataItem.GetItemCommonBaseAtr(ref ItemAtrList);//基本属性
                dataItem.GetGemsAtr(ref ItemAtrList);//荧光宝石属性
                                                     // dataItem.GetLevNeed(ref ItemAtrList);//等级需求
                dataItem.GetUserType(ref ItemAtrList);//职业限制
                dataItem.GetExtraEntryAtr(ref ItemAtrList);//套装附带的额外属性
                dataItem.GetItemSkill(ref ItemAtrList);//技能
                dataItem.GetLuckyAtr(ref ItemAtrList);//幸运属性
                dataItem.GetAppendAtr(ref ItemAtrList);//追加属性
                dataItem.GetExecllentEntry(ref ItemAtrList);//卓越属性
                dataItem.GetSpecialEntry(ref ItemAtrList);//特殊属性-翅膀
                dataItem.GetReginAtr(ref ItemAtrList);//再生属性
                dataItem.GetInlayAtr(ref ItemAtrList);//镶嵌属性
                dataItem.GetSuitAtr(ref ItemAtrList);//套装属性
                dataItem.GetVaildTime(ref ItemAtrList);//有效时间
                dataItem.GetRemarks(ref ItemAtrList);//备注提示信息
                dataItem.GetAdmissionTicketOpenTime(ref ItemAtrList);//副本开放时间
            }
            ShowAtr(ItemAtrList,Infos,itemInfo);
            
            //显示属性
            void ShowAtr(List<string> list, Transform content, Transform childitem)
            {
                int atrCount = list.Count;
                int introChildCount = content.childCount;
                if (introChildCount > atrCount)//隐藏多余的Item
                {
                    for (int i = atrCount; i < introChildCount; i++)
                    {
                        content.GetChild(i).gameObject.SetActive(false);
                    }
                }

                for (int i = 0; i < atrCount; i++)
                {
                    Transform item;
                    if (i < introChildCount)
                    {
                        item = content.GetChild(i);
                        item.gameObject.SetActive(true);

                    }
                    else
                    {
                        item = GameObject.Instantiate<Transform>(childitem, content);
                    }
                    item.GetComponent<Text>().text = list[i].ToString();

                }
            }
        }
        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
            dataItem.Dispose();
            UIComponent.Instance.Remove(UIType.UIIntroduction);
            
            ResourcesComponent.Instance.DestoryGameObjectImmediate(good, goodsResName.StringToAB());
          
        }
    }
}
