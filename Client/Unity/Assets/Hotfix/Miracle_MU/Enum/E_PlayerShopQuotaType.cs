using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ETHotfix
{
    /// <summary>
    /// 充值类型
    /// </summary>
    public enum E_PlayerShopQuotaType 
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
    }
}
