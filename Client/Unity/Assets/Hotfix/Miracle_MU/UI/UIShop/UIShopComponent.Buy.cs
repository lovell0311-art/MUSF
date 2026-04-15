using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System.Linq;

namespace ETHotfix
{
    /// <summary>
    /// 购买面板
    /// </summary>
    public partial class UIShopComponent
    {
        public GameObject BuyPanel;
        public ReferenceCollector buyReferenceCollector;
        public Text Title,Des,buyprice, cacheCount, limitTime;
        public Transform icon;
        public InputField InputField;
        GameObject Limitcount;
        public void Init_BuyPanel() 
        {
            BuyPanel = collector.GetImage("buyPanel").gameObject;
            buyReferenceCollector=BuyPanel.GetComponent<ReferenceCollector>();
            Title=buyReferenceCollector.GetText("title");//标题
            limitTime = buyReferenceCollector.GetText("limittime");//限时
            icon = buyReferenceCollector.GetImage("icon").transform;//模型显示的位置
            Des = buyReferenceCollector.GetText("des");//商品简介
            InputField = buyReferenceCollector.GetInputField("InputField");//购买数量
            buyprice = buyReferenceCollector.GetText("Price");//购买价格
            cacheCount = buyReferenceCollector.GetText("cacheCount");//剩余数量
            Limitcount = buyReferenceCollector.GetGameObject("count");//剩余数量
            InputField.onValueChanged.AddSingleListener(value => 
            {
                if (string.IsNullOrEmpty(value)) return;

                if (int.TryParse(value, out int count))
                {
                    if (curshopItemInfo.BuyMaxLimit==1)
                    {
                        count = 1;
                        InputField.text = count.ToString();
                    }

                    buyCount=count/curshopItemInfo.UnitQuantity;//组数
                    if (buyCount < 1)
                    { 
                    buyCount=1;
                    }
                   
                    var priceType = (curshopItemInfo.ShopMall == (int)E_ShopType.QiJiBi) ? "U币" : "金石";
                   // buyprice.text = $"{buyCount * curshopItemInfo.Price * (curshopItemInfo.Discount / 100)}{priceType}";//组数*价格*折扣
                    buyprice.text = $"{buyCount * curshopItemInfo.Price}{priceType}";//组数*价格*折扣
                }
            });
            buyReferenceCollector.GetButton("addBtn").onClick.AddSingleListener(() =>
            {
                if (curshopItemInfo.ShopMall== (int)E_ShopType.YuanBao)
                {
                    var count = int.Parse(InputField.text);
                    count = Mathf.Clamp(++count, curshopItemInfo.BuyMinLimit, curshopItemInfo.BuyMaxLimit);//组数
                    InputField.text = $"{count * curshopItemInfo.UnitQuantity}";//组数*单组的数量
                }
             
            });
            buyReferenceCollector.GetButton("subBtn").onClick.AddSingleListener(() =>
            {
                if (curshopItemInfo.ShopMall == (int)E_ShopType.YuanBao)
                {
                    var count = int.Parse(InputField.text);
                    count = Mathf.Clamp(--count, curshopItemInfo.BuyMinLimit, curshopItemInfo.BuyMinLimit);
                    InputField.text = $"{count * curshopItemInfo.UnitQuantity}";
                }
               
            });
            buyReferenceCollector.GetButton("BuyBtn").onClick.AddSingleListener(() => 
            {
                if (curshopItemInfo.CacheCount == 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint,"库存不足,无法购买");
                    return;
                }

                if (curshopItemInfo.ShopMall!= (int)E_ShopType.YuanBao)
                {
                    bool satisfy = false;
                    Log.DebugBrown("当前要购买的物品id" + curshopItemInfo.Id);
                    int itemid = -1;
                    itemid = curshopItemInfo.Gemtypes;
                    if (itemid == -1)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "找不到配置的内容");
                        return;
                    }
                    int quantity = 0;//当前数量
                    foreach (var item1 in KnapsackItemsManager.KnapsackItems.Values)
                    {
                        //  Debug.LogError("打印" + item1.ConfigId + ":" + item1.GetProperValue(E_ItemValue.Quantity));
                        if (item1.ConfigId == itemid)
                        {
                            quantity += item1.GetProperValue(E_ItemValue.Quantity);
                            //if (item1.GetProperValue(E_ItemValue.Quantity)>= curshopItemInfo.Price)
                            //{
                            //    satisfy = true;
                            //    break;
                            //}
                        }
                    }
                    Log.DebugBrown("所需物品的最终持有" + quantity);
                    if (quantity >= curshopItemInfo.Price)
                    {
                        satisfy = true;
                    }
                    //判断当前所需的货币时否满足
                    if (satisfy == false)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "当前所需物品不足");
                        return;
                    }
                }
                ShowConfirmationPanel();
                HideBuyPanel();
                
            });
            buyReferenceCollector.GetButton("CancleBtn").onClick.AddSingleListener(() =>
            {
                HideBuyPanel();
            });
            BuyPanel.SetActive(false);
        }
        /// <summary>
        /// 显示购买窗口
        /// </summary>
        public void ShowBuyPanel() 
        {
            limitTime.text = curshopItemInfo.EndTime > 0 ? $"限时：{TimerComponent.Instance.ScendToYMDs(curshopItemInfo.StartTime)} —— {TimerComponent.Instance.ScendToYMDs(curshopItemInfo.EndTime)}" : "";//限时
            Title.text = $"{curshopItemInfo.ItemName}";//更新物品名字
            //显示物品模型资源
            if (icon.childCount != 0)
            {
                for (int j = 0, length = icon.childCount; j < length; j++)
                {
                    ResourcesComponent.Instance.RecycleGameObject(icon.GetChild(j).gameObject);
                }
            }
            var itemobj = ResourcesComponent.Instance.LoadGameObject(curshopItemInfo.ItemIcon.StringToAB(), curshopItemInfo.ItemIcon);
            itemobj.SetLayer(LayerNames.UI);
            itemobj.transform.SetParent(icon);
            itemobj.transform.localPosition = Vector3.forward * -50;
            //显示商品简介
            Des.text = curshopItemInfo.Introduce.Replace("\\n", "\n");//unity 会默认把\n替换为\\n
            //显示默认数量
            this.InputField.text = (curshopItemInfo.BuyMinLimit * curshopItemInfo.UnitQuantity).ToString();
            //显示价格
            string GemName = "异常";
            if (curshopItemInfo.Gemtypes == 280004)
            {
                GemName = "灵魂宝石";
            }
            else if (curshopItemInfo.Gemtypes == 280003)
            {
                GemName = "祝福宝石";
            }
            else if (curshopItemInfo.Gemtypes == 280005)
            {
                GemName = "生命宝石";
            }
            else if (curshopItemInfo.Gemtypes == 280001)
            {
                GemName = "玛雅之石";
            }
            else if (curshopItemInfo.Gemtypes == 280006)
            {
                GemName = "创造宝石";
            }
            if (curshopItemInfo.ShopMall == (int)E_ShopType.ZhanMeng)
            {
                GemName = "战盟积分";
            }
            else if (curshopItemInfo.ShopMall == (int)E_ShopType.YuanBao)
            {
                GemName = "金石";
            }
            //IConfig[] shops = ConfigComponent.Instance.GetAll<ShopMall_PropConfig>();
            //string GemName = "异常";
            //foreach (var items in shops.Cast<ShopMall_PropConfig>())
            //{
            //    if (items.Id == curshopItemInfo.Gemtypes)
            //    {
            //        if (items.Gemtypes == 280004)
            //        {
            //            GemName = "灵魂宝石";
            //        }
            //        else if (items.Gemtypes == 280003)
            //        {
            //            GemName = "祝福宝石";
            //        }
            //        else if (items.Gemtypes == 280005)
            //        {
            //            GemName = "生命宝石";
            //        }
            //        break;
            //    }

            //}
            // var priceType = (curshopItemInfo.ShopMall == (int)E_ShopType.QiJiBi) ? "\t奇迹币" : "\t魔晶";
            //   buyprice.text = $"{curshopItemInfo.Price*curshopItemInfo.BuyMinLimit * (curshopItemInfo.Discount / 100)}{priceType}";
            buyprice.text = $"{curshopItemInfo.Price*curshopItemInfo.BuyMinLimit}{GemName}";
            
            if (curshopItemInfo.CacheCount == -1||curshopItemInfo.CacheCount<0)
            {
                Limitcount.SetActive(false);
            }
            else
            {
                cacheCount.text = $"{curshopItemInfo.CacheCount}";
                Limitcount.SetActive(true);
            }
            BuyPanel.SetActive(true);
        }

        public void HideBuyPanel()
        {
            if (icon.childCount != 0)
            {
                for (int j = 0, length = icon.childCount; j < length; j++)
                {
                    ResourcesComponent.Instance.RecycleGameObject(icon.GetChild(j).gameObject);
                }
            }
            BuyPanel.SetActive(false);
        }
    }
}