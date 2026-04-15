
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CustomFrameWork.Baseic;

namespace CustomFrameWork.Component
{
    public partial class FrameActionComponent
    {
        public enum E_FrameSyncTimerResult
        {
            Dispose,

            NextRound,

            AutoNextRound,
        }
        public class Inner_FrameSyncTimer : ADataContext
        {
            public bool IsRun { get; set; }
            public long InstanceId { get; set; }

            public long SyncWaitTime { get; set; }
            public long NextWaitTimeTick { get; set; }
            public ADataContextSource CacheObject { get; set; }
            public Func<long, Inner_FrameSyncTimer, FrameActionComponent, E_FrameSyncTimerResult> SyncAction { get; set; }
            public Action<Inner_FrameSyncTimer> DisposeAction { get; set; }
            public CustomComponent UpdateComponent { get; set; }
            public override void ContextAwake()
            {
                IsRun = true;
            }
            public override void Dispose()
            {
                if (IsDisposeable) return;

                CacheObject = null;
                DisposeAction = null;
                SyncAction = null;
                SyncWaitTime = default;
                NextWaitTimeTick = default;

                InstanceId = default;
                IsRun = false;
                base.Dispose();
            }
        }
    }
}
