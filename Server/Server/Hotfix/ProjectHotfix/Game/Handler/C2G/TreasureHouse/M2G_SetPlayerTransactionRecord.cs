using CustomFrameWork.Component;
using CustomFrameWork;

using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TencentCloud.Bri.V20190328.Models;
using TencentCloud.Hcm.V20181106.Models;
using TencentCloud.Tci.V20190318.Models;
using System.Runtime.InteropServices;
using TencentCloud.Gse.V20191112.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    [FriendOf(typeof(DBAccountZoneData))]
    public class M2G_SetPlayerTransactionRecordHandler : AMHandler<M2G_SetPlayerTransactionRecord>
    {
        protected override async Task<bool> Run(M2G_SetPlayerTransactionRecord b_Request)
        {
            var Item = b_Request.ItemList;

            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.GameUserID))
            {
                var mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(b_Request.MAreaId, b_Request.GameUserID);
                if (mPlayer != null)
                {
                    if (mPlayer.OnlineStatus != EOnlineStatus.Online) return false;
                    mPlayer.GetCustomComponent<GamePlayer>().UpdateCoin(E_GameProperty.YuanbaoCoin, -Item.Price, "藏宝阁购物消耗");
                    mPlayer.GetCustomComponent<PlayerShopMallComponent>().SendPlayerShopState();

                    var TH = mPlayer.GetCustomComponent<TreasureHouseComponent>();
                    if (TH != null)
                    {
                        THRecord tHRecord = new THRecord
                        {
                            ItemName = Item.Name,
                            Price = Item.Price,
                            ActualPrice = 0,
                            Type = 1
                        };
                        TH.tHRecords.Add(tHRecord);
                    }
                    DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                    var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                    var info = await mDBProxy.Query<DBTHRecord>(p => p.GameUserId == mPlayer.GameUserId && p.PatronId == Item.UserID && p.ItemUID == Item.Uid && p.IntoOneAccount == 1);
                    if (info != null && info.Count > 0)
                    {
                        var R = (info[0] as DBTHRecord);
                        R.IntoOneAccount = 0;
                        await mDBProxy.Save(R);
                    }
                    var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
                    mWriteDataComponent.Save(mPlayer.Data, mDBProxy).Coroutine();
                    //return true;
                    //DBTHRecord mPlayerRecord = new DBTHRecord
                    //{
                    //    Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId),
                    //    GameUserId = mPlayer.GameUserId,//Id
                    //    PatronId = Item.UserID,//对方Id
                    //    ItemUID = Item.Uid,//物品唯一Id
                    //    ItemName = Item.Name,//道具名称
                    //    Price = Item.Price,//售价
                    //    ActualPrice = 0,//扣除手续费实际到账
                    //    Type = 1,//出售还是购买0=出售1=购买
                    //    IntoOneAccount = 0,//是否到账0到了1没有
                    //    Tiem = Help_TimeHelper.GetNowSecond()
                    //};
                    //DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                    //var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                    //await mDBProxy.Save(mPlayerRecord);
                }
            }

            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, Item.UserID))
            {
                var mShopPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(Item.AreaId, Item.UserID);
                if (mShopPlayer != null)
                {
                    if (mShopPlayer.OnlineStatus != EOnlineStatus.Online) return false;
                    int Value1 =(int)Math.Round(Item.Price * 0.05f);
                    if (Value1 <= 0) Value1 = 1;
                    int Value = Item.Price - Value1;
                    mShopPlayer.GetCustomComponent<GamePlayer>().UpdateCoin(E_GameProperty.YuanbaoCoin, Value, "藏宝阁出售所得");
                    mShopPlayer.GetCustomComponent<PlayerShopMallComponent>().SendPlayerShopState();

                    var TH = mShopPlayer.GetCustomComponent<TreasureHouseComponent>();
                    if (TH != null)
                    {
                        var ItemDB = TH.GetItem(Item.Uid);
                        if (ItemDB != null)
                        {
                            DataCacheManageComponent mDataCacheComponent = mShopPlayer.AddCustomComponent<DataCacheManageComponent>();
                            var mDataCache = mDataCacheComponent.Get<THItemInfo>();
                            mDataCache?.DataRemove(ItemDB.Id);
                        }
                        TH.Dle(Item.Uid);
                        THRecord tHRecord = new THRecord
                        {
                            ItemName = Item.Name,
                            Price = Item.Price,
                            ActualPrice = Value,
                            Type = 0
                        };
                        TH.tHRecords.Add(tHRecord);
                        TH.SortFill();
                    }
                    DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                    var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, Item.AreaId);
                    var info = await mDBProxy.Query<DBTHRecord>(p => p.GameUserId == Item.UserID && p.PatronId == b_Request.GameUserID && p.ItemUID == Item.Uid && p.IntoOneAccount == 1);
                    if (info != null && info.Count > 0)
                    {
                        var R = (info[0] as DBTHRecord);
                        R.IntoOneAccount = 0;
                        await mDBProxy.Save(R);
                    }
                    var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(Item.AreaId);
                    mWriteDataComponent.Save(mShopPlayer.Data, mDBProxy).Coroutine();
                    //return true;
                }
            }
            //{
            //    DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            //    var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, Item.AreaId);
            //    var info = await mDBProxy.Query<DBTHRecord>(p => p.GameUserId == Item.UserID && p.PatronId == b_Request.GameUserID && p.ItemUID == Item.Uid && p.IntoOneAccount == 1);
            //    if (info.Count > 0)
            //    {
            //        return false;
            //    }
            //    int Value = (int)(Item.Price - Item.Price * 0.03);
            //    DBTHRecord mShopPlayerRecord = new DBTHRecord
            //    {
            //        Id = IdGeneraterNew.Instance.GenerateUnitId(Item.AreaId),
            //        GameUserId = Item.UserID,
            //        PatronId = b_Request.GameUserID,
            //        ItemUID = Item.Uid,//物品唯一Id
            //        ItemName = Item.Name,//道具名称
            //        Price = Item.Price,//售价
            //        ActualPrice = Value,//扣除手续费实际到账
            //        Type = 0,//出售还是购买0=出售1=购买
            //        IntoOneAccount = 1,//是否到账0到了1没有
            //        Tiem = Help_TimeHelper.GetNowSecond()
            //    };
            //    await mDBProxy.Save(mShopPlayerRecord);
            //}

            return true;
        }
    }
}