
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
        public partial class InnerC_TimerLink : ADataContext<InnerC_TimerLinkHelper>
        {
            public long InstanceId { get; private set; }
            public long Time { get; set; }
            public List<InnerC_Timer> FrameSyncTimerlist { get; set; }

            public InnerC_TimerLinkHelper Parent { get; set; }
            public override void ContextAwake(InnerC_TimerLinkHelper b_args)
            {
                Parent = b_args;
                Parent.Count++;
                FrameSyncTimerlist = new List<InnerC_Timer>();
                InstanceId = Help_UniqueValueHelper.GetUniqueValue();
                isIndexNode = false;
            }
            public override void Dispose()
            {
                if (IsDisposeable) return;

                if (FrameSyncTimerlist != null)
                {
                    if (FrameSyncTimerlist.Count > 0)
                    {
                        for (int i = 0, len = FrameSyncTimerlist.Count; i < len; i++)
                        {
                            FrameSyncTimerlist[i].Dispose();
                        }
                        FrameSyncTimerlist.Clear();
                    }
                    FrameSyncTimerlist = null;
                }
                NodeLast = null;
                NodeNext = null;
                isIndexNode = false;
                IndexNodeLast = null;
                IndexNodeNext = null;

                InstanceId = default;
                Time = default;
                Parent.Count--;
                Parent = null;
                base.Dispose();
            }
        }
        public partial class InnerC_TimerLink
        {
            public InnerC_TimerLink NodeLast { get; set; }
            public InnerC_TimerLink NodeNext { get; set; }

            /// <summary>
            /// 是不是索引节点
            /// </summary>
            private bool isIndexNode;
            public InnerC_TimerLink IndexNodeLast { get; set; }
            public InnerC_TimerLink IndexNodeNext { get; set; }

            public void SetIndexNode()
            {
                isIndexNode = true;
            }
            public void CancelIndex()
            {
                isIndexNode = false;
            }
            public bool IsIndexNode => isIndexNode;
        }
    }
}
