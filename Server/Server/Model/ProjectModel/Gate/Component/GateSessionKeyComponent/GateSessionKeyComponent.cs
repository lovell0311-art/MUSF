using CustomFrameWork.Component;
using CustomFrameWork.Baseic;
using CustomFrameWork;

using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace ETModel
{
    public class GateSessionKeyComponent : TCustomComponent<MainFactory>
    {
        private readonly Dictionary<long, string> sessionKey = new Dictionary<long, string>();
        public override void Awake()
        {
            if (sessionKey != null && sessionKey.Count > 0)
            {
                sessionKey.Clear();
            }
        }
        public override void Dispose()
        {
            if (IsDisposeable)
            {
                return;
            }
            if (sessionKey != null && sessionKey.Count > 0)
            {
                sessionKey.Clear();
            }
            base.Dispose();
        }



        public void Add(string account, long key,long timeout = 20000)
        {
            this.sessionKey.Add(key, account);
            this.TimeoutRemoveKey(key, timeout).Coroutine();
        }

        public string Get(long key)
        {
            string account = null;
            this.sessionKey.TryGetValue(key, out account);
            return account;
        }

        public void Remove(long key)
        {
            this.sessionKey.Remove(key);
        }

        private async Task TimeoutRemoveKey(long key,long timeout)
        {
            await CustomFrameWork.Root.MainFactory.GetCustomComponent<TimerComponent>().WaitAsync(timeout);
            this.sessionKey.Remove(key);
        }
    }
}
