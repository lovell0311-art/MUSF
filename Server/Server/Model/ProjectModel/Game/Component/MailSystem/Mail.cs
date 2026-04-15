using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ETModel
{

    [BsonIgnoreExtraElements]
    public class MailInfo : CustomComponent
    {
        /// <summary>
        ///邮件ID
        /// </summary>
        public long MailId { get; set; }
        /// <summary>
        /// 邮件名称
        /// </summary>
        public string MailName { get; set; }
        /// <summary>
        /// 接收时间
        /// </summary>
        public long MailAcceptanceTime { get; set; }
        /// <summary>
        /// 有效时间
        /// </summary>
        public long MailValidTime { get; set; } //接收时间 + 15天
        /// <summary>
        /// 邮件内容
        /// </summary>
        public string MailContent { get; set; }
        /// <summary>
        /// 附件道具
        /// </summary>
        public List<MailItem> MailEnclosure { get; set; } = new List<MailItem>();
        /// <summary>
        /// 邮件状态，0未读，1读取过
        /// </summary>
        public int MailState { get; set; }
        /// <summary>
        /// 道具是否领取 0未领取 1领取过
        /// </summary>
        public int ReceiveOrNot { get; set; }
        public MailInfo CreatMailInfo()
        {
            MailInfo mailInfo = new MailInfo() 
            {
                MailId = this.MailId,
                MailName = this.MailName,
                MailAcceptanceTime = this.MailAcceptanceTime,
                MailValidTime = this.MailValidTime,
                MailContent = this.MailContent,
                MailEnclosure = new List<MailItem>(this.MailEnclosure),
                MailState = this.MailState,
                ReceiveOrNot = this.ReceiveOrNot
            };
            return mailInfo;
        }
    }
    public class MailItem
    { 
        /// <summary>
        /// 道具配置ID，如果是进入过数据库的道具，这个字段存着大区ID
        /// </summary>
        public int ItemConfigID { get; set; }
        /// <summary>
        /// 道具唯一ID，没有为0
        /// </summary>
        public long ItemID { get; set; }
        public int AreaId { get; set; }
        /// <summary>
        /// 道具属性
        /// </summary>
        public ItemCreateAttr CreateAttr { get; set; }
    }
}