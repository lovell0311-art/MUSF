using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
namespace ETHotfix
{
    /// <summary>
    /// Ó«¹â±¦Ê¯ÏâÇ¶
    /// </summary>
    public partial class UIKnapsackComponent
    {
        ReferenceCollector collector_YingGuangBaoShiXiangQian;
        readonly int Glod_XiangQian = 1_000_000;
        readonly int Success_XiangQian= 100;
        public void Init_YingGuangBaoShiXiangQian()
        {
            collector_YingGuangBaoShiXiangQian = YingGuangBaoShiXiangQianPanel.GetReferenceCollector();
            InitItemGrid(collector_YingGuangBaoShiXiangQian, 4, E_InlayType.YingGuangBaoShiXiangQian, Glod_XiangQian, Success_XiangQian);

        }
    }
}
