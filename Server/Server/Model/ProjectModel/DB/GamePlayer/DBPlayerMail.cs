using ETModel;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    [BsonIgnoreExtraElements]
    public class DBMailData : DBBase
    {
        [DBMongodb(1)]
        [DBMongodb(11)]
        [DBMongodb(12)]
        public long GameUserID { get; set; }//==0时是系统邮件用于离线玩家
        public long MaliID { get; set; }
        public string MailName { get; set; }
        [DBMongodb(12)]
        public long MailAcceptanceTime { get; set; }
        public long MailValidTime { get; set; } //接收时间 + 15天
        public string MailContent { get; set; }
        public string MailEnclosure { get; set; }
        /// <summary>物品日志</summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(null)]
        public string ItemLog { get; set; }
        public int MailState { get; set; }
        public int ReceiveOrNot { get; set; }
        [DBMongodb(11)]
        public int IsDisabled { get; set; }
    }
}