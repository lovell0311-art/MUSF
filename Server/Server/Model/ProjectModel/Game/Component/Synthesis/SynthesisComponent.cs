using Aop.Api.Domain;
using CustomFrameWork.Baseic;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ETModel
{
    public class SynthesisComponent : TCustomComponent<Player>
    {
        [BsonIgnore]
        public const int I_PackageWidth = 8;
        [BsonIgnore]
        public const int I_PackageHigh = 13;

        public ItemsBoxStatus mItemBox;
        public Dictionary<long, Item> mItemDict = new Dictionary<long, Item>();

        public Player mPlayer
        {
            get
            {
                return Parent;
            }
        }

        public GamePlayer mGamePlayer
        {
            get
            {
                if (gamePlayer == null)
                {
                    gamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
                }
                return gamePlayer;
            }
        }

        private GamePlayer gamePlayer;

        public override void Dispose()
        {
            if (IsDisposeable) return;
            mItemBox = null;
            mItemDict.Clear();
        }
    }
}
