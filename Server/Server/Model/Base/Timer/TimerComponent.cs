using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CustomFrameWork;

namespace ETModel.ET
{
    public enum TimerClass
    {
        None,
        OnceTimer,
        OnceWaitTimer,
        RepeatedTimer,
    }
    
    [ObjectSystem]
    public class TimerActionAwakeSystem: AwakeSystem<TimerAction, TimerClass, long, int, object>
    {
        public override void Awake(TimerAction self, TimerClass timerClass, long time, int type, object obj)
        {
            self.TimerClass = timerClass;
            self.Object = obj;
            self.StartTime = 0;
            self.Time = time;
            self.TillTime = 0;
            self.Type = type;
        }
    }

    [ObjectSystem]
    public class TimerActionDestroySystem: DestroySystem<TimerAction>
    {
        public override void Destroy(TimerAction self)
        {
            self.Object = null;
            self.StartTime = 0;
            self.Time = 0;
            self.TillTime = 0;
            self.TimerClass = TimerClass.None;
            self.Type = 0;
        }
    }
    
    public class TimerAction: Entity
    {
        public TimerClass TimerClass;

        public object Object;

        public long StartTime;

        public long Time;

        public long TillTime;

        public int Type;
    }

    [ObjectSystem]
    public class TimerComponentAwakeSystem: AwakeSystem<TimerComponent>
    {
        public override void Awake(TimerComponent self)
        {
            TimerComponent.Instance = self;
            self.Awake();
        }
    }

    [ObjectSystem]
    public class TimerComponentUpdateSystem: UpdateSystem<TimerComponent>
    {
        public override void Update(TimerComponent self)
        {
            self.Update();
        }
    }
    
