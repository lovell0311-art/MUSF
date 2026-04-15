using CustomFrameWork.Baseic;
using ETModel;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    [BsonIgnoreExtraElements]
    public class DBTHRecord : DBBase
    {
        [DBMongodb(1)]
        [DBMongodb(11)]
        public long GameUserId = 0;//Id
        [DBMongodb(11)]
        public long PatronId = 0;//对方Id
        [DBMongodb(11)]
        public long ItemUID = 0;//物品唯一Id
        public string ItemName = "";//道具名称
        public int Price = 0;//售价
        public int ActualPrice = 0;//扣除手续费实际到账
        public int Type = 0;//出售还是购买0=出售1=购买
        public long Tiem = 0;
        [DBMongodb(11)]
        public int IntoOneAccount = 0;//是否到账0到了1没有
    }
    /// <summary>
    /// 交易记录需要读取的信息
    /// </summary>
    public class THRecord
    {
        public string ItemName = "";//道具名
        public int  Price = 0;//交易金额
        public int  ActualPrice = 0;//扣除手续费后到账金额
        public int Type = 0;//出售还是购买0=出售1=购买
    }
    /// <summary>
    /// 上架藏宝阁存储的信息
    /// </summary>
    [BsonIgnoreExtraElements]
    public class THItemInfo : DBBase
    {
        public long Uid = 0;//道具ID
        [DBMongodb(1)]
        [DBMongodb(11)]
        public long UserID = 0;//出售者ID
        public string Name = "";//道具名称
        public string Class = "";//职业类型
        public int Excellent = 0;//卓越条数
        public int Enhance = 0;//强化等级
        public int Readdition = 0;//追加等级
        public int Price = 0;//价格
        public int Page = 0;//页签
        public int MaxType = 0;
        public int MinType = 0;
        public int mAreaId = 0;
        public long ListingTime = 0;//上架时间
        public int ConfigId = 0;
        [DBMongodb(11)]
        public int IsDispose = 0;//是否有效
        public int Cnt = 0;//数量
        [BsonIgnore]
        [JsonIgnore]
        public Dictionary<int,int> ClassList = new Dictionary<int,int>();
    }
    [BsonIgnoreExtraElements]
    public class TreasureHouseComponent :TCustomComponent<Player>
    {
        public Dictionary<int,List<THItemInfo>> keyValuePairs = new Dictionary<int,List<THItemInfo>>();
        public List<THRecord> tHRecords = new List<THRecord>();
        public int PageCntMax = 5;//每页最多五个道具信息0-4
        public override void Awake()
        {
            keyValuePairs = new Dictionary<int, List<THItemInfo>>() { { 1,new  List<THItemInfo>() } };
            tHRecords = new List<THRecord>();
            PageCntMax = 5;
        }
        public override void Dispose()
        {
            if (IsDisposeable) return;
            keyValuePairs.Clear();
        }
    }
}
