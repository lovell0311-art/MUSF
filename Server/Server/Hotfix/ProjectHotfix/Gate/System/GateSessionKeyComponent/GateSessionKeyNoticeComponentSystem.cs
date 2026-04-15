using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;

namespace ETHotfix
{
    [Timer(TimerType.GateSessionKeyNotice)]
    public class GateSessionKeyNoticeTimer : ATimer<GateSessionKeyNoticeComponent>
    {
        public override void Run(GateSessionKeyNoticeComponent self)
        {
            Session session = self.GetParent<Session>();
            SessionGateUserComponent gateUser = session.GetComponent<SessionGateUserComponent>();
            if (gateUser == null) return;
            // 重新生成一个key,用于断线重连
            long newKey = Help_UniqueValueHelper.GetServerUniqueValue();
            Root.MainFactory.GetCustomComponent<GateSessionKeyComponent>().Add(gateUser.UserId.ToString(), newKey, 1000 * 60 * 8);

            GateSessionKeyNoticeComponent.gate2C_GateSessionKeyChange.NewKey = newKey;
            session.Send(GateSessionKeyNoticeComponent.gate2C_GateSessionKeyChange);
        }
    }

    [ObjectSystem]
    public class GateSessionKeyNoticeComponentAwakeSystem : AwakeSystem<GateSessionKeyNoticeComponent>
    {
        public override void Awake(GateSessionKeyNoticeComponent self)
        {
            self._timerId = ETModel.ET.TimerComponent.Instance.NewRepeatedTimer(GateSessionKeyNoticeComponent.KEY_NOTICE_INTERVAL, TimerType.GateSessionKeyNotice, self);
        }
    }

    [ObjectSystem]
    public class GateSessionKeyNoticeComponentDestroySystem : DestroySystem<GateSessionKeyNoticeComponent>
    {
        public override void Destroy(GateSessionKeyNoticeComponent self)
        {
            ETModel.ET.TimerComponent.Instance.Remove(ref self._timerId);
        }
    }

}
