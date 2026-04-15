using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;

namespace ETModel
{
    public class PingComponent : TCustomComponent<MainFactory>
    {
        public long Ping = 5; //延迟值
        public Dictionary<long, long> ReceiveTickDic = new Dictionary<long, long>();


        public override void Dispose()
        {
            if (IsDisposeable) return;

            if (ReceiveTickDic != null && ReceiveTickDic.Count > 0)
            {
                ReceiveTickDic.Clear();
            }
            base.Dispose();
        }
    }
}
