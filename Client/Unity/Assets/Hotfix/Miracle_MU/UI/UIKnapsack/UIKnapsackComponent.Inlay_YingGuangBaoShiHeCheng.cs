using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 荧光宝石 合成
    /// </summary>
    public partial class UIKnapsackComponent
    {
        ReferenceCollector collector_YingGuangBaoShiHeCheng;
        readonly int Glod_HeCheng = 1_000_000;
        readonly int Success_HeCheng = 100;
        public void Init_YingGuangBaoShiHeCheng()
        {
            collector_YingGuangBaoShiHeCheng = YingGuangBaoShiHeChengPanel.GetReferenceCollector();
            InitItemGrid(collector_YingGuangBaoShiHeCheng, 4, E_InlayType.YingGuangBaoShiHeCheng, Glod_HeCheng, Success_HeCheng);

        }
    }
}
