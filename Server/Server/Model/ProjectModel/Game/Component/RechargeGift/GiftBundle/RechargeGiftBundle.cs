using CustomFrameWork.Baseic;
using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    [PrivateObject]
    public class RechargeGiftBundle : CustomComponent
    {
        public long startTime;
        public long endTime;
        public RechargeGiftBundle_TypeConfig config { get; set; }
        public Dictionary<int, Item> configId2Item = new Dictionary<int, Item>();

        public RechargeGiftBundle_TypeConfig Config => config;
    }
}
