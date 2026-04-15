using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    public enum ELoginLogType
    {
        /// <summary>注册</summary>
        Register = 1,
        /// <summary>登录</summary>
        Login = 2,
        /// <summary>游戏上线</summary>
        GameOnline = 3,
        /// <summary>游戏下线</summary>
        GameOffline = 4,
    }

    public enum ELoginType
    {
        /// <summary>手机登录</summary>
        Phone = 1,
        /// <summary>新游uid</summary>
        XYUID = 2,
    }

    /// <summary>k
    /// 服务器注册表
    /// </summary>
    [BsonIgnoreExtraElements]
    public class DbLoginLog : DBBase
    {
        public ELoginLogType LoginLogType { get; set; }
        public ELoginType LoginType { get; set; }

        public long UserId { get; set; }

        /// <summary>服务器ID</summary>
        public int GameServerId { get; set; }

        /// <summary>创建时间</summary>
        public long CreateTime { get; set; }

        /// <summary>角色id</summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue( 0)]
        public long GameUserId { get; set; }

        [BsonIgnoreIfDefault]
        [BsonDefaultValue(null)]
        public string ChannelId { get; set; }
        /// <summary>
        /// 登录设型号(IPhone 7, vivo NEX 等)
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(null)]
        public string DeviceType { get; set; }
        /// <summary>
        /// 设备码
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(null)]
        public string DeviceNum { get; set; }
        /// <summary>
        /// CPU型号
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(null)]
        public string CPUType { get; set; }

        [BsonIgnoreIfDefault]
        [BsonDefaultValue(null)]
        public string BaseVersion { get; set; }
        /// <summary>
        /// 系统类型 1苹果, 2安卓, 3其他
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        public int OSType { get; set; }
        /// <summary>
        /// 终端IP
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(null)]
        public string TerminalIP { get; set; }

    }
}
