using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    [BsonIgnoreExtraElements]
    public class DBLotteryLog : DBBase
    {
        public long UserId { get; set; }
        public long GameUserId { get; set; }
        public long CreateTime { get; set; }
        public string NickName { get; set; }
        public string Desc { get; set;}
    }
}
