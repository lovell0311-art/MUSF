using CustomFrameWork.Baseic;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TencentCloud.Cat.V20180409.Models;

namespace ETModel
{
    [BsonIgnoreExtraElements]
    public class PlayerMailComponent : TCustomComponent<Player>
    {
        [BsonIgnore]
        public Player mPlayer
        {
            get
            {
                return Parent;
            }
        }

        public Dictionary<long,MailInfo> mailInfos = new Dictionary<long,MailInfo>();
        public Dictionary<long,MailInfo> ServerMail = new Dictionary<long,MailInfo>();
        public override void Dispose()
        {
            //清理数据
            mailInfos.Clear();
            ServerMail.Clear();
            base.Dispose();

        }
    }
}