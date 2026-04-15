using CustomFrameWork;
using CustomFrameWork.Baseic;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    /// <summary>
    /// 奖池
    /// </summary>
    public class LotteryPoolComponent : TCustomComponent<MainFactory>
    {
        public RandomSelector<int> giftPool = new RandomSelector<int>();
    }
}
