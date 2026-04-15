using System;
using System.Threading.Tasks;
using ETHotfix;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{

    public enum GateUserState
    {
        Disconnect,
        Gate,
        Game,
        
    }

    /// <summary>
    /// gate 服 专用对象
    /// </summary>
    public sealed partial class GateUser
    {
        /// <summary>
        /// 区服Id
        /// </summary>
        public int GameAreaId { get; set; }
        /// <summary>
        /// 区服线路Id
        /// </summary>
        public int GameAreaLineId { get; set; }
        /// <summary>
        /// 渠道Id
        /// </summary>
        public string ChannelId { get; set; }
        /// <summary>
        /// 会话
        /// </summary>
        public Session ClientSession { get; set; }
        /// <summary>
        /// 会话 intanceId,用来判断 ClientSession 是否已销毁
        /// </summary>
        public long CSessionInstanceId { get; set; }

        public GatePlayer GatePlayer { get; set; }

        public int GameServerId { get; set; }

        public GateUserState GateUserState { get; set; }

        private void Clear()
        {
            ChannelId = null;
            GameAreaId = 0;
            GameAreaLineId = 0;
            ClientSession = null;
            CSessionInstanceId = 0;
            GatePlayer = null;
            GameServerId = 0;
            GateUserState = GateUserState.Disconnect;
        }
    }
    public sealed partial class GateUser : ADataContext<long>
    {
        protected override int PoolCountMax => 0;

        public long UserID { get; private set; }

        public override void ContextAwake(long b_Args)
        {
            UserID = b_Args;
        }
        public override void Dispose()
        {
            if (IsDisposeable) return;

            UserID = 0;
            Clear();
            base.Dispose();
        }
    }
}