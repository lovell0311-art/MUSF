using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TencentCloud.Mrs.V20200910.Models;
using System.Linq;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_ShopMallBuyItemRequestHandler : AMActorRpcHandler<C2G_ShopMallBuyItemRequest, G2C_ShopMallBuyItemResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_ShopMallBuyItemRequest b_Request, G2C_ShopMallBuyItemResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_ShopMallBuyItemRequest b_Request, G2C_ShopMallBuyItemResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(102);
                b_Reply(b_Response);
                return false;
            }
            var PlayerShop = mPlayer.GetCustomComponent<PlayerShopMallComponent>();
            if (PlayerShop == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2200);
                b_Reply(b_Response);
                return false;
            }
            var gameplayer = mPlayer.GetCustomComponent<GamePlayer>();
            if (gameplayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2200);
                b_Reply(b_Response);
                return false;
            }
            BackpackComponent mBackpackComponent = mPlayer.GetCustomComponent<BackpackComponent>();
            if (mBackpackComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2200);
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
            var WarAlliance = mPlayer.GetCustomComponent<PlayerWarAllianceComponent>();
            if (WarAlliance == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2200);
                b_Reply(b_Response);
                return false;
            }

            var ShopInfo = Root.MainFactory.GetCustomComponent<ShopMallComponent>();
            if (ShopInfo != null)
            {
                long Tiem = Help_TimeHelper.GetNowSecond();
                ShopMallType mallType = (ShopMallType)b_Request.ShopMall;
                ItemSortType sortType = (ItemSortType)b_Request.ShopType;
                Dictionary<int, ShopItem> ItemList = ShopInfo.GetShopTypeList(mallType, sortType);
                if (mallType == ShopMallType.TITLE)
                {
                    if (Title.CheckTitle(60010) && Title.CheckTitle(60011) && Title.CheckTitle(60012))
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2202);
                        b_Reply(b_Response);
                        return false;
                    }
                }
                if (ItemList.TryGetValue(b_Request.ShopId, out ShopItem ItemInfo))
                {
                    if (b_Request.ItemCnt > ItemInfo.BuyMaxlimit || b_Request.ItemCnt < ItemInfo.BuyMinlimit)
                    {
                        // 数据异常
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2200);
                        b_Reply(b_Response);
                        return false;
                    }
                    if (ItemInfo.limitCnt != -1 && b_Request.ItemCnt > ItemInfo.limitCnt)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2200);
                        b_Reply(b_Response);
                        return false;
                    }
                   
                    if (ItemInfo.Id == b_Request.Id)
                    {
                        if (ItemInfo.EndTime != 0 && (ItemInfo.StartTime > Tiem || Tiem > ItemInfo.EndTime))
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2212);
                            b_Reply(b_Response);
                            return false;
                        }
                        int Price = ItemInfo.Price * b_Request.ItemCnt;
                        if (ItemInfo.DiscountEndTime != 0 && ItemInfo.DiscountStartTime < Tiem && Tiem < ItemInfo.DiscountEndTime)
                            Price = (int)(Price * ItemInfo.Discount * 0.01f);

                        var DelItemList = mBackpackComponent.GetAllItemByConfigID(ItemInfo.Gemtypes);
                        switch (mallType)
                        {
                            case ShopMallType.None:
                                if (Price > gameplayer.Data.MiracleCoin)
                                {
                                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2204);
                                    b_Reply(b_Response);
                                    return false;
                                }
                                break;
                            case ShopMallType.CRYSTAL:
                            case ShopMallType.TITLE:
                                if (Price > gameplayer.Player.Data.YuanbaoCoin)
                                {
                                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2205);
                                    b_Reply(b_Response);
                                    return false;
                                }
                                break;
                            case ShopMallType.GEM:
                                {
                                    if(DelItemList==null) 
                                    {
                                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1609);
                                        b_Reply(b_Response);
                                        return false;
                                    }
                                    if (DelItemList.Sum(P => P.Value.GetDBProp(EDBItemValue.Quantity)) < Price)
                                    {
                                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1609);
                                        b_Reply(b_Response);
                                        return false;
                                    }
                                }
                                break;
                            case ShopMallType.ALLIANCESCORE:
                                if (Price > WarAlliance.AllianceScore)
                                {
                                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2205);
                                    b_Reply(b_Response);
                                    return false;
                                }
                                break;
                        }

                        ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                        itemCreateAttr.Quantity = b_Request.ItemCnt * ItemInfo.UnitQuantity;
                        itemCreateAttr.ValidTime = ItemInfo.ItemTime * 24 * 60 * 60;
                        itemCreateAttr.SetId = ItemInfo.SetId;
                        itemCreateAttr.CustomAttrMethod.AddRange(ItemInfo.CustomAttrMathod);
                        Item mDropItem = ItemFactory.Create(ItemInfo.Id, mPlayer.GameAreaId, itemCreateAttr);

                        string itmeLog = $"购买商品 ({ItemInfo.ItemName}:{ItemInfo.ShopId}) ({mDropItem.ToLogString()})";
                       
                        if (mBackpackComponent.CanAddItem(mDropItem) == false)
                        {
                            Log.PLog("BuyItem", $"角色:{mPlayer.GameUserId}道具ID:{ItemInfo.Id}数量:{itemCreateAttr.Quantity}价格:{Price}够买失败");
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2205);
                            b_Reply(b_Response);
                            return false;
                        }
                        else
                        {
                            Log.PLog("BuyItem", $"角色:{mPlayer.GameUserId}道具ID:{ItemInfo.Id}数量:{itemCreateAttr.Quantity}价格:{Price}够买成功");
                        }

                        if (ItemInfo.limitCnt != -1)
                        {
                            ShopInfo.SetLimitCnt(mallType, sortType, ItemInfo.ShopId, ItemInfo.Id, b_Request.ItemCnt, mAreaId);
                        }
                            
                        switch (mallType)
                        {
                            //U币商城
                            case ShopMallType.None:
                                gameplayer.SendItem(8, mDropItem).Coroutine();
                                Log.PLog("PlayerShop", $"名称:{gameplayer.Data.NickName} UserID:{mPlayer.UserId} 总奇迹币:{gameplayer.Data.MiracleCoin} 消耗奇迹币:{Price} ShopItem:{ItemInfo.ShopId}");
                                PlayerShop.SetMiracleCoin(-Price, $"购买商品 ({ItemInfo.ItemName}:{ItemInfo.ShopId}) ({mDropItem.ToLogString()})");
                                break;
                            case ShopMallType.CRYSTAL:
                            case ShopMallType.TITLE:
                                Log.PLog("PlayerShop", $"名称:{gameplayer.Data.NickName} UserID:{mPlayer.UserId} 总奇元宝:{gameplayer.Player.Data.YuanbaoCoin} 消耗元宝:{Price} ShopItem:{ItemInfo.ShopId}");
                                PlayerShop.SetPlayerYuanBao(-Price, itmeLog);
                                break;
                            case ShopMallType.GEM:
                                while (Price != 0) 
                                {
                                    var ItemNoe = DelItemList.FirstOrDefault();
                                    Log.PLog("PlayerShop", $"名称:{gameplayer.Data.NickName} UserID:{mPlayer.UserId} 消耗宝石:{ItemInfo.Gemtypes}数量:{Price} ShopItem:{ItemInfo.ShopId}");
                                    int ItemCnt = ItemNoe.Value.GetDBProp(EDBItemValue.Quantity);
                                    if (ItemCnt <= Price)
                                    {
                                        mBackpackComponent.UseItem(ItemNoe.Value, itmeLog, ItemCnt);
                                        Price -= ItemCnt;
                                    }
                                    else if (ItemCnt > Price)
                                    {
                                        mBackpackComponent.UseItem(ItemNoe.Value, itmeLog, Price);
                                        Price = 0;
                                    }
                                }
                                break;
                            case ShopMallType.ALLIANCESCORE:
                                WarAlliance.AllianceScore -= Price;
                                Log.PLog("PlayerShop", $"名称:{gameplayer.Data.NickName} UserID:{mPlayer.UserId} 战盟积分兑换:{Price} ShopItem:{ItemInfo.ShopId}");
                                WarAlliance.UpDateWarAlliancePlayerInfo();
                                G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                                G2C_BattleKVData mGoldCoinData = new G2C_BattleKVData();
                                mGoldCoinData.Key = (int)E_GameProperty.AllianceScoreChange;
                                mGoldCoinData.Value = WarAlliance.AllianceScore;
                                mChangeValueMessage.Info.Add(mGoldCoinData);
                                mPlayer.Send(mChangeValueMessage);
                                break;
                        }
                        mBackpackComponent.AddItem(mDropItem, "商城购买");
                    }
                }
            }
            b_Reply(b_Response);
            return false;
        }
    }
}