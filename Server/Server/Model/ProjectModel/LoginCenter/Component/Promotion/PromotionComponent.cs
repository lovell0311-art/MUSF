using System;
using System.Collections.Generic;
using System.Diagnostics;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using System.Threading.Tasks;
using ETHotfix;
using CustomFrameWork.Component;
using System.Collections;
using TencentCloud.Ecm.V20190719.Models;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using TencentCloud.Scf.V20180416.Models;

namespace ETModel
{
    public class PlayerPromotionInfo : DBBase
    {
        public long UserID { get; set; } = 0;
        public string Code { get; set; } = "";
        /// <summary>
        /// 邀请人数
        /// </summary>
        public int MemberI { get; set; } = 0;
        public int MemberStatusI { get; set; } = 0;
        public string MemberIdI { get; set; } = "";
        [BsonIgnore]
        [JsonIgnore]
        public List<long> MemberIUserId { get; set; } = new List<long>();
        /// <summary>
        /// 到达120级人数
        /// </summary>
        public int MemberII { get; set; } = 0;
        public int MemberStatusII { get; set; } = 0;
        public string MemberIdII { get; set; } = "";
        [BsonIgnore]
        [JsonIgnore]
        public List<long> MemberIIUserId { get; set; } = new List<long>();
        /// <summary>
        /// 到达220级人数
        /// </summary>
        public int MemberIII { get; set; } = 0;
        public int MemberStatusIII { get; set; } = 0;
        public string MemberIdIII { get; set; } = "";
        [BsonIgnore]
        [JsonIgnore]
        public List<long> MemberIIIUserId { get; set; } = new List<long>();

        public int IsDispose { get; set; } = 0;
    }

    public class PromotionComponent : TCustomComponent<MainFactory>
    {
        public Dictionary<long, PlayerPromotionInfo> PromotionUser = new Dictionary<long, PlayerPromotionInfo>();
        public Dictionary<string,long> PromotionUserCode = new Dictionary<string,long>();

        public List<string> chars = new List<string>() {};
        public override void Awake()
        {
            PromotionUser = new Dictionary<long, PlayerPromotionInfo>();
            PromotionUserCode = new Dictionary<string, long>();
            chars = new List<string>() { "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"};
            Log.Info($"ServerMGMT PromotionComponent Loading......");
        }
        public override void Dispose()
        {
            PromotionUser.Clear();
            PromotionUserCode.Clear();
        }
    }
}
