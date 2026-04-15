using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_OpenShopMallRequestHandler : AMActorRpcHandler<C2G_OpenShopMallRequest, G2C_OpenShopMallResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_OpenShopMallRequest b_Request, G2C_OpenShopMallResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_OpenShopMallRequest b_Request, G2C_OpenShopMallResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(102);
                b_Reply(b_Response);
                return false;
            }
            var Title = mPlayer.GetCustomComponent<PlayerTitle>();
            if (Title == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2200);
                b_Reply(b_Response);
                return false;
            }
            int TitleDiscount = 0;
            
            long Tiem = Help_TimeHelper.GetNowSecond();
            var ShopInfo = Root.MainFactory.GetCustomComponent<ShopMallComponent>();
            if (ShopInfo != null)
            {
                ShopMallType MallType = (ShopMallType)b_Request.ShopMall;
                ItemSortType SortType = (ItemSortType)b_Request.ShopType;
                Dictionary<int, ShopItem> ItemList = ShopInfo.GetShopTypeList(MallType, SortType);
                
                foreach (var Item in ItemList)
                {
                    ShopProp shopItem = new ShopProp();
                    shopItem.ShopId = Item.Value.ShopId;// 商城ID
                    shopItem.Id = Item.Value.Id;// 道具ID

                    if (Item.Value.DiscountEndTime != 0 && Item.Value.DiscountStartTime < Tiem && Tiem < Item.Value.DiscountEndTime)
                        shopItem.Price = (int)(Item.Value.Price * Item.Value.Discount * 0.01f);
                    else
                        shopItem.Price = Item.Value.Price;// 价格

                    if (TitleDiscount > 0)
                    {
                        int NewPrice = Item.Value.Price;
                        NewPrice = (int)(NewPrice * TitleDiscount * 0.01f);
                        if (NewPrice < shopItem.Price)
                            shopItem.Price = NewPrice;
                    }

                    shopItem.ItemIcon = Item.Value.ItemIcon;// 物品标识
                    shopItem.Introduce = Item.Value.Introduce;// 物品介绍
                    shopItem.StartTime = Item.Value.StartTime;// 限时起点秒
                    shopItem.EndTime = Item.Value.EndTime;// 限时终点秒
                    shopItem.Discount = Item.Value.Discount;// 折扣
                    shopItem.BuyMaxlimit = Item.Value.BuyMaxlimit;//购买限制
                    shopItem.BuyMinlimit = Item.Value.BuyMinlimit;
                    shopItem.UnitQuantity = Item.Value.UnitQuantity;
                    shopItem.ItemName = Item.Value.ItemName;
                    shopItem.ItemTime = Item.Value.ItemTime;
                    shopItem.LimitCnt = Item.Value.limitCnt;
                    shopItem.Gemtypes = Item.Value.Gemtypes;
                    b_Response.ItemList.Add(shopItem);
                }

                b_Reply(b_Response);
                return true;
            }

            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2201);
            b_Reply(b_Response);
            return false;
        }
    }
}