using CustomFrameWork;
using CustomFrameWork.Baseic;
using ETModel;
using NLog.LayoutRenderers.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    [PrivateObject]
    public class RechargeGiftBundleComponent : TCustomComponent<MainFactory>
    {
        public Dictionary<int, RechargeGiftBundle> allGiftBundle = new Dictionary<int, RechargeGiftBundle>();
        public UnOrderMultiMap<int, int> giftBundleId2ConfigId = new UnOrderMultiMap<int, int>();
    }
}
