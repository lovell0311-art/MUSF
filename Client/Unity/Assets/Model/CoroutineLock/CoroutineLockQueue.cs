using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ETModel
{
    [ObjectSystem]
    public class CoroutineLockQueueAwakeSystem: AwakeSystem<CoroutineLockQueue>
    {
        public override void Awake(CoroutineLockQueue self)
        {
            self.queue.Clear();
        }
    }

    [ObjectSystem]
    public class CoroutineLockQueueDestroySystem: DestroySystem<CoroutineLockQueue>
    {
        public override void Destroy(CoroutineLockQueue self)
        {
            self.queue.Clear();
        }
    }

    public struct CoroutineLockInfo
    {
        public TaskCompletionSource<CoroutineLock> Tcs;
        public int Time;
    }
    
    public class CoroutineLockQueue: Entity
    {
        public Queue<CoroutineLockInfo> queue = new Queue<CoroutineLockInfo>();

        public void Add(TaskCompletionSource<CoroutineLock> tcs, int time)
        {
            this.queue.Enqueue(new CoroutineLockInfo(){Tcs = tcs, Time = time});
        }

        public int Count
        {
            get
            {
                return this.queue.Count;
            }
        }

        public CoroutineLockInfo Dequeue()
        {
            return this.queue.Dequeue();
        }
    }
}