using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CustomFrameWork;


namespace ETModel
{
    public static class WaitTypeError
    {
        public const int Success = 0;
        public const int Destroy = 1;
        public const int Cancel = 2;
        public const int Timeout = 3;
    }
    
    public interface IWaitType
    {
        int Error
        {
            get;
            set;
        }
    }

    [ObjectSystem]
    public class ObjectWaitAwakeSystem: AwakeSystem<ObjectWait>
    {
        public override void Awake(ObjectWait self)
        {
            self.tcss.Clear();
        }
    }

    [ObjectSystem]
    public class ObjectWaitDestroySystem: DestroySystem<ObjectWait>
    {
        public override void Destroy(ObjectWait self)
        {
            foreach (object v in self.tcss.Values.ToArray())
            {
                ((ObjectWait.IDestroyRun) v).SetResult();
            }
        }
    }

    public class ObjectWait: Entity
    {
        public interface IDestroyRun
        {
            void SetResult();
        }

        public class ResultCallback<K>: IDestroyRun where K : struct, IWaitType
        {
            private TaskCompletionSource<K> tcs;

            public ResultCallback()
            {
                this.tcs = new TaskCompletionSource<K>();
            }

            public bool IsDisposed
            {
                get
                {
                    return this.tcs == null;
                }
            }

            public Task<K> Task => this.tcs.Task;

            public void SetResult(K k)
            {
                var t = tcs;
                this.tcs = null;
                t.SetResult(k);
            }

            public void SetResult()
            {
                var t = tcs;
                this.tcs = null;
                t.SetResult(new K() { Error = WaitTypeError.Destroy });
            }
        }

        public Dictionary<Type, object> tcss = new Dictionary<Type, object>();

        public async Task<T> Wait<T>(ETCancellationToken cancellationToken = null) where T : struct, IWaitType
        {
            Scene clientScene = GetParent<Scene>();
            if (clientScene != null)
            {
                Log.Debug($"[{clientScene.Name}] Wait {typeof(T).Name}");
            }
            ResultCallback<T> tcs = new ResultCallback<T>();
            Type type = typeof (T);
            this.tcss.Add(type, tcs);
            
            void CancelAction()
            {
                if(clientScene != null) Log.Debug($"[{clientScene.Name}] Wait.CancelAction {type.Name}");
                this.Notify(new T() { Error = WaitTypeError.Cancel });
            }

            T ret;
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

        public async Task<T> Wait<T>(int timeout, ETCancellationToken cancellationToken = null) where T : struct, IWaitType
        {
            ResultCallback<T> tcs = new ResultCallback<T>();
            async Task WaitTimeout()
            {
                bool retV = await ETModel.ET.TimerComponent.Instance.WaitAsync(timeout, cancellationToken);
                if (!retV)
                {
                    return;
                }
                if (tcs.IsDisposed)
                {
                    return;
                }
                Notify(new T() { Error = WaitTypeError.Timeout });
            }
            
            WaitTimeout().Coroutine();
            
            this.tcss.Add(typeof (T), tcs);
            
            void CancelAction()
            {
                Notify(new T() { Error = WaitTypeError.Cancel });
            }
            
            T ret;
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

        public void Notify<T>(T obj) where T : struct, IWaitType
        {
            Type type = typeof (T);
            if (!this.tcss.TryGetValue(type, out object tcs))
            {
                return;
            }

            this.tcss.Remove(type);
            Scene clientScene = GetParent<Scene>();
            if (clientScene != null)
                Log.Debug($"[{clientScene.Name}] Notify {type.Name}");
            ((ResultCallback<T>) tcs).SetResult(obj);
        }
    }
}