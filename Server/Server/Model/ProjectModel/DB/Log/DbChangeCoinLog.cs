using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    /// <summary>
    /// 服务器注册表
    /// </summary>
    [BsonIgnoreExtraElements]
    public class DbChangeCoinLog: DBBase
    {
        public long UserId { get; set; }
        public long GameUserId { get; set; }

        public int GameAreaId { get; set; }
        /// <summary>
        /// 服务器ID
        /// </summary>
        public int GameServerId { get; set; }

        public string Str { get; set; }

        public int CoinType { get; set; }
        public long SourceValue { get; set; }
        public long ChangeValue { get; set; }
        public long CoinValue { get; set; }

        public long CreateTime { get; set; }
    }
}
