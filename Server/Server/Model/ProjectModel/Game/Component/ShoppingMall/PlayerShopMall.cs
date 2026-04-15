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
    public enum PlayerShopQuotaType
    {
        /// <summary>
        /// 首充一档 金额 == PlayerShopQuota.FirstTopUp 单位：元
        /// </summary>
        FirstTopUp = 1,
        /// <summary>
        /// 首充二档 金额 == PlayerShopQuota.SecondTopUp 单位：元
        /// </summary>
        SecondTopUp,
        /// <summary>
        /// 首充三档 金额 == PlayerShopQuota.ThirdTopUp 单位：元
        /// </summary>
        ThirdTopUp,
        /// <summary>
        /// 首充四档 金额 == PlayerShopQuota.FourthlyTopUp 单位：元
        /// </summary>
        FourthlyTopUp,
        /// <summary>
        /// 首充五档 金额 == PlayerShopQuota.FifthTopUp 单位：元
        /// </summary>
        FifthTopUp,
        /// <summary>
        /// 首充一次性充值 金额 == PlayerShopQuota.FifthTopUp 单位：元
        /// </summary>
        OneTimeRecharge,
        /// <summary>
        /// 七天充值 金额 == PlayerShopQuota.SevenDaysTopUp 单位：元
        /// </summary>
        SevenDaysTopUp,
        /// <summary>
        /// 旗帜 金额 == PlayerShopQuota.AwardFlag 单位：元
        /// </summary>
        AwardFlag,
        /// <summary>
        /// 变身戒指 金额 == PlayerShopQuota.TransformationRing 单位：元
        /// </summary>
        TransformationRing,
        /// <summary>
        /// 凤凰坐骑 金额 == PlayerShopQuota.PhoenixMount 单位：元
        /// </summary>
        PhoenixMount,
        /// <summary>
        /// 商城充值第一档次 单位：元
        /// </summary>
        StoreRechargeI,
        /// <summary>
        /// 商城充值第二档次 单位：元
        /// </summary>
        StoreRechargeII,
        /// <summary>
        /// 商城充值第三档次 单位：元
        /// </summary>
        StoreRechargeIII,
        /// <summary>
        /// 商城充值第四档次 单位：元
        /// </summary>
        StoreRechargeIV,
        /// <summary>
        /// 商城充值第五档次 单位：元
        /// </summary>
        StoreRechargeV,
        /// <summary>
        /// 商城充值第六档次 单位：元
        /// </summary>
        StoreRechargeVI,
        /// <summary>
        /// 商城充值第七档次 单位：元
        /// </summary>
        StoreRechargeVII,
        /// <summary>
        /// 商城充值第八档次 单位：元
        /// </summary>
        StoreRechargeVIII,
        /// <summary>
        /// 等级充值第一档次 单位：元
        /// </summary>
        LevelTopUpI,
        /// <summary>
        /// 等级充值第二档次 单位：元
        /// </summary>
        LevelTopUpII,
        /// <summary>
        /// 等级充值第三档次 单位：元
        /// </summary>
        LevelTopUpIII,
        /// <summary>
        /// 等级充值第四档次 单位：元
        /// </summary>
        LevelTopUpIV,
        /// <summary>
        /// 等级充值第五档次 单位：元
        /// </summary>
        LevelTopUpV,
        /// <summary>
        /// 等级充值第六档次 单位：元
        /// </summary>
        LevelTopUpVI,
        /// <summary>
        /// 等级充值第七档次 单位：元
        /// </summary>
        LevelTopUpVII,
        /// <summary>
        /// 限时活动的充值同时送旗帜和变身戒指
        /// </summary>
        ActiveTopUpI,
        /// <summary>
        /// 指定金额充值
        /// </summary>
        SpecifiedAmount,
    }
    /// <summary>
    /// 玩家充值相关状态
    /// </summary>
    public enum DeviationType
    {
        /// <summary>
        /// 没有充值
        /// </summary>
        NoRecharge = 1 << 0,
        /// <summary>
        /// 首充6元
        /// </summary>
        FirstCharge6 = 1 << 1,
        /// <summary>
        /// 首充16元
        /// </summary>
        FirstCharge38 = 1 << 2,
        /// <summary>
        /// 首充36元
        /// </summary>
        FirstCharge68 = 1 << 3,
        /// <summary>
        /// 首充66元
        /// </summary>
        FirstCharge198 = 1 << 4,
        /// <summary>
        /// 首充96元
        /// </summary>
        FirstCharge288 = 1 << 5,
        /// <summary>
        /// 小月卡
        /// </summary>
        MinMonthlyCard = 1 << 6,
        /// <summary>
        /// 大月卡
        /// </summary>
        MaxMonthlyCard = 1 << 7,
    }
    [BsonIgnoreExtraElements]
    public class PlayerShopMallComponent : TCustomComponent<Player>
    {
        [BsonIgnore]
        public Player mPlayer
        {
            get
            {
                return Parent;
            }
        }

        public DBPlayerShopMall dBPlayerShopMall;
        public override void Awake()
        {
            dBPlayerShopMall = new DBPlayerShopMall();
        }
        public override void Dispose()
        {
            //清理数据
            /*RechargeStatus = 0;
            MinMCEndTime = 0;
            MaxMCEndTime = 0;
            AccumulatedRecharge = 0;*/
            dBPlayerShopMall.Dispose();
            base.Dispose();

        }
    }
}