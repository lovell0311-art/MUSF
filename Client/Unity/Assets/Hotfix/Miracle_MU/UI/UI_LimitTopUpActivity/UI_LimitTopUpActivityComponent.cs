using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System;

namespace ETHotfix
{

    [ObjectSystem]
    public class UI_LimitTopUpActivityComponentAwake : AwakeSystem<UI_LimitTopUpActivityComponent>
    {
        //龙王旗帜  赤焰兽
        public override void Awake(UI_LimitTopUpActivityComponent self)
        {
            self.collectorData = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.collectorData.GetButton("CloseBtn").onClick.AddSingleListener(() => UIComponent.Instance.Remove(UIType.UI_LimitTopUpActivity));
            self.collectorData.GetButton("topUp_1").onClick.AddSingleListener(() => TopUpComponent.Instance.TopUp((int)E_PlayerShopQuotaType.ActiveTopUpI).Coroutine()) ;
         //   self.collectorData.GetButton("topUp_2").onClick.AddSingleListener(() => self.TopUp((int)E_PlayerShopQuotaType.AwardFlag).Coroutine());

            self.target_1 = self.collectorData.GetImage("target_1").transform;
            self.target_2 = self.collectorData.GetImage("target_2").transform;
            self.target_3 = self.collectorData.GetImage("target_3").transform;
            self.target_4 = self.collectorData.GetImage("target_4").transform;

            self.LimitTimeTxt = self.collectorData.GetText("LimitTimeTxt");
            var info = ConfigComponent.Instance.GetItem<Activity_InfoConfig>(6);
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
            self.InitItem();
        }
    }

    [ObjectSystem]
    public class UI_LimitTopUpActivityComponentUpdate : UpdateSystem<UI_LimitTopUpActivityComponent>
    {
        public override void Update(UI_LimitTopUpActivityComponent self)
        {
            if (self.span.TotalSeconds <= 0)
            {
                self.LimitTimeTxt.text = "限时活动已结束";
                return;
            }
            if (Time.time >= self.nexttime)
            {
                self.span = self.span.Subtract(self.subSpane);
                self.nexttime = Time.time + 1;
                self.LimitTimeTxt.text = $"{self.span.Days}天{self.span.Hours}时{self.span.Minutes}分{self.span.Seconds}秒";
            }
        }
    }
    public class UI_LimitTopUpActivityComponent : Component
    {
        public ReferenceCollector collectorData;
        public Transform target_1, target_2, target_3, target_4;
        private List<GameObject> ItemObjList = new List<GameObject>();

        KnapsackDataItem KnapsackDataItem;
        UIIntroductionComponent uIIntroduction;
        List<string> diratr = new List<string>();

