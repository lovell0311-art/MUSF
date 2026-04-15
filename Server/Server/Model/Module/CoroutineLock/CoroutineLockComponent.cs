using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CustomFrameWork;

namespace ETModel
{
    [ObjectSystem]
    public class CoroutineLockComponentAwakeSystem: AwakeSystem<CoroutineLockComponent>
    {
        public override void Awake(CoroutineLockComponent self)
        {
            CoroutineLockComponent.Instance = self;
            
            self.list = new List<CoroutineLockQueueType>(CoroutineLockType.Max);
            for (int i = 0; i < CoroutineLockType.Max; ++i)
            {
                CoroutineLockQueueType coroutineLockQueueType = ComponentFactory.CreateWithId<CoroutineLockQueueType>(++self.idGenerator);
                self.list.Add(coroutineLockQueueType);
            }

        }
    }

    [ObjectSystem]
    public class CoroutineLockComponentDestroySystem: DestroySystem<CoroutineLockComponent>
    {
        public override void Destroy(CoroutineLockComponent self)
        {
            self.list.Clear();
            self.nextFrameRun.Clear();
            self.timers.Clear();
            self.timeOutIds.Clear();
            self.timerOutTimer.Clear();
            self.idGenerator = 0;
            self.minTime = 0;
        }
    }

    [ObjectSystem]
    public class CoroutineLockComponentUpdateSystem : UpdateSystem<CoroutineLockComponent>
    {
        public override void Update(CoroutineLockComponent self)
        {
            // 检测超时的CoroutineLock
            TimeoutCheck(self);
            
            // 循环过程中会有对象继续加入队列
            while(self.nextFrameRun.Count > 0)
            {
                (int coroutineLockType, long key, int count) = self.nextFrameRun.Dequeue();
                self.Notify(coroutineLockType, key, count);
            }
        }

        private void TimeoutCheck(CoroutineLockComponent self)
        {
            // 超时的锁
            if (self.timers.Count == 0)
            {
                return;
            }

            self.timeNow = Help_TimeHelper.GetNow();

            if (self.timeNow < self.minTime)
            {
                return;
            }

            foreach (var kv in self.timers.GetDictionary())
            {
                if (kv.Key > self.timeNow)
                {
                    self.minTime = kv.Key;
                    break;
                }

                self.timeOutIds.Enqueue(kv.Key);
            }

            self.timerOutTimer.Clear();
            
            while (self.timeOutIds.Count > 0)
            {
                long time = self.timeOutIds.Dequeue();
                var list = self.timers[time];
                for (int i = 0; i < list.Count; ++i)
                {
                    CoroutineLockTimer coroutineLockTimer = list[i];
                    self.timerOutTimer.Enqueue(coroutineLockTimer);
                }
                self.timers.Remove(time);
            }
            
            while (self.timerOutTimer.Count > 0)
            {
                CoroutineLockTimer coroutineLockTimer = self.timerOutTimer.Dequeue();
                if (coroutineLockTimer.CoroutineLockInstanceId != coroutineLockTimer.CoroutineLock.InstanceId)
                {
                    continue;
                }

                CoroutineLock coroutineLock = coroutineLockTimer.CoroutineLock;
                // 超时直接调用下一个锁
                self.RunNextCoroutine(coroutineLock.coroutineLockType, coroutineLock.key, coroutineLock.level + 1);
                coroutineLock.coroutineLockType = CoroutineLockType.None; // 上面调用了下一个, dispose不再调用
            }
        }
    }

    public static class CoroutineLockComponentSystem
    {
        public static void RunNextCoroutine(this CoroutineLockComponent self, int coroutineLockType, long key, int level)
        {
            // 一个协程队列一帧处理超过100个,说明比较多了,打个warning,检查一下是否够正常
            if (level == 100)
            {
                Log.Warning($"too much coroutine level: {coroutineLockType} {key}");
            }
            self.nextFrameRun.Enqueue((coroutineLockType, key, level));
        }

        private static void AddTimer(this CoroutineLockComponent self, long tillTime, CoroutineLock coroutineLock)
        {
            self.timers.Add(tillTime, new CoroutineLockTimer(coroutineLock));
            if (tillTime < self.minTime)
            {
                self.minTime = tillTime;
            }
        }

        public static async Task<CoroutineLock> Wait(this CoroutineLockComponent self, int coroutineLockType, long key, int time = 60000)
        {
            CoroutineLockQueueType coroutineLockQueueType = self.list[coroutineLockType];
   
            if (!coroutineLockQueueType.TryGetValue(key, out CoroutineLockQueue queue))
            {
                CoroutineLockQueue coroutineLockQueue = ComponentFactory.CreateWithId<CoroutineLockQueue>(++self.idGenerator, true);
                coroutineLockQueueType.Add(key, coroutineLockQueue);
                return self.CreateCoroutineLock(coroutineLockType, key, time, 1);
            }
            TaskCompletionSource<CoroutineLock> tcs = new TaskCompletionSource<CoroutineLock>();
            queue.Add(tcs, time);
            return await tcs.Task;
        }

        private static CoroutineLock CreateCoroutineLock(this CoroutineLockComponent self, int coroutineLockType, long key, int time, int level)
        {
            CoroutineLock coroutineLock = ComponentFactory.CreateWithId<CoroutineLock, int, long, int>(++self.idGenerator, coroutineLockType, key, level, true);
            if (time > 0)
            {
                self.AddTimer(Help_TimeHelper.GetNow() + time, coroutineLock);
            }
            return coroutineLock;
        }

        public static void Notify(this CoroutineLockComponent self, int coroutineLockType, long key, int level)
        {
            CoroutineLockQueueType coroutineLockQueueType = self.list[coroutineLockType];
            if (!coroutineLockQueueType.TryGetValue(key, out CoroutineLockQueue queue))
            {
                return;
            }

            if (queue.Count == 0)
            {
                coroutineLockQueueType.Remove(key);
                return;
            }

            CoroutineLockInfo coroutineLockInfo = queue.Dequeue();
            coroutineLockInfo.Tcs.SetResult(self.CreateCoroutineLock(coroutineLockType, key, coroutineLockInfo.Time, level));
        }
    }

    public class CoroutineLockComponent: Entity
    {
        public static CoroutineLockComponent Instance;

        public List<CoroutineLockQueueType> list;
        public Queue<(int, long, int)> nextFrameRun = new Queue<(int, long, int)>();
        public MultiMap<long, CoroutineLockTimer> timers = new MultiMap<long, CoroutineLockTimer>();
        public Queue<long> timeOutIds = new Queue<long>();
        public Queue<CoroutineLockTimer> timerOutTimer = new Queue<CoroutineLockTimer>();
        public long idGenerator;
        public long minTime;
        public long timeNow;

    }
}