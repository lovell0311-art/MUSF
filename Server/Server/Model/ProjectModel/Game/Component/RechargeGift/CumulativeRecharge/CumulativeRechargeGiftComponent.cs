using CustomFrameWork.Baseic;
using CustomFrameWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    /// <summary>
    /// 累计充值礼包
    /// </summary>
    [PrivateObject]
    public class CumulativeRechargeGiftComponent : TCustomComponent<MainFactory>
    {
        public UnOrderMultiMap<(int typeId, int id2, E_GameOccupation roleType), int> id2ItemInfoId = new UnOrderMultiMap<(int, int, E_GameOccupation), int>();
    }
}