        public Text LimitTimeTxt;
        public TimeSpan span;
        public float nexttime = 1;
        public TimeSpan subSpane = new TimeSpan(0, 0, 1);
        public void InitItem()
        {
            KnapsackDataItem = ComponentFactory.Create<KnapsackDataItem>();

            //  GameObject fenghuang=  ResourcesComponent.Instance.LoadGameObject("Shop_fenghuang".StringToAB(), "Shop_fenghuang");
            GameObject fenghuang = ResourcesComponent.Instance.LoadGameObject("Shop_ZhuZaiQiZhi".StringToAB(), "Shop_ZhuZaiQiZhi");
            fenghuang.transform.SetParent(target_1, false);
            fenghuang.transform.localPosition = Vector3.zero;
            ItemObjList.Add(fenghuang);
            target_1.GetComponent<UGUITriggerProxy>().OnPointerClickEvent = () =>
            {
                diratr.Clear();
                // KnapsackDataItem.ConfigId = 260017;
                KnapsackDataItem.ConfigId = 340006;
               /* diratr.Add("合成说明");
                diratr.Add("");
                diratr.Add("<color=yellow>龙王旗杆*3(打变异怪获取)</color>");
                diratr.Add("<color=yellow>龙王宝藏(商城道具)</color>");
                diratr.Add("<color=yellow>使用后可创建变异怪</color>");
                diratr.Add("<color=yellow>魔晶石*1</color>");
                diratr.Add("<color=yellow>金币*1000000</color>");
                diratr.Add($"前往<color=green>仙踪林[233,279]</color>到<color=yellow>哥布林</color>");*/

                ShowAtr(target_1.position);
            };

            GameObject jiezhi=  ResourcesComponent.Instance.LoadGameObject(GetRoleResName().StringToAB(), GetRoleResName());
          //  GameObject jiezhi = ResourcesComponent.Instance.LoadGameObject("shop_ChiYanShou".StringToAB(), "shop_ChiYanShou");
            jiezhi.transform.SetParent(target_2, false);
            jiezhi.transform.localPosition = Vector3.zero;
            ItemObjList.Add(jiezhi);
            target_2.GetComponent<UGUITriggerProxy>().OnPointerClickEvent = () =>
            {
                diratr.Clear();
                KnapsackDataItem.ConfigId = 240019;
              //  KnapsackDataItem.ConfigId = 260018;
                //  diratr.Add($"<color={ColorTools.LuckyItemColor}>随机3条卓越属性</color>");
              /*  diratr.Add("合成说明");
                diratr.Add("");
                diratr.Add("<color=yellow>赤焰兽碎片*3(打变异怪获取)</color>");
                diratr.Add("<color=yellow>赤焰兽孵化蛋(商城道具)</color>");
                diratr.Add("<color=yellow>使用后可创建变异怪</color>");
                diratr.Add("<color=yellow>金币*1000000</color>");
                diratr.Add($"前往<color=green>仙踪林[233,279]</color>到<color=yellow>哥布林</color>");*/

                ShowAtr(target_2.position);
            };

           /* GameObject tianshizhiwen = ResourcesComponent.Instance.LoadGameObject("Shop_tianshizhiwen".StringToAB(), "Shop_tianshizhiwen");
            tianshizhiwen.transform.SetParent(target_3, false);
            tianshizhiwen.transform.localPosition = Vector3.zero;
            ItemObjList.Add(tianshizhiwen);
            target_3.GetComponent<UGUITriggerProxy>().OnPointerClickEvent = () =>
            {
                diratr.Clear();
                //  KnapsackDataItem.ConfigId = 240019;
                KnapsackDataItem.ConfigId = 240020;
                diratr.Add("佩戴等级：50级以上");
                diratr.Add("卓越属性随机3条以上");
                diratr.Add("");
                diratr.Add("合成说明");
                diratr.Add("");
                diratr.Add("<color=yellow>天使戒指*3(打变异怪获取)</color>");
                diratr.Add("<color=yellow>玛雅+祝福</color>");
                diratr.Add("<color=yellow>+灵魂+魔晶石</color>");
                diratr.Add("<color=yellow>+终极勾尾</color>");
                diratr.Add("<color=yellow>金币*1000000</color>");
                diratr.Add($"前往<color=green>仙踪林[233,279]</color>到<color=yellow>哥布林</color>");
                //  diratr.Add($"<color={ColorTools.LuckyItemColor}>随机3条卓越属性</color>");
                diratr.Add("");
                diratr.Add("天使戒指获取途径");
                diratr.Add("");
                diratr.Add("使用<color=yellow>纳戒(商城道具)</color>");
                diratr.Add("<color=yellow>创建变异怪击杀获得</color>");
                ShowAtr(target_3.position);
            };

            GameObject tianshixianglian = ResourcesComponent.Instance.LoadGameObject("Shop_tianshixianglian".StringToAB(), "Shop_tianshixianglian");
            tianshixianglian.transform.SetParent(target_4, false);
            tianshixianglian.transform.localPosition = Vector3.zero;
            ItemObjList.Add(tianshixianglian);
            target_4.GetComponent<UGUITriggerProxy>().OnPointerClickEvent = () =>
            {
                diratr.Clear();
                //  KnapsackDataItem.ConfigId = 240019;
                KnapsackDataItem.ConfigId = 230016;
                diratr.Add("佩戴等级：50级以上");
                diratr.Add("卓越属性随机3条以上");
                diratr.Add("");
                diratr.Add("合成说明");
                diratr.Add("");
                diratr.Add("<color=yellow>小天使项链*3(打变异怪获取)</color>");
                diratr.Add("<color=yellow>玛雅+祝福</color>");
                diratr.Add("<color=yellow>+灵魂+魔晶石</color>");
                diratr.Add("<color=yellow>+坚硬之物</color>");
                diratr.Add("<color=yellow>金币*1000000</color>");
                diratr.Add($"前往<color=green>仙踪林[233,279]</color>到<color=yellow>哥布林</color>");
                //  diratr.Add($"<color={ColorTools.LuckyItemColor}>随机3条卓越属性</color>");
                diratr.Add("");
                diratr.Add("小天使项链获取途径");
                diratr.Add("");
                diratr.Add("使用<color=yellow>小天使(商城道具)</color>");
                diratr.Add("<color=yellow>创建变异怪击杀获得</color>");
                ShowAtr(target_4.position);
            };*/

            string GetRoleResName() => UnitEntityComponent.Instance.LocalRole.RoleType switch
            {
                E_RoleType.Magician => "Shop_tunian_man",
                E_RoleType.Swordsman => "Shop_tunian_man",
                E_RoleType.Archer => "Shop_tunian_woman",
                E_RoleType.Magicswordsman => "Shop_tunian_man",
                E_RoleType.Holymentor => "Shop_tunian_man",
                E_RoleType.Summoner => "Shop_tunian_woman",
                _ => "Shop_tunian_man"
            };
        }
        public void ShowAtr(Vector3 pos)
        {
            uIIntroduction ??= UIComponent.Instance.VisibleUI(UIType.UIIntroduction).GetComponent<UIIntroductionComponent>();
            uIIntroduction.GetAllAtrs(KnapsackDataItem);
            uIIntroduction.AddDiyAtr(diratr);
            uIIntroduction.ShowAtrs();

            uIIntroduction.SetPos(pos += Vector3.left, 1);
        }
        public async ETVoid TopUp(int value)
        {
            ///领取奖励

            if (ETModel.Init.instance.e_SDK == E_SDK.XY_SDK)
            {
                C2G_CreateAnOrderResponse c2G_CreateAnOrder = (C2G_CreateAnOrderResponse)await SessionComponent.Instance.Session.Call(new C2G_CreateAnOrderRequest
                {
                    RechargeType = value,
                });
                if (c2G_CreateAnOrder.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, c2G_CreateAnOrder.Error.GetTipInfo());
                    //  Log.DebugRed($"g2C_RechargeResponse.Error:{c2G_CreateAnOrder.Error}");
                }
                else
                {
                    //   Log.DebugBrown($"可以充值：{1} {c2G_CreateAnOrder.OrderId} url:{c2G_CreateAnOrder.AppCallbackUrl}");
                    //Pay($"{1}",c2G_CreateAnOrder.OrderId,topuptype.ToString(),c2G_CreateAnOrder.AppCallbackUrl);
                    SdkCallBackComponent.Instance.sdkUtility.Pay(new string[] { $"{c2G_CreateAnOrder.PayRmb}", $"{c2G_CreateAnOrder.OrderId}", $"{(int)E_TopUpType.SevenDays}", $"六一活动充值", $"{GlobalDataManager.EnterZoneID}", $"{GlobalDataManager.EnterZoneName}", $"{GlobalDataManager.XYUUID}", $"{UnitEntityComponent.Instance.LocaRoleUUID}", $"{UnitEntityComponent.Instance.LocalRole.RoleName}", $"{UnitEntityComponent.Instance.LocalRole.Level}", $"{c2G_CreateAnOrder.AppCallbackUrl}", "", "", $"{SdkCallBackComponent.Instance.sdkUtility.CallAllObjName}", "PaySuccess", "PayFaulure" });

                }
            }
            else if (ETModel.Init.instance.e_SDK == E_SDK.HAO_YI_SDK)
            {

                G2C_ShopMallReceiveResponse g2C_ShopMallBuyItemResponse = (G2C_ShopMallReceiveResponse)await SessionComponent.Instance.Session.Call(new C2G_ShopMallReceiveRequest
                {

                    Type = value
                });
                if (g2C_ShopMallBuyItemResponse.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ShopMallBuyItemResponse.Message);

                }
                else
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "领取成功");
                }

                //开好易充值
                // GlobalDataManager.HaoYiTopUp();
            }
            else if (ETModel.Init.instance.e_SDK == E_SDK.ZHIFUBAO_WECHAT)
            {
                UIComponent.Instance.VisibleUI(UIType.UIPay,value);
                
                /*G2C_MyPayTopUp c2G_MyPayTopUp = (G2C_MyPayTopUp)await SessionComponent.Instance.Session.Call(new C2G_MyPayTopUp
                {
                    RechargeType = value,
                });
                if (c2G_MyPayTopUp.Error != 0)
                {
                    Log.DebugRed($"c2G_MyPayTopUp.Error:{c2G_MyPayTopUp.Error}");
                    UIComponent.Instance.VisibleUI(UIType.UIHint, c2G_MyPayTopUp.Error.GetTipInfo());
                }
                else
                {

                    SdkCallBackComponent.Instance.sdkUtility.Pay(new string[] { c2G_MyPayTopUp.OrderStr });
                }*/
            }
            else if (ETModel.Init.instance.e_SDK == E_SDK.SHOU_Q)
            {
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
                    RechargeType = value,
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
            else
            {
                G2C_RechargeResponse g2C_RechargeResponse = (G2C_RechargeResponse)await SessionComponent.Instance.Session.Call(new C2G_RechargeRequest
                {

                    Page = value
                });
                if (g2C_RechargeResponse.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_RechargeResponse.Error.GetTipInfo());
                    // Log.DebugRed($"g2C_RechargeResponse.Error:{g2C_RechargeResponse.Error}");
                }
                else
                {
                    //item.Find("TopUp").Find("Text").GetComponent<Text>().text = "领取";

                    UIComponent.Instance.VisibleUI(UIType.UIHint, $"成功充值 {value} 元");
                }

            }


        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
            for (int i = 0; i < ItemObjList.Count; i++)
            {
                ResourcesComponent.Instance.DestoryGameObjectImmediate(ItemObjList[i], ItemObjList[i].name.StringToAB());
            }
            ItemObjList.Clear();
            UIComponent.Instance.Remove(UIType.UIIntroduction);
            uIIntroduction?.Dispose();
            KnapsackDataItem.Dispose();
            diratr.Clear();
        }
    }
}