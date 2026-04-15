using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{

    /// <summary>
    /// 荧之石合成
    /// </summary>
    public partial class UIKnapsackComponent
    {
        ReferenceCollector collector_YingZhiShiHeCheng;
        readonly int Glod = 1_000_000;
        readonly int Success = 100;
        public void Init_YingZhiShiHeCheng() 
        {
            collector_YingZhiShiHeCheng = YingZhiShiHeChengPanel.GetReferenceCollector();
            InitItemGrid(collector_YingZhiShiHeCheng,5,E_InlayType.YingZhiShiHeCheng,Glod, Success);

        }

       
    }
}
