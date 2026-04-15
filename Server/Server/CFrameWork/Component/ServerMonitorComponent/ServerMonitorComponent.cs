using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomFrameWork.Baseic;


namespace CustomFrameWork.Component
{
    /// <summary>
    /// 监视器 事件类型
    /// </summary>
    public enum E_PolicyType
    {
        /// <summary>
        /// no
        /// </summary>
        NONE,
        /// <summary>
        /// 回退
        /// </summary>
        FALLBACK,
        /// <summary>
        /// 超时
        /// </summary>
        TIMEOUT,
        /// <summary>
        /// 重试
        /// </summary>
        RETRY,
        /// <summary>
        /// 断路
        /// </summary>
        CIRCUIT_BREAKER,
        /// <summary>
        /// 降级
        /// </summary>
        DOWNGRADE
    }
    /// <summary>
    /// 监视器   对象
    /// </summary>
    public class C_ServerMonitor : ADataContext<string>
    {
        /// <summary>
        /// ID
        /// </summary>
        private string mTag;
        /// <summary>
        /// 超时时间
        /// </summary>
        private int mOverTime;
        /// <summary>
        /// 重试次数
        /// </summary>
        private int mRetryCount;
        /// <summary>
        /// 第一次重试间隔时间
        /// </summary>
        private int mFirstRetryTime;
        /// <summary>
        /// 非第一次重试间隔时间
        /// </summary>
        private int[] mWaitTimeArray;
        /// <summary>
        /// 状态
        /// </summary>
        public E_PolicyType CurrentType { get; private set; }
        /// <summary>
        /// 超时判定
        /// </summary>
        public bool IsOverTimeDeteMine { get; private set; } = false;

        /// <summary>
        /// 熔断间隔时间
        /// </summary>
        private int mCircuitBreakerTime;
        private long mCircuitBreakerTargetTime;

        /// <summary>
        /// 降级间隔时间
        /// </summary>
        private int mDownGradeTime;
        private long mDownGradeTargetTime;



        private Func<E_PolicyType> mFallbackAction = null;
        private Func<E_PolicyType> mTimeOutAction = null;
        private Func<E_PolicyType> mDownGradeAction = null;
        private Func<E_PolicyType> mCircuitBreakerAction = null;

        public override void ContextAwake(string b_args)
        {
            mTag = b_args;
        }
        /// <summary>
        /// 熔断
        /// </summary>
        /// <param name="b_CircuitBreakerTime">熔断时间</param>
        /// <param name="b_CircuitBreakerAction"></param>
        /// <returns></returns>
        public C_ServerMonitor CircuitBreaker(int b_CircuitBreakerTime, Func<E_PolicyType> b_CircuitBreakerAction)
        {
            mCircuitBreakerTime = b_CircuitBreakerTime;
            mCircuitBreakerAction = b_CircuitBreakerAction;
            return this;
        }
        /// <summary>
        /// 降级
        /// </summary>
        /// <param name="b_DownGradeTime">降级时间</param>
        /// <param name="b_DownGradeAction"></param>
        /// <returns></returns>
        public C_ServerMonitor DownGrade(int b_DownGradeTime, Func<E_PolicyType> b_DownGradeAction)
        {
            mDownGradeTime = b_DownGradeTime;
            mDownGradeAction = b_DownGradeAction;
            return this;
        }
        /// <summary>
        /// 回退
        /// </summary>
        /// <param name="b_FallbackAction"></param>
        /// <returns></returns>
        public C_ServerMonitor Fallback(Func<E_PolicyType> b_FallbackAction)
        {
            mFallbackAction = b_FallbackAction;
            return this;
        }
        /// <summary>
        /// 超时
        /// </summary>
        /// <param name="b_OverTime">超时时间</param>
        /// <param name="b_TimeOutAction">超时时间</param>
        /// <returns></returns>
        public C_ServerMonitor TimeOut(int b_OverTime, Func<E_PolicyType> b_TimeOutAction)
        {
            mOverTime = b_OverTime;
            mTimeOutAction = b_TimeOutAction;
            return this;
        }
        /// <summary>
        /// 重试
        /// </summary>
        /// <param name="b_RetryCount">重试次数</param>
        /// <param name="b_FirstRetryTime">首次重试间隔时间</param>
        /// <param name="b_WaitTimeArray">非首次重试间隔时间</param>
        /// <returns></returns>
        public C_ServerMonitor Retry(int b_RetryCount, int b_FirstRetryTime, int[] b_WaitTimeArray)
        {
            mRetryCount = b_RetryCount;
            mFirstRetryTime = b_FirstRetryTime;
            mWaitTimeArray = b_WaitTimeArray;
            return this;
        }

