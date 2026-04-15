using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    /// <summary>
    /// 定时为该玩家生成一个登录gate key,用于断线重连
    /// </summary>
    public class GateSessionKeyNoticeComponent : Component
    {
        public static readonly ETHotfix.Gate2C_GateSessionKeyChange gate2C_GateSessionKeyChange = new ETHotfix.Gate2C_GateSessionKeyChange();
        /// <summary>
        /// Key 变动的间隔
        /// </summary>
        public const long KEY_NOTICE_INTERVAL = 1000 * 60 * 5;
        public long _timerId = 0;
    }
}
