using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ETModel
{
    /// <summary>
    /// 验证码
    /// </summary>
    public class DBIdInspect: DBBase
    {
        [DBMongodb(1)]
        public string IdCard { get; set; }
        public string Name { get; set; }
    }
}