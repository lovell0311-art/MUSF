using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ETModel
{
    /// <summary>
    /// 角色
    /// </summary>
    [PrivateObject]
    [BsonIgnoreExtraElements]
    public class DBPlayerUnitData : DBBase
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        [DBMongodb(1, true)]
        public long GameUserId { get; set; }
        #region 私有成员
        [BsonElement("Index")]
        public int index;
        [BsonElement("X")]
        public int x;
        [BsonElement("Y")]
        public int y;
        #endregion
        /// <summary>
        /// 地图Id
        /// </summary>
        [BsonIgnore]
        [JsonIgnore]
        public int Index { get { return index; } }
        /// <summary>
        /// 地图x坐标
        /// </summary>
        [BsonIgnore]
        [JsonIgnore]
        public int X { get { return x; } }
        /// <summary>
        /// 地图y坐标
        /// </summary>
        [BsonIgnore]
        [JsonIgnore]
        public int Y { get { return y; } }
        /// <summary>
        /// 角度
        /// </summary>
        public int Angle { get; set; }


        public int Hp { get; set; }
        public int Mp { get; set; }
        public int SD { get; set; }
        public int AG { get; set; }

        public int PkPoint { get; set; }


    }
}