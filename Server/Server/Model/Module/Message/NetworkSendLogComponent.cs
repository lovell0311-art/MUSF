using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    [PrivateObject]
    public class NetworkSendLogComponent : Entity
    {
        public uint lastSendBytesCount;
        public uint lastSendPackCount;
        public long timerId;
        public KChannel kchannel;
        public Session session;
    }
}
