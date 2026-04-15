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
    public class FriendComponent : TCustomComponent<Player>
    {
        [BsonIgnore]
        public Player mPlayer
        {
            get
            {
                return Parent;
            }
        }

        ///<summary>好友数据<列表类型,<角色ID,好友数据结构>> CountMAX:100 目前列表4类型，1拉黑，2仇人，3申请，4好友</summary>
        public Dictionary<int, Dictionary<long, Friend>> FriendLiset = new Dictionary<int, Dictionary<long, Friend>>();

        //好系统推荐列表，不存库，保证第二次刷新推荐好友没有与第一次的重复<UID,UID> CountMAX:10
        public Dictionary<long, long> RecommendList = new Dictionary<long, long>();

        public long Refreshtime;
        public override void Dispose()
        {
            if (IsDisposeable) return;

            //清理数据
            FriendLiset.Clear();
            RecommendList.Clear();
            Refreshtime = 0;
            base.Dispose();

        }
    }

    [BsonIgnoreExtraElements]
    public class PlayerTitle : TCustomComponent<Player>
    {
        [BsonIgnore]
        public Player mPlayer
        {
            get
            {
                return Parent;
            }
        }

        public int UseTitle { get; set; }
        public List<DBPlayerTitle> ListString { get; set; }
        public override void Dispose()
        {
            if (IsDisposeable) return;

            //清理数据
            UseTitle = 0;
            ListString.Clear();
            base.Dispose();

        }
    }
}