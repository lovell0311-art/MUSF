using CustomFrameWork.Baseic;
using ETModel;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    public class PlayerOnlineRewardComponent : TCustomComponent<Player>
    {
        [BsonIgnore]
        public Player mPlayer
        {
            get
            {
                return Parent;
            }
        }

        public DBOnlineReward dBOnlineReward;

        public long TimerId;
        public override void Awake()
        {
            dBOnlineReward = new DBOnlineReward();
        }
        public override void Dispose()
        {
            dBOnlineReward.Dispose();
            ETModel.ET.TimerComponent.Instance.Remove(ref TimerId);
            base.Dispose();

        }
    }
}
