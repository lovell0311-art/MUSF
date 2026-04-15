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
    /// <summary>
    /// 玩家充值相关状态
    /// </summary>
    public enum ActivitRewardType
    {
        OneBit      = 1,
        TowBit      = 1 << 1,
        ThreeBit    = 1 << 2,
        FourBit     = 1 << 3,
        FiveBit     = 1 << 4,
        SixBit      = 1 << 5,
        SevenBit    = 1 << 6,
        EightBit    = 1 << 7,
        NineBit     = 1 << 8,
        TenBit      = 1 << 9,
        ElevenBit   = 1 << 10,
        TwelveBit   = 1 << 11,
        ThirteenBit = 1 << 12,
        FourteenBit = 1 << 13,
        FifteenBit  = 1 << 14,
        SixteenBit  = 1 << 15,
        SeventeenBit    = 1 << 16,
        EightteenBit    = 1 << 17,
        NineteenBit     = 1 << 18,
        TwentyBit       = 1 << 19,
    }
    [BsonIgnoreExtraElements]
    public class PlayerActivitComponent : TCustomComponent<Player>
    {
        [BsonIgnore]
        public Player mPlayer
        {
            get
            {
                return Parent;
            }
        }

        public Dictionary<int, DBMiracleActivities> MiracleActivitInfo;

        public override void Awake()
        {
            MiracleActivitInfo = new Dictionary<int, DBMiracleActivities>();
        }
        public override void Dispose()
        {
            MiracleActivitInfo.Clear();
            base.Dispose();

        }
    }
}