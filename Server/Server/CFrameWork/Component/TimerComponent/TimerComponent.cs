
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Xml.Linq;
//using CustomFrameWork.Baseic;


//namespace CustomFrameWork.Component
//{
//    /// <summary>
//    /// 时间组件
//    /// </summary>
//    [EventMethod(typeof(TimerComponent), EventSystemType.UPDATE)]
//    public class TimerComponent : TCustomComponent<MainFactory>
//    {
//        public class C_Timer : ADataContext
//        {
//            public long Time { get; set; }
//            public TaskCompletionSource<bool> tcs { get; set; }

//            public override void Dispose()
//            {
//                if (IsDisposeable) return;

//                Time = default;
//                if (tcs != null)
//                {
//                    tcs = null;
//                }
//                base.Dispose();
//            }
//        }
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
//            if (mKeyValuePairs.Count > 0)
//            {
//                var mTemps = mKeyValuePairs.Values.ToArray();
//                for (int i = 0, len = mTemps.Length; i < len; i++)
//                {
//                    var mTemps2 = mTemps[i];
//                    for (int j = 0, jlen = mTemps2.Count; j < jlen; j++)
//                    {
//                        mTemps2[j].Dispose();
//                    }
//                }
//                mKeyValuePairs.Clear();
//            }
//        }

//        /// <summary>
//        /// 等待时间后进行返回操作  lym-2020-0225-1027-1
//        /// 如果返回false 说明有问题 可跳过当下逻辑  true 为正常到时返回
//        /// </summary>
//        /// <param name="b_TimeValue">等待时间</param>
//        /// <param name="b_TagID">别名</param>
//        /// <returns>任务</returns>
//        public async Task<bool> WaitAsync(long b_TimeValue)
//        {
//            if (b_TimeValue <= 0) return false;
//            long timeKey = Help_TimeHelper.GetNow() + b_TimeValue;
//            TaskCompletionSource<bool> source = new TaskCompletionSource<bool>();
//            List<C_Timer> taskList = default;
//            //lock (objLock)
//            {
//                if (mKeyValuePairs.TryGetValue(timeKey, out taskList) == false)
//                    mKeyValuePairs[timeKey] = taskList = new List<C_Timer>();
//            }

//            C_Timer mTimer = Root.CreateBuilder.GetInstance<C_Timer>();
//            mTimer.tcs = source;
//            mTimer.Time = timeKey;
//            taskList.Add(mTimer);

//            if (mMinTimeValue > timeKey)
//            {
//                mMinTimeValue = timeKey;
//            }

//            return await source.Task;
//        }
//        public async Task<bool> WaitAsync(long b_TimeValue, CancellationToken b_CancelToken)
//        {
//            if (b_TimeValue <= 0) return false;
//            long timeKey = Help_TimeHelper.GetNow() + b_TimeValue;
//            TaskCompletionSource<bool> source = new TaskCompletionSource<bool>();
//            List<C_Timer> taskList = default;
//            //lock (objLock)
//            {
//                if (mKeyValuePairs.TryGetValue(timeKey, out taskList) == false)
//                    mKeyValuePairs[timeKey] = taskList = new List<C_Timer>();
//            }

//            C_Timer mTimer = Root.CreateBuilder.GetInstance<C_Timer>();
//            mTimer.tcs = source;
//            mTimer.Time = timeKey;
//            taskList.Add(mTimer);

//            if (mMinTimeValue > timeKey)
//            {
//                mMinTimeValue = timeKey;
//            }

//            b_CancelToken.Register(() =>
//            {
//                // timer没有销毁 时间戳也对应
//                if (mTimer.IsDisposeable == false && timeKey == mTimer.Time)
//                {
//                    if (mKeyValuePairs.TryGetValue(timeKey, out taskList))
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



//        private readonly object objLock = new object();
//        private readonly SortedDictionary<long, List<C_Timer>> mKeyValuePairs = new SortedDictionary<long, List<C_Timer>>();
//        private long mMinTimeValue = long.MaxValue;



//        private long GetTimerMinKey()
//        {
//            long mResult = long.MaxValue;

//            var mWaitSyncKeylist = mKeyValuePairs.Keys.ToList();
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
//                var mWaitKeylist = mKeyValuePairs.Keys.ToList();
//                for (int i = 0, len = mWaitKeylist.Count; i < len; i++)
//                {
//                    long mWaitKey = mWaitKeylist[i];

//                    if (b_TimeValue < mWaitKey)
//                    {
//                        break;
//                    }

//                    if (mKeyValuePairs.TryGetValue(mWaitKey, out var mWaitValue))
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

//                        mKeyValuePairs.Remove(mWaitKey);
//                    }
//                }
//            }
//        }

//        public override void Update()
//        {
//            if (mKeyValuePairs.Count == 0) return;

//            long timeNow = Help_TimeHelper.GetNow();

//            if (timeNow < mMinTimeValue) return;

//            RemoveTimer(timeNow);
//            mMinTimeValue = GetTimerMinKey();
//        }

//        public Task<bool> WaitAsyncSetTime(long time, long timeNow = 0)
//        {
//            long currentTime = 0;
//            if (timeNow > 0) currentTime = timeNow;
//            else currentTime = Help_TimeHelper.GetCurrenTimeStamp();

//            long waitTime = (time - currentTime) / 10000;
//            return WaitAsync(waitTime);
//        }

//    }
//}
