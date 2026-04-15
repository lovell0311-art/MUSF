using CustomFrameWork;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    /// <summary>
    /// 抽奖数据
    /// </summary>
    [BsonIgnoreExtraElements]
    public class DBLotteryData : DBBase
    {
        /// <summary>
        /// 保底触发计数
        /// </summary>
        public int TotalCount { get; set; } = 0;

        public int GameAreaId { get; set; } = 0;

    }
}
