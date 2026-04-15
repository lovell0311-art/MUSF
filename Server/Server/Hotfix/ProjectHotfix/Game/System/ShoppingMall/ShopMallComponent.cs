using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using TencentCloud.Ame.V20190916.Models;
using TencentCloud.Bri.V20190328.Models;
using TencentCloud.Scf.V20180416.Models;

namespace ETHotfix
{
    [EventMethod(typeof(ShopMallComponent), EventSystemType.INIT)]
    public class ShopMallComponentEventOnInit : ITEventMethodOnInit<ShopMallComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(ShopMallComponent b_Component)
        {
            b_Component.OnInit().Coroutine();
        }
    }

    [EventMethod(typeof(ShopMallComponent), EventSystemType.LOAD)]
    public class ShopMallComponentEventOnLoad : ITEventMethodOnLoad<ShopMallComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public override void OnLoad(ShopMallComponent b_Component)
        {
            b_Component.OnInit().Coroutine();
        }
    }

    public static class ShopMallSystemComponent
    {
        public static async Task OnInit(this ShopMallComponent self)
        {
            if (self.ShopItemDic != null)
                self.ShopItemDic.Clear();
            else
                self.ShopItemDic = new Dictionary<ShopMallType, Dictionary<ItemSortType, Dictionary<int, ShopItem>>>();

            TimerComponent mTimerComponent = Root.MainFactory.GetCustomComponent<TimerComponent>();
            await mTimerComponent.WaitAsync(10000);
            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ShopMall_PropConfigJson>().JsonDic;
            if (mJsonDic != null)
            {
                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var ServerInfo = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, List<int>>>(OptionComponent.Options.RunParameter);
                int AreaId = ServerInfo.FirstOrDefault().Key >> 16;
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, AreaId);
                var ItemInfo = await dBProxy.Query<ShopItemDB>(P => P.IsDisabled != 1);
                Dictionary<int, ShopItemDB> keyValuePairs = new Dictionary<int, ShopItemDB>();
                if (ItemInfo != null && ItemInfo.Count >= 1)
                {
                    foreach (var Item in ItemInfo)
                    {
                        ShopItemDB shopItemDB = Item as ShopItemDB;
                        keyValuePairs.Add(shopItemDB.ShopId, shopItemDB);
                    }
                }

                for (ShopMallType shopType = ShopMallType.None; shopType < ShopMallType.ShopMax; shopType++)
                {
                    if (!self.ShopItemDic.ContainsKey(shopType))
                        self.ShopItemDic.Add(shopType, new Dictionary<ItemSortType, Dictionary<int, ShopItem>>());

                    for (ItemSortType SortType = ItemSortType.Buff; SortType < ItemSortType.Max; SortType++)
                    {
                        if (!self.ShopItemDic[shopType].ContainsKey(SortType))
                            self.ShopItemDic[shopType].Add(SortType, new Dictionary<int, ShopItem>());
                    }

                }

                foreach (var ShopItem in mJsonDic)
                {
                    ShopMallType shopMallType = (ShopMallType)ShopItem.Value.ShopMall;
                    ItemSortType Index = (ItemSortType)ShopItem.Value.ShopType;

                    ShopItem shopItem = new ShopItem();
                    shopItem.ShopId = ShopItem.Value.Id;
                    shopItem.Id = ShopItem.Value.ItemID;
                    shopItem.Price = ShopItem.Value.Price;
                    shopItem.ItemIcon = ShopItem.Value.ItemIcon;
                    shopItem.Introduce = ShopItem.Value.Introduce;
                    shopItem.ItemName = ShopItem.Value.ItemName;
                    shopItem.ItemTime = ShopItem.Value.ItemTime;
                    if (ShopItem.Value.StartTime != "")
                    {
                        shopItem.StartTime = Help_TimeHelper.DateConversionTime(Convert.ToDateTime(ShopItem.Value.StartTime));
                        shopItem.EndTime = shopItem.StartTime + ShopItem.Value.Duration;
                    }
                    shopItem.Discount = ShopItem.Value.Discount;
                    if (ShopItem.Value.DiscountstartTime != "")
                    {
                        shopItem.DiscountStartTime = Help_TimeHelper.DateConversionTime(Convert.ToDateTime(ShopItem.Value.DiscountstartTime));
                        shopItem.DiscountEndTime = shopItem.DiscountStartTime + ShopItem.Value.Discountduration;
                    }
                    shopItem.BuyMaxlimit = ShopItem.Value.BuyMaxlimit;
                    shopItem.BuyMinlimit = ShopItem.Value.BuyMinlimit;
                    shopItem.UnitQuantity = ShopItem.Value.UnitQuantity;
                    shopItem.SetId = ShopItem.Value.SetID;
                    shopItem.Gemtypes = ShopItem.Value.Gemtypes;
                    if (ShopItem.Value.CustomAttrMathod != null)
                    {
                        shopItem.CustomAttrMathod.AddRange(ShopItem.Value.CustomAttrMathod.Split(","));
                    }
                    shopItem.limitCnt = ShopItem.Value.InventoryMax;
                    if (keyValuePairs.TryGetValue(shopItem.ShopId, out var Item))
                    {
                        if (Item.ItemId == shopItem.Id)
                            shopItem.limitCnt = Item.limitCnt;
                        else
                            Log.Warning($"限制购买物品    ShopId重复:{shopItem.ShopId} ItemId:{Item.ItemId}!={shopItem.Id}");
                    }

                    if (self.ShopItemDic.TryGetValue(shopMallType, out var valuePairs))
                    {
                        if (valuePairs.TryGetValue(Index, out var valuePairs1))
                        {
                            if (!valuePairs1.ContainsKey(ShopItem.Value.Id))
                                self.ShopItemDic[shopMallType][Index].Add(shopItem.ShopId, shopItem);
                            else
                                Log.Warning($"商品ID重复：{ShopItem.Value.Id}");
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 获取商城中某分类的所有道具
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="Index"></param>
        /// <returns></returns>
        public static Dictionary<int, ShopItem> GetShopTypeList(this ShopMallComponent b_Component, ShopMallType MallType, ItemSortType sortType = ItemSortType.Buff)
        {
            if (b_Component.ShopItemDic != null)
            {
                if (b_Component.ShopItemDic.TryGetValue(MallType, out var keyValuePairs))
                {
                    if (keyValuePairs.TryGetValue(sortType, out var keyValues))
                        return keyValues;
                }
            }
            return null;
        }
        public static void SetLimitCnt(this ShopMallComponent b_Component, ShopMallType ShopType, ItemSortType SortType, int ShopID, int ItemID, int Cnt, int mAreaId)
        {
            if (b_Component.ShopItemDic != null)
            {
                if (b_Component.ShopItemDic.TryGetValue(ShopType, out var valuePairs))
                {
                    if (valuePairs.TryGetValue(SortType, out var valuePairs1))
                    {
                        if (valuePairs1.ContainsKey(ShopID))
                        {
                            b_Component.ShopItemDic[ShopType][SortType][ShopID].limitCnt -= Cnt;
                            int NewCnt = b_Component.ShopItemDic[ShopType][SortType][ShopID].limitCnt;
                            b_Component.SetDB(ShopID, ItemID, NewCnt, mAreaId).Coroutine();

                            G2G_UpDataShopItemInfo g2G_UpDataShopItemInfo = new G2G_UpDataShopItemInfo();
                            g2G_UpDataShopItemInfo.ShopType = (int)ShopType;
                            g2G_UpDataShopItemInfo.Type = (int)SortType;
                            g2G_UpDataShopItemInfo.ShopID = ShopID;
                            g2G_UpDataShopItemInfo.ItemId = ItemID;
                            g2G_UpDataShopItemInfo.ItemCnt = NewCnt;

                            var ServerInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.Game);
                            foreach (var item in ServerInfo)
                            {
                                var gameinfo = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, List<int>>>(item.RunParameter);
                                int AreaId = gameinfo.FirstOrDefault().Key >> 16;
                                if (AreaId == mAreaId)
                                {
                                    Session gameSession = Game.Scene.GetComponent<NetInnerComponent>().Get(item.ServerInnerIP);
                                    gameSession.Send(g2G_UpDataShopItemInfo);
                                }
                            }
                        }

                    }
                }
            }

        }
        public static async Task SetDB(this ShopMallComponent b_Component, int ShopID, int ItemID, int Cnt, int mAreaId)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, mAreaId);
            var ItemInfo = await dBProxy.Query<ShopItemDB>(P => P.ShopId == ShopID && P.ItemId == ItemID && P.IsDisabled != 1);
            if (ItemInfo != null && ItemInfo.Count >= 1)
            {
                ShopItemDB shopItemDB = ItemInfo[0] as ShopItemDB;

                shopItemDB.limitCnt = Cnt;
                await dBProxy.Save(shopItemDB);
            }
            else
            {
                ShopItemDB shopItemDB = new ShopItemDB();
                shopItemDB.Id = IdGeneraterNew.Instance.GenerateUnitId(mAreaId);
                shopItemDB.ShopId = ShopID;
                shopItemDB.ItemId = ItemID;
                shopItemDB.limitCnt = Cnt;
                shopItemDB.IsDisabled = 0;
                await dBProxy.Save(shopItemDB);
            }
        }
    }
}