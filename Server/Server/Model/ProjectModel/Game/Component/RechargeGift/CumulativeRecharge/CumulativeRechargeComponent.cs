using CustomFrameWork.Baseic;
using CustomFrameWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    /// <summary>
    /// 累计充值金额统计组件
    /// </summary>
    [PrivateObject]
    public class CumulativeRechargeComponent : TCustomComponent<Player>
    {
        public int totalAmount;
        /// <summary>
        /// 累计充值金额
        /// </summary>
        public int TotalAmount { get => totalAmount; }
    }
}
