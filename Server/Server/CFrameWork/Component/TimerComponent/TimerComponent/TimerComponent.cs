
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
    [EventMethod(typeof(TimerComponent), EventSystemType.UPDATE)]
    public partial class TimerComponent
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

                    var mOldIndexNodeNext = mOldNode.IndexNodeNext;
                    if (mOldIndexNodeNext != null && mOldIndexNodeNext.InstanceId != mNewNode.InstanceId)
                    {
                        mNewNode.IndexNodeNext = mOldIndexNodeNext;
                        mOldIndexNodeNext.IndexNodeLast = mNewNode;
                    }
                }
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
            void RunFramelist(List<InnerC_Timer> b_FrameSyncTimerlist)
            {
                for (int i = 0, len = b_FrameSyncTimerlist.Count; i < len; i++)
                {
                    var mFrameSyncTimer = b_FrameSyncTimerlist[i];

                    if (mFrameSyncTimer == null) continue;
                    if (mFrameSyncTimer.IsDisposeable) continue;

                    Run(mFrameSyncTimer);
                }
            }
            #endregion
        }
    }



    public partial class TimerComponent : TCustomComponent<MainFactory>
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

        private void Run(InnerC_Timer b_FrameSyncTimer)
        {
            b_FrameSyncTimer.tcs.SetResult(true);
            b_FrameSyncTimer.Dispose();
        }
    }
    public partial class TimerComponent
    {
        public void WaitAsync(InnerC_Timer b_FrameSyncTimer)
        {
            if (b_FrameSyncTimer.NextWaitTimeTick <= FrameUpdateTick)
            {
                Run(b_FrameSyncTimer);
                return;
            }

            _NodeLink.WaitAsync(b_FrameSyncTimer);
        }

        /// <summary>
        /// 等待时间后进行返回操作  lym-2020-0225-1027-1
        /// 如果返回false 说明有问题 可跳过当下逻辑  true 为正常到时返回
        /// </summary>
        /// <param name="b_TimeValue">等待时间</param>
        /// <returns>任务</returns>
        public async Task<bool> WaitAsync(long b_TimeValue)
        {
            if (b_TimeValue <= 0) return false;

            long timeKey = Help_TimeHelper.GetNow() + b_TimeValue;
            TaskCompletionSource<bool> source = new TaskCompletionSource<bool>();

            InnerC_Timer mTimer = Root.CreateBuilder.GetInstance<InnerC_Timer>();
            mTimer.tcs = source;
            mTimer.NextWaitTimeTick = timeKey;

            _NodeLink.WaitAsync(mTimer);

            return await source.Task;
        }
        //public async Task<bool> WaitAsync(long b_TimeValue, CancellationToken b_CancelToken)
        //{
        //    if (b_TimeValue <= 0) return false;

        //    long timeKey = Help_TimeHelper.GetNow() + b_TimeValue;
        //    TaskCompletionSource<bool> source = new TaskCompletionSource<bool>();

        //    InnerC_Timer mTimer = Root.CreateBuilder.GetInstance<InnerC_Timer>();
        //    mTimer.tcs = source;
        //    mTimer.NextWaitTimeTick = timeKey;

        //    _NodeLink.WaitAsync(mTimer);

        //    b_CancelToken.Register(() =>
        //    {
        //                // timer没有销毁 时间戳也对应
        //        if (mTimer.IsDisposeable == false && timeKey == mTimer.NextWaitTimeTick)
        //        {
        //            if (_WaitSyncActions.TryGetValue(timeKey, out taskList))
        //            {
        //                if (taskList.Count > 0 && taskList.Contains(mTimer))
        //                {
        //                    taskList.Remove(mTimer);

        //                    mTimer.tcs.SetResult(false);
        //                    mTimer.Dispose();
        //                }
        //            }
        //        }
        //    });

        //    return await source.Task;
        //}
        public Task<bool> WaitAsyncSetTime(long time, long timeNow = 0)
        {
            long currentTime = 0;
            if (timeNow > 0) currentTime = timeNow;
            else currentTime = Help_TimeHelper.GetCurrenTimeStamp();

            long waitTime = (time - currentTime) / 10000;
            return WaitAsync(waitTime);
        }
    }
}
