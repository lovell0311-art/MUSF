
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using CustomFrameWork.Baseic;

//namespace CustomFrameWork.Component
//{
//    /// <summary>
//    /// 时间组件
//    /// </summary>
//    [EventMethod(typeof(FrameActionComponent), EventSystemType.UPDATE)]
//    public partial class FrameActionComponent
//    {
//        public override void Update()
//        {
//            if (_WaitSyncActions.Count == 0) return;

//            FrameUpdateTick = Help_TimeHelper.GetNow();
//            if (FrameUpdateTick < _MinTimeValue) return;

//            RemoveTimer();
//            _MinTimeValue = GetTimerMinKey();
//        }
//    }


//    public partial class FrameActionComponent : TCustomComponent<MainFactory>
//    {
//        public override void Awake()
//        {
//            SyncTimerClear();
//            OnEnable();
//        }
//        public override void Dispose()
//        {
//            if (IsDisposeable) return;

//            SyncTimerClear();
//            base.Dispose();
//        }

//        /// <summary>
//        /// 延时程序
//        /// </summary>
//        public readonly SortedDictionary<long, List<Inner_FrameSyncTimer>> _WaitSyncActions = new SortedDictionary<long, List<Inner_FrameSyncTimer>>();
//        private long _MinTimeValue = long.MaxValue;
//        private long FrameUpdateTick;
//        private void SyncTimerClear()
//        {
//            if (_WaitSyncActions != null && _WaitSyncActions.Count > 0)
//            {
//                var mTemps = _WaitSyncActions.Values.ToArray();
//                for (int i = 0, len = mTemps.Length; i < len; i++)
//                {
//                    var mTemps2 = mTemps[i];
//                    for (int j = 0, jlen = mTemps2.Count; j < jlen; j++)
//                    {
//                        mTemps2[j].Dispose();
//                    }
//                }
//                _WaitSyncActions.Clear();
//            }
//            _MinTimeValue = long.MaxValue;
//        }

//        private void Run(Inner_FrameSyncTimer b_FrameSyncTimer)
//        {
//            var mRunResult = b_FrameSyncTimer.SyncAction.Invoke(FrameUpdateTick, b_FrameSyncTimer, this);

//            switch (mRunResult)
//            {
//                case E_FrameSyncTimerResult.Dispose:
//                    {
//                        TaskDispose(b_FrameSyncTimer);
//                    }
//                    break;
//                case E_FrameSyncTimerResult.NextRound:
//                    break;
//                case E_FrameSyncTimerResult.AutoNextRound:
//                    {
//                        if (b_FrameSyncTimer.SyncWaitTime == 0)
//                        {
//                            TaskDispose(b_FrameSyncTimer);
//                            break;
//                        }

//                        TaskNextTime(b_FrameSyncTimer);

//                        if (b_FrameSyncTimer.NextWaitTimeTick <= FrameUpdateTick)
//                            TaskDispose(b_FrameSyncTimer);
//                        else
//                            WaitAsync(b_FrameSyncTimer);
//                    }
//                    break;
//                default:
//                    break;
//            }

//            void TaskDispose(Inner_FrameSyncTimer b_TimerTask)
//            {
//                if (b_TimerTask.DisposeAction != null)
//                {
//                    b_TimerTask.DisposeAction.Invoke(b_TimerTask);
//                    b_TimerTask.DisposeAction = null;
//                }
//                b_TimerTask.Dispose();
//            }
//            void TaskNextTime(Inner_FrameSyncTimer b_TimerTask)
//            {
//                b_TimerTask.NextWaitTimeTick = b_TimerTask.NextWaitTimeTick + b_TimerTask.SyncWaitTime;
//                if (FrameUpdateTick > b_TimerTask.NextWaitTimeTick)
//                {
//                    var mTimeValue = FrameUpdateTick - b_TimerTask.NextWaitTimeTick;
//                    var mTimeValueMultiple = mTimeValue / b_TimerTask.SyncWaitTime;
//                    var mAddTimeValue = mTimeValueMultiple * b_TimerTask.SyncWaitTime;
//                    b_TimerTask.NextWaitTimeTick = b_TimerTask.NextWaitTimeTick + mAddTimeValue + b_TimerTask.SyncWaitTime;
//                }
//            }
//        }


//        private void RemoveTimer()
//        {
//            var mWaitKeylist = _WaitSyncActions.Keys.ToList();
//            for (int i = 0, len = mWaitKeylist.Count; i < len; i++)
//            {
//                long mWaitKey = mWaitKeylist[i];

//                if (FrameUpdateTick < mWaitKey)
//                {
//                    break;
//                }
//                if (_WaitSyncActions.TryGetValue(mWaitKey, out var mWaitlist))
//                {
//                    _WaitSyncActions.Remove(mWaitKey);

//                    if (mWaitlist.Count <= 0) continue;
//                    for (int j = 0, zlen = mWaitlist.Count; j < zlen; j++)
//                    {
//                        var mFrameSyncTimer = mWaitlist[j];
//                        if (mFrameSyncTimer == null) continue;
//                        if (mFrameSyncTimer.IsDisposeable) continue;

//                        if (IsRunUpdate == false || mFrameSyncTimer.IsRun == false)
//                        {
//                            mFrameSyncTimer.Dispose();
//                            continue;
//                        }
//                        if (mFrameSyncTimer.UpdateComponent != null)
//                        {// 依赖的组件停止运行
//                            if (mFrameSyncTimer.UpdateComponent.IsRunUpdate == false) continue;
//                        }
//                        try
//                        {
//                            Run(mFrameSyncTimer);
//                        }
//                        catch (Exception e)
//                        {
//                            Log.Error(e.Message);
//                            Log.Error(e.StackTrace);
//                        }
//                    }
//                    mWaitlist.Clear();
//                }
//            }
//        }
//        private long GetTimerMinKey()
//        {
//            long mResult = long.MaxValue;

//            var mWaitSyncKeylist = _WaitSyncActions.Keys.ToList();
//            for (int i = 0, len = mWaitSyncKeylist.Count; i < len; i++)
//            {
//                var mWaitSyncKey = mWaitSyncKeylist[i];
//                if (mResult > mWaitSyncKey)
//                {
//                    mResult = mWaitSyncKey;
//                }
//            }
//            return mResult;
//        }

//        public void WaitAsync(Inner_FrameSyncTimer b_FrameSyncTimer)
//        {
//            if (b_FrameSyncTimer.NextWaitTimeTick <= FrameUpdateTick)
//            {
//                Run(b_FrameSyncTimer);
//                return;
//            }

//            var timeTick = b_FrameSyncTimer.NextWaitTimeTick;

//            if (_WaitSyncActions.TryGetValue(timeTick, out var taskList) == false)
//                _WaitSyncActions[timeTick] = taskList = new List<Inner_FrameSyncTimer>();

//            taskList.Add(b_FrameSyncTimer);

//            if (_MinTimeValue > timeTick)
//            {
//                _MinTimeValue = timeTick;
//            }
//        }
//    }
//}
