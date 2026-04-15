using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace ETModel
{
    public enum DBType
    {
        Core,

        Log,
    }
    /// <summary>
    /// 用来缓存数据
    /// </summary>
    public class DBComponent : DBComponentBase
    {
        public const int CommonDBId = 0;


        /// <summary>
        /// 数据库类型
        /// </summary>
        public DBType DBType { get; private set; }

        public int DBZone { get; private set; }
        public int DBId { get; private set; }

        public void SetDBInfo(int id, DBType b_DbType, int b_DBZone)
        {
            this.DBId = id;
            this.DBType = b_DbType;
            this.DBZone = b_DBZone;
        }
    }
}
