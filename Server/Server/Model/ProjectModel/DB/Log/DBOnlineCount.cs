using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    /// <summary>k
    /// 在线人数
    /// </summary>
    [BsonIgnoreExtraElements]
    public class DBOnlineCount : DBBase
    {
        /// <summary>在线人数</summary>
        public int Count { get; set; }
        
        /// <summary>创建时间</summary>
        [DBMongodb(1)]
        public long CreateTime { get; set; }
    }
}
