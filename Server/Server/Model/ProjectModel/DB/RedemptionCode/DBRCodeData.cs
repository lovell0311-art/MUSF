using System;
using System.Collections.Generic;
using CustomFrameWork;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace ETModel
{
    /// <summary>
    /// 账户区数据
    /// </summary>
    [BsonIgnoreExtraElements]
    public class DBRCodeData : DBBase
    {
        /// <summary>
        /// RedemptionCodeType 兑换码类型
        /// </summary>
        [DBMongodb(1, true)]
        public int CodeType { get; set; }
        [DBMongodb(1, true)]
        public int RewardType { get; set; }

        /// <summary>
        /// 兑换码
        /// </summary>
        public string CodeStr { get; set; }
        public string RewardStr { get; set; } = "{}";

        /// <summary>
        /// 有效期 0:无期限
        /// </summary>
        public long TimeTick { get; set; }

        /// <summary>
        /// 使用次数 0无限次
        /// </summary>
        public int UseCount { get; set; }
        public string UseIds { get; set; } = "[]";

        public int IsDispose { get; set; } = 0;   //1代表已删除

        [BsonIgnore]
        [JsonIgnore]
        public List<long> UseIdslist { get; set; }

        public void DeSerialize()
        {
            UseIdslist = Help_JsonSerializeHelper.DeSerialize<List<long>>(UseIds);
        }
        public void Serialize()
        {
            UseIds = Help_JsonSerializeHelper.Serialize(UseIdslist);
        }

    }
    [BsonIgnoreExtraElements]
    public class DBRCodeLogData : DBBase
    {
        [DBMongodb(1, true)]
        public long UserId { get; set; }
        /// <summary>角色ID</summary>
        [DBMongodb(2, true)]
        public long GameUserId { get; set; }

        /// <summary>
        /// RedemptionCodeType 兑换码类型
        /// </summary>
        [DBMongodb(1, true)]
        [DBMongodb(2, true)]
        public int CodeType { get; set; }

        public int RewardType { get; set; } = 1;

        public string CodeStr { get; set; }

        public int IsDispose { get; set; } = 0;   //1代表已删除

    }
    [BsonIgnoreExtraElements]
    public class DBRCodeErrorData : DBBase
    {
        [DBMongodb(1, true)]
        public long UserId { get; set; }
        public long GameUserId { get; set; }

        public int ErrorCount { get; set; }

        public long TimeTick { get; set; }
    }
}
