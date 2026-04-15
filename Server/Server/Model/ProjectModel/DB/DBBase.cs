using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
namespace ETModel
{
    /// <summary>
    /// 
    /// </summary>
    public class DBBase : ComponentWithId
    {
        //[Newtonsoft.Json.JsonIgnore()]
        //[BsonIgnoreIfDefault]
        //[BsonDefaultValue(0L)]
        //[BsonElement]
        //[BsonId]
        //public long Id { get; set; }


        //[BsonIgnoreIfDefault]
        //[BsonId]
        //[BsonElement]
        ////[JsonIgnore]
        ////[BsonIgnoreIfNull]
        //[BsonIgnore]
        //public string _id { get; set; }

        /// <summary>
        /// 表名字
        /// </summary>
        [JsonIgnore]
        [BsonIgnore]
        public string _t { get { return this.GetType().Name; } }
        /// <summary>
        /// 数据是否修改 操作该值要加锁 type:CoroutineLockType.DB key:DB.Id
        /// </summary>
        [JsonIgnore]
        [BsonIgnore]
        public bool IsChange { get; set; }
        /// <summary>
        /// 数据修改时间
        /// </summary>
        [JsonIgnore]
        [BsonIgnore]
        public long ChangeTick { get; set; }
        [JsonIgnore]
        [BsonIgnore]
        public DBProxyComponent ProxyComponent { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class DBMongodbAttribute : BaseAttribute
    {
        public int GroupID { get; private set; }
        public bool SortType { get; private set; }
        /// <summary>
        /// 组合索引id  
        /// </summary>
        /// <param name="b_GroupID"></param>
        /// <param name="b_SortType">正序 倒序</param>
        public DBMongodbAttribute(int b_GroupID, bool b_SortType = true)
        {
            GroupID = b_GroupID;
            SortType = b_SortType;
        }
    }
}