using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using UnityEngine.U2D;
using System;
using System.Linq;
using static NPOI.HSSF.Util.HSSFColor;
using static UnityEditor.Progress;
using static UnityEditor.ShaderData;


namespace ETHotfix
{

    [ObjectSystem]
    public class UIPurchaseLimitComponentAwake : AwakeSystem<UIPurchaseLimitComponent>
    {
        public override void Awake(UIPurchaseLimitComponent self)
        {
            self.reference = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.reference.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                UIComponent.Instance.Remove(UIType.UIPurchaseLimit);
            });

            LimitedPurchase_RewardPropsConfig gold = ConfigComponent.Instance.GetItem<LimitedPurchase_RewardPropsConfig>((1));
            if (gold != null)
            {
                self.ActivityId = gold.ActivityID;
            }
            self.OpenMiracleActivitiesRequest(self.ActivityId).Coroutine();
            self.InitUi();
        }
    }

    [ObjectSystem]
    public class UIPurchaseLimitComponentUpdate : UpdateSystem<UIPurchaseLimitComponent>
    {
        public override void Update(UIPurchaseLimitComponent self)
        {
            if (self.times != 0)
            {
              //  self.reference.GetText("Text_Time").text = TimeHelper.GetCurrentTimestamp(self.times);
            }
        }

    }

    public partial class UIPurchaseLimitComponent : Component
    {
        //1728136800
        public int times = 0;
        public int Index = 0, Onindex = 0, receiveStatus = 0;
        public IConfig[] config;
        public Image DrawPrizes, Good;//获奖面板/物品详情
        public int pass = 0, itemcount = 0;//领奖状态/抽奖卷数量
        /// <summary>
        /// ///////////
        /// </summary>
        public ReferenceCollector reference;
        public bool IsMultiDraw = false;//是否连抽
        // 点了抽奖按钮正在抽奖
        public bool isOnClickPlaying;
        public bool IsOnClickPlaying
        {
            get => isOnClickPlaying;
            set
            {
                isOnClickPlaying = value;
            }
        }

        //------------------------------------
        public GameObject obj;
        public Text item_des, item_name, Text_Time;
        public Button CloseBtn, Btn_Purchase;
        KnapsackDataItem knapsackDataItem;
        UIIntroductionComponent uIIntroduction;
        public int ActivityId = 0;


        /// <summary>
        /// 获取活动相关的数据/拿到服务器给的活动到期时间，这边根据该数据做判断
        /// </summary>
        /// <param name="ActiveId"></param>
        /// <returns></returns>
        public async ETTask OpenMiracleActivitiesRequest(int ActiveId)
        {
            Log.DebugBrown("经过" + ActiveId);
            G2C_OpenMiracleActivitiesResponse g2C_OpenMiracleActivities = (G2C_OpenMiracleActivitiesResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenMiracleActivitiesRequest()
            {
                MiracleActivitiesID = ActiveId
            });
            Log.DebugBrown("获取限时礼包的信息" + g2C_OpenMiracleActivities.Error + "状态" + g2C_OpenMiracleActivities.Info.Value32A + ":" + g2C_OpenMiracleActivities.Info.Value64A);
            if (g2C_OpenMiracleActivities.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenMiracleActivities.Error.GetTipInfo());
            }
            else
            {
                //pass = g2C_OpenMiracleActivities.Info.Value32A;
                times = (int)g2C_OpenMiracleActivities.Info.Value64A;
                receiveStatus = g2C_OpenMiracleActivities.Info.Status;
            }
        }










        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            // SpriteUtility.Instance.ClearAtals(AtalsType.UI_Welfare_Icons); ;
            base.Dispose();
            knapsackDataItem.Dispose();
            UIComponent.Instance.Remove(UIType.UIIntroduction);
            uIIntroduction.Dispose();
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        internal void InitUi()
        {
            Log.DebugBrown("当前职业类型" +
            UnitEntityComponent.Instance.LocalRole.RoleType + "|||" + (int)UnitEntityComponent.Instance.LocalRole.RoleType);
            LimitedPurchase_RewardPropsConfig gold = ConfigComponent.Instance.GetItem<LimitedPurchase_RewardPropsConfig>((int)UnitEntityComponent.Instance.LocalRole.RoleType);
            if (gold.Id == 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "未获取到配置数据id" + (int)UnitEntityComponent.Instance.LocalRole.RoleType);
                return;
            }
            knapsackDataItem = ComponentFactory.Create<KnapsackDataItem>();
            // UGUITriggerProxy triggerProxy = reference.GetImage("mubiao").GetComponent<UGUITriggerProxy>();
            uIIntroduction ??= UIComponent.Instance.VisibleUI(UIType.UIIntroduction).GetComponent<UIIntroductionComponent>();
            //triggerProxy.OnPointerClickEvent = () =>
            //{
            //    //knapsackDataItem.ConfigId = gold.ItemId;
            //    //uIIntroduction.GetAllAtrs(knapsackDataItem);
            //    //uIIntroduction.ShowAtrs();
            //    //var pos = reference.GetImage("mubiao").transform.position;
            //    //uIIntroduction.SetPos(pos += Vector3.left, 1);
            //};
            knapsackDataItem.ConfigId = gold.ItemId;
            int DamageMin = 0, DamageMax = 0, AttackSpeed = 0, Durable = 0, ReqStr = 0, ReqAgi = 0, Defense = 0, DefenseRate = 0, MagicPct = 0, Curse = 0;
            switch ((int)UnitEntityComponent.Instance.LocalRole.RoleType)
            {
                case 4://传说之杖
                case 1://传说之杖
                    DamageMin = 67;
                    DamageMax = 69;
                    AttackSpeed = 25;
                    Durable = 25;
                    MagicPct = 67;
                    break;
                case 2://雷神之剑
                    DamageMin = 110;
                    DamageMax = 118;
                    AttackSpeed = 30;
                    Durable = 75;
                    break;
                case 3://蓝翎弩
                    DamageMin = 119;
                    DamageMax = 133;
                    AttackSpeed = 40;
                    Durable = 75;
                    ReqStr = 122;
                    ReqAgi = 297;
                    break;

                case 5://战斗权杖
                    DamageMin = 85;
                    DamageMax = 96;
                    AttackSpeed = 45;
                    Durable = 65;
                    ReqStr = 185;
                    ReqAgi = 56;
                    break;
                case 6://红翼之杖
                    DamageMin = 67;
                    DamageMax = 69;
                    AttackSpeed = 25;
                    Durable = 65;
                    ReqStr = 98;
                    ReqAgi = 51;
                    MagicPct = 67;
                    break;
                case (int)E_RoleType.Gladiator:
                    break;
                case (int)E_RoleType.GrowLancer:
                    break;
                case (int)E_RoleType.Runemage:
                    break;
                case (int)E_RoleType.StrongWind:
                    break;
                case (int)E_RoleType.Gunners:
                    break;
                case (int)E_RoleType.WhiteMagician:
                    break;
                case (int)E_RoleType.WomanMagician:
                    break;
                default:

                    break;
            }
            Log.DebugBrown("攻击速度" + AttackSpeed);
            knapsackDataItem.ConfigId.GetItemInfo_Out(out Item_infoConfig item_Info);
            knapsackDataItem.SetProperValue(E_ItemValue.RequireLevel, item_Info.ReqLvl);
            knapsackDataItem.SetProperValue(E_ItemValue.DamageMin, DamageMin);
            knapsackDataItem.SetProperValue(E_ItemValue.DamageMax, DamageMax);
            knapsackDataItem.SetProperValue(E_ItemValue.AttackSpeed, AttackSpeed);
            knapsackDataItem.SetProperValue(E_ItemValue.Durability, Durable);
            knapsackDataItem.SetProperValue(E_ItemValue.Defense, Defense);
            knapsackDataItem.SetProperValue(E_ItemValue.DefenseRate, DefenseRate);
            knapsackDataItem.SetProperValue(E_ItemValue.RequireStrength, ReqStr);
            knapsackDataItem.SetProperValue(E_ItemValue.RequireAgile, ReqAgi);
            knapsackDataItem.SetProperValue(E_ItemValue.RequireEnergy, item_Info.ReqEne);
            knapsackDataItem.SetProperValue(E_ItemValue.RequireVitality, item_Info.ReqVit);
            knapsackDataItem.SetProperValue(E_ItemValue.RequireCommand, item_Info.ReqCom);
            knapsackDataItem.SetProperValue(E_ItemValue.MagicDamage, MagicPct);
            knapsackDataItem.SetProperValue(E_ItemValue.Curse, Curse);
            knapsackDataItem.SetProperValue(E_ItemValue.LuckyEquip, 1);
            knapsackDataItem.SetProperValue(E_ItemValue.Level, 7);
            uIIntroduction.GetAllAtrs(knapsackDataItem);
            uIIntroduction.ShowAtrs();
            var pos = reference.GetImage("mubiao").transform.position;
            // uIIntroduction.SetPos(Vector3.left*3,1);
            reference.GetText("item_des").text = "本期活动限时购买，活动期间结束下架";
            reference.GetText("item_name").text = item_Info.Name;
            //  uIIntroduction.SetPos(pos += Vector3.left, 1);
            var itemobj = ResourcesComponent.Instance.LoadGameObject(gold.ResourceName.StringToAB(), gold.ResourceName);
            itemobj.SetLayer(LayerNames.UI);
            itemobj.transform.localScale = Vector3.one * 80;
            itemobj.transform.SetParent(reference.GetGameObject("obj").transform);
            itemobj.transform.localPosition = Vector3.forward * -50;
            for (int i = 0; i < itemobj.transform.childCount; i++)
            {
                if (itemobj.transform.GetChild(i).name=="UI")
                {
                    itemobj.transform.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    itemobj.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            reference.GetButton("Btn_Purchase").onClick.AddSingleListener(() =>
            {
                GetG2C_ShopMallBuyItemResponse().Coroutine();
            });
        }

        async ETVoid GetG2C_ShopMallBuyItemResponse()
        {
            Log.DebugBrown("请求购买的限时礼包" + (int)UnitEntityComponent.Instance.LocalRole.RoleType);
            G2C_ShopMallReceiveResponse g2C_ShopMallBuyItemResponse = (G2C_ShopMallReceiveResponse)await SessionComponent.Instance.Session.Call(new C2G_ShopMallReceiveRequest
            {
                Type = (int)UnitEntityComponent.Instance.LocalRole.RoleType
            });
            Log.DebugBrown("请求购买限时礼包错误码" + g2C_ShopMallBuyItemResponse.Error);
            if (g2C_ShopMallBuyItemResponse.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ShopMallBuyItemResponse.Error.GetTipInfo());
                // Log.DebugRed($"g2C_ShopMallBuyItemResponse.Error:{g2C_ShopMallBuyItemResponse.Error}");
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "购买成功");
                reference.GetButton("Btn_Purchase").transform.GetComponentInChildren<Text>().text = "已购买";
            }
        }
    }
}
