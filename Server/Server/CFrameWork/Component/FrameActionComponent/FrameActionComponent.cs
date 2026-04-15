
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CustomFrameWork.Baseic;

namespace CustomFrameWork.Component
{
    /// <summary>
    /// 时间组件
    /// </summary>
    [EventMethod(typeof(FrameActionComponent), EventSystemType.UPDATE)]
    public partial class FrameActionComponent
    {
        public override void Update()
        {
            if (_NodeLink.NodeFirst == null) return;

            FrameUpdateTick = Help_TimeHelper.GetNow();

            if (FrameUpdateTick < _NodeLink.NodeFirst.Time) return;

            while (_NodeLink.NodeFirst != null && _NodeLink.NodeFirst.Time <= FrameUpdateTick)
            {
                var mFrameSyncTimerlist = _NodeLink.NodeFirst.FrameSyncTimerlist;
                if (mFrameSyncTimerlist != null && mFrameSyncTimerlist.Count > 0)
                {
                    RunFramelist(mFrameSyncTimerlist);
                    mFrameSyncTimerlist.Clear();
                }

                var mOldNode = _NodeLink.NodeFirst;
                var mNewNode = mOldNode.NodeNext;
                if (mNewNode != null)
                {
                    mNewNode.IndexNodeLast = null;
                    mNewNode.NodeLast = null;

                    if (mNewNode.IsIndexNode == false)
                    {
                        _NodeLink.NodeFirst.SetIndexNode();
                        var mOldIndexNodeNext = mOldNode.IndexNodeNext;
                        if (mOldIndexNodeNext != null)
                        {
                            mNewNode.IndexNodeNext = mOldIndexNodeNext;
                            mOldIndexNodeNext.IndexNodeLast = mNewNode;
                        }
                    }
                }
                mOldNode.NodeNext = null;
                mOldNode.Dispose();

                _NodeLink.NodeFirst = mNewNode;
            }
            if (_NodeLink.NodeFirst == null)
            {
                _NodeLink.NodeEnd = null;
            }
            else if (_NodeLink.NodeEnd != null && _NodeLink.NodeFirst.InstanceId == _NodeLink.NodeEnd.InstanceId)
            {
                _NodeLink.NodeEnd = null;
            }

            #region RunFramelist()
            void RunFramelist(List<Inner_FrameSyncTimer> b_FrameSyncTimerlist)
            {
                for (int i = 0, len = b_FrameSyncTimerlist.Count; i < len; i++)
                {
                    var mFrameSyncTimer = b_FrameSyncTimerlist[i];

                    if (mFrameSyncTimer == null) continue;
                    if (mFrameSyncTimer.IsDisposeable) continue;

                    if (mFrameSyncTimer.UpdateComponent != null)
                    {// 依赖的组件停止运行
                        if (mFrameSyncTimer.UpdateComponent.IsRunUpdate == false) continue;
                    }

                    if (mFrameSyncTimer.IsRun == false)
                    {
                        mFrameSyncTimer.Dispose();
                        continue;
                    }

                    Run(mFrameSyncTimer);
                }
            }
            #endregion
        }
    }


    public partial class FrameActionComponent : TCustomComponent<MainFactory>
    {
        InnerC_TimerLinkHelper _NodeLink;
        public override void Awake()
        {
            _NodeLink = Root.CreateBuilder.GetInstance<InnerC_TimerLinkHelper>();
            OnEnable();
        }
        public override void Dispose()
        {
            if (IsDisposeable) return;

            if (_NodeLink != null)
            {
                _NodeLink.Dispose();
                _NodeLink = null;
            }
            base.Dispose();
        }

        private long FrameUpdateTick;


        private void Run(Inner_FrameSyncTimer b_FrameSyncTimer)
        {
            var mRunResult = b_FrameSyncTimer.SyncAction.Invoke(FrameUpdateTick, b_FrameSyncTimer, this);

            switch (mRunResult)
            {
                case E_FrameSyncTimerResult.Dispose:
                    {
                        TaskDispose(b_FrameSyncTimer);
                    }
                    break;
                case E_FrameSyncTimerResult.NextRound:
                    break;
                case E_FrameSyncTimerResult.AutoNextRound:
                    {
                        if (b_FrameSyncTimer.SyncWaitTime == 0)
                        {
                            TaskDispose(b_FrameSyncTimer);
                            break;
                        }

                        TaskNextTime(b_FrameSyncTimer);

                        if (b_FrameSyncTimer.NextWaitTimeTick <= FrameUpdateTick)
                            TaskDispose(b_FrameSyncTimer);
                        else
                            WaitAsync(b_FrameSyncTimer);
                    }
                    break;
                default:
                    break;
            }

            void TaskDispose(Inner_FrameSyncTimer b_TimerTask)
            {
                if (b_TimerTask.DisposeAction != null)
                {
                    b_TimerTask.DisposeAction.Invoke(b_TimerTask);
                    b_TimerTask.DisposeAction = null;
                }
                b_TimerTask.Dispose();
            }
            void TaskNextTime(Inner_FrameSyncTimer b_TimerTask)
            {
                b_TimerTask.NextWaitTimeTick = b_TimerTask.NextWaitTimeTick + b_TimerTask.SyncWaitTime;
                if (FrameUpdateTick > b_TimerTask.NextWaitTimeTick)
                {
                    var mTimeValue = FrameUpdateTick - b_TimerTask.NextWaitTimeTick;
                    var mTimeValueMultiple = mTimeValue / b_TimerTask.SyncWaitTime;
                    var mAddTimeValue = mTimeValueMultiple * b_TimerTask.SyncWaitTime;
                    b_TimerTask.NextWaitTimeTick = b_TimerTask.NextWaitTimeTick + mAddTimeValue + b_TimerTask.SyncWaitTime;
                }
            }
        }


        public void WaitAsync(Inner_FrameSyncTimer b_FrameSyncTimer)
        {
            if (b_FrameSyncTimer.NextWaitTimeTick <= FrameUpdateTick)
            {
                Run(b_FrameSyncTimer);
                return;
            }

            _NodeLink.WaitAsync(b_FrameSyncTimer);
        }
    }
}
