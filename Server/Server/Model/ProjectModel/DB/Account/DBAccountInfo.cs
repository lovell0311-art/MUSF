using System;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.Diagnostics;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;

namespace ETModel
{
    /// <summary>
    /// 
    /// </summary>
    public class DBAccountSimpleInfo : DBBase
    {
        public long PlayerId { get; set; }
        /// <summary>
        /// 短ID
        /// </summary>
        public long ShowId { get; set; }
        public string Phone { get; set; }

        public int GateId { get; set; } = -1;

        /// <summary>
        /// 在线状态(1在线,0 离线)
        /// </summary>
        public int OnlineStatus { get; set; } = 1;
    }

    public enum AccountIdentity
    {
        None = 0,
        /// <summary>测试身份</summary>
        Test = 1,
        /// <summary>
        /// 工作室
        /// </summary>
        Studio = 2,
    }

    /// <summary>
    /// 账号信息
    /// </summary>
    [BsonIgnoreExtraElements]
    public class DBAccountInfo : DBBase
    {
        [DBMongodb(1, true)]
        public string Phone { get; set; }
        public string Password { get; set; }
        [DBMongodb(2, true)]
        public string XYAccountNumber { get; set; }
        public DateTime RegisterTime { get; set; }
        public string RegisterIP { get; set; } = "";
        /// <summary>
        /// 0手机 1电脑
        /// </summary>
        public int LastLoginType { get; set; } = 0;
        public DateTime LastLoginTime { get; set; }
        public string LastLoginIP { get; set; } = "";


        /// <summary>
        /// 登录设型号(IPhone 7, vivo NEX 等)
        /// </summary>
        public string DeviceType { get; set; } = "";
        /// <summary>
        /// CPU型号
        /// </summary>
        public string CPUType { get; set; } = "";

        /// <summary>
        /// 账号状态
        /// </summary>
        public int Disabled { get; set; } = 0;
        /// <summary>
        /// 渠道Id
        /// </summary>
        [DBMongodb(3, true)]
        public string ChannelId { get; set; } = "";
        /// <summary>
        /// 封禁时间
        /// </summary>
        public long BanTillTime { get; set; } = 0;
        /// <summary>
        /// 封禁原因
        /// </summary>
        public string BanReason { get; set; } = "";

        public int IdInspect { get; set; } = 0;
        public string IdCard { get; set; } = "";
        public string Name { get; set; } = "";
        /// <summary>账号身份</summary>
        public AccountIdentity Identity { get; set; } = AccountIdentity.None;
        public string Pi { get; set; } = "";//国网防沉迷对应Id

        public long LastIdInspect { get; set; }

        /// <summary>
        /// 抖音id
        /// </summary>
        public string DouyinAccountNumber { get; set; }
        public string ShouQAccountNumber { get; set; }
        /// <summary>
        /// 上次登录大区
        /// </summary>
        public int LastLoginToTheRegion { get; set; }
        /// <summary>
        /// 上次登录大区线路
        /// </summary>
        public int LastLoginToLine { get; set; }
        /// <summary>
        /// 别人的推广码
        /// </summary>
        public string Code { get; set; }
    }

    public class ReturnSDK
    {
        public string errno { get; set; }
        public string msg { get; set; }
        //public List result { get; set; }
    }
    public class RealNameError
    {
        public int errcode { get; set; } = 0;
        public string errmsg { get; set; } = "";
    }
    public class result
    {
        public int status { get; set; } = 0;
        public string pi { get; set; } = "";
    }
    public class data
    {
        public result result { get; set; } = new result();
    }
    public class RealNameResponse
    {
        public int errcode { get; set; } = 0;
        public string errmsg { get; set; } = "";
        public data data { get; set; } = new data();
    }

    public class BehaviorData
    {
        public int no = 1;
        public string si = "";//游 戏内 部会 话标识
        public int bt = 1; // 上线行为类型
        public long ot = 0; // 行为发生时间
        public int ct = 0; // 已认证通过用户
        public string di = "";//游客模式设备标识，由游戏运营单位生成，游客用户下必填
        public string pi = "";// 已认证通过用户的唯一标识
    }

    public class results
    {
        public int no { get; set; } = 0;
        public int errcode { get; set; } = 0;
        public string errmsg { get; set; } = "";

    }
    public class datas
    {
        public List<results> results { get; set; } = new List<results>();
    }
    public class LoginOutInfo
    {
        public int errcode { get; set; } = 0;
        public string errmsg { get; set; } = "";
        public datas data { get; set; } = new datas();
    }


    public class DouYinLoginReturnSDK
    {
        public int code { get; set; }
        public SdkData data { get; set; }
        public string log_id { get; set; }
        public string message { get; set; }
        public class SdkData
        {
            public string sdk_open_id { get; set; }
            public string open_id { get; set; }
            public string user_id { get; set; }

            public int age_type { get; set; }

            public string log_id { get; set; }
        }
    }
    public class DouYinPrePayResult
    {
        public string sdk_param { get; set; }
        public int code { get; set; }
        public string message { get; set; }

    }
    public class V4Data
    {
        public string Mchid { get; set; }
        public string Out_trade_no { get; set; }
        public string Trade_no { get; set; }
        public string Total_amount { get; set; }
        public string Float_amount { get; set; }
        public string Payurl { get; set; }
        public string PayInfo { get; set; }
        public string Trade_type { get; set; }
    }
}

