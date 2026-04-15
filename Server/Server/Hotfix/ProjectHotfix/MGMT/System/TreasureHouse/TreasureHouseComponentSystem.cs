using CustomFrameWork.Baseic;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CustomFrameWork.Component;
using TencentCloud.Hcm.V20181106.Models;
using System.Linq;
using System.Drawing.Drawing2D;
using TencentCloud.Tics.V20181115.Models;
using Aop.Api.Domain;
using System.Net;
using TencentCloud.Ecm.V20190719.Models;
using TencentCloud.Npp.V20190823.Models;
using TencentCloud.Mrs.V20200910.Models;
using MongoDB.Bson;
using System.Text.RegularExpressions;

namespace ETHotfix
{
    [EventMethod(typeof(MGMTTreasureHouse), EventSystemType.INIT)]
    public class MGMTTreasureHouseOnInit : ITEventMethodOnInit<MGMTTreasureHouse>
    {
        /// <summary>
        /// 服务器开启时读取数据
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(MGMTTreasureHouse b_Component)
        {
            OnInitAsync(b_Component).Coroutine();
        }

        public async Task OnInitAsync(MGMTTreasureHouse b_Component)
        {
            try
            {
                if (await b_Component.InitServerDBID())
                    b_Component.OnInit().Coroutine();
            }
            catch (Exception e)
            {
                Log.Fatal("战盟初始化失败", e);
            }
        }
    }
    public static class MGMTTreasureHouseSystem
    {
        public static async Task<bool> InitServerDBID(this MGMTTreasureHouse self)
        {
            TimerComponent mTimerComponent = Root.MainFactory.GetCustomComponent<TimerComponent>();
            await mTimerComponent.WaitAsync(5000);

            var startConfig = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(OptionComponent.Options.AppId);
            if (startConfig != null)
            {
                List<int> ServerDB = Help_JsonSerializeHelper.DeSerialize<List<int>>(startConfig.RunParameter);
                if (ServerDB.Count > 0)
                {
                    foreach (int DBID in ServerDB)
                    {
                        int Id = DBID >> 16;
                        if (self.DBID.Contains(Id))
                            continue;

                        self.DBID.Add(Id);
                    }
                    return true;
                }
            }
            return false;
        }