        /// <summary>
        /// 开始监测
        /// </summary>
        /// <param name="b_PolicyTask"></param>
        public async void Execute(Func<E_PolicyType> b_PolicyTask)
        {
            if (b_PolicyTask == null) return;
            IsOverTimeDeteMine = false;
            switch (CurrentType)
            {
                case E_PolicyType.CIRCUIT_BREAKER:
                    {
                        if (Help_TimeHelper.GetNow() < mCircuitBreakerTargetTime)
                        {
                            LogToolComponent.Error($"任务监控: 任务Tag:{mTag},已熔断,熔断时间{mCircuitBreakerTime}ms", false);
                            mCircuitBreakerAction?.Invoke();
                            Clear();
                            return;
                        }
                    }
                    break;
                case E_PolicyType.DOWNGRADE:
                    {
                        if (Help_TimeHelper.GetNow() < mDownGradeTargetTime)
                        {
                            LogToolComponent.Warning($"任务监控: 任务Tag:{mTag},已降级,降级时间{mDownGradeTime}ms", false);
                            mDownGradeAction?.Invoke();
                            Clear();
                            return;
                        }
                    }
                    break;
                default:
                    break;
            }
            int mRetryIndex = 1;
            int mWaitTime = 0;

            if (mTimeOutAction != null)
            {
                async void TimeOutAsync()
                {
                    await Root.MainFactory.GetCustomComponent<TimerComponent>().WaitAsync(mOverTime);
                    if (IsDisposeable) return;

                    LogToolComponent.Warning($"任务监控: 任务Tag:{mTag},任务超时,已降级,降级时间{mDownGradeTime}ms", false);
                    IsOverTimeDeteMine = true;
                    CurrentType = E_PolicyType.DOWNGRADE;
                    mDownGradeTargetTime = Help_TimeHelper.GetNow() + mDownGradeTime;
                    mTimeOutAction?.Invoke();
                    Clear();
                }
                TimeOutAsync();
            }

        PolicyType_Start:
            E_PolicyType mE_PolicyType = b_PolicyTask.Invoke();
            if (mE_PolicyType == E_PolicyType.TIMEOUT) return;
            if (IsDisposeable) return;

            switch (mE_PolicyType)
            {
                case E_PolicyType.NONE:
                    CurrentType = E_PolicyType.NONE;
                    break;
                case E_PolicyType.FALLBACK:
                    CurrentType = E_PolicyType.DOWNGRADE;
                    mFallbackAction?.Invoke();
                    break;
                case E_PolicyType.RETRY:
                    {
                        CurrentType = E_PolicyType.DOWNGRADE;

                        if (b_PolicyTask == null) break;
                        if (IsDisposeable) return;
                        if (mE_PolicyType == E_PolicyType.RETRY)
                        {
                            if (mRetryIndex < mRetryCount)
                            {
                                if (mRetryIndex == 1)
                                {
                                    mWaitTime = mFirstRetryTime;
                                }
                                else
                                {
                                    if (mWaitTimeArray != null)
                                    {
                                        if (mRetryIndex <= (mWaitTimeArray.Length - 1))
                                        {
                                            mWaitTime = mWaitTimeArray[mRetryIndex - 1];
                                        }
                                    }
                                }
                                if (mWaitTime != 0)
                                    await Root.MainFactory.GetCustomComponent<TimerComponent>().WaitAsync(mWaitTime);
                                if (IsDisposeable) return;
                                mRetryIndex++;
                                goto PolicyType_Start;
                            }
                            else
                            {
                                LogToolComponent.Error($"任务监控: 任务Tag:{mTag},已熔断,熔断时间{mCircuitBreakerTime}ms", false);

                                mCircuitBreakerTargetTime = Help_TimeHelper.GetNow() + mCircuitBreakerTime;
                                CurrentType = E_PolicyType.CIRCUIT_BREAKER;
                                mCircuitBreakerAction?.Invoke();
                            }
                        }
                    }
                    break;
                case E_PolicyType.CIRCUIT_BREAKER:
                    LogToolComponent.Error($"任务监控: 任务Tag:{mTag},已熔断,熔断时间{mCircuitBreakerTime}ms", false);

                    CurrentType = E_PolicyType.CIRCUIT_BREAKER;
                    mCircuitBreakerTargetTime = Help_TimeHelper.GetNow() + mCircuitBreakerTime;
                    mCircuitBreakerAction?.Invoke();
                    break;
                case E_PolicyType.DOWNGRADE:
                    LogToolComponent.Warning($"任务监控: 任务Tag:{mTag},已降级,降级时间{mDownGradeTime}ms", false);

                    CurrentType = E_PolicyType.DOWNGRADE;
                    mDownGradeTargetTime = Help_TimeHelper.GetNow() + mDownGradeTime;
                    mDownGradeAction?.Invoke();
                    break;
                default:
                    break;
            }
            Clear();
        }
        /// <summary>
        /// 清理
        /// </summary>
        public void Clear()
        {
            mOverTime = 0;
            mRetryCount = 0;
            mFirstRetryTime = 0;

            mWaitTimeArray = null;
            mFallbackAction = null;
            mTimeOutAction = null;
            mDownGradeAction = null;
            mCircuitBreakerAction = null;
        }

