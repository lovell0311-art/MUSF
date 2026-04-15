using Aop.Api.Domain;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    /// <summary>
    /// 交易日志
    /// </summary>
    [BsonIgnoreExtraElements]
    public class DBTradeLog : DBBase
    {
        [DBMongodb(1)]
        public long UserId = 0;
        [DBMongodb(2)]
        public long GameUserId = 0;
        [DBMongodb(3)]
        public long TargetUserId = 0;
        [DBMongodb(4)]
        public long TargetGameUserId = 0;
        ///<summary> 交易的物品uid </summary>
        [DBMongodb(5)]
        public long ItemUid = 0;
        /// <summary> 魔晶 </summary>
        public int Yuanbao = 0;
        /// <summary> 金币 </summary>
        public int GoldCoin = 0;
        public int GameServerId = 0;
        public long CreateTime = 0;
        public string Str = "";
    }
}
