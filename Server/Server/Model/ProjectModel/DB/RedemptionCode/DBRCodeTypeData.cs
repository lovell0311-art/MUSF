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
    [BsonIgnoreExtraElements]
    public class DBRCodeTypeData : DBBase
    {
        /// <summary>
        /// RedemptionCodeType 兑换码类型
        /// </summary>
        [DBMongodb(1, true)]
        public int CodeType { get; set; }

        [DBMongodb(1, true)]
        public int RewardType { get; set; }

        public string RewardStr { get; set; } = "{}";

        public int IsDispose { get; set; } = 0;   //1代表已删除

    }
    /// <summary>
    /// 兑换码类型
    /// </summary>
    public enum RedemptionCodeType
    {
        /// <summary>
        ///  1.一次性兑换码：此兑换码只能兑换一次，兑换1次获得配置物品后失效，兑换码失效则悬浮提示“兑换失败，此兑换码已失效！”。
        /// </summary>
        OneType = 1,
        /// <summary>
        /// 2.无限制兑换码（绑定账号）：此兑换码不同账号可以重复兑换（同一个账号只能兑换一次），输入此兑换码都可获得配置物品，如果兑换码失效悬浮提示“兑换失败，此兑换码已失效！”。
        /// </summary>
        WuXianZhi = 2,
        /// <summary>
        /// 3.无限制兑换码（绑定角色）：此兑换码同一个账号下不同角色可以重复兑换（同一个角色只能兑换一次），输入此兑换码都可获得配置物品，如果兑换码失效悬浮提示“兑换失败，此兑换码已失效！”。
        /// </summary>
        WuXianZhiByPlayer = 3,
        /// <summary>
        /// 4.限制次数兑换码（绑定账号）：此兑换码可配置兑换次数，不同账号可以重复兑换（同一个账号只能兑换一次），超出兑换次数失效，如果兑换码失效悬浮提示“兑换失败，此兑换码已失效！”。
        /// </summary>
        LimitCount = 4,
        /// <summary>
        /// 5.限制次数兑换码（绑定角色）：此兑换码可配置兑换次数，同一个账号下不同角色可以重复兑换（同一个角色只能兑换一次），超出兑换次数失效，如果兑换码失效悬浮提示“兑换失败，此兑换码已失效！”。
        /// </summary>
        LimitCountByPlayer = 5,

    }
}
