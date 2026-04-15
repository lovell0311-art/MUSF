using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ETModel
{
    /// <summary>
    /// 玩家身份信息
    /// </summary>
    public class DBPlayerIDInfo : DBBase
    {
        public long PlayerId { get; set; }
        /// <summary>
        /// 短ID
        /// </summary>
        public long ShowId { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IDNumber { get; set; } = "未绑定身份";
        /// <summary>
        /// 姓名
        /// </summary>
        public string IDName { get; set; } = "未绑定身份";
    }
}