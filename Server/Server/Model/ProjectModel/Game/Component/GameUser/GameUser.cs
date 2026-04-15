using System;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    public enum DisconnectType
    {
        RepeatLogin = 0, // 顶号
        ServerShutdown = 1, // 服务器关闭
        GMKickOffline = 2, // gm 踢下线
        Ban = 3,    // 封号

    }


    public sealed class GameUser : ACustomComponent
	{
        public long UserId { get; set; }
        public int GameAreaId { get; set; }

        public int GameAreaLineId { get; set; }

        public string ConnectIp { get; set; }

        public int AppendData { get { return GameAreaId << 16 | GameAreaLineId; } }

        /// <summary>
        /// 新网关ID
        /// </summary>
        public int GateServerId { get; set; } = -1;

        public Player Player { get; set; } = null;

        public override void Dispose()
        {
            if (this.IsDisposeable) return;

            UserId = 0;
            GateServerId = -1;
            GameAreaId = 0;
            GameAreaLineId = 0;
            Player = null;
            ConnectIp = null;

            base.Dispose();
        }
    }
}