using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System.Linq;
using UnityEngine.UI;
using System;


namespace ETHotfix
{
    /// <summary>
    /// 商城类型
    /// </summary>
    public enum E_ShopType
    {
        //Normal = 0,//一般商城
        QiJiBi = 0,
        YuanBao = 1,
        ChenHao = 2,
        BaoShi = 3,
        ZhanMeng = 4

    }
    /// <summary>
    /// 商品类型
    /// </summary>
    public enum E_ShopItemType
    {
        Buffs = 1,//增强Buffer
        Expends = 2,//消耗类
        Pet_Mounts = 3,//宠物坐骑类
        Specials = 4,//特殊类
        Equip = 5, //装备类
        Suit = 6, // 套装类
        Fashion = 7, //时装类
        TreasureBox = 8,//宝箱类
        Max

    }

    public enum E_TopUpType
    {
        ShopTopUp = 0,//商城充值
        FristTopUp = 1,//首充
        ActivityTopUp = 2,//限时充值
        SevenDays = 3,//七天限时充值
        FiveActivity = 4,//五一限时充值
    }

    [ObjectSystem]
    public class UIShopComponentAwake : AwakeSystem<UIShopComponent>
    {
        public override void Awake(UIShopComponent self)
        {
            self.collector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.collector.gameObject.GetComponent<Canvas>().planeDistance = 70;
            self.collector.GetButton("CloseBtn").onClick.AddSingleListener(() => UIComponent.Instance.Remove(UIType.UIShop));
            self.ShopItemParent = self.collector.GetGameObject("ShopView").transform;
            self.pageText = self.collector.GetText("PageText");
            self.QiJiCoin = self.collector.GetText("jinbi");
            self.YuanBao = self.collector.GetText("yuanbao");
            //反页
            self.collector.GetButton("left").onClick.AddSingleListener(() =>
            {
                if (self.CurPaeg == 1) return;
                self.CurPaeg = Mathf.Clamp(--self.CurPaeg, 1, self.MaxPage);
                self.RefreshShopInfo(self.CurPaeg, self.CurShopType);
            });
            self.collector.GetButton("right").onClick.AddSingleListener(() =>
            {
                if (self.CurPaeg == self.MaxPage) return;
                self.CurPaeg = Mathf.Clamp(++self.CurPaeg, 1, self.MaxPage);
                self.RefreshShopInfo(self.CurPaeg, self.CurShopType);
            });
            //点卡改
            self.GetShopItems(3, (int)E_ShopItemType.Buffs).Coroutine();
            self.Init_ShopType();
            self.Init_ShopItemType();
            self.Init_BuyPanel();
            self.Init_Confirmation();
            self.InitRoleInfo();


            self.InitTopUp();
            //限时倒计时
            self.CloseActivity();


        }
    }
    /// <summary>
    /// 商城
    /// </summary>
    public partial class UIShopComponent : Component
    {
        public ReferenceCollector collector;
        /// <summary>
        /// 商城 物品字典
        /// key: 商城类型_物品类型
        /// value: 对应的物品集合
        /// </summary>
        public Dictionary<string, List<ShopItemInfo>> shopItemsDic = new Dictionary<string, List<ShopItemInfo>>();
        public E_ShopType shopType = E_ShopType.BaoShi;//当前商城类型
        public E_ShopItemType E_ShopItem = E_ShopItemType.Buffs;//当前商品类型
        public Transform ShopItemParent;
        public Text pageText;//页数
        public string CurShopType;//当前商品类型
        public int CurPaeg;//当前页数
        public int MaxPage;//最大页数
        public ShopItemInfo curshopItemInfo;//当前要购买物品的信息
        public Text QiJiCoin;//奇迹币数量
        public Text YuanBao;//元宝数量

        /// <summary>
        /// 充值类型
        /// 0 商城充值
        /// 1 首充
        /// 2 累计活动
        /// </summary>
        public int topuptype = 0;

