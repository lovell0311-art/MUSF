
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CustomFrameWork.Baseic;

namespace CustomFrameWork.Component
{
    public partial class TimerComponent
    {
        public enum E_FrameSyncTimerResult
        {
            Dispose,

            NextRound,

            AutoNextRound,
        }
        public class InnerC_Timer : ADataContext
        {
            public long NextWaitTimeTick { get; set; }
            public TaskCompletionSource<bool> tcs { get; set; }

            public override void Dispose()
            {
                if (IsDisposeable) return;

                NextWaitTimeTick = default;
                if (tcs != null)
                {
                    tcs = null;
                }
                base.Dispose();
            }
        }
    }
}
