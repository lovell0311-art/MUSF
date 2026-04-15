using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CustomFrameWork;


namespace ETModel
{
    [UnsafeAsync]
    public static class TaskHelper
    {
        private class CoroutineBlocker<T>
        {
            private int count;

            private TaskCompletionSource<bool> tcs;

            private Exception exception;

            public List<T> resultList;

            public CoroutineBlocker(int count)
            {
                this.count = count;
                this.exception = null;
                this.resultList = new List<T>(count);
                for(int i=0;i<this.count;i++)
                {
                    this.resultList.Add(default(T));
                }
            }

            public async Task RunSubCoroutineAsync(int index,Task<T> task)
            {
                try
                {
                    this.resultList[index] = await task;
                }
                catch (Exception e)
                {
                    if (this.count <= 0 && this.tcs != null)
                    {
                        TaskCompletionSource<bool> t = this.tcs;
                        this.tcs = null;
                        t.SetException(e);
                    }
                    else
                    {
                        exception = e;
                    }
                }
                finally
                {
                    --this.count;

                    if (this.count <= 0 && this.tcs != null)
                    {
                        TaskCompletionSource<bool> t = this.tcs;
                        this.tcs = null;
                        if(this.exception == null)
                        {
                            t.SetResult(true);
                        }
                        else
                        {
                            t.SetException(this.exception);
                        }
                    }
                }
            }

            public async Task<bool> WaitAsync()
            {
                if (this.count <= 0)
                {
                    if (this.exception != null)
                    {
                        throw exception;
                    }
                    return true;
                }
                this.tcs = new TaskCompletionSource<bool>();
                return await tcs.Task;
            }


        }


        public static async Task<List<T>> WaitAll<T>(Task<T>[] tasks)
        {
            if (tasks.Length == 0)
            {
                return new List<T>();
            }

            CoroutineBlocker<T> coroutineBlocker = new CoroutineBlocker<T>(tasks.Length);

            for (int i = 0; i < tasks.Length; i++)
            {
                coroutineBlocker.RunSubCoroutineAsync(i, tasks[i]).Coroutine();
            }

            await coroutineBlocker.WaitAsync();
            return coroutineBlocker.resultList;
        }

        public static async Task<List<T>> WaitAll<T>(List<Task<T>> tasks)
        {
            if (tasks.Count == 0)
            {
                return new List<T>();
            }

            CoroutineBlocker<T> coroutineBlocker = new CoroutineBlocker<T>(tasks.Count);

            for (int i = 0; i < tasks.Count; i++)
            {
                coroutineBlocker.RunSubCoroutineAsync(i, tasks[i]).Coroutine();
            }

            await coroutineBlocker.WaitAsync();
            return coroutineBlocker.resultList;
        }

        /// <summary>
        /// 开启新协程
        /// <para>由于此调用不会等待，因此在调用完成前将继续执行当前方法</para>
        /// </summary>
        public static async void Coroutine(this Task self)
        {
            try
            {
                await self;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        /// <summary>
        /// 开启新协程
        /// <para>由于此调用不会等待，因此在调用完成前将继续执行当前方法</para>
        /// </summary>
        public static async void Coroutine<T>(this Task<T> self)
        {
            try
            {
                await self;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