        /// <summary>
        /// 初始化充值
        /// </summary>
        public void InitTopUp() 
        {
            Init_TopUp();
            return;
            switch (ETModel.Init.instance.e_SDK)
            {
                case E_SDK.NONE:
                    Init_TopUp();
                    break;
                case E_SDK.HAO_YI_SDK:
                    Init_TopUp_HaoYi();
                    break;
                case E_SDK.TIKTOK_SDK:
                    Init_TopUp_TikTokSdk();
                    break;
                case E_SDK.XY_SDK:
                    Init_TopUp_XySdk();
                    break;
                case E_SDK.SHOU_Q:
                    Init_TopUp_ShouQSdk();
                    break; 
                case E_SDK.ZHIFUBAO_WECHAT:
                    Init_TopUp_ZhiFuBaoSdk();
                    break;
                default:
                    break;
            }

            SdkCallBackComponent.Instance.PayFailureCallBack = (value) =>
            {
                Log.DebugRed($"支付失败：{value}");
            };
        }

        TimeSpan closeSpane;
        GameObject Tips;
        public void CloseActivity()
        {
            Tips = collector.GetText("Tips").gameObject;
            //  DateTime starttime =System.DateTime.Now;
            // DateTime endtime = Convert.ToDateTime(info.EndTime); 
            DateTime starttime = Convert.ToDateTime("2023-10-31 23:00:00");
            DateTime endtime = Convert.ToDateTime("2023-05-05 23:00:00");
            closeSpane = endtime.Subtract(DateTime.Now);
            if ((long)closeSpane.TotalMilliseconds > 0)
            {
                Tips.SetActive(true);
                TimerComponent.Instance.RegisterTimeCallBack((long)closeSpane.TotalMilliseconds, () =>
                {
                    Tips.SetActive(false);
                });

            }
            else
            {
                Tips.SetActive(false);
            }
            
        }
        //初始化玩家的信息
        public void InitRoleInfo()
        {
            collector.GetText("rolename").text = $"{UnitEntityComponent.Instance.LocalRole.RoleName}";
            QiJiCoin.text = $"{UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.AllianceIntegral)}";
            YuanBao.text = $"{UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.MoJing)}";
            Log.DebugBrown("积分" + UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.AllianceIntegral));
        }
        public void UpdateCoin()
        {
            QiJiCoin.text = $"{UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.AllianceIntegral)}";
            YuanBao.text = $"{UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.MoJing)}";
        }

