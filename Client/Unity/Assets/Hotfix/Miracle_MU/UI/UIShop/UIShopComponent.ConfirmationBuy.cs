using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System.Linq;

namespace ETHotfix
{
    /// <summary>
    /// 确认购买窗口
    /// </summary>
    public partial class UIShopComponent
    {
        public GameObject ConfirmBuyPanel;
        public ReferenceCollector referenceCollector_Confirmation;
        public Text itemName, count, price, itemtime;
        public int buyCount = 1;//购买数量

        public void Init_Confirmation() 
        {
            ConfirmBuyPanel = collector.GetImage("ConfirmBuyPanel").gameObject;
            referenceCollector_Confirmation=ConfirmBuyPanel.GetComponent<ReferenceCollector>();
            itemName = referenceCollector_Confirmation.GetText("name");
            count = referenceCollector_Confirmation.GetText("count");
            price = referenceCollector_Confirmation.GetText("price");
            itemtime = referenceCollector_Confirmation.GetText("time");

            referenceCollector_Confirmation.GetButton("BuyBtn").onClick.AddSingleListener(async () => 
            {
               
                G2C_ShopMallBuyItemResponse g2C_ShopMallBuyItemResponse = (G2C_ShopMallBuyItemResponse)await SessionComponent.Instance.Session.Call(new C2G_ShopMallBuyItemRequest
                {
                    ShopMall = curshopItemInfo.ShopMall,
                    ShopId = curshopItemInfo.ShopId,
                    Id = curshopItemInfo.Id,
                    ItemCnt = buyCount,
                    ShopType = curshopItemInfo.ShopItemType
                });
                if (g2C_ShopMallBuyItemResponse.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ShopMallBuyItemResponse.Error.GetTipInfo());
                  
                }
                else
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, $"{curshopItemInfo.ItemName}-购买成功");
                    if (curshopItemInfo.CacheCount != -1)
                    {
                        curshopItemInfo.CacheCount -= buyCount;
                        if (curshopItemInfo.CacheCount < 0)
                        {
                            curshopItemInfo.CacheCount = 0;
                        }
                    }
                    ConfirmBuyPanel.SetActive(false);
                }
            });
            referenceCollector_Confirmation.GetButton("CancleBtn").onClick.AddSingleListener(() => 
            {
                ConfirmBuyPanel.SetActive(false);
            });
            ConfirmBuyPanel.SetActive(false);
        }
        public void ShowConfirmationPanel() 
        {
            
            itemName.text = $"{curshopItemInfo.ItemName}";
            count.text = $"{buyCount*curshopItemInfo.UnitQuantity}";
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
            // var priceType = (curshopItemInfo.ShopMall == (int)E_ShopType.QiJiBi) ? "\tU币" : "\t魔晶";
            // price.text = $"{buyCount * curshopItemInfo.Price * (curshopItemInfo.Discount / 100)}{priceType}";
            price.text = $"{buyCount * curshopItemInfo.Price}{GemName}";
            itemtime.text = curshopItemInfo.ItemTime == 0 ? "永久" :$"{curshopItemInfo.ItemTime}天";
            ConfirmBuyPanel.SetActive(true);
        }
    }
}
