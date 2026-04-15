using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    /// <summary>
    /// 服务器注册表
    /// </summary>
    [BsonIgnoreExtraElements]
    public class DbChangePasswordLog : DBBase
    {
        public long LogId { get; set; }
        public long UserId { get; set; }

        public int GameAreaId { get; set; }
        /// <summary>
        /// 服务器ID
        /// </summary>
        public int GameServerId { get; set; }

        public string LastLoginIP { get; set; } = "";

        /// <summary>
        /// 登录设型号(IPhone 7, vivo NEX 等)
        /// </summary>
        public string DeviceType { get; set; } = "";
        /// <summary>
        /// CPU型号
        /// </summary>
        public string CPUType { get; set; } = "";

        public string BaseVersion { get; set; } = "";
        /// <summary>
        /// 系统类型 1苹果, 2安卓, 3windows
        /// </summary>
        public int OSType { get; set; }
        /// <summary>
        /// 运营商Id
        /// </summary>
        public string ChannelId { get; set; }

        public DateTime UpdateTime2 { get; set; }
    }
}
