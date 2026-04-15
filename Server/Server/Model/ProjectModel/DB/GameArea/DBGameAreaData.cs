using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ETModel
{
    /// <summary>
    /// 区服信息
    /// </summary>
    public class DBGameAreaData : DBBase
    {
        public int GameAreaId { get; set; }
        public int RealLine { get; set; } = 0;
        public string NickName { get; set; } = "";
        public long CreateTime { get; set; }
        public int State { get; set; } = 0;
    }
    /// <summary>
    /// 合区数据
    /// </summary>
    public class DBCoincidentdata : DBBase
    {
        public int OldAreaId { get; set; }
        public int NewAreaId { get; set; }
        public int IsDispose { get; set; } = 0;
    }
}