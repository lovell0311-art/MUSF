using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using CustomFrameWork;

namespace ETModel
{

    [ObjectSystem]
    public class ThreadPoolComponentAwakeSystem : AwakeSystem<ThreadPoolComponent>
    {
        public override void Awake(ThreadPoolComponent self)
        {
            ThreadPoolComponent.Instance = self;
            ThreadPool.SetMaxThreads(Environment.ProcessorCount * 2, Environment.ProcessorCount * 2);
        }
    }

    [ObjectSystem]
    public class ThreadPoolComponentUpdateSystem : UpdateSystem<ThreadPoolComponent>
    {
        public override void Update(ThreadPoolComponent self)
        {
            while(self.ResultTasks.Count > 0)
            {
                if(self.ResultTasks.TryDequeue(out Action task))
                {
                    task.Invoke();
                }
            }
        }
    }


    [ObjectSystem]
    public class ThreadPoolComponentDestroySystem :DestroySystem<ThreadPoolComponent>
    {
        public override void Destroy(ThreadPoolComponent self)
        {
            ThreadPoolComponent.Instance = null;

            // 停止线程池

            while (self.ResultTasks.Count > 0)
            {
                if (self.ResultTasks.TryDequeue(out Action task))
                {
                    task.Invoke();
                }
            }
        }
    }


    public class ThreadPoolComponent : Entity
    {
        public static ThreadPoolComponent Instance = null;

        public readonly ConcurrentQueue<Action> ResultTasks = new ConcurrentQueue<Action>();

        public Task<T> WaitTaskAsync<T>(Func<T> task)
        {
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            ThreadPool.QueueUserWorkItem(p =>
            {
                T result = default;
                try
                {
                    result = task.Invoke();
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
                finally
                {
                    ResultTasks.Enqueue(() =>
                    {
                        tcs.SetResult(result);
                    });
                }
            });

            return tcs.Task;
        }


    }
}
