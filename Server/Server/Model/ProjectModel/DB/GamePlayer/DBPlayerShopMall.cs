using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TencentCloud.Mongodb.V20190725.Models;

namespace ETModel
{
    /// <summary>
    /// 角色充值信息
    /// </summary>
    [BsonIgnoreExtraElements]
    public class DBPlayerShopMall : DBBase
    {
        /// <summary>
        /// GameID
        /// </summary>
        [DBMongodb(1)]
        public long GameUserID { get; set; }
        ///// <summary>
        ///// 奇迹币
        ///// </summary>
        //public int MiracleCoin { get; set; }
        ///// <summary>
        ///// 当前元宝数量
        ///// </summary>
        //public int CurrentYuanbao { get; set; }
        /// <summary>
        /// 1 >> 位数。每一位代表不同状态DeviationType
        /// </summary>
        public int RechargeStatus { get; set; } = 1;
        /// <summary>
        /// 小月卡到期时间
        /// </summary>
        public long MinMCEndTime { get; set; } = 0;
        /// <summary>
        /// 大月卡到期时间
        /// </summary>
        public long MaxMCEndTime { get; set; } = 0;
        /// <summary>
        /// 累计充值
        /// </summary>
        public int AccumulatedRecharge { get; set; } = 0;
        /// <summary>
        /// 原地复活CD
        /// </summary>
        public long InSituCd { get; set; } = 0;
        public string StrSevenRecharge { get; set; } = "";

        public string RechargeRecord { get; set; } = "";
        /// <summary>
        /// 首充
        /// </summary>
        [BsonIgnore]
        [JsonIgnore]
        public Dictionary<int, int> RechargeRecordList { get; set; } = new Dictionary<int, int>();
        [BsonIgnore]
        [JsonIgnore]
        public Dictionary<(string, int), (int, bool)> SevenRecharge = new Dictionary<(string, int), (int, bool)>();
    }

    /// <summary>交易平台</summary>
    public enum TradePlatform
    {
        /// <summary>恺英支付接口</summary>
        XY = 0,
        /// <summary>第三方支付接口</summary>
        Hy = 1,
        /// <summary>
        /// 抖音支付
        /// </summary>
        DY = 2,
        /// <summary>
        /// 抖音支付 预下单
        /// </summary>
        DYPre = 3,
        /// <summary>
        /// 手Q
        /// </summary>
        ShouQ = 4,
        /// <summary>
        /// 游戏方
        /// </summary>
        MyPay = 5,
        /// <summary>
        /// GM充值
        /// </summary>
        GM = 99,
    }



    [BsonIgnoreExtraElements]
    public class DBPlayerPayOrderInfo : DBBase
    {
        /// <summary>
        /// 恺英订单ID (注:XY字段)
        /// </summary>
        public string Ordef_id { get; set; } = "";
        /// <summary>
        /// 游戏方ID (注:支付订单id)
        /// </summary>
        [DBMongodb(1)]
        public long App_Ordef_id { get; set; } = 0;
        /// <summary>
        /// 支付平台
        /// </summary>
        [DBMongodb(2)]
        public TradePlatform TradePlatform { get; set; } = TradePlatform.XY;

        /// <summary>
        /// 恺英游戏ID (注:XY字段)
        /// </summary>
        public long Gid { get; set; } = 0;
        /// <summary>
        /// 用户ID (注:XY字段)
        /// </summary>
        public long Uid { get; set; } = 0;
        /// <summary>
        /// 游戏方用户ID (注:UserId)
        /// </summary>
        [DBMongodb(3)]
        [DBMongodb(10)]
        [DBMongodb(11)]
        public long GUid { get; set; } = 0;
        /// <summary>
        /// 角色ID (注:GameUserId)
        /// </summary>
        [DBMongodb(4)]
        public long Rid { get; set; } = 0;
        /// <summary>
        /// 充值类型 (注:XY字段)
        /// </summary>
        public int Product_id { get; set; } = 0;
        /// <summary>
        /// 充值金额
        /// </summary>
        public int Money { get; set; } = 0;
        /// <summary>
        /// 时间戳
        /// </summary>
        [DBMongodb(10)]
        public long Time { get; set; } = 0;
        /// <summary>
        /// 角色名
        /// </summary>
        public string RName { get; set; } = "";
        /// <summary>
        /// 充值到账时间如果(Success==false && SuccessTime != 0 )那么就是收到订单回复但没到玩家账号
        /// </summary>
        [DBMongodb(10)]
        [DBMongodb(11)]
        public long SuccessTime { get; set; } = 0;
        /// <summary>
        /// 订单是否有效
        /// </summary>
        [DBMongodb(11)]
        public bool Effective { get; set; } = false;
        /// <summary>
        /// 充值是否到账
        /// </summary>
        [DBMongodb(11)]
        public bool Success { get; set; } = false;
        /// <summary>
        /// 渠道id
        /// </summary>
        public string ChannelId { get; set; } = "";

        /// <summary>
        /// 金额 分
        /// </summary>
        public int MoneyFen { get; set; } = 0;
        /// <summary>
        /// 统计充值金额
        /// </summary>
        [DBMongodb(10)]
        public bool StatisticalAmount { get; set; } = true;

    }


    [BsonIgnoreExtraElements]
    public class DBOnlineReward : DBBase
    {
        public long GameUerId { get; set; }//玩家
        public string Time { get; set; }//年月日
        public int OnlineTime { get; set; }//在线时长分钟
    }
}
