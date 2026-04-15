using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    /// <summary>
    /// 服务器注册表
    /// </summary>
    public class DBServiceRegistryInfo: DBBase
    {
        /// <summary>
        /// 服务器ID
        /// </summary>
        public int GameServerId { get; set; }
        /// <summary>
        /// 区服id 
        /// 可能是当前服务器在运行多个区服
        /// 数组"[]"
        /// </summary>
        public string GameAreaIds { get; set; } = "[]";
        /// <summary>
        /// 区服在线人数
        /// 对应多个区服 各自的在线人数
        /// Dictionary int,int
        /// </summary>
        public string PlayerCount { get; set; } = "{}";
        /// <summary>
        /// 更新时间
        /// </summary>
        public long UpdateTime { get; set; }



        public DateTime UpdateTime2 { get; set; }
        /// <summary>
        /// 内网地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 外网地址
        /// </summary>
        public string Address2 { get; set; }
        /// <summary>
        /// 服务端版本号，如：1.1
        /// </summary>
        public string Version { get; set; }
    }
}
