using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ETModel
{
    /// <summary>
    /// 验证码
    /// </summary>
    public class DBSmsCode : DBBase
    {
       

        public string Phone { get; set; }
        public string Code { get; set; }
        /// <summary>
        /// 日期 用于排除一些查询
        /// </summary>
        public int Day { get; set; }
        public DateTime LastSendTime { get; set; }
    }
}