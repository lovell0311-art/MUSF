using UnityEngine;
using ETModel;
using System;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;
using System.Text;


namespace ETHotfix
{
	[ObjectSystem]
	public class PingComponentAwakeSystem : AwakeSystem<PingComponent>
	{
        public override void Awake(PingComponent self)
        {
            self.timerId = TimerComponent.Instance.RegisterTimeCallBack(5000, self.Ping).Id;
			self.Session = self.GetParent<Session>();
        }
    }


	[ObjectSystem]
	public class PingComponentDestroySystem : DestroySystem<PingComponent>
	{
		public override void Destroy(PingComponent self)
		{
            if (self.timerId != 0)
			{
                TimerComponent.Instance.Remove(self.timerId);
                self.timerId = 0;
            }
			self.Session = null;

            self.startTime = 0;
            self.lastNetworkDelay = 0;
        }
	}




    public class PingComponent : Entity
    {
        public long timerId = 0;
        public Session Session = null;

        public long startTime = 0;
        public long lastNetworkDelay = 0;
        /// <summary>网络延迟 ms</summary>
        public long NetworkDelay
        {
            get
            {
                if (startTime == 0) return lastNetworkDelay;
                long delay = (TimeHelper.Now() - startTime) / 2;
                if(delay > lastNetworkDelay)return delay;
                return lastNetworkDelay;
            }
        }
        public void Ping()
		{
            PingAsync().Coroutine();

            long oldTimerId = timerId;
            timerId = TimerComponent.Instance.RegisterTimeCallBack(5000, Ping).Id;
        }

		public async ETVoid PingAsync()
		{
            if (this.startTime != 0)
            {
                LogCollectionComponent.Instance.Info($"#Ping# keep3(t={TimeHelper.Now()})(ms={this.NetworkDelay})");
                return;
            }
            this.startTime = TimeHelper.Now();
            long instanceId = this.InstanceId;
            try
            {
                if (Session != null && Session.session != null)
                {
                    LogCollectionComponent.Instance.Info($"#Ping# keep(t={startTime})(ms={this.NetworkDelay})");
                    await Session.session.Call(new C2R_Ping() { });
                    if (instanceId != this.InstanceId) return; // 收到回复时，Ping已销毁
                    this.lastNetworkDelay = (int)((TimeHelper.Now() - startTime) / 2);
                }
                else
                {
                    LogCollectionComponent.Instance.Info($"#Ping# keep2(ms={this.NetworkDelay})");
                }
            }
            finally
            {
                if (instanceId != this.InstanceId)
                {
                    // 收到回复时，Ping已销毁
                }
                else
                {
                    this.startTime = 0;
                }
            }

        }

    }



}
