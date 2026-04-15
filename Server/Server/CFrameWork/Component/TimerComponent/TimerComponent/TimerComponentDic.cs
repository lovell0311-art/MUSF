
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
//    [EventMethod(typeof(TimerComponent), EventSystemType.UPDATE)]
//    public partial class TimerComponent
//    {
//        public override void Update()
//        {
//            if (_WaitSyncActions.Count == 0) return;

//            long timeNow = Help_TimeHelper.GetNow();

//            if (timeNow < _MinTimeValue) return;

//            RemoveTimer(timeNow);
//            _MinTimeValue = GetTimerMinKey();
//        }
//    }



//    public partial class TimerComponent : TCustomComponent<MainFactory>
//    {
//        public override void Awake()
//        {
//            TimerClear();
//            OnEnable();
//        }
//        public override void Dispose()
//        {
//            if (IsDisposeable) return;

//            TimerClear();

//            base.Dispose();
//        }
//        private void TimerClear()
//        {
//            if (_WaitSyncActions.Count > 0)
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
//        }

//        private readonly object objLock = new object();
//        private readonly SortedDictionary<long, List<InnerC_Timer>> _WaitSyncActions = new SortedDictionary<long, List<InnerC_Timer>>();
//        private long _MinTimeValue = long.MaxValue;

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
//        private void RemoveTimer(long b_TimeValue)
//        {
//            //lock (objLock)
//            {
//                var mWaitKeylist = _WaitSyncActions.Keys.ToList();
//                for (int i = 0, len = mWaitKeylist.Count; i < len; i++)
//                {
//                    long mWaitKey = mWaitKeylist[i];

//                    if (b_TimeValue < mWaitKey)
//                    {
//                        break;
//                    }

//                    if (_WaitSyncActions.TryGetValue(mWaitKey, out var mWaitValue))
//                    {
//                        if (mWaitValue.Count > 0)
//                        {
//                            for (int j = 0, zlen = mWaitValue.Count; j < zlen; j++)
//                            {
//                                var timer = mWaitValue[j];
//                                timer.tcs.SetResult(true);

//                                timer.Dispose();
//                            }
//                            mWaitValue.Clear();
//                        }

//                        _WaitSyncActions.Remove(mWaitKey);
//                    }
//                }
//            }
//        }

//    }
//    public partial class TimerComponent
//    {
//        /// <summary>
//        /// 等待时间后进行返回操作  lym-2020-0225-1027-1
//        /// 如果返回false 说明有问题 可跳过当下逻辑  true 为正常到时返回
//        /// </summary>
//        /// <param name="b_TimeValue">等待时间</param>
//        /// <returns>任务</returns>
//        public async Task<bool> WaitAsync(long b_TimeValue)
//        {
//            if (b_TimeValue <= 0) return false;

//            long timeKey = Help_TimeHelper.GetNow() + b_TimeValue;
//            TaskCompletionSource<bool> source = new TaskCompletionSource<bool>();
//            List<InnerC_Timer> taskList = default;
//            //lock (objLock)
//            {
//                if (_WaitSyncActions.TryGetValue(timeKey, out taskList) == false)
//                    _WaitSyncActions[timeKey] = taskList = new List<InnerC_Timer>();
//            }

//            InnerC_Timer mTimer = Root.CreateBuilder.GetInstance<InnerC_Timer>();
//            mTimer.tcs = source;
//            mTimer.NextWaitTimeTick = timeKey;
//            taskList.Add(mTimer);

//            if (_MinTimeValue > timeKey)
//            {
//                _MinTimeValue = timeKey;
//            }

//            return await source.Task;
//        }
//        public async Task<bool> WaitAsync(long b_TimeValue, CancellationToken b_CancelToken)
//        {
//            if (b_TimeValue <= 0) return false;

//            long timeKey = Help_TimeHelper.GetNow() + b_TimeValue;
//            TaskCompletionSource<bool> source = new TaskCompletionSource<bool>();
//            List<InnerC_Timer> taskList = default;
//            //lock (objLock)
//            {
//                if (_WaitSyncActions.TryGetValue(timeKey, out taskList) == false)
//                    _WaitSyncActions[timeKey] = taskList = new List<InnerC_Timer>();
//            }

//            InnerC_Timer mTimer = Root.CreateBuilder.GetInstance<InnerC_Timer>();
//            mTimer.tcs = source;
//            mTimer.NextWaitTimeTick = timeKey;
//            taskList.Add(mTimer);

//            if (_MinTimeValue > timeKey)
//            {
//                _MinTimeValue = timeKey;
//            }

//            b_CancelToken.Register(() =>
//            {
//                // timer没有销毁 时间戳也对应
//                if (mTimer.IsDisposeable == false && timeKey == mTimer.NextWaitTimeTick)
//                {
//                    if (_WaitSyncActions.TryGetValue(timeKey, out taskList))
//                    {
//                        if (taskList.Count > 0 && taskList.Contains(mTimer))
//                        {
//                            taskList.Remove(mTimer);

//                            mTimer.tcs.SetResult(false);
//                            mTimer.Dispose();
//                        }
//                    }
//                }
//            });

//            return await source.Task;
//        }
//    }
//}
