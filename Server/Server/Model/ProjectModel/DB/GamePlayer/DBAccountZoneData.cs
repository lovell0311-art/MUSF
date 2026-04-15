using System;
using System.Collections.Generic;
using CustomFrameWork;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace ETModel
{
    /// <summary>
    /// 账户区数据
    /// </summary>
    [PrivateObject]
    [BsonIgnoreExtraElements]
    public class DBAccountZoneData : DBBase
    {
        /// <summary>
        /// 新账号，还没进入游戏 1.新账号 0.不是第一次进入游戏了
        /// </summary>
        public int NewAccount { get; set; } = 1;

        [BsonElement("YuanbaoCoin")]
        public int yuanbaoCoin;


        /// <summary>
        /// 账号充值累计
        /// </summary>
        public int AccumulatedRecharge { get; set; } = 0;
        /// <summary>
        /// 累计每周充值
        /// </summary>
        public int WeeklyTotalPay { get; set; } = 0;
        /// <summary>
        /// 每周充值跟新时间
        /// </summary>
        public long WeeklyTotalPayTiem { get; set; } = 0;
        /// <summary>
        /// 第三方充值累计值，用于领奖时扣除对应值
        /// </summary>
        public int HyPaytotal { get; set; } = 0;
        /// <summary>
        /// 累计值获得称号
        /// </summary>
        public int PayTitle { get; set; } = 0;
        /// <summary>
        /// 当前元宝数量
        /// </summary>
        [BsonIgnore]
        [JsonIgnore]
        public int YuanbaoCoin { get => yuanbaoCoin; }
        /// <summary>
        /// 已经领取的充值礼包id
        /// CumulativeRechargeGiftId
        /// </summary>
        public HashSet<int> ReceivedCRGiftId { get; set; } = new HashSet<int>();

        /// <summary>
        /// 角色卡
        /// </summary>
        public string Role = "{}";
        [BsonIgnore]
        [JsonIgnore]
        public Dictionary<int, long> RoleDic { get; set; } = null;
        public void DeSerialize()
        {
            RoleDic = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, long>>(Role);
        }
        public void Serialize()
        {
            Role = Help_JsonSerializeHelper.Serialize(RoleDic);
        }

    }
}
