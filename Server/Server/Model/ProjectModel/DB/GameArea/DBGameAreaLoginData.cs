using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ETModel
{
    /// <summary>
    /// 区服登陆信息
    /// </summary>
    public class DBGameAreaLoginData : DBBase
    {
        public long UserId { get; set; }
        public int GameAreaId { get; set; }
        public int Realine { get; set; }

        public string NickName { get; set; } = "";
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        public int LoginCount { get; set; }

        public long LastLoginTime { get; set; }
    }
}