        //初始化商城类型
        public void Init_ShopType()
        {
            Transform ShopType = collector.GetGameObject("ShopType").transform;
            for (int i = 0, length = ShopType.childCount; i < length; i++)
            {
                Toggle toggle = ShopType.GetChild(i).GetComponent<Toggle>();
                int index = 2+i;
                toggle.onValueChanged.AddSingleListener(value =>
                {
                    if (value == false) return;
                    if (index==2)
                    {
                        index = 1;
                    }
                     shopType = (E_ShopType)index;
                    // GetShopItems(3, (int)E_ShopItem).Coroutine();
                    Log.DebugBrown("商城类型" + (int)shopType+"::"+index);
                     GetShopItems((int)shopType, (int)E_ShopItem).Coroutine();
                });
            }
        }
        //初始化 商品类型
        public void Init_ShopItemType()
        {
            //Transform ShopItemType = collector.GetGameObject("ShopItemType").transform;
            //for (int i = 0, length = ShopItemType.childCount; i < length; i++)
            //{
            //    Toggle toggle = ShopItemType.GetChild(i).GetComponent<Toggle>();
            //    int index = i + 1;
            //    toggle.onValueChanged.AddSingleListener(value =>
            //    {
            //        if (value == false) return;
            //        E_ShopItem = (E_ShopItemType)index;
            //        GetShopItems(3, index).Coroutine();
            //    });
            //}
            GameObject fab = collector.GetGameObject("ShopItemType").transform.GetChild(0).gameObject;
            fab.SetActive(false);
            var list = new string[(int)E_ShopItemType.Max];
            list[0] = "";
            list[(int)E_ShopItemType.Buffs] = "增强类BUFF";
            list[(int)E_ShopItemType.Expends] = "消耗类";
            list[(int)E_ShopItemType.Pet_Mounts] = "宠物坐骑类";
            list[(int)E_ShopItemType.Specials] = "特殊物品";
            list[(int)E_ShopItemType.Equip] = "技能装备类";
            list[(int)E_ShopItemType.Suit] = "套装类";
            list[(int)E_ShopItemType.Fashion] = "时装";
            list[(int)E_ShopItemType.TreasureBox] = "宝箱";

            List<IConfig> shops = ConfigComponent.Instance.GetAll<ShopMall_PropConfig>().ToList();//获取怪物 技能配置表

            Toggle toggle1 = null;
            for (int i = (int)E_ShopItemType.Buffs; i < (int)E_ShopItemType.Max; i++)
            {
              //  Log.DebugBrown("商品的类型" + shops.FindIndex(p => ((ShopMall_PropConfig)p).ShopType == i));
                if (shops.FindIndex(p => ((ShopMall_PropConfig)p).ShopType == i) == -1)
                {
                    continue;
                }
                GameObject item = GameObject.Instantiate(fab, collector.GetGameObject("ShopItemType").transform);
                item.SetActive(true);
                item.transform.Find("Label").GetComponent<Text>().text = list[i];
                Toggle toggle = item.GetComponent<Toggle>();

                if (toggle1 == null)
                {
                    toggle1 = toggle;
                }

                int type = i;
                toggle.onValueChanged.AddSingleListener(value =>
                {
                    if (value == false) return;
                    E_ShopItem = (E_ShopItemType)type;
                    //    Log.DebugBrown("初始化"+ (int)shopType+"::type"+type);
                    // GetShopItems(3, type).Coroutine();
                    GetShopItems((int)shopType, type).Coroutine();
                });
            }
            if (toggle1)
            {
                toggle1.isOn = true;
            }
        }
        //刷新商城物品
        public void RefreshShopInfo(int page, string shopkey)
        {
          
            //if (CurShopType== shopkey) return;

            var shopLists = shopItemsDic[shopkey];
            CurShopType = shopkey;
            CurPaeg = page;
            for (int i = 0; i < 6; i++)
            {
                var shopitemIndex = i + (page - 1) * 6;
                var item = ShopItemParent.GetChild(i).gameObject;
                if (shopitemIndex < shopLists.Count)
                {
                    var info = shopLists[shopitemIndex];
                    item.transform.Find("name").GetComponent<Text>().text = info.ItemName+" "+(info.EndTime > 0 ? "<color=red>(限时)</color>":"");
                   // item.transform.Find("limit").GetComponent<Text>().text = info.EndTime>0?"限时":"";
                    item.transform.Find("kucun").GetComponent<Text>().text = info.CacheCount == -1 ? "": $"库存:{info.CacheCount}";
                    string GemName = "异常";
                    if (info.Gemtypes == 280004)
                    {
                        GemName = "灵魂宝石";
                    }
                    else if (info.Gemtypes == 280003)
                    {
                        GemName = "祝福宝石";
                    }
                    else if (info.Gemtypes == 280005)
                    {
                        GemName = "生命宝石";
                    }
                    else if (info.Gemtypes == 280001)
                    {
                        GemName = "玛雅之石";
                    }
                    else if (info.Gemtypes == 280006)
                    {
                        GemName = "创造宝石";
                    }
                    //IConfig[] shops = ConfigComponent.Instance.GetAll<ShopMall_PropConfig>();
                    //foreach (var items in shops.Cast<ShopMall_PropConfig>())
                    //{
                    //    if (items.Id == info.Gemtypes)
                    //    {
                    //        if (items.Gemtypes== 280004)
                    //        {
                    //            GemName = "灵魂宝石";
                    //        }
                    //        else if (items.Gemtypes== 280003)
                    //        {
                    //            GemName = "祝福宝石";
                    //        }
                    //        else if (items.Gemtypes== 280005)
                    //        {
                    //            GemName = "生命宝石";
                    //        }
                    //        break;
                    //    }

                    //}
                    //  var priceType = (info.ShopMall == (int)E_ShopType.QiJiBi) ? "\tU币" : "\t魔晶";
                    // var price = info.Price * ((float)info.Discount / 100) == 0 ? 1 : info.Price * ((float)info.Discount / 100);
                    if (info.ShopMall==(int)E_ShopType.ZhanMeng)
                    {
                        GemName = "战盟积分";
                    }
                    else if(info.ShopMall == (int)E_ShopType.YuanBao)
                    {
                        GemName = "金石";
                    }
                    item.transform.Find("price").GetComponent<Text>().text = $"{info.Price}{GemName}";
                    var Icon = item.transform.Find("icon");
                    if (Icon.childCount != 0)
                    {
                        for (int j = 0, length = Icon.childCount; j < length; j++)
                        {
                           // ResourcesComponent.Instance.RecycleGameObject(Icon.GetChild(j).gameObject);
                            ResourcesComponent.Instance.DestoryGameObjectImmediate(Icon.GetChild(j).gameObject,Icon.GetChild(j).gameObject.name.StringToAB());
                        }
                    }
                    var itemobj = ResourcesComponent.Instance.LoadGameObject(info.ItemIcon.StringToAB(), info.ItemIcon);
                    itemobj.SetLayer(LayerNames.UI);
                    itemobj.transform.SetParent(Icon);
                    itemobj.transform.localPosition = Vector3.forward * -50;
                    item.transform.Find("buybtn").GetComponent<Button>().onClick.AddSingleListener(() =>
                    {
                        curshopItemInfo = info;
                        ShowBuyPanel();
                    });
                    item.SetActive(true);
                }
                else
                {
                    item.SetActive(false);
                }
            }
            MaxPage = shopLists.Count / 6 + (shopLists.Count % 6 != 0 ? 1 : 0);
            pageText.text = $"{page}/{MaxPage}";
        }

