using ETModel;
using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    [BsonIgnoreExtraElements]
    public class DBFriendData : DBBase
    {
        /// <summary>角色ID</summary>
        [DBMongodb(1)]
        [DBMongodb(11)]
        public long GameUserId { get; set; }
        /// <summary>好友ID</summary>
        [DBMongodb(11)]
        public long FriendUserId { get; set; }
        /// <summary>类型(0:默认 1:拉黑 2:仇人 3:申请 4:好友)</summary>
        public int ListType { get; set; }
        /// <summary>角色名</summary>
        public string CharName { get; set; }
        /// <summary>拉黑或者被击杀时间</summary>
        public long TimeDate { get; set; }
        /// <summary>玩家的区服务ID</summary>
        public int AreaId { get; set; }

        /// <summary>是否删除 0:未删 1:删除</summary>
        public int IsDisabled { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class DBPlayerTitle : DBBase
    {

        /// <summary>角色ID或者账号ID</summary>
        [DBMongodb(1)]
        [DBMongodb(11)]
        public long UserId { get; set; }
        /// <summary>0：角色称号 1:账号称号 /// </summary>
        public int Type { get; set; }
        /// <summary>正在使用这个称号的是那个角色，主要用于账号共享称号        /// </summary>
        public long UseID { get; set; }
        [DBMongodb(11)]
        public int TitleID { get; set; }
        public long BingTime { get; set; }
        public long EndTime { get; set; }
        public int IsDisabled { get; set; }
    }
}