        public override void Dispose()
        {
            if (IsDisposeable) return;

            mTag = null;
            mCircuitBreakerTime = 0;
            mCircuitBreakerTargetTime = 0;
            mDownGradeTime = 0;
            mDownGradeTargetTime = 0;
            CurrentType = E_PolicyType.NONE;
            Clear();
            base.Dispose();
        }
    }
    /// <summary>
    /// 监视器
    /// </summary>
    public class ServerMonitorComponent : TCustomComponent<MainFactory>
    {
        private Dictionary<string, C_ServerMonitor> mC_ServerMonitorDic;
        public override void Awake()
        {
            mC_ServerMonitorDic = new Dictionary<string, C_ServerMonitor>();
        }

        //public C_ServerMonitor CreateMonitor()
        //{
        //    if (IsDisposeable) return null;
        //    long mKey = Help_UniqueValueHelper.GetUniqueValue();
        //    C_ServerMonitor mServerMonitor = Root.CreateBuilder.GetInstance<C_ServerMonitor, long>(mKey);
        //    mC_ServerMonitorDic[mKey] = mServerMonitor;
        //    return mServerMonitor;
        //}
        /// <summary>
        /// 创建一个监视器 
        /// </summary>
        /// <param name="b_TaskName"></param>
        /// <returns></returns>
        public C_ServerMonitor CreateMonitor(string b_TaskName)
        {
            if (IsDisposeable) return null;
            if (mC_ServerMonitorDic != null)
            {
                if (mC_ServerMonitorDic.TryGetValue(b_TaskName, out C_ServerMonitor mResult))
                {
                    return mResult;
                }
            }
            C_ServerMonitor mServerMonitor = Root.CreateBuilder.GetInstance<C_ServerMonitor, string>(b_TaskName);
            mC_ServerMonitorDic[b_TaskName] = mServerMonitor;
            return mServerMonitor;
        }

        public void RemoveMonitor(string b_TaskName)
        {
            if (IsDisposeable) return;
            if (mC_ServerMonitorDic != null)
            {
                if (mC_ServerMonitorDic.ContainsKey(b_TaskName))
                {
                    mC_ServerMonitorDic.Remove(b_TaskName);
                }
            }
        }

        public override void Dispose()
        {
            if (IsDisposeable) return;

            if (mC_ServerMonitorDic != null)
            {
                C_ServerMonitor[] mC_ServerMonitors = mC_ServerMonitorDic.Values.ToArray();
                for (int i = 0, len = mC_ServerMonitors.Length; i < len; i++)
                {
                    C_ServerMonitor mServerMonitor = mC_ServerMonitors[i];
                    if (mServerMonitor != null)
                    {
                        mServerMonitor.Dispose();
                    }
                }
                mC_ServerMonitorDic.Clear();
                mC_ServerMonitorDic = null;
            }
            base.Dispose();
        }
    }
}

//示例
//int a = 0;
//C_ServerMonitor monitor = Root.MainFactory.AddCustomComponent<ServerMonitorComponent>().CreateMonitor("nam");
//monitor.Retry(3, 1000, null)
//.Execute(() =>
//{
//    a++;
//    Console.WriteLine($"第{a}次重试");
//    {
//        string _sd = Help_MD5EncryptionHelper.MD5EncryptionGetString("sdsdsdswwww" + a, Encoding.UTF8);
//        Console.WriteLine(_sd);


//    }
//    if (a <= 2)
//    {
//        return E_PolicyType.RETRY;
//    }
//    else
//    {
//        return E_PolicyType.NONE;
//    }
//});