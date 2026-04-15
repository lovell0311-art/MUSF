using System;
using System.Threading.Tasks;
using ETHotfix;
using CustomFrameWork;
using CustomFrameWork.Component;


namespace ETModel
{
    public sealed partial class GatePlayer
    {
        public Session Session { get; set; }

        public long SessionInstanceId { get; set; }

        public int GameServerId { get; set; }

        public long UserId { get; set; }

        private void Clear()
        {
            Session = null;
            SessionInstanceId = 0;
            GameServerId = 0;
            UserId = 0;

        }
    }


    public sealed partial class GatePlayer : ADataContext<long>
    {
        protected override int PoolCountMax => 0;
        public long GameUserId { get; private set; }

        public override void ContextAwake(long b_Args)
        {
            GameUserId = b_Args;
        }
        public override void Dispose()
        {
            if (this.IsDisposeable)
            {
                return;
            }

            Clear();
            base.Dispose();
        }
    }
}