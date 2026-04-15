using ETModel;
using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    [BsonIgnoreExtraElements]
    public class DBMiracleActivities : DBBase
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [DBMongodb(11)]
        public long GameUesrID { get; set; }
        /// <summary>
        /// 活动ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 领奖状态，位于运算每一位代表一个独立的状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 32位数值A，根据活动内容代表数据
        /// </summary>
        public int Value32_A { get; set; }
        /// <summary>
        /// 32位数值B，根据活动内容代表数据
        /// </summary>
        public int Value32_B { get; set; }
        /// <summary>
        /// 64位数值A，根据活动内容代表数据
        /// </summary>
        public long Value64_A { get; set; }
        /// <summary>
        /// 64位数值A，根据活动内容代表数据
        /// </summary>
        public long Value64_B { get; set; }
        /// <summary>
        /// 活动是否结束，数据是否有效
        /// </summary>
        [DBMongodb(11)]
        public int IsDisabled { get; set; }
    }
}