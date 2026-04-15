using System;
using System.Collections.Concurrent;
using System.Threading;

namespace CustomFrameWork
{
    public class CustomSynchonization : SynchronizationContext
    {
        public static CustomSynchonization Instance { get; } = new CustomSynchonization();

        public readonly int MainThreadID = Thread.CurrentThread.ManagedThreadId;

        private readonly ConcurrentQueue<Action> mConcurrentQueue = new ConcurrentQueue<Action>();

        public override void Post(SendOrPostCallback b_Callback, object b_State)
        {
            if (MainThreadID == Thread.CurrentThread.ManagedThreadId)
            {
                b_Callback(b_State);
                return;
            }
            mConcurrentQueue.Enqueue(() => { b_Callback(b_State); });
        }

        private Action mAction;
        public void Update()
        {
            while (true)
            {
                if (mConcurrentQueue.TryDequeue(out mAction)) mAction();
                else break;
            }
        }
    }


	public class OneThreadSynchronizationContext : SynchronizationContext
	{
		public static OneThreadSynchronizationContext Instance { get; } = new OneThreadSynchronizationContext();

		private readonly int mainThreadId = Thread.CurrentThread.ManagedThreadId;

		// 线程同步队列,发送接收socket回调都放到该队列,由poll线程统一执行
		private readonly ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();

		private Action a;

		public void Update()
		{
			while (true)
			{
				if (!this.queue.TryDequeue(out a))
				{
					return;
				}
				a();
			}
		}

		public override void Post(SendOrPostCallback callback, object state)
		{
			if (Thread.CurrentThread.ManagedThreadId == this.mainThreadId)
			{
				callback(state);
				return;
			}

			this.queue.Enqueue(() => { callback(state); });
		}
	}
}