    [ObjectSystem]
    public class TimerComponentLoadSystem: LoadSystem<TimerComponent>
    {
        public override void Load(TimerComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class TimerComponentDestroySystem: DestroySystem<TimerComponent>
    {
        public override void Destroy(TimerComponent self)
        {
            TimerComponent.Instance = null;
        }
    }

    public class TimerComponent: Entity
    {
        public static TimerComponent Instance
        {
            get;
            set;
        }

        /// <summary>
        /// key: time, value: timer id
        /// </summary>
        private readonly MultiMap<long, long> TimeId = new MultiMap<long, long>();

        private readonly Queue<long> timeOutTime = new Queue<long>();

        private readonly Queue<long> timeOutTimerIds = new Queue<long>();
        
        private readonly Queue<long> everyFrameTimer = new Queue<long>();

        private readonly Dictionary<long, TimerAction> timerId2TimerAction = new Dictionary<long, TimerAction>();

        // 记录最小时间，不用每次都去MultiMap取第一个值
        private long minTime;

        private const int TimeTypeMax = 10000;

        private ITimer[] timerActions;

        public void Awake()
        {
            this.foreachFunc = (k, v) =>
            {
                if (k > this.timeNow)
                {
                    minTime = k;
                    return false;
                }

                this.timeOutTime.Enqueue(k);
                return true;
            };
            
            this.timerActions = new ITimer[TimeTypeMax];

            List<Type> types = Game.EventSystem.GetTypes(typeof (TimerAttribute));

            foreach (Type type in types)
            {
                ITimer iTimer = Activator.CreateInstance(type) as ITimer;
                if (iTimer == null)
                {
                    Log.Error($"Timer Action {type.Name} 需要继承 ITimer");
                    continue;
                }
                
                object[] attrs = type.GetCustomAttributes(typeof(TimerAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }

                foreach (object attr in attrs)
                {
                    TimerAttribute timerAttribute = attr as TimerAttribute;
                    this.timerActions[timerAttribute.Type] = iTimer;
                }
            }
        }

        private long timeNow;
        public long TimeNow { get { return timeNow; } }
        private Func<long, List<long>, bool> foreachFunc;

        public void Update()
        {
            if (this.TimeId.Count == 0)
            {
                return;
            }

            timeNow = Help_TimeHelper.GetNow();


            #region 每帧执行的timer，不用foreach TimeId，减少GC

            int count = this.everyFrameTimer.Count;
            for (int i = 0; i < count; ++i)
            {
                long timerId = this.everyFrameTimer.Dequeue();
                if(timerId2TimerAction.TryGetValue(timerId,out var timerAction))
                {
                    try
                    {
                        Run(timerAction);
                    }
                    catch(Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }

            #endregion


            if (timeNow < this.minTime)
            {
                return;
            }

            foreach (var kv in TimeId.GetDictionary())
            {
                if (kv.Key > this.timeNow)
                {
                    minTime = kv.Key;
                    break;
                }
                this.timeOutTime.Enqueue(kv.Key);
            }

            while (this.timeOutTime.Count > 0)
            {
                long time = this.timeOutTime.Dequeue();
                var list = this.TimeId[time];
                for (int i = 0; i < list.Count; ++i)
                {
                    long timerId = list[i];
                    this.timeOutTimerIds.Enqueue(timerId);
                }

                this.TimeId.Remove(time);
            }

            while (this.timeOutTimerIds.Count > 0)
            {
                long timerId = this.timeOutTimerIds.Dequeue();

                if (timerId2TimerAction.TryGetValue(timerId, out var timerAction))
                {
                    if (timerAction.TillTime > timeNow)
                    {
                        this.AddTimer(timerAction.TillTime, timerAction);
                        continue;
                    }
                    try
                    {
                        Run(timerAction);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }

        private void Run(TimerAction timerAction)
        {
            switch (timerAction.TimerClass)
            {
                case TimerClass.OnceTimer:
                {
                    long timerId = timerAction.Id;
                    int type = timerAction.Type;
                    ITimer timer = this.timerActions[type];
                    if (timer == null)
                    {
                        Log.Error($"not found timer action: {type}");
                        this.Remove(timerId);
                        return;
                    }
                    timer.Handle(timerAction.Object);
                    this.Remove(timerId);
                    break;
                }
                case TimerClass.OnceWaitTimer:
                {
                    TaskCompletionSource<bool> tcs = timerAction.Object as TaskCompletionSource<bool>;
                    this.Remove(timerAction.Id);
                    tcs.SetResult(true);
                    break;
                }
                case TimerClass.RepeatedTimer:
                {
                    int type = timerAction.Type;
                    long tillTime = timerAction.StartTime + timerAction.Time;
                    this.AddTimer(tillTime, timerAction);

                    ITimer timer = this.timerActions[type];
                    if (timer == null)
                    {
                        Log.Error($"not found timer action: {type}");
                        return;
                    }
                    timer.Handle(timerAction.Object);
                    break;
                }
            }
        }
        
        private void AddTimer(long tillTime, TimerAction timer)
        {
            timer.StartTime = tillTime;
            if (timer.TimerClass == TimerClass.RepeatedTimer && timer.Time == 0)
            {
                this.everyFrameTimer.Enqueue(timer.Id);
                return;
            }
            long time = timeNow + (1000 * 60 * 60);
            if (tillTime > time)
            {
                // 定时器超过一小时，设置1小时后从 this.TimeId 中移除
                // 防止 this.TimeId 元素过多，暂用内存
                this.TimeId.Add(time, timer.Id);
                if (time < this.minTime)
                {
                    this.minTime = time;
                }
            }
            else
            {
                this.TimeId.Add(tillTime, timer.Id);
                if (tillTime < this.minTime)
                {
                    this.minTime = tillTime;
                }
            }
            
        }

        public bool Remove(ref long id)
        {
            long i = id;
            id = 0;
            return this.Remove(i);
        }
        
        private bool Remove(long id)
        {
            if (id == 0)
            {
                return false;
            }

            if(timerId2TimerAction.TryGetValue(id,out var timerAction))
            {
                timerId2TimerAction.Remove(id);
                timerAction.Dispose();
                return true;
            }
            return false;
        }

        public async Task<bool> WaitTillAsync(long tillTime, ETCancellationToken cancellationToken = null)
        {
            if (timeNow >= tillTime)
            {
                return true;
            }

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            TimerAction timer = ComponentFactory.Create<TimerAction, TimerClass, long, int, object>(TimerClass.OnceWaitTimer, tillTime - timeNow, 0, tcs, true);
            timer.TillTime = tillTime;
            timerId2TimerAction.Add(timer.Id, timer);
            this.AddTimer(tillTime, timer);
            long timerId = timer.Id;

            void CancelAction()
            {
                if (this.Remove(timerId))
                {
                    tcs.SetResult(false);
                }
            }
            
            bool ret;
            try
            {
                cancellationToken?.Add(CancelAction);
                ret = await tcs.Task;
            }
            finally
            {
                cancellationToken?.Remove(CancelAction);    
            }
            return ret;
        }

        public async Task<bool> WaitFrameAsync(ETCancellationToken cancellationToken = null)
        {
            bool ret = await WaitAsync(1, cancellationToken);
            return ret;
        }

        public async Task<bool> WaitAsync(long time, ETCancellationToken cancellationToken = null)
        {
            if (time == 0)
            {
                return true;
            }
            long tillTime = Help_TimeHelper.GetNow() + time;

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            
            TimerAction timer = ComponentFactory.Create<TimerAction, TimerClass, long, int, object>(TimerClass.OnceWaitTimer, time, 0, tcs, true);
            timer.TillTime = tillTime;
            timerId2TimerAction.Add(timer.Id, timer);
            this.AddTimer(tillTime, timer);
            long timerId = timer.Id;

            void CancelAction()
            {
                if (this.Remove(timerId))
                {
                    tcs.SetResult(false);
                }
            }

            bool ret;
            try
            {
                cancellationToken?.Add(CancelAction);
                ret = await tcs.Task;
            }
            finally
            {
                cancellationToken?.Remove(CancelAction); 
            }
            return ret;
        }
        
        // 用这个优点是可以热更，缺点是回调式的写法，逻辑不连贯。WaitTillAsync不能热更，优点是逻辑连贯。
        // wait时间短并且逻辑需要连贯的建议WaitTillAsync
        // wait时间长不需要逻辑连贯的建议用NewOnceTimer
        public long NewOnceTimer(long tillTime, int type, object args)
        {
            TimerAction timer = ComponentFactory.Create<TimerAction, TimerClass, long, int, object>(TimerClass.OnceTimer, tillTime, type, args, true);
            timer.TillTime = tillTime;
            timerId2TimerAction.Add(timer.Id, timer);
            this.AddTimer(tillTime, timer);
            return timer.Id;
        }

        /// <summary>
        /// 创建一个RepeatedTimer
        /// </summary>
        private long NewRepeatedTimerInner(long time, int type, object args)
        {
            long tillTime = Help_TimeHelper.GetNow() + time;
            TimerAction timer = ComponentFactory.Create<TimerAction, TimerClass, long, int, object>(TimerClass.RepeatedTimer, time, type, args, true);
            timer.TillTime = tillTime;
            timerId2TimerAction.Add(timer.Id, timer);

            // 每帧执行的不用加到timerId中，防止遍历
            this.AddTimer(tillTime, timer);
            return timer.Id;
        }

        public long NewRepeatedTimer(long time, int type, object args)
        {
            return NewRepeatedTimerInner(time, type, args);
        }
    }
}
