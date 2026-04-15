using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;

namespace ETModel.Robot
{
    // 客户端挂在ClientScene上，服务端挂在Unit上
    public partial class AIComponent : Entity
    {
        /// <summary>
        /// 一帧的时间
        /// </summary>
        public const long FrameTime = 1000;

        public int AIConfigId;

        public ETCancellationToken CancellationToken;

        public long Timer;

        public int Current;

        public string LastNodeName = "Empty";
    }
}
