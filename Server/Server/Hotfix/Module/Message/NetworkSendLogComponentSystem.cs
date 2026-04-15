using ETModel;
using ETModel.ET;
using System;
using CustomFrameWork;

namespace ETHotfix
{
    [FriendOf(typeof(NetworkSendLogComponent))]
    [Timer(TimerType.NetworkSendLog)]
    public class NetworkSendLogTimer : ATimer<NetworkSendLogComponent>
    {
        public override void Run(NetworkSendLogComponent self)
        {
            self.SecondsUpdate();
        }
    }

    [FriendOf(typeof(NetworkSendLogComponent))]
    [ObjectSystem]
    public class NetworkSendLogComponentAwakeSystem : AwakeSystem<NetworkSendLogComponent>
    {
        public override void Awake(NetworkSendLogComponent self)
        {
            self.lastSendBytesCount = 0;
            self.lastSendPackCount = 0;

            self.session = self.GetParent<Session>();
            self.kchannel = self.session.Channel as KChannel;
            if (self.kchannel == null)
            {
                throw new Exception("NetworkSendLogComponent 只能添加到KCP的Session上");
            }
            self.timerId = TimerComponent.Instance.NewRepeatedTimer(1000, TimerType.NetworkSendLog, self);
        }
    }

    [FriendOf(typeof(NetworkSendLogComponent))]
    [ObjectSystem]
    public class NetworkSendLogComponentDestroySystem : DestroySystem<NetworkSendLogComponent>
    {
        public override void Destroy(NetworkSendLogComponent self)
        {
            TimerComponent.Instance.Remove(ref self.timerId);
            self.kchannel = null;
            self.session = null;
        }
    }


    [FriendOf(typeof(NetworkSendLogComponent))]
    public static class NetworkSendLogComponentSystem
    {
        public static void SecondsUpdate(this NetworkSendLogComponent self)
        {
            uint sendBytesCount = self.kchannel.SendBytesCount - self.lastSendBytesCount;
            uint sendPackCount = self.kchannel.SendPackCount - self.lastSendPackCount;

            self.lastSendBytesCount = self.kchannel.SendBytesCount;
            self.lastSendPackCount = self.kchannel.SendPackCount;

            if(sendBytesCount > 1000)
            {
                Log.Info($"#流量统计# ({self.session.Id}) bytes:{sendBytesCount} pack:{sendPackCount}");
            }
        }


        public static void PrintLog(this NetworkSendLogComponent self)
        {
            uint sendBytesCount = self.kchannel.SendBytesCount - self.lastSendBytesCount;
            uint sendPackCount = self.kchannel.SendPackCount - self.lastSendPackCount;

            Log.Info($"#流量统计##PrintLog# ({self.session.Id}) bytes:{sendBytesCount} pack:{sendPackCount} LastRecvTime:{self.kchannel.LastRecvTimestamp} LastPingTime:{self.kchannel.LastPingTimestamp} LastRecvPingTime:{self.kchannel.LastRecvPingTimestamp} PeekSize:{self.kchannel.KcpPeekSize()}");
        }
    }
}
