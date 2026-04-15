
using System;
using System.Collections.Generic;
using CustomFrameWork.Baseic;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace CustomFrameWork.Component
{
    /// <summary>
    /// 异步锁组件 全局不唯一 在相关处add即可 注意保证与其他任务锁不冲突
    /// </summary>
    public class AsyncLockComponent : TCustomComponent<CustomComponent, int>
    {
        public class C_AsyncLockSource : ADataContext<int>
        {
            /// <summary>
            /// 超时时间
            /// </summary>
            public int OverTime { get; private set; }
            public TaskCompletionSource<bool> Source { get; private set; }
            public override void ContextAwake(int b_args)
            {
                OverTime = b_args;
                Source = new TaskCompletionSource<bool>();
            }
            public void SetResult(bool b_TaskResult, bool b_Dispose = true)
            {
                if (IsDisposeable) return;

                Source.SetResult(b_TaskResult);
                Source = null;
                if (b_Dispose) Dispose();
            }
            public override void Dispose()
            {
                if (IsDisposeable) return;
         
                OverTime = 0;
                if (Source != null)
                {
                    Source.SetResult(false);
                    Source = null;
                }
                base.Dispose();
            }
        }

        /// <summary>
        /// 任务队列对象 自定义异步锁
        /// </summary>
        public class C_AsyncLock : ADataContext
        {
            /// <summary>
            /// 是不是工作中
            /// </summary>
            public bool IsWorking { get; set; } = false;
            /// <summary>
            /// 正在处理的任务位置
            /// </summary>
            public long WorkingPosition { get; private set; } = 0;
            /// <summary>
            /// 待处理的队列任务
            /// </summary>
            private readonly Queue<C_AsyncLockSource> SourceQueue = new Queue<C_AsyncLockSource>();
            /// <summary>
            /// 放入一个待处理的任务
            /// </summary>
            /// <param name="b_Source"></param>
            public void Enqueue(C_AsyncLockSource b_Source)
            {
                SourceQueue.Enqueue(b_Source);
            }
            /// <summary>
            /// 拿出一个任务
            /// </summary>
            public void Dequeue(long b_Key)
            {
                if (b_Key != 0 && WorkingPosition != b_Key) { return; }

                if (SourceQueue.Count == 0)
                {
                    IsWorking = false;
                    WorkingPosition = 0;
                    return;
                }
                C_AsyncLockSource C_ASource = SourceQueue.Dequeue();
                if (C_ASource != null)
                {
                    if (b_Key != 0)
                    {
                        if (IsWorking)
                            WorkingOverTimeAsync(C_ASource.OverTime);

                        C_ASource.SetResult(true);
                    }
                    else
                    {
                        C_ASource.SetResult(false);
                    }
                }
                else if (b_Key != 0)
                {
                    Dequeue(b_Key);
                }
            }
            /// <summary>
            /// 工作超时检测
            /// </summary>
            public async void WorkingOverTimeAsync(int b_OverTime)
            {
                WorkingPosition = Help_UniqueValueHelper.GetUniqueValue();
                if (b_OverTime > 0)
                {
                    long mWorkingPositionCopy = WorkingPosition;
                    if (IsWorking)
                        await Root.MainFactory.GetCustomComponent<TimerComponent>().WaitAsync(b_OverTime);
                    if (IsWorking && mWorkingPositionCopy == WorkingPosition)
                    {
                        LogToolComponent.Warning($"async overtime ! 超时时间大于:{b_OverTime}毫秒...");
                        Dequeue(mWorkingPositionCopy);
                    }
                }
            }

            /// <summary>
            /// 清理队列
            /// </summary>
            public override void Dispose()
            {
                if (IsDisposeable) return;

                IsWorking = false;
                WorkingPosition = 0;
                while (SourceQueue.Count != 0)
                {
                    Dequeue(0);
                }
                base.Dispose();
            }
        }
        /// <summary>
        /// 获取父节点
        /// </summary>
        /// <typeparam name="K">父节点类型</typeparam>
        /// <returns>父节点</returns>
        public K GetParent<K>() where K : CustomComponent
        {
            return base.Parent as K;
        }
        /// <summary>
        /// 超时时间
        /// </summary>
        private int mOverTime = 0;
        /// <summary>
        /// 对key加锁
        /// </summary>
        private readonly Dictionary<int, C_AsyncLock> mSourceIDLockDic = new Dictionary<int, C_AsyncLock>();
        /// <summary>
        /// 对key加锁
        /// </summary>
        private readonly Dictionary<object, C_AsyncLock> mSourceObjectLockDic = new Dictionary<object, C_AsyncLock>();
        private readonly object idLock = new object();
        private readonly object objLock = new object();

        /// <summary>
        /// 清理锁队列
        /// </summary>
        public override void Awake(int b_OverTime)
        {
            mSourceIDLockDic.Clear();
            mSourceObjectLockDic.Clear();
            mOverTime = b_OverTime;
        }
        /// <summary>
        /// 添加一个任务锁 不要锁-1 已占用
        /// </summary>
        /// <param name="b_TaskLockID">任务锁ID</param>
        /// <returns>任务</returns>
        public async Task<C_AsyncLock> AddTaskLock(int b_TaskLockID)
        {
            C_AsyncLock mSourceLockQueue;
            //lock (idLock)
            {
                if (mSourceIDLockDic.TryGetValue(b_TaskLockID, out mSourceLockQueue) == false)
                {
                    mSourceIDLockDic[b_TaskLockID] = mSourceLockQueue = Root.CreateBuilder.GetInstance<C_AsyncLock>();
                    mSourceLockQueue.IsWorking = true;
                    mSourceLockQueue.WorkingOverTimeAsync(mOverTime);
                    return mSourceLockQueue;
                }
            }

            if (mSourceLockQueue.IsWorking == false)
            {
                mSourceLockQueue.IsWorking = true;
                mSourceLockQueue.WorkingOverTimeAsync(mOverTime);
                return mSourceLockQueue;
            }

            C_AsyncLockSource C_ASource = Root.CreateBuilder.GetInstance<C_AsyncLockSource, int>(mOverTime);
            mSourceLockQueue.Enqueue(C_ASource);
            await C_ASource.Source.Task;
            return mSourceLockQueue;
        }
        /// <summary>
        /// 添加一个任务锁
        /// </summary>
        /// <param name="b_TaskLockID"></param>
        /// <param name="b_OverTime"></param>
        /// <returns></returns>
        public async Task<C_AsyncLock> AddTaskLock(int b_TaskLockID, int b_OverTime)
        {
            C_AsyncLock mSourceLockQueue;
            //lock (idLock)
            {
                if (mSourceIDLockDic.TryGetValue(b_TaskLockID, out mSourceLockQueue) == false)
                {
                    mSourceIDLockDic[b_TaskLockID] = mSourceLockQueue = Root.CreateBuilder.GetInstance<C_AsyncLock>();
                    mSourceLockQueue.IsWorking = true;
                    mSourceLockQueue.WorkingOverTimeAsync(b_OverTime);
                    return mSourceLockQueue;
                }
            }

            if (mSourceLockQueue.IsWorking == false)
            {
                mSourceLockQueue.IsWorking = true;
                mSourceLockQueue.WorkingOverTimeAsync(b_OverTime);
                return mSourceLockQueue;
            }

            C_AsyncLockSource C_ASource = Root.CreateBuilder.GetInstance<C_AsyncLockSource, int>(b_OverTime);
            mSourceLockQueue.Enqueue(C_ASource);
            await C_ASource.Source.Task;
            return mSourceLockQueue;
        }



        /// <summary>
        /// 添加一个任务锁
        /// </summary>
        /// <param name="b_TaskLockObject">任务锁对象</param>
        /// <returns>任务</returns>
        public async Task<C_AsyncLock> AddTaskLock(object b_TaskLockObject)
        {
            C_AsyncLock mSourceLockQueue;
            //lock (objLock)
            {
                if (mSourceObjectLockDic.TryGetValue(b_TaskLockObject, out mSourceLockQueue) == false)
                {
                    mSourceObjectLockDic[b_TaskLockObject] = mSourceLockQueue = Root.CreateBuilder.GetInstance<C_AsyncLock>();
                    mSourceLockQueue.IsWorking = true;
                    mSourceLockQueue.WorkingOverTimeAsync(mOverTime);
                    return mSourceLockQueue;
                }
            }

            if (mSourceLockQueue.IsWorking == false)
            {
                mSourceLockQueue.IsWorking = true;
                mSourceLockQueue.WorkingOverTimeAsync(mOverTime);
                return mSourceLockQueue;
            }

            C_AsyncLockSource C_ASource = Root.CreateBuilder.GetInstance<C_AsyncLockSource, int>(mOverTime);
            mSourceLockQueue.Enqueue(C_ASource);
            await C_ASource.Source.Task;
            return mSourceLockQueue;
        }
        /// <summary>
        /// 添加一个任务锁
        /// </summary>
        /// <param name="b_TaskLockObject">任务锁对象</param>
        /// <param name="b_OverTime"></param>
        /// <returns>任务</returns>
        public async Task<C_AsyncLock> AddTaskLock(object b_TaskLockObject, int b_OverTime)
        {
            C_AsyncLock mSourceLockQueue;
            //lock (objLock)
            {
                if (mSourceObjectLockDic.TryGetValue(b_TaskLockObject, out mSourceLockQueue) == false)
                {
                    mSourceObjectLockDic[b_TaskLockObject] = mSourceLockQueue = Root.CreateBuilder.GetInstance<C_AsyncLock>();
                    mSourceLockQueue.IsWorking = true;
                    mSourceLockQueue.WorkingOverTimeAsync(b_OverTime);
                    return mSourceLockQueue;
                }
            }

            if (mSourceLockQueue.IsWorking == false)
            {
                mSourceLockQueue.IsWorking = true;
                mSourceLockQueue.WorkingOverTimeAsync(b_OverTime);
                return mSourceLockQueue;
            }

            C_AsyncLockSource C_ASource = Root.CreateBuilder.GetInstance<C_AsyncLockSource, int>(b_OverTime);
            mSourceLockQueue.Enqueue(C_ASource);
            await C_ASource.Source.Task;
            return mSourceLockQueue;
        }
        /// <summary>
        /// 移除一个任务锁 不要锁-1 已占用
        /// </summary>
        /// <param name="b_TaskLockID">任务锁ID</param>
        public async void RemoveTaskLock(int b_TaskLockID)
        {
            C_AsyncLock mLock = await AddTaskLock(-1);
            long mWorkingPositionCopy = mLock.WorkingPosition;
            if (IsDisposeable) return;
            if (mSourceIDLockDic.TryGetValue(b_TaskLockID, out C_AsyncLock mSourceLockQueue))
            {
                mSourceIDLockDic.Remove(b_TaskLockID);
                mSourceLockQueue.Dispose();
            }
            mLock.Dequeue(mWorkingPositionCopy);
        }
        /// <summary>
        /// 移除一个任务锁
        /// </summary>
        /// <param name="b_TaskLockObject">任务锁对象</param>
        public async void RemoveTaskLock(object b_TaskLockObject)
        {
            C_AsyncLock mLock = await AddTaskLock(-1);
            long mWorkingPositionCopy = mLock.WorkingPosition;
            if (IsDisposeable) return;
            if (mSourceObjectLockDic.TryGetValue(b_TaskLockObject, out C_AsyncLock mSourceLockQueue))
            {
                mSourceObjectLockDic.Remove(b_TaskLockObject);
                mSourceLockQueue.Dispose();
            }
            mLock.Dequeue(mWorkingPositionCopy);
        }

        /// <summary>
        /// 清理锁队列
        /// </summary>
        public override void Dispose()
        {
            if (IsDisposeable) return;
            {
                C_AsyncLock[] c_AsyncLocks = mSourceIDLockDic.Values.ToArray();
                for (int i = 0, len = c_AsyncLocks.Length; i < len; i++)
                    c_AsyncLocks[i].Dispose();
                mSourceIDLockDic.Clear();
            }
            {
                C_AsyncLock[] c_AsyncLocks = mSourceObjectLockDic.Values.ToArray();
                for (int i = 0, len = c_AsyncLocks.Length; i < len; i++)
                    c_AsyncLocks[i].Dispose();
                mSourceObjectLockDic.Clear();
            }
            base.Dispose();
        }
    }
}
