
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using TencentCloud.Aa.V20200224.Models;
using TencentCloud.Mrs.V20200910.Models;
using TencentCloud.Scf.V20180416.Models;
using TencentCloud.Tci.V20190318.Models;

namespace ETHotfix
{
    public static partial class TreasureHouseComponentSystem
    {
        public static async Task<bool> LoadPlayerTreasureHouseInfo(this TreasureHouseComponent treasureHouseComponent)
        {
            if (treasureHouseComponent.Parent == null) return false;

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DataCacheManageComponent mDataCacheComponent = treasureHouseComponent.Parent.AddCustomComponent<DataCacheManageComponent>();
            var mDataCache = mDataCacheComponent.Get<THItemInfo>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, treasureHouseComponent.Parent.GameAreaId);
            if (mDataCache == null)
            {
                mDataCache = await mDataCacheComponent.Add<THItemInfo>(dBProxy2, p => p.UserID == treasureHouseComponent.Parent.GameUserId && p.IsDispose != 1);
            }
            //物品信息
            var ListInfo = mDataCache.DataQuery(p => p.UserID == treasureHouseComponent.Parent.GameUserId);
            if (ListInfo.Count >= 1)
            {
                foreach (var item in ListInfo)
                {
                    treasureHouseComponent.Add(item);
                }
                treasureHouseComponent.SortFill();
            }
            //交易信息
            var Info = await dBProxy2.Query<DBTHRecord>(e => e.GameUserId == treasureHouseComponent.Parent.GameUserId);
            if (Info != null)
            {
                foreach (var Item in Info)
                {
                    var R = (Item as DBTHRecord);
                    if (R.IntoOneAccount == 1)
                    {
                        if(R.Type == 0)
                            treasureHouseComponent.Parent.GetCustomComponent<GamePlayer>().UpdateCoin(E_GameProperty.YuanbaoCoin, +R.ActualPrice, "离线时藏宝阁出售物品");
                        else if(R.Type == 1)
                            treasureHouseComponent.Parent.GetCustomComponent<GamePlayer>().UpdateCoin(E_GameProperty.YuanbaoCoin, -R.Price, "藏宝阁购买物品结算异常登录时重新结算");
                        
                        R.IntoOneAccount = 0;
                        await dBProxy2.Save(R);
                        
                        var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(treasureHouseComponent.Parent.GameAreaId);
                        mWriteDataComponent.Save(treasureHouseComponent.Parent.Data, dBProxy2).Coroutine();

                    }
                    treasureHouseComponent.Add(R.ItemName, R.Price, R.ActualPrice, R.Type);
                }
            }
            return true;
        }
        public static async Task<bool> AddItemLoadDB(this TreasureHouseComponent self, long DBId)
        {
            if (self.Parent == null) return false;

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DataCacheManageComponent mDataCacheComponent = self.Parent.AddCustomComponent<DataCacheManageComponent>();
            var mDataCache = mDataCacheComponent.Get<THItemInfo>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, self.Parent.GameAreaId);
            if (mDataCache == null)
            {
                mDataCache = await mDataCacheComponent.Add<THItemInfo>(dBProxy2, p => p.UserID == self.Parent.GameUserId);
            }
            //物品信息
            var ListInfo = mDataCache.DataQuery(p => p.Id == DBId);
            if (ListInfo == null ||ListInfo.Count <= 0)
            {
                var info = await dBProxy2.Query<THItemInfo>(e => e.Id == DBId);
                if (info != null && info.Count == 1)
                {
                    var item = info[0] as THItemInfo;
                    self.Add(item);
                    self.SortFill();
                }
               
            }
            return true;
        }
        /// <summary>
        /// 增加物品信息
        /// </summary>
        /// <param name="self"></param>
        /// <param name="tHItemInfo"></param>
        /// <returns></returns>
        public static bool Add(this TreasureHouseComponent self, THItemInfo tHItemInfo)
        {
            if (self == null) return false;
            if (tHItemInfo == null) return false;
            int Page = self.keyValuePairs.Count == 0? 1 : self.keyValuePairs.Count;

            if (!self.keyValuePairs.ContainsKey(Page))
                self.keyValuePairs.Add(Page,new List<THItemInfo>());

            if (self.keyValuePairs.TryGetValue(Page, out var tHItemInfos))
            {
                if (tHItemInfos.Count >= self.PageCntMax)
                {
                    tHItemInfo.Page = ++Page;
                    List<THItemInfo> tHItemInfos1 = new List<THItemInfo>() { tHItemInfo };
                    self.keyValuePairs.Add(tHItemInfo.Page, tHItemInfos1);
                    return true;
                }
                else
                {
                    self.keyValuePairs[Page].Add(tHItemInfo);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 删除出售物信息
        /// </summary>
        /// <param name="self"></param>
        /// <param name="ItemUId"></param>
        /// <returns></returns>
        public static bool Dle(this TreasureHouseComponent self, long ItemUId)
        {
            if (self == null) return false;
            if (ItemUId <= 0) return false;

            for (int Index = 1, Cnt = self.keyValuePairs.Count; Index <= Cnt; ++Index)
            {
                for (int i = 0,Cnt1 = self.keyValuePairs[Index].Count; i < Cnt1; ++i)
                {
                    if (self.keyValuePairs[Index][i].Uid == ItemUId)
                    {
                        self.keyValuePairs[Index].Remove(self.keyValuePairs[Index][i]);
                        return true;
                    }
                }
            }
            return false;
        }
        public static List<THItemInfo> ConditionsGetItem(this TreasureHouseComponent self, string Name, string Class, int Excellent, int Enhance, int Readdition)
        {
            if (self.keyValuePairs == null) return null;

            List<THItemInfo> tHItemInfos = new List<THItemInfo>();
            bool One = true;
            bool Tow = false;
            bool Three = true;
            bool Four = true;
            bool Five = true;
            foreach (var List in self.keyValuePairs)
            {
                foreach (var Item in List.Value)
                {
                    One = Name == "" || Item.Name.Contains(Name);

                    if (Class != "")
                    {
                        Dictionary<int, int> keys = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, int>>(Class);
                        if (Item.ClassList.TryGetValue(keys.First().Key, out int Value))
                            if (keys.First().Value == Value) Tow = true;
                    }

                    Tow = Tow || Class == "";
                    Three = Excellent == 0 || Item.Excellent == Excellent;
                    Four = Enhance == 0 || Item.Enhance == Enhance;
                    Five = Readdition == 0 || Item.Readdition == Readdition;

                    if (One && Tow && Three && Four && Five)
                    {
                        tHItemInfos.Add(Item);
                    }
                }
            }
            return tHItemInfos;
        }
        public static THItemInfo GetItem(this TreasureHouseComponent self, long ItemUId)
        {
            if (self == null) return null;
            if (ItemUId <= 0) return null;

            for (int Index = 1, Cnt = self.keyValuePairs.Count; Index <= Cnt; ++Index)
            {
                for (int i = 0,Cnt1 = self.keyValuePairs[Index].Count; i < Cnt1; ++i)
                {
                    if (self.keyValuePairs[Index][i].Uid == ItemUId)
                    {
                        return self.keyValuePairs[Index][i]; 
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 页签填充补齐空位
        /// </summary>
        /// <param name="self"></param>
        public static void SortFill(this TreasureHouseComponent self)
        {
            if (self.keyValuePairs == null) return;
            Dictionary<int, List<THItemInfo>> keyValuePairs = new Dictionary<int, List<THItemInfo>>(self.keyValuePairs);
            self.keyValuePairs = new Dictionary<int, List<THItemInfo>>() { { 1, new List<THItemInfo>() } };
            int Page = 1;
            foreach (var list in keyValuePairs)
            {
                foreach (var item in list.Value)
                {
                    item.Page = Page;
                    if (self.keyValuePairs.TryGetValue(Page, out var Value))
                    {
                        if (Value.Count >= self.PageCntMax)
                        {
                            Page++;
                            item.Page = Page;
                            List<THItemInfo> tHItemInfos = new List<THItemInfo>() { item };
                            self.keyValuePairs.Add(Page, tHItemInfos);
                        }
                        else
                        {
                            self.keyValuePairs[Page].Add(item);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 增加交易记录
        /// </summary>
        /// <param name="self"></param>
        /// <param name="Name"></param>
        /// <param name="Price"></param>
        /// <param name="ActualPrice"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static bool Add(this TreasureHouseComponent self, string Name, int Price, int ActualPrice, int Type = 0)
        {
            if (self.tHRecords == null) return false;

            THRecord tHRecord = new THRecord() { ItemName = Name, Price = Price, ActualPrice = ActualPrice, Type = Type };
            self.tHRecords.Add(tHRecord);
            return true;
        }
        public static bool AddItem(this TreasureHouseComponent self, Item item)
        {
            var mPlayer = self.Parent;

            item.data.posX = 0;
            item.data.posY = 0;
            item.data.InComponent = EItemInComponent.TreasureHouse;
            item.data.GameUserId = mPlayer.GameUserId;
            item.data.UserId = mPlayer.UserId;
            item.SaveDBNow().Coroutine();
            return true;
        }
        public static async Task<Item> CreateFormDB(this TreasureHouseComponent self,long ItemUid,long GameUserId,int AreaId)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, AreaId);

            var info = await dBProxy2.Query<DBItemData>(p => p.Id == ItemUid && p.GameUserId == GameUserId);

            DBItemData curDB = null;
            if (info != null && info.Count >= 1) 
                curDB = info[0] as DBItemData;

            if (curDB == null) return null;

            Item item = ItemFactory.TryCreate(curDB.ConfigID, AreaId);
            if (item != null)
            {
                item.NotSevedToDB = false;
                item.Init(curDB);
                item.UpdateProp();
            }
            else
            {
                Log.Error($"藏宝阁查看物品失败 itemUID={ItemUid}  player.GameUserId={GameUserId}");
            }
            return item;
        }
        public static async Task<IResponse> SendGM(this TreasureHouseComponent self, IRequest request )
        {
            IResponse mResult = await self.Parent.GetSessionMGMT().Call(request);
            return mResult;
        }
    }
}