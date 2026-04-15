using System;

using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    public enum EOnlineStatus
    {
        None,
        /// <summary>
        /// 刚进入游戏，Player 对象上啥都没有
        /// </summary>
        StartGame,
        /// <summary>
        /// 准备中，这时玩家正在加载数据
        /// </summary>
        Ready,
        /// <summary>
        /// 正常在线，可以对玩家进行操作
        /// </summary>
        Online,
        /// <summary>
        /// 正在下线，这时 Player 身上的组件会缺失
        /// </summary>
        Offline,
    }

    /// <summary>
    /// game 服务器 游戏对象 其他服务器不要用
    /// </summary>
    public sealed partial class Player
    {
        public long UserId { get; set; }
        public int GameAreaId { get; set; }
        public long GameUserId { get; set; }
        public int SourceGameAreaId { get; set; }

        /// <summary>
        /// 新网关ID
        /// </summary>
        public int GateServerId { get; set; } = -1;

        public DBAccountZoneData Data { get; set; }
        public ClientTimeComponent ClientTime { get; set; }

        public int AppID { get; set; }
        public Session Session { get; set; }

        /// <summary>
        /// 在线状态
        /// <para>注意:</para>
        /// <para>非 Message 函数中访问Player时，如果中途使用 await 切换上下文，在恢复后务必对 OnlineStatus 进行判断。如不在 'Online' 状态，应中断操作。（如果此操作无法中断，请加Actor锁）</para>
        /// </summary>
        public EOnlineStatus OnlineStatus { get; set; } = EOnlineStatus.None;

        public override void Clear()
        {
            Data = null;
            ClientTime = null;
            GateServerId = -1;
            Session = null;

            UserId = 0;
            OnlineStatus = EOnlineStatus.None;
        }
    }


    public sealed partial class Player : CustomComponent
    {
        public override void Dispose()
        {
            if (this.IsDisposeable)
            {
                return;
            }
            base.Dispose();
        }
    }
}