        /// <summary>
        /// 获取商城物品
        /// </summary>
        /// <param name="shopmall">商城类型</param>
        /// <param name="shoptype">物品类型</param>
        /// <returns></returns>
        public async ETVoid GetShopItems(int shopmall, int shoptype)
        {
            var shopkey = $"{shopmall}_{shoptype}";
           // Log.DebugBrown("获取的" + shopmall + ":::" + shoptype);
            if (shopItemsDic.ContainsKey(shopkey) == false)
            {
                //获取 对应的物品
                G2C_OpenShopMallResponse g2C_OpenShopMall = (G2C_OpenShopMallResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenShopMallRequest
                {
                    ShopMall = shopmall,
                    ShopType = shoptype,
                });
                if (g2C_OpenShopMall.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenShopMall.Error.GetTipInfo());
                }
                else
                {
                  
                    var shopItemInfos = g2C_OpenShopMall.ItemList.ToList();
                    List<ShopItemInfo> shopItems = new List<ShopItemInfo>();
                 //   Log.DebugBrown("当前分类物品的数量" + shopmall + "::count" + shopItemInfos.Count);
                    for (int i = 0, length = shopItemInfos.Count; i < length; i++)
                    {
                        var item = shopItemInfos[i];
                     //   Debug.Log("获取的id" + item.ShopId+"::类型"+item.Gemtypes+":名字"+item.ItemName);
                        shopItems.Add(new ShopItemInfo
                        {
                            
                            ShopId = item.ShopId,
                            Id = item.Id,
                            Price = item.Price,
                            ItemIcon = item.ItemIcon,
                            Introduce = item.Introduce,
                            StartTime = item.StartTime,
                            EndTime = item.EndTime,
                            Discount = item.Discount,
                            BuyMinLimit = item.BuyMinlimit,
                            BuyMaxLimit = item.BuyMaxlimit,
                            UnitQuantity = item.UnitQuantity,
                            ItemName = item.ItemName,
                            ShopMall = shopmall,
                            ShopItemType = shoptype,
                            ItemTime = item.ItemTime,
                            Gemtypes=item.Gemtypes,
                            CacheCount =item.LimitCnt
                        });
                    }
                    shopItemsDic[shopkey] = shopItems;
                }
            }
            RefreshShopInfo(1, shopkey);
        }
        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
            shopItemsDic.Clear();

        }
    }
}