        public static async Task OnInit(this MGMTTreasureHouse self)
        {
            TimerComponent mTimerComponent = Root.MainFactory.GetCustomComponent<TimerComponent>();
            await mTimerComponent.WaitAsync(10000);

            foreach (var ID in self.DBID)
            {

                var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, ID);
                if (mDBProxy == null) return;
                using ListComponent<BsonDocument> pipeline = ListComponent<BsonDocument>.Create();
                long startId = 0;
                int PageNum = 2000;
                List<BsonDocument> results;
                do
                {
                    long LsatId = (startId == 0) ? long.MaxValue : startId;
                    BsonDocument match = new BsonDocument()
                       {
                            { "IsDispose" ,new BsonInt32(0)},
                            { "_id" ,new BsonDocument("$lt",new BsonInt64(LsatId))}
                        };
                    // 降序
                    BsonDocument sort = new BsonDocument()
                    {
                        { "_id",-1}
                    };
                    pipeline.Clear();  // 清空上一次的pipeline，避免重复条件
                    pipeline.Add(new BsonDocument("$match", match));
                    pipeline.Add(new BsonDocument("$sort", sort));
                    pipeline.Add(new BsonDocument("$limit", PageNum));

                    results = await mDBProxy.Aggregate<THItemInfo>(pipeline);
                    
                    if (results == null || results.Count == 0) break;
                    for (int i = results.Count - 1; i >= 0; --i)
                    {
                        
                        BsonDocument dBData = results[i];
                        THItemInfo Info = new THItemInfo();
                        Info.Id = dBData["_id"].AsInt64;
                        Info.Uid = dBData["Uid"].AsInt64;//道具ID
                        Info.UserID = dBData["UserID"].AsInt64;//出售者ID
                        Info.Name = dBData["Name"].AsString;//道具名称
                        Info.Class = dBData["Class"].AsString;//职业类型
                        Info.Excellent = dBData["Excellent"].AsInt32;//卓越条数
                        Info.Enhance = dBData["Enhance"].AsInt32;//强化等级
                        Info.Readdition = dBData["Readdition"].AsInt32;//追加等级
                        Info.Price = dBData["Price"].AsInt32;//价格
                        Info.Page = dBData["Page"].AsInt32;//页签
                        Info.MaxType = dBData["MaxType"].AsInt32;
                        Info.MinType = dBData["MinType"].AsInt32;
                        Info.mAreaId = dBData["mAreaId"].AsInt32;
                        Info.ListingTime = dBData["ListingTime"].AsInt64;//上架时间
                        Info.ConfigId = dBData["ConfigId"].AsInt32;
                        Info.IsDispose = dBData["IsDispose"].AsInt32;//是否有效
                        Info.Cnt = dBData["Cnt"].AsInt32;//数量
                        Info.ClassList = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, int>>(Info.Class);
                        await self.AddItem(Info, false);
                        if (i == results.Count - 1)  // 只有在第一次循环（降序的第一个元素）中更新startId
                        {
                            startId = Info.Id;
                        }
                    }
                    //foreach (var Info in NewList)
                    //{
                    //    THItemInfo dBData = Info as THItemInfo;
                    //    dBData.ClassList = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, int>>(dBData.Class);
                    //    await self.AddItem(dBData, false);
                    //    LastId = dBData.Id;
                    //}

                } while (results.Count == PageNum);
            }
            Log.Info($"ServerMGMT  TreasureHouse Loading......OK");
            self.CheckItmeTime().Coroutine();
        }
        public static bool CheckPlayerItemList(this MGMTTreasureHouse self, long GameUserId, long ItemId, out int MaxType, out int MinType)
        {
            if (!self.TemporaryPlayer.ContainsKey(GameUserId))
            {
                MaxType = 0;
                MinType = 0;
                return false;
            }

            foreach (var item in self.TemporaryPlayer[GameUserId])
            {
                foreach (var info in item.Value)
                {
                    if (info.Uid == ItemId)
                    {
                        MaxType = info.MaxType;
                        MinType = info.MinType;
                        return true;
                    }
                }
            }

            MaxType = 0;
            MinType = 0;
            return false;
        }
        public static List<THItemInfo> GetPlayerPage(this MGMTTreasureHouse self, long GameUserId, int Page, out int MasterPage)
        {
            if (self == null)
            {
                MasterPage = 0;
                return null;
            }
            if (Page == 0)
            {
                MasterPage = 0;
                return null;
            }
            if (!self.TemporaryPlayer.ContainsKey(GameUserId))
            {
                MasterPage = 0;
                return null;
            }

            MasterPage = self.TemporaryPlayer[GameUserId].Count;
            if (Page > MasterPage) return null;

            return self.TemporaryPlayer[GameUserId][Page];
        }
        public static bool CreatePlayerItemList(this MGMTTreasureHouse self, long GameUserId, List<THItemInfo> tHItemInfos)
        {
            if (tHItemInfos == null) return false;
            if (self.TemporaryPlayer == null) return false;

            if (!self.TemporaryPlayer.ContainsKey(GameUserId))
                self.TemporaryPlayer[GameUserId] = new Dictionary<int, List<THItemInfo>>();
            else
                self.TemporaryPlayer[GameUserId].Clear();

            int Page = 1;
            foreach (var item in tHItemInfos)
            {
                if (!self.TemporaryPlayer[GameUserId].ContainsKey(Page))
                    self.TemporaryPlayer[GameUserId][Page] = new List<THItemInfo>();

                if (self.TemporaryPlayer[GameUserId][Page].Count >= 5)
                {
                    ++Page;
                    item.Page = Page;
                    self.TemporaryPlayer[GameUserId][Page] = new List<THItemInfo>() { item };
                    continue;
                }
                self.TemporaryPlayer[GameUserId][Page].Add(item);
            }
            return true;
        }
        public static List<THItemInfo> ConditionsGetItem(this MGMTTreasureHouse self, int MaxType, string Name, string Class, int Excellent, int Enhance, int Readdition)
        {
            if (self.keyValuePairs == null) return null;
            if (!self.keyValuePairs.ContainsKey(MaxType)) return null;

            List<THItemInfo> tHItemInfos = new List<THItemInfo>();
            var ItmeList = self.keyValuePairs[MaxType];
            bool One = true;
            bool Tow = false;
            bool Three = true;
            bool Four = true;
            bool Five = true;
            foreach (var List in ItmeList)
            {
                foreach (var Item in List.Value)
                {
                    One = true;
                    Tow = false;
                    Three = true;
                    Four = true;
                    Five = true;

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
        public static bool DelItem(this MGMTTreasureHouse self, THItemInfo tHItemInfo)
        {
            if (self.keyValuePairs[tHItemInfo.MaxType] == null) return false;
            if (self.keyValuePairs[tHItemInfo.MaxType][tHItemInfo.MinType] == null) return false;

            var List = self.keyValuePairs[tHItemInfo.MaxType][tHItemInfo.MinType];
            List.Remove(tHItemInfo);
            tHItemInfo.IsDispose = 1;
            self.SetDB(tHItemInfo).Coroutine();
            return true;
        }
        public static THItemInfo GetItem(this MGMTTreasureHouse self, long Uid, int MaxType, int MinType)
        {
            if (!self.keyValuePairs.ContainsKey(MaxType)) return null;
            if (!self.keyValuePairs[MaxType].ContainsKey(MinType)) return null;

            var List = self.keyValuePairs[MaxType][MinType];
            foreach (var item in List)
            {
                if (item.Uid == Uid) return item;
            }
            return null;
        }
        public static List<THItemInfo> GetItemList(this MGMTTreasureHouse self, int MaxType, int MinType)
        {
            if (!self.keyValuePairs.ContainsKey(MaxType)) return null;
            if (!self.keyValuePairs[MaxType].ContainsKey(MinType)) return null;

            return self.keyValuePairs[MaxType][MinType];
        }
        public static async Task AddItem(this MGMTTreasureHouse self, THItemInfo tHItemInfo, bool IsDB = true)
        {
            if (!self.keyValuePairs.ContainsKey(tHItemInfo.MaxType))
                self.keyValuePairs[tHItemInfo.MaxType] = new Dictionary<int, List<THItemInfo>>();
            if (!self.keyValuePairs[tHItemInfo.MaxType].ContainsKey(tHItemInfo.MinType))
                self.keyValuePairs[tHItemInfo.MaxType][tHItemInfo.MinType] = new List<THItemInfo>();

            self.keyValuePairs[tHItemInfo.MaxType][tHItemInfo.MinType].Add(tHItemInfo);

            if (IsDB)
                await self.SetDB(tHItemInfo);

            Log.Info($"上架藏宝阁player:{tHItemInfo.UserID} Name:{tHItemInfo.Name} Id:{tHItemInfo.Uid} 成功");
        }
        public static async Task CheckItmeTime(this MGMTTreasureHouse self)
        {
            var mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            TimerComponent mTimerComponent = Root.MainFactory.GetCustomComponent<TimerComponent>();
            while (true)
            {
                await mTimerComponent.WaitAsync(1000);
                var Itemlist = new Dictionary<int, Dictionary<int, List<THItemInfo>>>(self.keyValuePairs);
                self.Dlelist.Clear();
                foreach (var List in Itemlist)
                {
                    foreach (var item in List.Value)
                    {
                        foreach (var tHItemInfo in item.Value)
                        {
                            if (tHItemInfo.ListingTime <= Help_TimeHelper.GetNowSecond())
                            {
                                self.Dlelist.Add(tHItemInfo);
                            }
                        }
                    }
                }
                foreach (var tHItemInfo in self.Dlelist)
                {
                    self.keyValuePairs[tHItemInfo.MaxType][tHItemInfo.MinType].Remove(tHItemInfo);
                    tHItemInfo.IsDispose = 1;
                    self.SetDB(tHItemInfo).Coroutine();

                    var dBProxy2 = mDBProxyManager?.GetZoneDB(DBType.Core, tHItemInfo.mAreaId);
                    DBMailData dBMailData = new DBMailData();
                    dBMailData.Id = IdGeneraterNew.Instance.GenerateUnitId(tHItemInfo.mAreaId);
                    dBMailData.MaliID = IdGeneraterNew.Instance.GenerateUnitId(tHItemInfo.mAreaId);
                    dBMailData.MailName = "藏宝阁";
                    dBMailData.MailAcceptanceTime = 0;
                    dBMailData.MailContent = "玩家在售物品到期";
                    dBMailData.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                    dBMailData.MailState = 0;
                    dBMailData.ReceiveOrNot = 0;
                    dBMailData.GameUserID = tHItemInfo.UserID;
                    MailItem mailItem = new MailItem
                    {
                        ItemConfigID = tHItemInfo.ConfigId,
                        AreaId = tHItemInfo.mAreaId,
                        ItemID = tHItemInfo.Uid,
                        CreateAttr = new ItemCreateAttr() { Quantity = tHItemInfo.Cnt }
                    };
                    List<MailItem> mailItems = new List<MailItem>() { mailItem };
                    dBMailData.MailEnclosure = Help_JsonSerializeHelper.Serialize(mailItems, true);
                    await dBProxy2.Save(dBMailData);

                    M2G_ItemeXpire m2G_ItemeXpire = new M2G_ItemeXpire();
                    m2G_ItemeXpire.GameUserID = tHItemInfo.UserID;
                    m2G_ItemeXpire.ItemUid = tHItemInfo.Uid;
                    m2G_ItemeXpire.MAreaId = tHItemInfo.mAreaId;
                    self.SendMessage(m2G_ItemeXpire);
                }
            }
        }

        public static bool SendMessage(this MGMTTreasureHouse b_component, IMessage G_Mmessage)
        {
            List<int> list = Help_JsonSerializeHelper.DeSerialize<List<int>>(OptionComponent.Options.RunParameter);
            int mAreaId = list[0] >> 16;
            var mMatchConfigs = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.Game);
            //int mServerIndex = RandomHelper.RandomNumber(0, mMatchConfigs.Length);
            foreach (var Info in mMatchConfigs)
            {
                Dictionary<int, List<int>> keyValuePairs = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, List<int>>>(Info.RunParameter);
                int AreaId = 1;
                foreach (var KeyValuePair in keyValuePairs)
                {
                    AreaId = KeyValuePair.Key >> 16;
                    break;
                }
                if (AreaId == mAreaId)
                    Game.Scene.GetComponent<NetInnerComponent>().Get(Info.ServerInnerIP).Send(G_Mmessage);
            }
            return true;
        }

        public static async Task SetDB(this MGMTTreasureHouse self, THItemInfo tHItemInfo)
        {
            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, tHItemInfo.mAreaId);
            if (mDBProxy == null) return;

            await mDBProxy.Save(tHItemInfo);
        }
        public static async Task SetItemDB(this MGMTTreasureHouse self, long ItemId, int mAreaId)
        {
            try
            {
                var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, mAreaId);
                if (mDBProxy == null) return;

                var ItemInfo = await mDBProxy.Query<DBItemData>(p => p.Id == ItemId);
                if (ItemInfo != null && ItemInfo.Count >= 1)
                {
                    (ItemInfo[0] as DBItemData).InComponent = EItemInComponent.Mail;
                    await mDBProxy.Save(ItemInfo[0] as DBItemData);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

        }

        public static async Task<bool> SetDBTHRecord(this MGMTTreasureHouse self, int Type, int AreaId, long GameUserId, long UserID, long ItemUid, string Name, int Price)
        {
            //Type = 1 : 购买者的记录Type = 0 : 出售者的记录
            try
            {
                DBTHRecord mPlayerRecord = new DBTHRecord
                {
                    Id = IdGeneraterNew.Instance.GenerateUnitId(AreaId),
                    GameUserId = GameUserId,//Id
                    PatronId = UserID,//对方Id
                    ItemUID = ItemUid,//物品唯一Id
                    ItemName = Name,//道具名称
                    Price = Price,//售价
                    ActualPrice = 0,//扣除手续费实际到账
                    Type = Type,//出售还是购买0=出售1=购买
                    IntoOneAccount = 1,//是否到账0到了1没有
                    Tiem = Help_TimeHelper.GetNowSecond()
                };
                if (Type == 0)
                {
                    int Value1 = (int)Math.Round(Price * 0.05f);
                    if (Value1 <= 0) Value1 = 1;
                    mPlayerRecord.ActualPrice = Price - Value1;
                }

                DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, AreaId);
                await mDBProxy.Save(mPlayerRecord);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
            return true;
        }
    }
}
